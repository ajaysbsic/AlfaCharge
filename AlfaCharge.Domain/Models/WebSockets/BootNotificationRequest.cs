namespace AlfaCharge.Domain.Models.WebSockets
{
    public class BootNotificationRequest
    {
        // For 1.6: vendor/model etc.
        // For 2.0.1: nested “chargingStation” object
        public string? Reason { get; set; }
        public ChargingStationInfo ChargingStation { get; set; } = new();
    }
}