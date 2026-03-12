namespace AlfaGrid.Framework.Data.Network.Connectivity
{
    public class InternetConnectionManager : IInternetConnectivity
    {

        private readonly IConnectivity _Connectivity;
        public event EventHandler ConnectivityChanged;

        public InternetConnectionManager(
            IConnectivity InternetConnectivity,
            EventHandler ConnectivityChangedParam)
        {
            _Connectivity = InternetConnectivity;
            ConnectivityChanged = ConnectivityChangedParam;

            _Connectivity.ConnectivityChanged += (sender, argument) =>
            {
                ConnectivityChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        public Task<bool> GetIsConnectedAsync()
        {
            return Task.FromResult
            (
            _Connectivity.NetworkAccess == NetworkAccess.Internet
            );
        }
    }
}