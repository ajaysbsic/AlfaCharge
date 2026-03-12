namespace AlfaCharge.Domain.Models.WebSockets
{
    public class IdTagInfo
    {
        public IdTagStatus Status { get; set; } = IdTagStatus.Accepted;
        public DateTime? ExpiryDate { get; set; }
        public string ParentIdTag { get; set; }
    }
}