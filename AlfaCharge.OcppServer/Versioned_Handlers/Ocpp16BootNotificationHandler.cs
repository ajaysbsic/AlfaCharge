using AlfaCharge.Domain.Models;
using AlfaCharge.Domain.Models.WebSockets;
using AlfaCharge.Infrastructure.DB.Contracts;
using AlfaCharge.OcppServer.Contracts;

namespace AlfaCharge.OcppServer.Versioned_Handlers
{
    public class Ocpp16BootNotificationHandler : IBootNotificationHandler
    {
        private readonly IBootNotificationService _service;

        public Ocpp16BootNotificationHandler(IBootNotificationService service)
        {
            _service = service;
        }

        public async Task<BootNotificationResponse> HandleAsync(BootNotificationRequest request)
        {
            // OCPP 1.6 specific logic
            await _service.SaveAsync(request);

            return new BootNotificationResponse
            {
                CurrentTime = DateTime.UtcNow,
                Interval = 300,
                Status = NotificationStatus.Accepted
            };
        }
    }
}