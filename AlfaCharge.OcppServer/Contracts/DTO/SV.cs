namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class SV
    {
        public string Value { get; set; } = default!;
        public string? Measurand { get; set; }
        public string? UnitOfMeasure { get; set; } // e.g., Wh, kWh, W
    }
}