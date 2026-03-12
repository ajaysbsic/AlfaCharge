using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlfaGrid.Source.Models
{
    public class RegisterModel : ObservableValidator
    {
        string _firstName = string.Empty;
        string _lastName = string.Empty;
        string _localPhone = string.Empty; // 9 digits for KSA; can vary by country if you choose later
        string? _email;
        string _password = string.Empty;
        string _confirmPassword = string.Empty;

        // New: selected dial code (from picker)
        string _countryDialCode = "+966";
        string _countryIso2 = "SA";

        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value, true); }

        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value, true); }

        [Required(ErrorMessage = "Enter your phone number")]
        // Keep 9 digits rule if KSA is common; you can make this dynamic later per SelectedCountry
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Enter a valid phone number (9 digits)")]
        public string LocalPhone
        {
            get => _localPhone;
            set
            {
                var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
                SetProperty(ref _localPhone, digits, true);
            }
        }

        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string? Email { get => _email; set => SetProperty(ref _email, value, true); }

        [Required(ErrorMessage = "Enter a password")]
        [MinLength(8, ErrorMessage = "Min 8 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "Must include letters & numbers")]
        public string Password { get => _password; set => SetProperty(ref _password, value, true); }

        [Required(ErrorMessage = "Confirm your password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value, true); }

        public string CountryDialCode
        {
            get => _countryDialCode;
            set => SetProperty(ref _countryDialCode, value, true);
        }

        public string CountryIso2
        {
            get => _countryIso2;
            set => SetProperty(ref _countryIso2, value, true);
        }

        public string E164Phone => string.IsNullOrWhiteSpace(LocalPhone) ? string.Empty : $"{CountryDialCode}{LocalPhone}";

        public void ValidateAll() => ValidateAllProperties();
    }
}