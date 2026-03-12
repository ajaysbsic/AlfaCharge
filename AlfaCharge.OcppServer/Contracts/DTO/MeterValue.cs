namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class MeterValue
    {
        public DateTimeOffset Timestamp { get; set; }
        public List<SampledValue> SampledValue { get; set; } = new();
    }
}