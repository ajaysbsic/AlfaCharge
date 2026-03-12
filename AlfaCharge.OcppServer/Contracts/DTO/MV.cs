namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class MV
    {
        public DateTimeOffset Timestamp { get; set; }
        public List<SV> SampledValue { get; set; } = new();
    }
}