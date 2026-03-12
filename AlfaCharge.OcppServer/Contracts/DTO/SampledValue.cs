namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class SampledValue
    {
        public string Value { get; set; } = default!; // numeric string
        public string? Measurand { get; set; } // default Energy.Active.Import.Register
        public string? Unit { get; set; }      // Wh by default on many devices
        public string? Context { get; set; }   // Sample.Periodic, Transaction.Begin, etc.
        public string? Location { get; set; }
        public string? Phase { get; set; }
        public string? Format { get; set; }
    }
}