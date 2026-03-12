using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Domain.Entities
{
    public class OCPPLog
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string ChargePointId { get; set; } = default!; // e.g., "CP123"

        [MaxLength(16)]
        public string Direction { get; set; } = "inbound"; // inbound|outbound

        /// <summary>2=CALL, 3=CALLRESULT, 4=CALLERROR</summary>
        public int MessageTypeId { get; set; }

        [MaxLength(64)]
        public string? MessageId { get; set; }

        [MaxLength(128)]
        public string? Action { get; set; } // e.g., StatusNotification

        public string PayloadJson { get; set; } = "{}";

        [MaxLength(64)]
        public string? ResultCode { get; set; } // ok/errorCode

        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}