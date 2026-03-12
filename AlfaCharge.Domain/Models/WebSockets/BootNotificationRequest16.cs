namespace AlfaCharge.Domain.Models.WebSockets
{
    public sealed class BootNotificationRequest16
    {
        // Canonical 1.6 names (use whatever you already use in your project)
        public string chargePointVendor { get; set; }
        public string chargePointModel { get; set; }
        public string firmwareVersion { get; set; }
        public string chargeBoxSerialNumber { get; set; }
        public string meterSerialNumber { get; set; }
    }
}