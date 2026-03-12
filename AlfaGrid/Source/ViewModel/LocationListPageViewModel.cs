using AlfaGrid.Source.Models;
using AlfaGrid.Source.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AlfaGrid.Source.ViewModel
{
    public class LocationListPageViewModel : BaseViewModel
    {
        private readonly IChargingLocationService _chargingLocationService;
        private List<ChargingLocation> _allLocations;
        private string _searchText;
        private bool _isRefreshing;

        public ObservableCollection<ChargingLocation> ChargingLocations { get; set; } = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                Task.Run(async () => await FilterLocationsAsync());
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public LocationListPageViewModel(IChargingLocationService chargingLocationService)
        {
            _chargingLocationService = chargingLocationService;
            RefreshCommand = new Command(async () => await RefreshLocationsAsync());

            // Load data on initialization
            Task.Run(async () => await LoadChargingLocationsAsync());
        }

        private async Task LoadChargingLocationsAsync()
        {
            try
            {
                IsBusy = true;

                var locations = await _chargingLocationService.GetLocationsWithStationsAsync();
                _allLocations = locations;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ChargingLocations.Clear();
                    foreach (var location in _allLocations)
                    {
                        ChargingLocations.Add(location);
                    }
                });
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading charging locations: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshLocationsAsync()
        {
            try
            {
                IsRefreshing = true;
                await LoadChargingLocationsAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task FilterLocationsAsync()
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(_searchText))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ChargingLocations.Clear();
                        foreach (var location in _allLocations)
                        {
                            ChargingLocations.Add(location);
                        }
                    });
                }
                else
                {
                    var filtered = _allLocations
                        .Where(l =>
                            l.Name.Contains(_searchText, System.StringComparison.OrdinalIgnoreCase) ||
                            l.Address.Contains(_searchText, System.StringComparison.OrdinalIgnoreCase) ||
                            l.City.Contains(_searchText, System.StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ChargingLocations.Clear();
                        foreach (var location in filtered)
                        {
                            ChargingLocations.Add(location);
                        }
                    });
                }
            });
        }
    }
}
