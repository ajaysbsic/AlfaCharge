namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class Ocpp16MeterValuesRequest
    {
        public int ConnectorId { get; set; }
        public int? TransactionId { get; set; }
        public List<MeterValue> MeterValue { get; set; } = new();
    }

    public class Ocpp16MeterValuesResponse { }
}