using AlfaCharge.Domain.Models;
using AlfaCharge.Domain.Models.WebSockets;
using AlfaCharge.OcppServer.Contracts;

namespace AlfaCharge.OcppServer.Versioned_Handlers
{
    public class Ocpp16AuthorizeHandler : IAuthorizeHandler
    {
        public Task<AuthorizeResponse> HandleAsync(AuthorizeRequest request)
        {
            // Replace with actual authorization logic
            return Task.FromResult(new AuthorizeResponse
            {
                IdTagInfo = new IdTagInfo
                {
                    Status = IdTagStatus.Accepted,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    ParentIdTag = null
                }
            });
        }
    }
}