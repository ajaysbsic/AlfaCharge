using AlfaCharge.OcppServer.Contracts.DTO;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IOcpp16TransactionHandler
    {
        Task<Ocpp16StartTransactionResponse> HandleStartAsync(string cpId, Ocpp16StartTransactionRequest req);
        Task<Ocpp16StopTransactionResponse> HandleStopAsync(string cpId, Ocpp16StopTransactionRequest req);
        Task<Ocpp16MeterValuesResponse> HandleMeterValuesAsync(string cpId, Ocpp16MeterValuesRequest req);
    }
}