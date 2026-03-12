namespace AlfaGrid.Source.AppConfiguration.EnviornmentConfig
{
	public interface IConfig
	{
        string ApiBaseURL { get; }

        string DbCacheName { get; }

        string AppCenterKeyAndroid { get; }

        string AppCenterKeyIos { get; }

        string AdAuthRedirectUrl { get; }

        string Authority { get; }

        string[] Scopes { get; }

        string ClientId { get; }

        string TenantId { get; }

        string ClientSecret { get; }

        string SubscriptionKey { get; }

        string Token { get; set; }

        string CodesignEntitlement { get; }

        public string GoogleAppId { get; }

        public string GSMSenderId { get; }

        public string IOSNotificationApiKey { get; }

        public string IOSNotificationClientId { get; }

        public string FirebaseProjectId { get; }
    }
}