namespace AlfaGrid.Source.Services
{
    public interface IAlertService
    {
        Task Info(string title, string message, string ok = "OK");
        Task<bool> Confirm(string title, string message, string accept = "OK", string cancel = "Cancel");
    }
}