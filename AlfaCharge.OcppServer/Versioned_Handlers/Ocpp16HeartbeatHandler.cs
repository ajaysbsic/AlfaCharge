using AlfaCharge.Domain.Models.WebSockets;
using AlfaCharge.OcppServer.Contracts;

namespace AlfaCharge.OcppServer.Versioned_Handlers
{
    public class Ocpp16HeartbeatHandler : IHeartbeatHandler
    {
        public Task<HeartbeatResponse> HandleAsync()
        {
            // OCPP 1.6 specific logic
            return Task.FromResult(new HeartbeatResponse
            {
                CurrentTime = DateTime.UtcNow
            });
        }
    }
}
