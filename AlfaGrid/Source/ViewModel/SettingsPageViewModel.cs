using AlfaGrid.Source.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlfaGrid.Source.ViewModel
{
    public partial class SettingsPageViewModel : BaseViewModel
    {
        private readonly ILocalizationService _localizationService;

        [ObservableProperty]
        private string selectedLanguage;

        [ObservableProperty]
        private bool isPushNotificationsEnabled;

        [ObservableProperty]
        private bool isEmailNotificationsEnabled;

        public string SelectedLanguageDisplay => SelectedLanguage == "ar" ? "العربية" : "English";

        public SettingsPageViewModel(ILocalizationService localizationService)
        {
            Title = localizationService.GetString("Settings_Title");
            _localizationService = localizationService;
            
            // Load current language
            SelectedLanguage = _localizationService.CurrentLanguage;
            _localizationService.LanguageChanged += (_, __) =>
            {
                // Ensure loader is hidden
                IsBusy = false;
                
                SelectedLanguage = _localizationService.CurrentLanguage;
                OnPropertyChanged(nameof(SelectedLanguageDisplay));

                if (Shell.Current.CurrentPage is Page page)
                {
                    page.FlowDirection = _localizationService.FlowDirection;
                    page.BindingContext = null;
                    page.BindingContext = this; // forces Translate bindings to re-evaluate
                }
            };
            
            // Load notification preferences
            IsPushNotificationsEnabled = Preferences.Get("push_notifications", true);
            IsEmailNotificationsEnabled = Preferences.Get("email_notifications", true);
        }

        [RelayCommand]
        private async Task SelectLanguage()
        {
            var english = "English";
            var arabic = "العربية";

            var action = await Shell.Current.DisplayActionSheetAsync(
                _localizationService.GetString("SelectLanguage"),
                _localizationService.GetString("Cancel"),
                null,
                english,
                arabic);

            if (action == english)
            {
                await ChangeLanguageAsync("en");
            }
            else if (action == arabic)
            {
                await ChangeLanguageAsync("ar");
            }
        }

        private void ChangeLanguage(string language)
        {
            if (SelectedLanguage != language)
            {
                // Ensure no loading operation is in progress
                IsBusy = false;
                
                SelectedLanguage = language;
                _localizationService.CurrentLanguage = language;
                OnPropertyChanged(nameof(SelectedLanguageDisplay));

                // Force refresh current page
                if (Shell.Current.CurrentPage is Page currentPage)
                {
                    currentPage.FlowDirection = _localizationService.FlowDirection;
                    
                    // Force all bindings to refresh by resetting BindingContext
                    var currentContext = currentPage.BindingContext;
                    currentPage.BindingContext = null;
                    currentPage.BindingContext = currentContext;
                }

                // No need to navigate back - messenger pattern handles refresh
            }
        }

        private async Task ChangeLanguageAsync(string language)
        {
            if (SelectedLanguage != language)
            {
                // Ensure no loading operation is in progress
                IsBusy = false;
                
                SelectedLanguage = language;
                _localizationService.CurrentLanguage = language;
                OnPropertyChanged(nameof(SelectedLanguageDisplay));

                // Navigate back to force page refresh with new language
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private void TogglePushNotifications()
        {
            Preferences.Set("push_notifications", IsPushNotificationsEnabled);
        }

        [RelayCommand]
        private void ToggleEmailNotifications()
        {
            Preferences.Set("email_notifications", IsEmailNotificationsEnabled);
        }

        [RelayCommand]
        private async Task ChangePassword()
        {
            await Shell.Current.DisplayAlertAsync(
                _localizationService.GetString("Settings_ChangePassword"),
                "Feature coming soon",
                _localizationService.GetString("OK"));
        }

        [RelayCommand]
        private async Task DeleteAccount()
        {
            var confirm = await Shell.Current.DisplayAlertAsync(
                _localizationService.GetString("Settings_DeleteAccount"),
                "Are you sure you want to delete your account? This action cannot be undone.",
                "Delete",
                _localizationService.GetString("Cancel"));

            if (confirm)
            {
                await Shell.Current.DisplayAlertAsync(
                    _localizationService.GetString("Error"),
                    "Feature coming soon",
                    _localizationService.GetString("OK"));
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            var confirm = await Shell.Current.DisplayAlertAsync(
                _localizationService.GetString("Settings_Logout"),
                "Are you sure you want to logout?",
                "Logout",
                _localizationService.GetString("Cancel"));

            if (confirm)
            {
                Preferences.Clear();
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
