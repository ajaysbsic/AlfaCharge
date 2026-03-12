namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class VariableAttribute201
    {
        public AttributeEnumType201? Type { get; set; }  // attributeType in spec
        public string? Value { get; set; }
        public MutabilityEnumType201? Mutability { get; set; }
        public bool? Persistent { get; set; }
        public bool? Constant { get; set; }
    }
}