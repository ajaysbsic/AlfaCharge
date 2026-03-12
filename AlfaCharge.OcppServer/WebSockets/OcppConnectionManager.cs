using System.Collections.Concurrent;

namespace AlfaCharge.OcppServer.WebSockets
{
    public sealed class OcppConnectionManager
    {
        private readonly ConcurrentDictionary<string, OcppConnection> _connections = new();

        public bool TryAdd(OcppConnection connection) =>
            _connections.TryAdd(connection.ChargePointId, connection);

        public bool TryGet(string chargePointId, out OcppConnection? connection) =>
            _connections.TryGetValue(chargePointId, out connection);

        public void Remove(string chargePointId)
        {
            if (_connections.TryRemove(chargePointId, out var conn))
            {
                conn.Dispose();
            }
        }

        public bool IsConnected(string chargePointId) =>
            _connections.ContainsKey(chargePointId);

        public int GetConnectedCount() =>
            _connections.Count;

        public IEnumerable<string> ConnectedIds => _connections.Keys;
    }
}