using AlfaCharge.Domain.Models;

namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class IdTagInfo
    {
        public IdTagStatus Status { get; set; } = IdTagStatus.Accepted; // Accepted|Blocked|Expired|Invalid|ConcurrentTx
        public DateTimeOffset? ExpiryDate { get; set; }
        public string? ParentIdTag { get; set; }
    }
}