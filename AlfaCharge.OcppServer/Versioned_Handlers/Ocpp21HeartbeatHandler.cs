using AlfaCharge.Domain.Models.WebSockets;
using AlfaCharge.OcppServer.Contracts;

namespace AlfaCharge.OcppServer.Versioned_Handlers
{
    public class Ocpp21HeartbeatHandler : IHeartbeatHandler
    {
        public Task<HeartbeatResponse> HandleAsync()
        {
            // OCPP 2.1 specific logic
            return Task.FromResult(new HeartbeatResponse
            {
                CurrentTime = DateTime.UtcNow
            });
        }
    }
}