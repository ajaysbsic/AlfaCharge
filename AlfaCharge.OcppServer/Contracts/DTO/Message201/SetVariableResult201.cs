namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class SetVariableResult201
    {
        public AttributeStatusEnumType201 AttributeStatus { get; set; }
        public Component201 Component { get; set; } = default!;
        public Variable201 Variable { get; set; } = default!;
        public AttributeEnumType201? AttributeType { get; set; }
        public string? AttributeStatusInfo { get; set; }
    }
}