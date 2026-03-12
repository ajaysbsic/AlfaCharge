namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class GetVariablesReq201
    {
        public List<GetVariableData201> GetVariableData { get; set; } = new();
    }
}