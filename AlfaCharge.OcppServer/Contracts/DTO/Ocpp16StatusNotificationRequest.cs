using AlfaCharge.Domain.Models;

namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class Ocpp16StatusNotificationRequest
    {
        public int ConnectorId { get; set; }
        public ConnectorStatus Status { get; set; } = default!;      // Available|Charging|Faulted|...
        public string ErrorCode { get; set; } = "NoError";  // per 1.6 enum
        public string? Info { get; set; }
        public string? VendorId { get; set; }
        public string? VendorErrorCode { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }

    public class Ocpp16StatusNotificationResponse { }
}