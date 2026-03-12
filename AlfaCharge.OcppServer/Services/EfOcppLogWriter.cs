using System.Threading.Channels;
using AlfaCharge.Domain.Entities;
using AlfaCharge.Infrastructure.DB;

namespace AlfaCharge.OcppServer.Services
{
    public class EfOcppLogWriter : IOcppLogWriter
    {
        private readonly ApplicationDbContext _db;

        //private readonly Channel<OcppFrameLog> _channel;
        //private readonly Channel<(string messageId, long latencyMs)> _correlations;


        public EfOcppLogWriter(ApplicationDbContext db) => _db = db;

        public async Task LogAsync(string cpId, string direction, int typeId,
                                   string? messageId, string? action, string payloadJson, string? resultCode = null)
        {
            _db.OcppLogs.Add(new OCPPLog
            {
                ChargePointId = cpId,
                Direction = direction,
                MessageTypeId = typeId,
                MessageId = messageId,
                Action = action,
                PayloadJson = payloadJson,
                ResultCode = resultCode
            });

            await _db.SaveChangesAsync();
        }
    }
}