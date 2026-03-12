using AlfaGrid.Source.Helpers;
using AlfaGrid.Source.Messages;
using AlfaGrid.Source.Services;
using AlfaGrid.Source.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AlfaGrid.Source.ViewModel
{
    public partial class AppShellViewModel : BaseViewModel, IRecipient<LanguageChangedMessage>
    {
        private readonly ILocalizationService _localizationService;

        [ObservableProperty]
        private string menuHome;

        [ObservableProperty]
        private string menuMyChargingProfile;

        [ObservableProperty]
        private string menuReservations;

        [ObservableProperty]
        private string menuSettings;

        [ObservableProperty]
        private string settingsLogout;

        public AppShellViewModel()
        {
            // Register for language change messages
            WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this);
            
            // Get localization service from ServiceHelper
            _localizationService = ServiceHelper.GetRequiredService<ILocalizationService>();
            
            // Initialize menu text
            UpdateMenuText();
        }

        private void UpdateMenuText()
        {
            MenuHome = _localizationService.GetString("Menu_Home");
            MenuMyChargingProfile = _localizationService.GetString("Menu_MyChargingProfile");
            MenuReservations = _localizationService.GetString("Menu_Reservations");
            MenuSettings = _localizationService.GetString("Menu_Settings");
            SettingsLogout = _localizationService.GetString("Settings_Logout");
        }

        /// <summary>
        /// Handles language change messages to refresh the flyout menu
        /// </summary>
        public void Receive(LanguageChangedMessage message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Update all menu text
                UpdateMenuText();
                
                // Force AppShell to update its FlowDirection
                if (Shell.Current != null)
                {
                    Shell.Current.FlowDirection = _localizationService.FlowDirection;
                    
                    // Force flyout to refresh by toggling presentation
                    // This ensures all menu items get their translations updated
                    var wasPresented = Shell.Current.FlyoutIsPresented;
                    if (wasPresented)
                    {
                        Shell.Current.FlyoutIsPresented = false;
                    }
                    
                    // Trigger property changed for all menu items
                    OnPropertyChanged(nameof(MenuHome));
                    OnPropertyChanged(nameof(MenuMyChargingProfile));
                    OnPropertyChanged(nameof(MenuReservations));
                    OnPropertyChanged(nameof(MenuSettings));
                    OnPropertyChanged(nameof(SettingsLogout));
                }
            });
        }

        [RelayCommand]
        async Task NavigateToProfile()
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage));
            Shell.Current.FlyoutIsPresented = false;
        }

        [RelayCommand]
        async Task NavigateToMyChargingProfile()
        {
            await Shell.Current.GoToAsync(nameof(MyChargingProfilePage));
            Shell.Current.FlyoutIsPresented = false;
        }

        [RelayCommand]
        async Task NavigateToReservations()
        {
            await Shell.Current.GoToAsync(nameof(ReservationsPage));
            Shell.Current.FlyoutIsPresented = false;
        }

        [RelayCommand]
        async Task NavigateToSettings()
        {
            await Shell.Current.GoToAsync(nameof(SettingsPage));
            Shell.Current.FlyoutIsPresented = false;
        }

        [RelayCommand]
        async void Logout()
        {
            if (Preferences.ContainsKey(nameof(App.UserDetails)))
            {
                Preferences.Remove(nameof(App.UserDetails));
            }
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}