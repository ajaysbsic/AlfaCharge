namespace AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware
{
    public class LogStatusNotificationReq201
    {
        public int RequestId { get; set; }
        public UploadLogStatusEnumType201 Status { get; set; }
    }
}