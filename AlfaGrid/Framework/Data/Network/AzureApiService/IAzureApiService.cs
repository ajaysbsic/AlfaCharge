using Refit;

namespace AlfaGrid.Framework.Data.Network.AzureApiService
{
    /**
     * T represents type of the entiry
     * {controller} placeholder represents the specific controller endpoint
     * */
    public interface IAzureApiService<Request, Response>
    {
        [Get("/{controller}/{method}")]
        Task<Response> GetAll(string controller, string method,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Get("/{controller}/{method}/id")]
        Task<Response> GetById(string controller, string method, int id,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Post("/{controller}/{method}")]
        Task<Response> Create(string controller, string method, [Body] Request item,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Put("/{controller}/{method}/{id}")]
        Task<Response> Update(string controller, string method, string id, [Body] Request item,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Delete("/{controller}/{method}/{id}")]
        Task<Response> Delete(string controller, string method, string id,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Get("/{controller}")]
        Task<Response> GetByQueryString(string controller, object QueryParam,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Get("/{controller}/{method}")]
        Task<Response> GetByQueryStringMethod(string controller, string method, object QueryParam,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Get("/{controller}/{method}/{id}")]
        Task<Response> GetByQueryStringValue(string controller, string method, string id,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Get("/{controller}/{method}/{id}")]
        Task<HttpResponseMessage> DownloadPhoto(string controller, string method, string id,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Get("/{controller}")]
        Task<Response> GetByQueryStringByValue(string controller,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);

        [Post("/{controller}/{method}")]
        Task<Response> CreateByQueryString(string controller, string method,
            [Header("Authorization")] string token, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
            [Header("IsDemo")] string IsDemo);
    }
}