namespace AlfaGrid.Source.Services
{
    public interface IAuthService
    {
        bool IsAuthenticated { get; }
        string DisplayName { get; }
        event EventHandler? AuthChanged;

        void SignIn(string displayName);
        void SignOut();

        void SetRoot(Page page);
    }
}