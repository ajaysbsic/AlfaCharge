using AlfaCharge.Domain.Models.WebSockets;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IHeartbeatHandler
    {
        Task<HeartbeatResponse> HandleAsync();
    }
}