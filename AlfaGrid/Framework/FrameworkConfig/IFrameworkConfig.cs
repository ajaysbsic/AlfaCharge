namespace AlfaGrid.Framework.FrameworkConfig
{
	public interface IFrameworkConfig
	{
        string ApiBaseURL { set;  get; }

        string DbCacheName { set;  get; }

        string AdAuthRedirectUrl { set;  get; }

        string Authority { set;  get; }

        string[] Scopes { set;  get; }

        string ClientId { set;  get; }

        string TenantId { set;  get; }

        string ClientSecret { set;  get; }

        string SubscriptionKey { set;  get; }

        string Token { get; set; }

        string CodesignEntitlement { get; set; }

        public string GoogleAppId { get; set; }

        public string GSMSenderId { get; set; }

        public string IOSNotificationApiKey { get; set; }

        public string IOSNotificationClientId { get; set; }

        public string FirebaseProjectId { get; set; }

    }
}