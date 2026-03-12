namespace AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware
{
    public class GetLogReq201
    {
        public int RequestId { get; set; }
        public LogEnumType201 LogType { get; set; }
        public LogParameters201 Log { get; set; } = default!;
        public DateTimeOffset? OldestTimestamp { get; set; }
        public DateTimeOffset? LatestTimestamp { get; set; }
    }
}