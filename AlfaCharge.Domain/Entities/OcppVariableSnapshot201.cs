namespace AlfaCharge.Domain.Entities
{
    public class OcppVariableSnapshot201
    {
        public Guid Id { get; set; }
        public string ChargePointId { get; set; } = default!;
        public string Component { get; set; } = default!;
        public string? ComponentInstance { get; set; }
        public string Variable { get; set; } = default!;
        public string? VariableInstance { get; set; }
        public string? AttributeType { get; set; }
        public string? Value { get; set; }
        public string? Mutability { get; set; }
        public bool? Persistent { get; set; }
        public bool? Constant { get; set; }
        public DateTimeOffset SnapshotAt { get; set; }
    }
}