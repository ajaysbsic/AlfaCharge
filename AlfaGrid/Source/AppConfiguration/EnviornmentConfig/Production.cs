namespace AlfaGrid.Source.AppConfiguration.EnviornmentConfig
{
	public class Production : IConfig
    {
        public string ApiBaseURL => ConfigEnvReader.Get("ALFAGRID_PROD_API_BASE_URL", string.Empty);

        public string AppCenterKeyAndroid => ConfigEnvReader.Get("ALFAGRID_PROD_APPCENTER_ANDROID", string.Empty);

        public string AppCenterKeyIos => ConfigEnvReader.Get("ALFAGRID_PROD_APPCENTER_IOS", string.Empty);

        public string DbCacheName => "cache_mth";

        public string AdAuthRedirectUrl => ConfigEnvReader.Get("ALFAGRID_PROD_AD_AUTH_REDIRECT", string.Empty);

        public string Authority => ConfigEnvReader.Get("ALFAGRID_PROD_AUTHORITY", string.Empty);

        public string[] Scopes => ConfigEnvReader.GetCsv("ALFAGRID_PROD_SCOPES", string.Empty);

        public string ClientId => ConfigEnvReader.Get("ALFAGRID_PROD_CLIENT_ID", string.Empty);

        public string TenantId => ConfigEnvReader.Get("ALFAGRID_PROD_TENANT_ID", string.Empty);

        public string ClientSecret => ConfigEnvReader.Get("ALFAGRID_PROD_CLIENT_SECRET", string.Empty);

        public string SubscriptionKey => ConfigEnvReader.Get("ALFAGRID_PROD_SUBSCRIPTION_KEY", string.Empty);

        public string Token { get; set; } = string.Empty;

        public string CodesignEntitlement => ConfigEnvReader.Get("ALFAGRID_PROD_CODESIGN_ENTITLEMENT", string.Empty);

        public string GoogleAppId => ConfigEnvReader.Get("ALFAGRID_PROD_GOOGLE_APP_ID", string.Empty);

        public string GSMSenderId => ConfigEnvReader.Get("ALFAGRID_PROD_GSM_SENDER_ID", string.Empty);

        public string IOSNotificationApiKey => ConfigEnvReader.Get("ALFAGRID_PROD_IOS_NOTIFICATION_API_KEY", string.Empty);

        public string IOSNotificationClientId => ConfigEnvReader.Get("ALFAGRID_PROD_IOS_NOTIFICATION_CLIENT_ID", string.Empty);

        public string FirebaseProjectId => ConfigEnvReader.Get("ALFAGRID_PROD_FIREBASE_PROJECT_ID", "com-clarios-mts-homedelivery");
    }
}