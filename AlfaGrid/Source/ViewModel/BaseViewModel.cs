using CommunityToolkit.Mvvm.ComponentModel;

namespace AlfaGrid.Source.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title;

        public static bool IsNetworkConnected()
            => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }
}