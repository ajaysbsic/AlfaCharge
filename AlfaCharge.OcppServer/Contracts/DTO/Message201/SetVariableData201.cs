namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class SetVariableData201
    {
        public Component201 Component { get; set; } = default!;
        public Variable201 Variable { get; set; } = default!;
        public AttributeEnumType201? AttributeType { get; set; }
        public string AttributeValue { get; set; } = default!;
    }
}