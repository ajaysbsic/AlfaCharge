namespace AlfaCharge.Domain.Entities
{
    public class OcppFrameLog
    {
        public long Id { get; set; }

        // Core identifiers
        public string ChargePointId { get; set; } = default!;
        public string Protocol { get; set; } = default!;      // "ocpp1.6" | "ocpp2.0.1"
        public int MessageTypeId { get; set; }                // 2 | 3 | 4
        public string? MessageId { get; set; }                // uniqueId if present
        public string? Action { get; set; }                   // for CALL / context

        // Direction and timing
        public string Direction { get; set; } = default!;     // "inbound" | "outbound"
        public DateTimeOffset TimestampUtc { get; set; }

        // Correlation & metrics
        public string? CorrelationId { get; set; }            // optional (guid or same as messageId)
        public long? LatencyMs { get; set; }                  // for CSMS CALLs once resolved

        // Payload & errors
        public string PayloadJson { get; set; } = "{}";       // raw (consider gzip if huge)
        public string? ErrorCode { get; set; }                // only for CALLERROR
        public string? ErrorDescription { get; set; }
        public string? ErrorDetailsJson { get; set; }

        // Extras useful for 2.x or reports
        public int? RequestSeqNo { get; set; }                // for NotifyReport seqNo
        public bool? Tbc { get; set; }                        // to be continued
        public int? RequestId { get; set; }                   // log/firmware/report correlation

        // Optional connection info for ops
        public string? ConnectionId { get; set; }             // your internal connection key
        public string? RemoteIp { get; set; }
    }
}