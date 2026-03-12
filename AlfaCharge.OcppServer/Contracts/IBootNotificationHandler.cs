using AlfaCharge.Domain.Models.WebSockets;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IBootNotificationHandler
    {
        Task<BootNotificationResponse> HandleAsync(BootNotificationRequest request);
    }
}
