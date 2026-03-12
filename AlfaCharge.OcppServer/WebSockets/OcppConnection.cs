using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using AlfaCharge.OcppServer.Helpers;
using AlfaCharge.OcppServer.Services;

namespace AlfaCharge.OcppServer.WebSockets
{

    public sealed class OcppConnection : IDisposable
    {
        private readonly WebSocket _socket;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pending =
            new(StringComparer.Ordinal);

        // Enforce one outstanding CSMS CALL per OCPP spec
        private readonly SemaphoreSlim _outgoingCallSemaphore = new(1, 1);

        public string ChargePointId { get; }
        public OcppProtocolVersion ProtocolVersion { get; }
        public CancellationToken Cancellation { get; }

        private readonly IOcppLogWriter _logWriter;

        public OcppConnection(
            string chargePointId,
            OcppProtocolVersion version,
            WebSocket socket,
            JsonSerializerOptions jsonOptions,
            CancellationToken cancellation,
            IOcppLogWriter logWriter)
        {
            ChargePointId = chargePointId;
            ProtocolVersion = version;
            _socket = socket;
            _jsonOptions = jsonOptions;
            Cancellation = cancellation;
            _logWriter = logWriter;
        }

        public async Task<string> SendCallAsync(string action, object payload, TimeSpan timeout, CancellationToken ct = default)
        {
            // Only one outstanding CSMS-initiated CALL at a time
            await _outgoingCallSemaphore.WaitAsync(ct);
            try
            {
                var messageId = Guid.NewGuid().ToString("N");
                var frame = new object[] { 2, messageId, action, payload };
                var json = JsonSerializer.Serialize(frame, _jsonOptions);

                var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
                if (!_pending.TryAdd(messageId, tcs))
                    throw new InvalidOperationException("Duplicate message id generation (unexpected).");

                await SendRawAsync(json, ct);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(Cancellation, ct);
                cts.CancelAfter(timeout);

                await using var _ = cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));

                // Log the outbound CALL
                await _logWriter.LogAsync(
                    this.ChargePointId,
                    direction: "outbound",
                    messageTypeId: 2,
                    messageId: messageId,
                    action: null,
                    payloadJson: JsonSerializer.Serialize(payload, _jsonOptions),
                    resultCode: "ok");

                return await tcs.Task; // resolve when CALLRESULT or CALLERROR arrives
            }
            finally
            {
                _outgoingCallSemaphore.Release();
            }
        }

        internal bool TryComplete(string messageId, string resultJson)
            => _pending.TryRemove(messageId, out var tcs) && tcs.TrySetResult(resultJson);

        internal bool TryFail(string messageId, OcppCallErrorException error)
            => _pending.TryRemove(messageId, out var tcs) && tcs.TrySetException(error);

        public async Task SendCallResultAsync(string messageId, object payload, CancellationToken ct = default)
        {
            var frame = new object[] { 3, messageId, payload };
            var json = JsonSerializer.Serialize(frame, _jsonOptions);
            await SendRawAsync(json, ct);
        }

        public async Task SendCallErrorAsync(string messageId, string errorCode, string description, object? details = null, CancellationToken ct = default)
        {
            var frame = new object[] { 4, messageId, errorCode, description, details ?? new { } };
            var json = JsonSerializer.Serialize(frame, _jsonOptions);
            await SendRawAsync(json, ct);
        }

        private async Task SendRawAsync(string json, CancellationToken ct)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            await _sendLock.WaitAsync(ct);
            try
            {
                await _socket.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public void Dispose()
        {
            foreach (var kvp in _pending)
            {
                if (_pending.TryRemove(kvp.Key, out var tcs))
                {
                    tcs.TrySetCanceled();
                }
            }
            _sendLock.Dispose();
            _outgoingCallSemaphore.Dispose();
        }
    }
}