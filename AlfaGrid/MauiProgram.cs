#if ANDROID
using AlfaGrid.Platforms.Android.Handlers;
using Microsoft.Maui.Maps;
#endif
#if IOS
using AlfaGrid.Platforms.iOS.Handlers;
#endif
using AlfaGrid.Source.Services;
using AlfaGrid.Source.View;
using AlfaGrid.Source.ViewModel;
using AlfaGrid.Source.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;

namespace AlfaGrid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                })
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    // Register custom map handler for Android to support custom pin icons
                    handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
#endif
                });

            // Register services
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<IChargingLocationService, ChargingLocationService>();
            builder.Services.AddSingleton<IFilterService, FilterService>();
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

            // Register viewmodels and pages used by DI
            builder.Services.AddTransient<LoadingPageViewModel>();
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<HomePage>();

            // Register registration page and viewmodel
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();

            // Register location list page and viewmodel
            builder.Services.AddTransient<LocationListPageViewModel>();
            builder.Services.AddTransient<LocationListPage>();

            // Register location details page and viewmodel
            builder.Services.AddTransient<LocationDetailsPageViewModel>();
            builder.Services.AddTransient<LocationDetailsPage>();

            // Register QR Scanner page and viewmodel
            builder.Services.AddTransient<QRScannerPageViewModel>();
            builder.Services.AddTransient<QRScannerPage>();

            // Register Add Card Details page and viewmodel
            builder.Services.AddTransient<AddCardDetailsPageViewModel>();
            builder.Services.AddTransient<AddCardDetailsPage>();

            // Register Filter page and viewmodel
            builder.Services.AddTransient<FilterPageViewModel>();
            builder.Services.AddTransient<FilterPage>();

            // Register Profile page and viewmodel
            builder.Services.AddTransient<ProfilePageViewModel>();
            builder.Services.AddTransient<ProfilePage>();

            // Register My Charging Profile page and viewmodel
            builder.Services.AddTransient<MyChargingProfilePageViewModel>();
            builder.Services.AddTransient<MyChargingProfilePage>();

            // Register Reservations page and viewmodel
            builder.Services.AddTransient<ReservationsPageViewModel>();
            builder.Services.AddTransient<ReservationsPage>();

            // Register Settings page and viewmodel
            builder.Services.AddTransient<SettingsPageViewModel>();
            builder.Services.AddTransient<SettingsPage>();

            // Register Charging Session page and viewmodel
            builder.Services.AddTransient<ChargingSessionPageViewModel>();
            builder.Services.AddTransient<ChargingSessionPage>();

            // Register Session Details page and viewmodel
            builder.Services.AddTransient<SessionDetailsPageViewModel>();
            builder.Services.AddTransient<SessionDetailsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            ServiceHelper.Initialize(app.Services);

            return app;
        }
    }
}
