namespace AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware
{
    public class LogParameters201
    {
        public Uri RemoteLocation { get; set; } = default!;
        public int? Retries { get; set; }
        public int? RetryInterval { get; set; }
    }
}