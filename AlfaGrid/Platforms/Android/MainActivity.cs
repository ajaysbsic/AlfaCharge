using Android.App;
using Android.Content.PM;
using Android.OS;

namespace AlfaGrid
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            // Set the main theme before calling base.OnCreate
            // Using GetIdentifier as fallback in case Resource.Style.MainTheme isn't generated yet
            int mainThemeId = Resources?.GetIdentifier("MainTheme", "style", PackageName) ?? 0;
            if (mainThemeId != 0)
            {
                SetTheme(mainThemeId);
            }
            
            base.OnCreate(savedInstanceState);
        }

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();
            }
            catch (System.ObjectDisposedException)
            {
                // defensive: service provider may be disposed during teardown in some MAUI versions
                // swallow to avoid crashing the app during shutdown
            }
        }
    }
}