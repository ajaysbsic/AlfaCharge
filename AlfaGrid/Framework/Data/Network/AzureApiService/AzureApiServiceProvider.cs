using Refit;

namespace AlfaGrid.Framework.Data.Network.AzureApiService
{
    public class AzureApiServiceProvider
    {
        private AzureApiServiceProvider()
        {
        }

        private static readonly Lazy<AzureApiServiceProvider> lazy
            = new(() => new AzureApiServiceProvider());

        public static AzureApiServiceProvider Instance => lazy.Value;

        public IAzureApiService<Request, Response> GetRestService<Request, Response>()
        {
            return RestService.
                For<IAzureApiService<Request, Response>>(new HttpClientHelper().GetHttpClient());
        }
    }
}

