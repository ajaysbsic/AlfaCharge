using AlfaGrid.Source.Models;
using AlfaGrid.Source.View;
using System.Text.Json;

namespace AlfaGrid.Source.ViewModel
{
    public class LoadingPageViewModel
    {
        public LoadingPageViewModel()
        {
            //CheckUserLoginDetails();
        }

        public async Task InitializeAsync()
        {
            string userDetailsStr = Preferences.Get(nameof(App.UserDetails), "");
            if (string.IsNullOrWhiteSpace(userDetailsStr))
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                var userDetails = JsonSerializer.Deserialize<UserBasicInfo>(userDetailsStr);
                if (userDetails is not null)
                {
                    App.UserDetails = userDetails;
                    await Shell.Current.GoToAsync("//home");
                }
                else
                {
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            }
        }

        //private async void CheckUserLoginDetails()
        //{
        //    string userDetailsStr = Preferences.Get(nameof(App.UserDetails), "");

        //    if (string.IsNullOrWhiteSpace(userDetailsStr))
        //    {
        //        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        //        {
        //            AppShell.Current.Dispatcher.Dispatch(async () =>
        //            {
        //                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        //            });
        //        }
        //        else
        //        {
        //            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        //        }
        //        // navigate to Login Page
        //    }
        //    else
        //    {
        //        var userInfo = JsonSerializer.Deserialize<UserBasicInfo>(userDetailsStr);
        //        App.UserDetails = userInfo;
        //        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        //    }
        //}
    }
}