using System.Text.Json;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Contracts.DTO.Messages;
using AlfaCharge.OcppServer.WebSockets;

namespace AlfaCharge.OcppServer.Versioned_Handlers.ConfigurationHandler
{
    public sealed class Ocpp16ConfigurationHandler : IConfigurationOps16
    {
        private readonly OcppConnectionManager _connections;
        private readonly JsonSerializerOptions _json;

        public Ocpp16ConfigurationHandler(OcppConnectionManager connections, JsonSerializerOptions json)
        {
            _connections = connections;
            _json = json;
        }

        public async Task<GetConfigurationConf16> GetConfigurationAsync(string cpId, string[]? keys, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null)
                throw new InvalidOperationException($"Charge point '{cpId}' not connected.");

            var req = new GetConfigurationReq16 { Key = keys };
            var payloadJson = await conn.SendCallAsync("GetConfiguration", req, timeout: TimeSpan.FromSeconds(30), ct);
            return JsonSerializer.Deserialize<GetConfigurationConf16>(payloadJson, _json)!;
        }

        public async Task<ChangeConfigurationConf16> ChangeConfigurationAsync(string cpId, string key, string value, CancellationToken ct)
        {
            if (!_connections.TryGet(cpId, out var conn) || conn is null)
                throw new InvalidOperationException($"Charge point '{cpId}' not connected.");

            var req = new ChangeConfigurationReq16 { Key = key, Value = value };
            var payloadJson = await conn.SendCallAsync("ChangeConfiguration", req, TimeSpan.FromSeconds(30), ct);
            return JsonSerializer.Deserialize<ChangeConfigurationConf16>(payloadJson, _json)!;
        }
    }
}