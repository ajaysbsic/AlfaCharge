using AlfaGrid.Source.Models;
using AlfaGrid.Source.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AlfaGrid.Source.ViewModel
{
    public partial class HomePageViewModel : BaseViewModel
    {
        private readonly IChargingLocationService _chargingLocationService;
        private readonly IFilterService _filterService;
        private List<ChargingLocation> _allLocations = new();

        public new string Title { get; set; }
        public ObservableCollection<ChargingLocation> ChargingLocations { get; set; } = new();

        [ObservableProperty]
        private ChargingLocation _selectedLocation;

        [ObservableProperty]
        private bool _isLocationPreviewVisible = false;

        [ObservableProperty]
        private double _currentLatitude;

        [ObservableProperty]
        private double _currentLongitude;

        [ObservableProperty]
        private bool _isMapReady = false;

        // Keep old property for backward compatibility if needed
        [Obsolete("Use ChargingLocations instead")]
        public ObservableCollection<CarouselItem> Places { get; set; } = new();

        public ICommand NavigateToLocationListCommand { get; }
        public ICommand SelectLocationCommand { get; }
        public ICommand CloseLocationPreviewCommand { get; }
        public ICommand NavigateToLocationDetailsCommand { get; }
        public ICommand OpenFilterCommand { get; }

        public HomePageViewModel(IChargingLocationService chargingLocationService, IFilterService filterService)
        {
            _chargingLocationService = chargingLocationService;
            _filterService = filterService;
            Title = "Home";
            
            // Subscribe to filter changes
            _filterService.FiltersChanged += OnFiltersChanged;
            
            // Initialize navigation command - use relative navigation instead of absolute
            NavigateToLocationListCommand = new AsyncRelayCommand(NavigateToLocationListAsync);
            SelectLocationCommand = new RelayCommand<ChargingLocation>(OnLocationSelected);
            CloseLocationPreviewCommand = new RelayCommand(CloseLocationPreview);
            NavigateToLocationDetailsCommand = new AsyncRelayCommand(NavigateToLocationDetailsAsync);
            OpenFilterCommand = new AsyncRelayCommand(OpenFilterAsync);
            
            // Load data asynchronously
            Task.Run(async () => await LoadChargingLocationsAsync());
        }

        private void OnFiltersChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _filterService.ApplyFilters(_allLocations);
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ChargingLocations.Clear();
                foreach (var location in filtered)
                {
                    ChargingLocations.Add(location);
                }
            });
        }

        private async Task LoadChargingLocationsAsync()
        {
            try
            {
                IsBusy = true;

                var locations = await _chargingLocationService.GetLocationsWithStationsAsync();
                _allLocations = locations;

                // Apply filters
                ApplyFilters();
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsMapReady = true;
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

        public async Task RefreshLocationsAsync()
        {
            await LoadChargingLocationsAsync();
        }

        private void OnLocationSelected(ChargingLocation location)
        {
            if (location != null)
            {
                SelectedLocation = location;
                IsLocationPreviewVisible = true;
            }
        }

        private void CloseLocationPreview()
        {
            IsLocationPreviewVisible = false;
            SelectedLocation = null;
        }

        private async Task NavigateToLocationDetailsAsync()
        {
            if (SelectedLocation != null)
            {
                try
                {
                    IsBusy = true;

                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "location", SelectedLocation }
                    };

                    await Shell.Current.GoToAsync(nameof(Source.View.LocationDetailsPage), navigationParameter);
                    
                    // Close preview after navigation
                    CloseLocationPreview();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task NavigateToLocationListAsync()
        {
            try
            {
                IsBusy = true;
                
                // Use relative navigation to push page as modal
                await Shell.Current.GoToAsync(nameof(Source.View.LocationListPage));
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenFilterAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(Source.View.FilterPage));
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }

        public void UpdateCurrentLocation(double latitude, double longitude)
        {
            CurrentLatitude = latitude;
            CurrentLongitude = longitude;
        }
    }
}