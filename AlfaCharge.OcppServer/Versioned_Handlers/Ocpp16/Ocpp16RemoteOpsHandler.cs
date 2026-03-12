using System.Text.Json;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Helpers;
using AlfaCharge.OcppServer.WebSockets;

namespace AlfaCharge.OcppServer.Versioned_Handlers.Ocpp16
{
    public class Ocpp16RemoteOpsHandler : IRemoteOps16
    {
        private readonly OcppConnectionManager _connections;
        private readonly JsonSerializerOptions _jsonOptions;
        public Ocpp16RemoteOpsHandler(OcppConnectionManager cm, JsonSerializerOptions jsonOptions)
        {
            _connections = cm; _jsonOptions = jsonOptions;
        }

        public async Task<string> RemoteStartTransactionAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "RemoteStartTransaction", payload, ct);

        public async Task<string> RemoteStopTransactionAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "RemoteStopTransaction", payload, ct);

        public async Task<string> ResetAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "Reset", payload, ct);

        public async Task<string> GetDiagnosticsAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "GetDiagnostics", payload, ct);

        public async Task<string> UpdateFirmwareAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "UpdateFirmware", payload, ct);

        public async Task<string> UnlockConnectorAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "UnlockConnector", payload, ct);

        private async Task<string> SendAsync(string cpId, string action, object payload, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null)
                throw new InvalidOperationException($"Charge point {cpId} not connected");
            var timeout = TimeSpan.FromSeconds(30);
            var jsonPayload = JsonSerializer.Deserialize<object>(JsonSerializer.Serialize(payload, _jsonOptions), _jsonOptions)!;
            return await conn.SendCallAsync(action, jsonPayload, timeout, ct);
        }
    }
}
