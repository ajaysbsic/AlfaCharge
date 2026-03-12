namespace AlfaGrid.Source.AppConfiguration.EnviornmentConfig
{
    public class Qa : IConfig
    {
        public string ApiBaseURL => ConfigEnvReader.Get("ALFAGRID_QA_API_BASE_URL", "https://apibseurl/api/");

        public string AppCenterKeyAndroid => ConfigEnvReader.Get("ALFAGRID_QA_APPCENTER_ANDROID", string.Empty);

        public string AppCenterKeyIos => ConfigEnvReader.Get("ALFAGRID_QA_APPCENTER_IOS", string.Empty);

        public string DbCacheName => "cache_mth_q";

        public string AdAuthRedirectUrl => ConfigEnvReader.Get("ALFAGRID_QA_AD_AUTH_REDIRECT", "mtshomedeliveryappqa://auth");

        public string Authority => ConfigEnvReader.Get("ALFAGRID_QA_AUTHORITY", "https://login.microsoftonline.com/74b72ba8-5684-402c-98da-e38799398d7d/saml2");

        public string[] Scopes => ConfigEnvReader.GetCsv("ALFAGRID_QA_SCOPES", "https://mbaas-api-qa.clarios.com/lth-hds-09-api//user_impersonation");

        public string ClientId => ConfigEnvReader.Get("ALFAGRID_QA_CLIENT_ID", string.Empty);

        public string TenantId => ConfigEnvReader.Get("ALFAGRID_QA_TENANT_ID", string.Empty);

        public string ClientSecret => ConfigEnvReader.Get("ALFAGRID_QA_CLIENT_SECRET", string.Empty);

        public string SubscriptionKey => ConfigEnvReader.Get("ALFAGRID_QA_SUBSCRIPTION_KEY", string.Empty);

        public string Token { get; set; } = string.Empty;

        public string CodesignEntitlement => ConfigEnvReader.Get("ALFAGRID_QA_CODESIGN_ENTITLEMENT", "com.clarios.mts.homedelivery.ios.qa");

        public string GoogleAppId => ConfigEnvReader.Get("ALFAGRID_QA_GOOGLE_APP_ID", string.Empty);

        public string GSMSenderId => ConfigEnvReader.Get("ALFAGRID_QA_GSM_SENDER_ID", string.Empty);

        public string IOSNotificationApiKey => ConfigEnvReader.Get("ALFAGRID_QA_IOS_NOTIFICATION_API_KEY", string.Empty);

        public string IOSNotificationClientId => ConfigEnvReader.Get("ALFAGRID_QA_IOS_NOTIFICATION_CLIENT_ID", string.Empty);

        public string FirebaseProjectId => ConfigEnvReader.Get("ALFAGRID_QA_FIREBASE_PROJECT_ID", "com-clarios-mts-homedelivery");
    }
}