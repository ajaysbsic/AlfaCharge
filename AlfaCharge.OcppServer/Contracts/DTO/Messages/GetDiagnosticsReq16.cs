namespace AlfaCharge.OcppServer.Contracts.DTO.Messages
{

    public class GetDiagnosticsReq16
    {
        public string Location { get; set; } = default!;
        public int? Retries { get; set; }
        public int? RetryInterval { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? StopTime { get; set; }
    }
}