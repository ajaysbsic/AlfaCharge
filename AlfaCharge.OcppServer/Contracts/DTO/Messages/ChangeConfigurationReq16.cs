namespace AlfaCharge.OcppServer.Contracts.DTO.Messages
{
    public class ChangeConfigurationReq16
    {
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;
    }
}