using System.Text.Json;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Contracts.DTO.Message201;
using AlfaCharge.OcppServer.WebSockets;

namespace AlfaCharge.OcppServer.Versioned_Handlers.ConfigurationHandler
{
    public sealed class Ocpp201ConfigurationHandler : IConfigurationOps201
    {
        private readonly OcppConnectionManager _connections;
        private readonly JsonSerializerOptions _json;

        public Ocpp201ConfigurationHandler(OcppConnectionManager connections, JsonSerializerOptions json)
        {
            _connections = connections;
            _json = json;
        }

        public async Task<GetVariablesConf201> GetVariablesAsync(string cpId, GetVariablesReq201 req, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null) throw new InvalidOperationException($"Charge point '{cpId}' not connected.");
            var payloadJson = await conn.SendCallAsync("GetVariables", req, TimeSpan.FromSeconds(30), ct);
            return JsonSerializer.Deserialize<GetVariablesConf201>(payloadJson, _json)!;
        }

        public async Task<SetVariablesConf201> SetVariablesAsync(string cpId, SetVariablesReq201 req, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null) throw new InvalidOperationException($"Charge point '{cpId}' not connected.");
            var payloadJson = await conn.SendCallAsync("SetVariables", req, TimeSpan.FromSeconds(30), ct);
            return JsonSerializer.Deserialize<SetVariablesConf201>(payloadJson, _json)!;
        }

        public async Task<GetBaseReportConf201> GetBaseReportAsync(string cpId, GetBaseReportReq201 req, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null) throw new InvalidOperationException($"Charge point '{cpId}' not connected.");
            var payloadJson = await conn.SendCallAsync("GetBaseReport", req, TimeSpan.FromSeconds(30), ct);
            return JsonSerializer.Deserialize<GetBaseReportConf201>(payloadJson, _json)!;
        }

        // CP -> CSMS (NotifyReport chunks)
        public Task HandleNotifyReportAsync(string cpId, NotifyReportReq201 req, CancellationToken ct)
            => Task.CompletedTask; // TODO: persist inventory + aggregate tbc=false end
    }
}