namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class Ocpp201TransactionEventResponse
    {
        // Keep minimal; spec supports totals/costs, etc.
        public int TotalCost { get; set; } = 0;
    }
}