using System;
namespace AlfaGrid.Framework.FrameworkConfig
{
	public class FrameworkConfig : IFrameworkConfig
    {
        public string ApiBaseURL { get; set; }

        public string DbCacheName { get; set; }

        public string AdAuthRedirectUrl { get; set; }

        public string Authority { get; set; }

        public string[] Scopes { get; set; }

        public string ClientId { get; set; }

        public string TenantId { get; set; }

        public string ClientSecret { get; set; }

        public string SubscriptionKey { get; set; }

        public string Token { get; set; }

        public string CodesignEntitlement { get; set; }

        public string GoogleAppId { get; set; }

        public string GSMSenderId { get; set; }

        public string IOSNotificationApiKey { get; set; }

        public string IOSNotificationClientId { get; set; }

        public string FirebaseProjectId { get; set; }
    }
}