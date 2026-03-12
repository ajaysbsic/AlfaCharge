using AlfaCharge.Domain.Entities;

namespace AlfaCharge.OcppServer.Services
{
    public interface IOcppLogWriter
    {
        Task LogAsync(string chargePointId, string direction, int messageTypeId,
                      string? messageId, string? action, string payloadJson, string? resultCode = null);

        //Task LogAsync(OcppFrameLog entry, CancellationToken ct = default);
        //Task LogCorrelationAsync(string messageId, long latencyMs, CancellationToken ct = default);
    }
}