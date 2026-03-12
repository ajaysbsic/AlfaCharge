using AlfaCharge.OcppServer.Contracts.DTO;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IOcpp201TransactionHandler
    {
        Task<Ocpp201TransactionEventResponse> HandleEventAsync(string cpId, Ocpp201TransactionEventRequest req);
    }
}