using AlfaGrid.Source.View;
using AlfaGrid.Source.ViewModel;

namespace AlfaGrid
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            this.BindingContext = new AppShellViewModel();
            
            // Register modal routes (not in flyout)
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(LocationListPage), typeof(LocationListPage));
            Routing.RegisterRoute(nameof(LocationDetailsPage), typeof(LocationDetailsPage));
            Routing.RegisterRoute(nameof(QRScannerPage), typeof(QRScannerPage));
            Routing.RegisterRoute(nameof(AddCardDetailsPage), typeof(AddCardDetailsPage));
            Routing.RegisterRoute(nameof(FilterPage), typeof(FilterPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(ReservationsPage), typeof(ReservationsPage));
            Routing.RegisterRoute(nameof(MyChargingProfilePage), typeof(MyChargingProfilePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(ChargingSessionPage), typeof(ChargingSessionPage));
            Routing.RegisterRoute(nameof(SessionDetailsPage), typeof(SessionDetailsPage));
        }

        protected override bool OnBackButtonPressed()
        {
            // consume back if you want to prevent navigating back into disposed contexts
            // return true; // prevents default behavior
            return base.OnBackButtonPressed();
        }
    }
}
