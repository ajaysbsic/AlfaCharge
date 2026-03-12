namespace AlfaCharge.OcppServer.Contracts.DTO.Messages
{
    public class ConfigurationKey16
    {
        public string Key { get; set; } = default!;
        public string? Value { get; set; }
        public bool Readonly { get; set; }
    }
}