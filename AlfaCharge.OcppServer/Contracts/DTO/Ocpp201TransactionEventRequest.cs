namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class Ocpp201TransactionEventRequest
    {
        public string EventType { get; set; } = default!; // Started/Updated/Ended
        public DateTimeOffset Timestamp { get; set; }
        public string TriggerReason { get; set; } = "ChargingStateChanged";
        public int SeqNo { get; set; }

        public TransactionInfo TransactionInfo { get; set; } = new();
        public List<MV> MeterValue { get; set; } = new();

    }
}