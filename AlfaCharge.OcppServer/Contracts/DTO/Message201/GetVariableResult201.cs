namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{

    public class GetVariableResult201
    {
        public AttributeStatusEnumType201 AttributeStatus { get; set; }
        public Component201 Component { get; set; } = default!;
        public Variable201 Variable { get; set; } = default!;
        public AttributeEnumType201? AttributeType { get; set; }
        public string? AttributeValue { get; set; }
        public MutabilityEnumType201? Mutability { get; set; }
        public bool? Persistent { get; set; }
        public bool? Constant { get; set; }
    }
}