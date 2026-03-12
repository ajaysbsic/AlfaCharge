using AlfaGrid.Source.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;

namespace AlfaGrid.Source.ViewModel
{
    public partial class ProfilePageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _fullName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _roleText;

        [ObservableProperty]
        private bool _isEditing = false;

        [ObservableProperty]
        private string _editButtonText = "Edit Profile";

        private string _originalFullName;
        private string _originalEmail;

        public ProfilePageViewModel()
        {
            LoadUserDetails();
        }

        private void LoadUserDetails()
        {
            if (App.UserDetails != null)
            {
                FullName = App.UserDetails.FullName;
                Email = App.UserDetails.Email;
                RoleText = App.UserDetails.RoleText;
            }
        }

        [RelayCommand]
        void ToggleEdit()
        {
            if (IsEditing)
            {
                // Save changes
                SaveUserDetails();
            }
            else
            {
                // Enter edit mode
                _originalFullName = FullName;
                _originalEmail = Email;
            }

            IsEditing = !IsEditing;
            EditButtonText = IsEditing ? "Save Changes" : "Edit Profile";
        }

        [RelayCommand]
        void CancelEdit()
        {
            // Restore original values
            FullName = _originalFullName;
            Email = _originalEmail;
            
            IsEditing = false;
            EditButtonText = "Edit Profile";
        }

        private async void SaveUserDetails()
        {
            try
            {
                // Update App.UserDetails
                if (App.UserDetails != null)
                {
                    App.UserDetails.FullName = FullName;
                    App.UserDetails.Email = Email;

                    // Save to preferences
                    if (Preferences.ContainsKey(nameof(App.UserDetails)))
                    {
                        Preferences.Remove(nameof(App.UserDetails));
                    }

                    string userDetailStr = JsonSerializer.Serialize(App.UserDetails);
                    Preferences.Set(nameof(App.UserDetails), userDetailStr);

                    await Shell.Current.DisplayAlertAsync("Success", "Profile updated successfully!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to save profile: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        async Task ChangePassword()
        {
            await Shell.Current.DisplayAlertAsync("Change Password", "This feature will be implemented soon.", "OK");
        }

        [RelayCommand]
        async Task NotificationSettings()
        {
            await Shell.Current.DisplayAlertAsync("Notification Settings", "This feature will be implemented soon.", "OK");
        }
    }
}
