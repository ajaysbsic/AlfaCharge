namespace AlfaCharge.OcppServer.Contracts.DTO.Messages
{
    public class DiagnosticsStatusNotificationReq16
    {
        public DiagnosticsStatus16 Status { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}