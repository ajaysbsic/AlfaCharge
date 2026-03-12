namespace AlfaCharge.Domain.Models.WebSockets
{
    public class ChargePointBootNotification
    {
        public int Id { get; set; }
        public string? ChargePointVendor { get; set; }
        public string? ChargePointModel { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string? SerialNumber { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? Reason { get; set; }
    }
}