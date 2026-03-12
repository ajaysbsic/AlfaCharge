using System.Text.Json;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware;
using AlfaCharge.OcppServer.WebSockets;

namespace AlfaCharge.OcppServer.Versioned_Handlers.ConfigurationHandler
{
    public sealed class Ocpp201DiagnosticsFirmwareHandler : IDiagnosticsFirmwareOps201
    {
        private readonly OcppConnectionManager _connections;
        private readonly JsonSerializerOptions _json;

        public Ocpp201DiagnosticsFirmwareHandler(OcppConnectionManager connections, JsonSerializerOptions json)
        {
            _connections = connections;
            _json = json;
        }

        public async Task<GetLogConf201> GetLogAsync(string cpId, GetLogReq201 req, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null) throw new InvalidOperationException($"Charge point '{cpId}' not connected.");
            var payloadJson = await conn.SendCallAsync("GetLog", req, TimeSpan.FromSeconds(30), ct);
            return JsonSerializer.Deserialize<GetLogConf201>(payloadJson, _json)!;
        }

        public async Task<UpdateFirmwareConf201> UpdateFirmwareAsync(string cpId, UpdateFirmwareReq201 req, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null) throw new InvalidOperationException($"Charge point '{cpId}' not connected.");
            var payloadJson = await conn.SendCallAsync("UpdateFirmware", req, TimeSpan.FromSeconds(30), ct);
            return JsonSerializer.Deserialize<UpdateFirmwareConf201>(payloadJson, _json)!;
        }

        public Task HandleLogStatusNotificationAsync(string cpId, LogStatusNotificationReq201 req, CancellationToken ct)
            => Task.CompletedTask; // TODO: persist + SignalR/event bus

        public Task HandleFirmwareStatusNotificationAsync(string cpId, FirmwareStatusNotificationReq201 req, CancellationToken ct)
            => Task.CompletedTask; // TODO: persist + SignalR/event bus
    }
}