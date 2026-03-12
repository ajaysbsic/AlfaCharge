namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class Ocpp16StopTransactionRequest
    {
        public long MeterStop { get; set; }
        public int TransactionId { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string? Reason { get; set; }
        public object? TransactionData { get; set; }
    }
}