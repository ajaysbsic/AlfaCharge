using AlfaGrid.Source.ViewModel;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using AlfaGrid.Source.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace AlfaGrid.Source.View;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _viewModel;
    private Pin _currentLocationPin;
    private readonly List<Pin> _locationPins = new();
    private readonly Dictionary<Pin, string> _pinIcons = new();
    
    // Alfanar Industrial City coordinates (fallback for emulator)
    private const double ALFANAR_LATITUDE = 24.53129;
    private const double ALFANAR_LONGITUDE = 46.93705;

    public HomePage(HomePageViewModel homePageViewModel)
    {
        InitializeComponent();
        this.BindingContext = homePageViewModel;
        _viewModel = homePageViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Reset busy state when page appears
        _viewModel.IsBusy = false;
        
        // Hide the tap overlay in case it was left visible
        TapOverlay.IsVisible = false;
        
        // Close flyout if it was left open
        if (Shell.Current.FlyoutIsPresented)
        {
            Shell.Current.FlyoutIsPresented = false;
        }
        
        // Refresh locations if filters were changed
        if (_viewModel.IsMapReady)
        {
            // Re-add pins for filtered locations
            AddChargingLocationPins();
        }
        else
        {
            // Initialize map with current location
            await InitializeMapAsync();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Ensure busy state is cleared when leaving page
        _viewModel.IsBusy = false;
        
        // Hide overlay when leaving page
        TapOverlay.IsVisible = false;
    }

    private async Task InitializeMapAsync()
    {
        try
        {
            _viewModel.IsBusy = true;

            Location currentLocation = null;
            
            // Try to get actual device location
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                    var location = await Geolocation.Default.GetLocationAsync(request);

                    if (location != null)
                    {
                        currentLocation = new Location(location.Latitude, location.Longitude);
                        _viewModel.UpdateCurrentLocation(location.Latitude, location.Longitude);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to get device location: {ex.Message}");
                }
            }

            // If we couldn't get actual location, use Alfanar Industrial City as fallback (for emulator)
            if (currentLocation == null)
            {
                currentLocation = new Location(ALFANAR_LATITUDE, ALFANAR_LONGITUDE);
                _viewModel.UpdateCurrentLocation(ALFANAR_LATITUDE, ALFANAR_LONGITUDE);
                System.Diagnostics.Debug.WriteLine("Using fallback location (Alfanar Industrial City) - likely running on emulator");
            }

            // Center map on current location
            var mapSpan = MapSpan.FromCenterAndRadius(
                currentLocation,
                Distance.FromKilometers(10));
            
            GoogleMap.MoveToRegion(mapSpan);

            // Add current location pin
            AddCurrentLocationPin(currentLocation.Latitude, currentLocation.Longitude);

            // Wait for locations to be loaded
            await WaitForLocationsAsync();
            
            // Add pins for all charging locations
            AddChargingLocationPins();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing map: {ex.Message}");
            await DisplayAlertAsync("Error", $"Unable to initialize map: {ex.Message}", "OK");
        }
        finally
        {
            _viewModel.IsBusy = false;
        }
    }

    private async Task WaitForLocationsAsync()
    {
        // Wait up to 5 seconds for locations to load
        var timeout = DateTime.Now.AddSeconds(5);
        while (!_viewModel.IsMapReady && DateTime.Now < timeout)
        {
            await Task.Delay(100);
        }
    }

    private void AddCurrentLocationPin(double latitude, double longitude)
    {
        if (_currentLocationPin != null)
        {
            GoogleMap.Pins.Remove(_currentLocationPin);
            _pinIcons.Remove(_currentLocationPin);
        }

        _currentLocationPin = new Pin
        {
            Label = "My Location",
            Address = "Current Location",
            Type = PinType.Place,
            Location = new Location(latitude, longitude)
        };

        // Track that this pin should use current_location_icon
        _pinIcons[_currentLocationPin] = "current_location_icon";

        GoogleMap.Pins.Add(_currentLocationPin);
    }

    private void AddChargingLocationPins()
    {
        // Clear existing pins except current location
        foreach (var pin in _locationPins)
        {
            GoogleMap.Pins.Remove(pin);
            _pinIcons.Remove(pin);
        }
        _locationPins.Clear();

        // Add pins for each charging location with custom charging station icon
        foreach (var location in _viewModel.ChargingLocations)
        {
            var pin = new Pin
            {
                Label = location.Name,
                Address = location.Address,
                Type = PinType.Place,
                Location = new Location(location.Latitude, location.Longitude)
            };

            // Store reference to location for click handling
            pin.BindingContext = location;
            pin.MarkerClicked += OnPinClicked;

            // Track that this pin should use charging_station icon
            _pinIcons[pin] = "charging_station";

            _locationPins.Add(pin);
            GoogleMap.Pins.Add(pin);
        }
    }

    private void OnPinClicked(object sender, PinClickedEventArgs e)
    {
        e.HideInfoWindow = true; // Hide default info window
        
        if (sender is Pin pin && pin.BindingContext is ChargingLocation location)
        {
            // Show location preview overlay
            _viewModel.SelectLocationCommand.Execute(location);
            
            // Update carousel to show the selected location
            var index = _viewModel.ChargingLocations.IndexOf(location);
            if (index >= 0)
            {
                PlacesCarousel.Position = index;
            }
        }
    }

    private void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        // Close location preview when map is clicked
        if (_viewModel.IsLocationPreviewVisible)
        {
            _viewModel.CloseLocationPreviewCommand.Execute(null);
        }
    }

    private void OnCarouselCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        if (e.CurrentItem is ChargingLocation location && !_viewModel.IsLocationPreviewVisible)
        {
            // Center map on the selected location when carousel changes
            var mapSpan = MapSpan.FromCenterAndRadius(
                new Location(location.Latitude, location.Longitude),
                Distance.FromKilometers(5));
            
            GoogleMap.MoveToRegion(mapSpan);
        }
    }

    protected override bool OnBackButtonPressed()
    {
        // Handle back button when location preview is visible
        if (_viewModel.IsLocationPreviewVisible)
        {
            _viewModel.CloseLocationPreviewCommand.Execute(null);
            return true; // Consume the back button
        }
        
        return base.OnBackButtonPressed();
    }

    private void OnHamburgerIconTapped(object sender, EventArgs e)
    {
        if (Shell.Current.FlyoutIsPresented)
        {
            Shell.Current.FlyoutIsPresented = false;
            TapOverlay.IsVisible = false;
        }
        else
        {
            Shell.Current.FlyoutIsPresented = true;
            TapOverlay.IsVisible = true;
        }
    }

    private void OnTappedOutsideFlyout(object sender, EventArgs e)
    {
        if (Shell.Current.FlyoutIsPresented)
        {
            Shell.Current.FlyoutIsPresented = false;
            TapOverlay.IsVisible = false;
        }
    }

    private async void OnCurrentLocationTapped(object sender, EventArgs e)
    {
        try
        {
            _viewModel.IsBusy = true;

            Location currentLocation = null;
            
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);
                
                if (location != null)
                {
                    currentLocation = new Location(location.Latitude, location.Longitude);
                    _viewModel.UpdateCurrentLocation(location.Latitude, location.Longitude);
                }
            }

            // Fallback to Alfanar if location not available
            if (currentLocation == null)
            {
                currentLocation = new Location(ALFANAR_LATITUDE, ALFANAR_LONGITUDE);
                await DisplayAlertAsync("Location", "Using fallback location (Alfanar Industrial City)", "OK");
            }

            // Center map on current location
            var mapSpan = MapSpan.FromCenterAndRadius(
                currentLocation,
                Distance.FromKilometers(5));
            
            GoogleMap.MoveToRegion(mapSpan);

            // Update current location pin
            AddCurrentLocationPin(currentLocation.Latitude, currentLocation.Longitude);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Unable to get location: {ex.Message}", "OK");
        }
        finally
        {
            _viewModel.IsBusy = false;
        }
    }

    private async void OnQrTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(QRScannerPage));
    }

    private async void OnDirectionsTapped(object sender, EventArgs e)
    {
        if (_viewModel.SelectedLocation != null)
        {
            try
            {
                var lat = _viewModel.SelectedLocation.Latitude;
                var lng = _viewModel.SelectedLocation.Longitude;
                var url = $"https://www.google.com/maps/dir/?api=1&destination={lat},{lng}";
                await Launcher.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Unable to open maps: {ex.Message}", "OK");
            }
        }
    }

    // Public method to get pin icon for custom map handler
    public string GetPinIcon(Pin pin)
    {
        return _pinIcons.TryGetValue(pin, out var icon) ? icon : "charging_station";
    }
}