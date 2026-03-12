using System.Text.Json;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.WebSockets;

namespace AlfaCharge.OcppServer.Versioned_Handlers.Ocpp201
{
    public class Ocpp201RemoteOpsHandler : IRemoteOps201
    {
        private readonly OcppConnectionManager _connections;
        private readonly JsonSerializerOptions _jsonOptions;
        public Ocpp201RemoteOpsHandler(OcppConnectionManager cm, JsonSerializerOptions jsonOptions)
        {
            _connections = cm; _jsonOptions = jsonOptions;
        }

        // OCPP 2.0.1/2.1 uses RequestStartTransaction (not RemoteStartTransaction)
        public async Task<string> RequestStartTransactionAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "RequestStartTransaction", payload, ct);

        // OCPP 2.0.1/2.1 uses RequestStopTransaction (not RemoteStopTransaction)
        public async Task<string> RequestStopTransactionAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "RequestStopTransaction", payload, ct);

        public async Task<string> ResetAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "Reset", payload, ct);

        // OCPP 2.0.1/2.1 uses GetLog (not GetDiagnostics)
        public async Task<string> GetLogAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "GetLog", payload, ct);

        public async Task<string> UpdateFirmwareAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "UpdateFirmware", payload, ct);

        public async Task<string> TriggerMessageAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "TriggerMessage", payload, ct);

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
