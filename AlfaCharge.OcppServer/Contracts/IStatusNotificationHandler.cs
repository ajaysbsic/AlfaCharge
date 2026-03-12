using AlfaCharge.OcppServer.Contracts.DTO;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IStatusNotificationHandler
    {
        Task<object> Handle16Async(string chargePointId, Ocpp16StatusNotificationRequest req);
        Task<object> Handle201Async(string chargePointId, Ocpp201StatusNotificationRequest req);
    }
}