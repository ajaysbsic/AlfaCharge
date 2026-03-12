using AlfaCharge.Domain.Models;

namespace AlfaCharge.Domain.Entities
{
    public class OcppJob
    {
        public Guid Id { get; set; }
        public string ChargePointId { get; set; } = default!;
        public OcppJobType JobType { get; set; }
        public int? RequestId { get; set; }            // used by ocpp 2.x
        public string? Location { get; set; }          // firmware/log location
        public string? Checksum { get; set; }          // firmware checksum
        public int? Retries { get; set; }
        public int? RetryInterval { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? LastUpdatedAt { get; set; }
        public OcppJobStatus Status { get; set; }
        public string? StatusInfo { get; set; }
    }
}