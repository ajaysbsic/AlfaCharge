namespace AlfaCharge.Api.DTO
{
    public sealed class PingRequestDto
    {
        public string? VendorId { get; set; } = "AlphaCharge";
        public string? MessageId { get; set; } = "Ping";
        public object? Data { get; set; } // string or any JSON
        public int TimeoutSeconds { get; set; } = 15;
    }
}
