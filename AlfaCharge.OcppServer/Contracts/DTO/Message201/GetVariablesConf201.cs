namespace AlfaCharge.OcppServer.Contracts.DTO.Message201
{
    public class GetVariablesConf201
    {
        public List<GetVariableResult201> GetVariableResult { get; set; } = new();
    }
}