using AlfaGrid.Framework.FrameworkConfig;

namespace AlfaGrid.Framework.Data.Network.AzureApiService
{
    public class HttpClientHelper
    {
        public HttpClientHelper()
        {
        }

        //TODO configuration to be checked
        public HttpClient GetHttpClient()
        {

#if DEBUG
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            HttpClient httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(FrameworkConfigManager.Instance.FrameworkConfig.ApiBaseURL),
            };
#else
         HttpClient httpClient = new HttpClient()
         {
             BaseAddress = new Uri(FrameworkConfigManager.Instance.FrameworkConfig.ApiBaseURL),
         };
#endif

            return httpClient;
        }
    }
}