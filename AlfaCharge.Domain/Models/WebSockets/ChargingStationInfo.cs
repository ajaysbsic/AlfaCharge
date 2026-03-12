namespace AlfaCharge.Domain.Models.WebSockets
{
    public class ChargingStationInfo
    {
        public string Model { get; set; }
        public string VendorName { get; set; }
        public string SerialNumber { get; set; }
        public string FirmwareVersion { get; set; }
        // Additional optional fields for 2.0.1 (ICCID, IMSI, modem etc.)
    }
}