namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class Ocpp16StartTransactionRequest
    {
        public int ConnectorId { get; set; }
        public string IdTag { get; set; } = default!;
        public long MeterStart { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public int? ReservationId { get; set; }
    }
}