namespace AlfaCharge.OcppServer.Contracts.DTO.Messages
{
    public class GetConfigurationConf16
    {
        public List<ConfigurationKey16>? ConfigurationKey { get; set; }
        public List<string>? UnknownKey { get; set; }
    }
}