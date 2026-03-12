using AlfaGrid.Framework.FrameworkConfig;
using AlfaGrid.Source.AppConfiguration;
using AlfaGrid.Source.Handler;
using AlfaGrid.Source.Models;
using AlfaGrid.Source.Services;
using Microsoft.Maui.Platform;

namespace AlfaGrid
{
    public partial class App : Application
    {
        public static UserBasicInfo UserDetails;
        private readonly ILocalizationService _localizationService;
        private Window? _currentWindow;

        public App(ILocalizationService localizationService)
        {
            InitializeComponent();
            _localizationService = localizationService;
            _localizationService.LanguageChanged += (_, __) => ApplyFlowDirection();

#if __ANDROID__
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
            {
                if (view is BorderlessEntry)
                {
                    handler.PlatformView.SetBackgroundColor(Colors.Transparent.ToPlatform());
                }
            });
#elif __IOS__
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
            {
                if (view is BorderlessEntry)
                {
                    handler.PlatformView.BorderStyle = UIKit.UIKit.UITextBorderStyle.None;
                }
            });
#endif

            InitialiseConfiguration();
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            _currentWindow = window;
            ApplyFlowDirection();
            // Unsubscribe when the window is being destroyed (app closed or OS reclaims)
            window.Destroying += (_, __) =>
            {
                Connectivity.ConnectivityChanged -= OnConnectivityChanged;
                if (ReferenceEquals(_currentWindow, window))
                {
                    _currentWindow = null;
                }
            };

            // (Optional) Pause listening when app goes background
            window.Stopped += (_, __) =>
            {
                Connectivity.ConnectivityChanged -= OnConnectivityChanged;
            };

            // (Optional) Resume when back to foreground
            window.Activated += (_, __) =>
            {
                Connectivity.ConnectivityChanged -= OnConnectivityChanged; // ensure not double-subscribed
                Connectivity.ConnectivityChanged += OnConnectivityChanged;
                _currentWindow = window;
                ApplyFlowDirection();
            };
            return window;
        }

        private void ApplyFlowDirection()
        {
            if (_currentWindow is null)
            {
                return;
            }

            var flowDirection = _localizationService.FlowDirection;
            _currentWindow.Dispatcher.Dispatch(() =>
            {
                _currentWindow.FlowDirection = flowDirection;
                if (_currentWindow.Page is Page page)
                {
                    page.FlowDirection = flowDirection;
                }
            });
        }

        private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            // TODO: handle connectivity changes
        }

        private static void InitialiseConfiguration()
        {
            //Swirch envionrmnt here in the ENUM to repoint to different back end environments
            AppConfigurationManager.Instance.Init(AppEnvironmentEnum.DEVELOPMENT);

            IFrameworkConfig frameworkConfig = new FrameworkConfig
            {
                ApiBaseURL = AppConfigurationManager.Instance.Config.ApiBaseURL,
                DbCacheName = AppConfigurationManager.Instance.Config.DbCacheName,
                AdAuthRedirectUrl = AppConfigurationManager.Instance.Config.AdAuthRedirectUrl,
                Authority = AppConfigurationManager.Instance.Config.Authority,
                ClientId = AppConfigurationManager.Instance.Config.ClientId,
                TenantId = AppConfigurationManager.Instance.Config.TenantId,
                ClientSecret = AppConfigurationManager.Instance.Config.ClientSecret,
                SubscriptionKey = AppConfigurationManager.Instance.Config.SubscriptionKey,
                Token = AppConfigurationManager.Instance.Config.Token,
                Scopes = AppConfigurationManager.Instance.Config.Scopes,
                CodesignEntitlement = AppConfigurationManager.Instance.Config.CodesignEntitlement,
                GoogleAppId = AppConfigurationManager.Instance.Config.GoogleAppId,
                GSMSenderId = AppConfigurationManager.Instance.Config.GSMSenderId,
                IOSNotificationApiKey = AppConfigurationManager.Instance.Config.IOSNotificationApiKey,
                IOSNotificationClientId = AppConfigurationManager.Instance.Config.IOSNotificationClientId,
                FirebaseProjectId = AppConfigurationManager.Instance.Config.FirebaseProjectId
            };
            FrameworkConfigManager.Instance.Init(frameworkConfig);
        }
    }
}