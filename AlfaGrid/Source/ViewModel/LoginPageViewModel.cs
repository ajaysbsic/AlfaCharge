using AlfaGrid.Source.Models;
using AlfaGrid.Source.Services;
using AlfaGrid.Source.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Text.Json;

namespace AlfaGrid.Source.ViewModel
{
    public partial class LoginPageViewModel : BaseViewModel, IRecipient<LanguageChangedMessage>
    {
        private readonly ILocalizationService _localizationService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string selectedLanguage;

        public string SelectedLanguageDisplay => SelectedLanguage == "ar" ? "العربية" : "English";

        public LoginPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            SelectedLanguage = _localizationService.CurrentLanguage;
            
            // Register for language change messages
            WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this);
        }

        public void Receive(LanguageChangedMessage message)
        {
            // Update display language
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SelectedLanguage = message.NewLanguage;
                OnPropertyChanged(nameof(SelectedLanguageDisplay));
                
                // Ensure loader is hidden
                IsBusy = false;
            });
        }

        [RelayCommand]
        private async Task SelectLanguage()
        {
            var action = await Shell.Current.DisplayActionSheetAsync(
                _localizationService.GetString("SelectLanguage"),
                _localizationService.GetString("Cancel"),
                null,
                "English",
                "العربية");

            if (action == "English" && SelectedLanguage != "en")
            {
                await ChangeLanguageAsync("en");
            }
            else if (action == "العربية" && SelectedLanguage != "ar")
            {
                await ChangeLanguageAsync("ar");
            }
        }

        private async Task ChangeLanguageAsync(string language)
        {
            // Ensure no loading operation is in progress
            IsBusy = false;
            
            SelectedLanguage = language;
            _localizationService.CurrentLanguage = language;
            OnPropertyChanged(nameof(SelectedLanguageDisplay));

            // Force page reload by navigating away and back
            await Shell.Current.GoToAsync("//LoadingPage");
            await Task.Delay(100); // Small delay to ensure navigation completes
            await Shell.Current.GoToAsync("//LoginPage");
        }

        #region Commands
        [RelayCommand]
        async Task Login()
        {
            if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
            {
                try
                {
                    IsBusy = true;

                    var userDetails = new UserBasicInfo();
                    userDetails.Email = Email;
                    userDetails.FullName = "Test User Name";

                    // Student Role, Teacher Role, Admin Role,
                    if (Email.ToLower().Contains("user"))
                    {
                        userDetails.RoleID = (int)RoleDetails.User;
                        userDetails.RoleText = "User Role";
                    }
                    else if (Email.ToLower().Contains("manager"))
                    {
                        userDetails.RoleID = (int)RoleDetails.Manager;
                        userDetails.RoleText = "Manager Role";
                    }
                    else
                    {
                        userDetails.RoleID = (int)RoleDetails.Admin;
                        userDetails.RoleText = "Admin Role";
                    }

                    // Simulate API call delay
                    await Task.Delay(500);

                    if (Preferences.ContainsKey(nameof(App.UserDetails)))
                    {
                        Preferences.Remove(nameof(App.UserDetails));
                    }

                    string userDetailStr = JsonSerializer.Serialize(userDetails);
                    Preferences.Set(nameof(App.UserDetails), userDetailStr);
                    App.UserDetails = userDetails;
                    await Shell.Current.GoToAsync("//home");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        async Task NavigateToRegister()
        {
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync("RegisterPage");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}