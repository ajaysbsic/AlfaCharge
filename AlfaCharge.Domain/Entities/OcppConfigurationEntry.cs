namespace AlfaCharge.Domain.Entities
{
    public class OcppConfigurationEntry
    {
        public Guid Id { get; set; }
        public string ChargePointId { get; set; } = default!;
        public string Key { get; set; } = default!;
        public string? Value { get; set; }
        public bool Readonly { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}