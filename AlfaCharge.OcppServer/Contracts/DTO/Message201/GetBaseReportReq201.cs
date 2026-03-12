namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class GetBaseReportReq201
    {
        public int RequestId { get; set; }
        public ReportBaseEnumType201 ReportBase { get; set; }
    }
}