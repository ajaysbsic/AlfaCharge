using AlfaGrid.Source.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlfaGrid.Source.ViewModel
{
    [QueryProperty(nameof(SelectedLocation), "location")]
    [QueryProperty(nameof(EvseId), "evseId")]
    public partial class LocationDetailsPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ChargingLocation _selectedLocation;

        [ObservableProperty]
        private string _evseId;

        [ObservableProperty]
        private bool _isFromQRScanner = false;

        [ObservableProperty]
        private string _selectedConnectorType;

        [ObservableProperty]
        private string _maxPower;

        [ObservableProperty]
        private int _selectedTab = 0;

        [ObservableProperty]
        private bool _isOverviewVisible = true;

        [ObservableProperty]
        private bool _isPhotosVisible = false;

        [ObservableProperty]
        private bool _isReviewsVisible = false;

        public LocationDetailsPageViewModel()
        {
        }

        partial void OnEvseIdChanged(string value)
        {
            // If EVSE ID is provided, we're coming from QR scanner
            if (!string.IsNullOrWhiteSpace(value))
            {
                IsFromQRScanner = true;
                LoadConnectorDetails();
            }
        }

        private void LoadConnectorDetails()
        {
            // TODO: In production, search through the location's stations and connectors
            // to find the one matching the EVSE ID and get its details
            
            // For now, use demo data from first connector group
            if (SelectedLocation?.ConnectorGroups?.Any() == true)
            {
                var firstGroup = SelectedLocation.ConnectorGroups.First();
                SelectedConnectorType = firstGroup.Standard;
                MaxPower = firstGroup.PowerRatingText;
            }
            else
            {
                // Fallback demo data
                SelectedConnectorType = "AC Type 2";
                MaxPower = "22.0000 kW";
            }
        }

        [RelayCommand]
        private async Task StartCharging()
        {
            if (SelectedLocation != null)
            {
                try
                {
                    IsBusy = true;

                    // Determine connector type - use first available or from EVSE
                    string connectorType = "Type 2"; // Default
                    if (SelectedLocation.ConnectorGroups?.Any() == true)
                    {
                        connectorType = SelectedLocation.ConnectorGroups.First().Standard;
                    }

                    // Navigate to Add Card Details page with all necessary info
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "locationName", SelectedLocation.Name },
                        { "stationName", SelectedLocation.Name }, // or specific station name if available
                        { "connectorType", connectorType },
                        { "evseId", EvseId ?? "EVSE001" }
                    };

                    await Shell.Current.GoToAsync($"{nameof(Source.View.AddCardDetailsPage)}", navigationParameter);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error starting charging: {ex.Message}");
                    await Shell.Current.DisplayAlertAsync("Error", "Unable to start charging session. Please try again.", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        private async Task Directions()
        {
            if (SelectedLocation != null)
            {
                try
                {
                    // Construct Google Maps URL with coordinates
                    var lat = SelectedLocation.Latitude;
                    var lng = SelectedLocation.Longitude;

                    // Google Maps URL format for directions
                    var url = $"https://www.google.com/maps/dir/?api=1&destination={lat},{lng}";

                    // Open in browser/maps app
                    await Launcher.OpenAsync(new Uri(url));
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", $"Unable to open maps: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task Reserve()
        {
            // TODO: Implement reservation functionality
            if (SelectedLocation != null)
            {
                await Shell.Current.DisplayAlertAsync("Reserve", $"Opening reservation for {SelectedLocation.Name}", "OK");
            }
        }

        [RelayCommand]
        private async Task ScanQR()
        {
            await Shell.Current.GoToAsync($"{nameof(Source.View.QRScannerPage)}");
        }

        [RelayCommand]
        private async Task Favorite()
        {
            // TODO: Implement favorite toggle
            if (SelectedLocation != null)
            {
                await Shell.Current.DisplayAlertAsync("Favorite", $"Toggle favorite for {SelectedLocation.Name}", "OK");
            }
        }

        [RelayCommand]
        private void SelectTab(object parameter)
        {
            if (parameter != null)
            {
                int tabIndex = 0;

                // Handle both int and string parameters
                if (parameter is int intValue)
                {
                    tabIndex = intValue;
                }
                else if (parameter is string strValue && int.TryParse(strValue, out int parsedValue))
                {
                    tabIndex = parsedValue;
                }

                SelectedTab = tabIndex;

                // Update visibility flags
                IsOverviewVisible = tabIndex == 0;
                IsPhotosVisible = tabIndex == 1;
                IsReviewsVisible = tabIndex == 2;
            }
        }
    }
}
