using System.Text.Json;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Contracts.DTO.Messages;
using AlfaCharge.OcppServer.WebSockets;

namespace AlfaCharge.OcppServer.Versioned_Handlers.ConfigurationHandler
{
    public sealed class Ocpp16DiagnosticsFirmwareHandler : IDiagnosticsFirmwareOps16
    {
        private readonly OcppConnectionManager _connections;
        private readonly JsonSerializerOptions _json;

        public Ocpp16DiagnosticsFirmwareHandler(OcppConnectionManager connections, JsonSerializerOptions json)
        {
            _connections = connections;
            _json = json;
        }

        public async Task<GetDiagnosticsConf16> GetDiagnosticsAsync(string cpId, GetDiagnosticsReq16 req, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null) throw new InvalidOperationException($"Charge point '{cpId}' not connected.");
            var payloadJson = await conn.SendCallAsync("GetDiagnostics", req, TimeSpan.FromSeconds(30), ct);
            return JsonSerializer.Deserialize<GetDiagnosticsConf16>(payloadJson, _json)!;
        }

        public async Task<UpdateFirmwareConf16> UpdateFirmwareAsync(string cpId, UpdateFirmwareReq16 req, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null) throw new InvalidOperationException($"Charge point '{cpId}' not connected.");
            var payloadJson = await conn.SendCallAsync("UpdateFirmware", req, TimeSpan.FromSeconds(30), ct);
            return JsonSerializer.Deserialize<UpdateFirmwareConf16>(payloadJson, _json)!;
        }

        // CP -> CSMS notifications (invoked by router)
        public Task HandleDiagnosticsStatusNotificationAsync(string cpId, DiagnosticsStatusNotificationReq16 req, CancellationToken ct)
            => Task.CompletedTask; // TODO: persist + SignalR/event bus

        public Task HandleFirmwareStatusNotificationAsync(string cpId, FirmwareStatusNotificationReq16 req, CancellationToken ct)
            => Task.CompletedTask; // TODO: persist + SignalR/event bus
    }
}