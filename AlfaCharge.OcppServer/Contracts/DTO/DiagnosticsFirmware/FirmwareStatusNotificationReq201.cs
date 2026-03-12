namespace AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware
{
    public class FirmwareStatusNotificationReq201
    {
        public int RequestId { get; set; }
        public FirmwareStatusEnumType201 Status { get; set; }
    }
}