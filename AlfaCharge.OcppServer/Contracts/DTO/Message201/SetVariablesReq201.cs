namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class SetVariablesReq201
    {
        public List<SetVariableData201> SetVariableData { get; set; } = new();
    }
}