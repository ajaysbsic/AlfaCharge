namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class NotifyReportReq201
    {
        public int RequestId { get; set; }
        public DateTimeOffset GeneratedAt { get; set; }
        public ReportBaseEnumType201 ReportBase { get; set; }
        public int SeqNo { get; set; }
        public bool? Tbc { get; set; } // to be continued
        public List<ReportData201> ReportData { get; set; } = new();
    }
}