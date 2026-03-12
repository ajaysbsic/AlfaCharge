namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class ReportData201
    {
        public Component201 Component { get; set; } = default!;
        public Variable201 Variable { get; set; } = default!;
        public List<VariableAttribute201> VariableAttribute { get; set; } = new();
    }
}