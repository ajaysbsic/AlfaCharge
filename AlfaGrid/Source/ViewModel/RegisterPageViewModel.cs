using AlfaGrid.Source.Models;
using AlfaGrid.Source.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AlfaGrid.Source.ViewModel
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAlertService _alertServices;
        public RegisterModel Model { get; } = new();
        private bool _isInitialized;

        // --- UI state ---
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PasswordToggleIcon))]
        private bool isPasswordHidden = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConfirmToggleIcon))]
        private bool isConfirmHidden = true;

        public string PasswordToggleIcon => IsPasswordHidden ? "eye_off.png" : "eye_on.png";
        public string ConfirmToggleIcon => IsConfirmHidden ? "eye_off.png" : "eye_on.png";

        // --- Country selection ---
        public ObservableCollection<CountryInfo> Countries { get; } = [];

        CountryInfo? _selectedCountry;
        public CountryInfo? SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                if (SetProperty(ref _selectedCountry, value))
                {
                    if (value is not null)
                    {
                        Model.CountryDialCode = value.DialCode;
                        Model.CountryIso2 = value.Iso2;
                    }
                    RegisterCommand.NotifyCanExecuteChanged();
                    OnPropertyChanged(nameof(CanRegister));
                }
            }
        }

        // ---- Errors for XAML ----
        public string FirstNameError => FirstError(nameof(Model.FirstName));
        public bool FirstNameHasError => !string.IsNullOrEmpty(FirstNameError);

        public string LastNameError => FirstError(nameof(Model.LastName));
        public bool LastNameHasError => !string.IsNullOrEmpty(LastNameError);

        public string PhoneError => FirstError(nameof(Model.LocalPhone));
        public bool PhoneHasError => !string.IsNullOrEmpty(PhoneError);

        public string EmailError => FirstError(nameof(Model.Email));
        public bool EmailHasError => !string.IsNullOrEmpty(EmailError);

        public string PasswordError => FirstError(nameof(Model.Password));
        public bool PasswordHasError => !string.IsNullOrEmpty(PasswordError);

        public string ConfirmError => FirstError(nameof(Model.ConfirmPassword));
        public bool ConfirmHasError => !string.IsNullOrEmpty(ConfirmError);

        public RegisterViewModel(IAlertService alertServices)
        {
            _alertServices = alertServices;
        }

        void RaiseErrorBindings()
        {
            OnPropertyChanged(nameof(FirstNameError)); OnPropertyChanged(nameof(FirstNameHasError));
            OnPropertyChanged(nameof(LastNameError)); OnPropertyChanged(nameof(LastNameHasError));
            OnPropertyChanged(nameof(PhoneError)); OnPropertyChanged(nameof(PhoneHasError));
            OnPropertyChanged(nameof(EmailError)); OnPropertyChanged(nameof(EmailHasError));
            OnPropertyChanged(nameof(PasswordError)); OnPropertyChanged(nameof(PasswordHasError));
            OnPropertyChanged(nameof(ConfirmError)); OnPropertyChanged(nameof(ConfirmHasError));
        }

        private void InitData()
        {
            Countries.Add(new CountryInfo { Name = "Saudi Arabia", Iso2 = "SA", DialCode = "+966", FlagImage = "sa.png" });
            Countries.Add(new CountryInfo { Name = "United Arab Emirates", Iso2 = "AE", DialCode = "+971", FlagImage = "ae.png" });
            Countries.Add(new CountryInfo { Name = "India", Iso2 = "IN", DialCode = "+91", FlagImage = "in.png" });
            Countries.Add(new CountryInfo { Name = "United Kingdom", Iso2 = "GB", DialCode = "+44", FlagImage = "gb.png" });

            SelectedCountry = Countries.FirstOrDefault(c => c.Iso2 == "SA") ?? Countries.FirstOrDefault();
            HookValidation();
        }

        private void HookValidation()
        {
            Model.PropertyChanged += (_, __) =>
            {
                RaiseErrorBindings();
                RegisterCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(CanRegister));
            };

            Model.ErrorsChanged += (_, __) =>
            {
                RaiseErrorBindings();
                RegisterCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(CanRegister));
            };
        }

        public void InitOnce()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            InitData(); // same Init() as above
        }

        private string FirstError(string propertyName)
        {
            var errors = Model.GetErrors(propertyName);
            if (errors is null) return string.Empty;

            foreach (var e in errors)
                if (e is ValidationResult vr && !string.IsNullOrWhiteSpace(vr.ErrorMessage))
                    return vr.ErrorMessage!;

            return string.Empty;
        }

        public bool CanRegister =>
            !Model.HasErrors &&
            !string.IsNullOrWhiteSpace(Model.FirstName) &&
            !string.IsNullOrWhiteSpace(Model.LastName) &&
            !string.IsNullOrWhiteSpace(Model.LocalPhone) &&
            !string.IsNullOrWhiteSpace(Model.Password) &&
            !string.IsNullOrWhiteSpace(Model.ConfirmPassword);

        [RelayCommand(CanExecute = nameof(CanRegister))]
        private async Task RegisterAsync()
        {
            try
            {
                IsBusy = true;

                Model.ValidateAll();
                if (Model.HasErrors)
                {
                    RaiseErrorBindings();
                    return;
                }

                var userDetailsPayload = new
                {
                    firstName = Model.FirstName.Trim(),
                    lastName = Model.LastName.Trim(),
                    phoneE164 = Model.E164Phone,
                    country = Model.CountryIso2,
                    email = string.IsNullOrWhiteSpace(Model.Email) ? null : Model.Email!.Trim(),
                    password = Model.Password
                };

                // Simulate API call delay
                await Task.Delay(500);

                // TODO: call our backend API here
                await _alertServices.Info("Success", "Account created successfully.");
                
                var userDetails = App.UserDetails = new UserBasicInfo {
                    Email = Model.Email ?? "test@gmail.com",
                    FullName = Model.FirstName + " " + Model.LastName ?? "Test User Name"
                };
                string userDetailStr = JsonSerializer.Serialize(userDetails);
                Preferences.Set(nameof(App.UserDetails), userDetailStr);
                await Shell.Current.GoToAsync("//home");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand] private void TogglePassword() => IsPasswordHidden = !IsPasswordHidden;
        [RelayCommand] private void ToggleConfirm() => IsConfirmHidden = !IsConfirmHidden;

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand] private Task OpenPrivacyAsync() => Launcher.Default.OpenAsync("https://yourdomain.com/privacy");
        [RelayCommand] private Task OpenTermsAsync() => Launcher.Default.OpenAsync("https://yourdomain.com/terms");
    }
}
