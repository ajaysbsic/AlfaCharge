namespace AlfaGrid.Source.Services
{
    public class AuthService : IAuthService
    {
        private const string AuthKey = "is_logged_in";
        private const string NameKey = "display_name";

        public bool IsAuthenticated { get; private set; }
        public string DisplayName { get; private set; } = string.Empty;

        public event EventHandler? AuthChanged;

        public AuthService()
        {
            IsAuthenticated = Preferences.Get(AuthKey, false);
            DisplayName = Preferences.Get(NameKey, string.Empty);
        }

        public void SignIn(string displayName)
        {
            IsAuthenticated = true;
            DisplayName = displayName ?? string.Empty;
            Preferences.Set(AuthKey, true);
            Preferences.Set(NameKey, DisplayName);
            AuthChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SignOut()
        {
            IsAuthenticated = false;
            DisplayName = string.Empty;
            Preferences.Set(AuthKey, false);
            Preferences.Remove(NameKey);
            AuthChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetRoot(Page page)
        {
            var app = Application.Current;
            if (app?.Windows.Count > 0)
                app.Windows[0].Page = page;
        }
    }
}