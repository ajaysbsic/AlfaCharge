namespace AlfaGrid.Framework.Domain.Repository
{
    public interface IRepository<Request, Response>
    {

        public Task<Response> GetAll(string controller, string method);

        public Task<Response> GetById(string controller, string method, int id);

        public Task<Response> Create(string controller, string method, Request item);

        public Task<Response> Update(string controller, string method, string id, Request item);

        public Task<Response> Delete(string controller, string method, string id);

        public Task<Response> GetByQueryString(string controller, object QueryParam);

        public Task<Response> GetByQueryStringMethod(string controller, string method, object QueryParam);

        public Task<Response> GetByQueryStringValue(string controller, string method, string QueryParam);

        public Task<HttpResponseMessage> DownloadPhoto(string controller, string method, string id);

        public Task<Response> GetByQueryStringByValue(string controller);

        public Task<Response> CreateByQueryString(string controller, string method);
    }
}