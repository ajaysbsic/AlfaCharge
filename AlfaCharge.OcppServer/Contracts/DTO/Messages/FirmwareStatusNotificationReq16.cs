namespace AlfaCharge.OcppServer.Contracts.DTO.Messages
{
    public class FirmwareStatusNotificationReq16
    {
        public FirmwareStatus16 Status { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}