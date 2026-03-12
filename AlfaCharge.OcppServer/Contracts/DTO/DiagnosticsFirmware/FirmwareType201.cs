namespace AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware
{
    public class FirmwareType201
    {
        public Uri Location { get; set; } = default!;
        public DateTimeOffset RetrieveDateTime { get; set; }
        public DateTimeOffset? InstallDateTime { get; set; }
        public string? Checksum { get; set; }
    }
}