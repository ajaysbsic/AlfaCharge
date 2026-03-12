namespace AlfaGrid.Framework.Data.Network.Connectivity
{
    /// <summary>
    /// Defines an interface for components to check current network conditions
    /// and changes to those conditions.
    /// </summary>
    public interface IInternetConnectivity
    {
        Task<bool> GetIsConnectedAsync();
        event EventHandler ConnectivityChanged;
    }
}