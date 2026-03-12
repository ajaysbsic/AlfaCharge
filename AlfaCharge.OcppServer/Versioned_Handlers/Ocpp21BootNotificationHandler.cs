using AlfaCharge.Domain.Models;
using AlfaCharge.Domain.Models.WebSockets;
using AlfaCharge.Infrastructure.DB.Contracts;
using AlfaCharge.OcppServer.Contracts;

namespace AlfaCharge.OcppServer.Versioned_Handlers
{
    public class Ocpp21BootNotificationHandler : IBootNotificationHandler
    {
        private readonly IBootNotificationService _service;

        public Ocpp21BootNotificationHandler(IBootNotificationService service)
        {
            _service = service;
        }

        public async Task<BootNotificationResponse> HandleAsync(BootNotificationRequest request)
        {
            await _service.SaveAsync(request);
            // OCPP 2.1 might have extended attributes or different validation

            // Example: Additional validation or different response fields
            return new BootNotificationResponse
            {
                Status = NotificationStatus.Accepted,
                CurrentTime = DateTime.UtcNow,
                Interval = 600,  // maybe different default for 2.0.1
                StatusInfo = new StatusInfo
                {
                    ReasonCode = "OK",
                    AdditionalInfo = "v2.0.1"
                }
            };
        }
    }
}