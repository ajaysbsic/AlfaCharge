using AlfaCharge.Domain.Models.WebSockets;

namespace AlfaCharge.Infrastructure.DB.Contracts
{
    public interface IBootNotificationService
    {
        Task<ChargePointBootNotification> SaveAsync(BootNotificationRequest request);
    }
}