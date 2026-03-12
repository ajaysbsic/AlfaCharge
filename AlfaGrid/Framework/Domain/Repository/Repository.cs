using AlfaGrid.Framework.Data.Network.AzureApiService;
using AlfaGrid.Framework.FrameworkConfig;
using AlfaGrid.Source.AppPreferences;

namespace AlfaGrid.Framework.Domain.Repository
{
    public class Repository<Request, Response> : IRepository<Request, Response>
    {

        private IAzureApiService<Request, Response> azureApiService;
        private string isDemo = AppPreferenceManager.Instance.GetBoolean(AppPreferenceKeyEnum.IS_DEMO).ToString().ToLower();

        public Repository()
        {
            azureApiService = AzureApiServiceProvider.Instance.GetRestService<Request, Response>();
        }

        public async Task<Response> Create(string controller, string method, Request item)
        {
            return await azureApiService.Create(controller, method, item,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<Response> Update(string controller, string method, string id, Request item)
        {
            return await azureApiService.Update(controller, method, id, item,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<Response> Delete(string controller, string method, string id)
        {
            return await azureApiService.Delete(controller, method, id,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<Response> GetAll(string controller, string method)
        {
            return await azureApiService.GetAll(controller, method,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<Response> GetById(string controller, string method, int id)
        {
            return await azureApiService.GetById(controller, method, id,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<Response> GetByQueryString(string controller, object QueryParam)
        {
            return await azureApiService.GetByQueryString(controller, QueryParam,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<Response> GetByQueryStringMethod(string controller, string method, object QueryParam)
        {
            return await azureApiService.GetByQueryStringMethod(controller, method, QueryParam,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<Response> GetByQueryStringValue(string controller, string method, string QueryParam)
        {
            return await azureApiService.GetByQueryStringValue(controller, method, QueryParam,
                    FrameworkConfigManager.Instance.FrameworkConfig.Token,
                    FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<HttpResponseMessage> DownloadPhoto(string controller, string method, string id)
        {
            return await azureApiService.DownloadPhoto(controller, method, id,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<Response> GetByQueryStringByValue(string controller)
        {
            return await azureApiService.GetByQueryStringByValue(controller,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }

        public async Task<Response> CreateByQueryString(string controller, string method)
        {
            return await azureApiService.CreateByQueryString(controller, method,
                FrameworkConfigManager.Instance.FrameworkConfig.Token,
                FrameworkConfigManager.Instance.FrameworkConfig.SubscriptionKey,
                isDemo);
        }
    }
}