namespace AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware
{
    public class UpdateFirmwareReq201
    {
        public int RequestId { get; set; }
        public FirmwareType201 Firmware { get; set; } = default!;
        public int? Retries { get; set; }
        public int? RetryInterval { get; set; }
    }
}