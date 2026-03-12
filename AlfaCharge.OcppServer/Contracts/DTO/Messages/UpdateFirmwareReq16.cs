namespace AlfaCharge.OcppServer.Contracts.DTO.Messages
{
    public class UpdateFirmwareReq16
    {
        public string Location { get; set; } = default!;
        public DateTimeOffset RetrieveDate { get; set; }
        public int? Retries { get; set; }
        public int? RetryInterval { get; set; }
    }
}