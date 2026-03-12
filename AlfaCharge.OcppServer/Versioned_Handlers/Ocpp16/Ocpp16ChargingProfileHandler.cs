using System.Text.Json;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.WebSockets;

namespace AlfaCharge.OcppServer.Versioned_Handlers.Ocpp16
{
    public class Ocpp16ChargingProfileHandler : IChargingProfileOps16
    {
        private readonly OcppConnectionManager _connections;
        private readonly JsonSerializerOptions _jsonOptions;
        public Ocpp16ChargingProfileHandler(OcppConnectionManager cm, JsonSerializerOptions jsonOptions)
        { _connections = cm; _jsonOptions = jsonOptions; }

        public async Task<string> ClearChargingProfileAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "ClearChargingProfile", payload, ct);

        public async Task<string> SetChargingProfileAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "SetChargingProfile", payload, ct);

        public async Task<string> GetCompositeScheduleAsync(string chargePointId, object payload, CancellationToken ct)
            => await SendAsync(chargePointId, "GetCompositeSchedule", payload, ct);

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
