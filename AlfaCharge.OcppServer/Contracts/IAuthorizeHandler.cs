using AlfaCharge.Domain.Models.WebSockets;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IAuthorizeHandler
    {
        Task<AuthorizeResponse> HandleAsync(AuthorizeRequest request);
    }
}