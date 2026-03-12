namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class TransactionInfo
    {
        public string TransactionId { get; set; } = default!;
        public string? ChargingState { get; set; }  // Charging/EVConnected/SuspendedEV/…
        public bool? Stopped { get; set; }
        public string? StoppedReason { get; set; }  // DeAuthorized/Local/Remote/PowerLoss…
    }
}