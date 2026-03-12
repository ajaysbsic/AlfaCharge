using AlfaGrid.Source.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlfaGrid.Source.ViewModel
{
    public partial class QRScannerPageViewModel : BaseViewModel
    {
        private readonly IChargingLocationService _chargingLocationService;

        [ObservableProperty]
        private string _evseId;

        [ObservableProperty]
        private bool _isFlashOn = false;

        public QRScannerPageViewModel(IChargingLocationService chargingLocationService)
        {
            _chargingLocationService = chargingLocationService;
        }

        partial void OnEvseIdChanged(string value)
        {
            VerifyCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void OnFlash()
        {
            IsFlashOn = !IsFlashOn;
            // TODO: Implement flash toggle for camera
            System.Diagnostics.Debug.WriteLine($"Flash {(IsFlashOn ? "ON" : "OFF")}");
        }

        [RelayCommand(CanExecute = nameof(CanVerifyExecute))]
        private async Task Verify()
        {
            if (string.IsNullOrWhiteSpace(EvseId))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter an EVSE ID or scan a QR code", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // Simulate API call to verify EVSE ID and get charging station details
                await Task.Delay(1000);

                // Get all locations to find the matching EVSE
                var locations = await _chargingLocationService.GetLocationsWithStationsAsync();
                
                // Find the location that contains this EVSE ID
                // For now, we'll use the first location as a demo
                // In production, you should search through stations/connectors to find matching EVSE ID
                var location = locations.FirstOrDefault();

                if (location != null)
                {
                    // Navigate to Location Details page with the location data and EVSE ID
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "location", location },
                        { "evseId", EvseId }
                    };

                    await Shell.Current.GoToAsync($"{nameof(Source.View.LocationDetailsPage)}", navigationParameter);
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "EVSE ID not found. Please check and try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verifying EVSE ID: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", "Unable to verify EVSE ID. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanVerifyExecute()
        {
            return !string.IsNullOrWhiteSpace(EvseId);
        }

        // Method to handle QR code scan result
        public async Task HandleQRCodeScanned(string qrCodeData)
        {
            EvseId = qrCodeData;
            await Verify();
        }
    }
}
