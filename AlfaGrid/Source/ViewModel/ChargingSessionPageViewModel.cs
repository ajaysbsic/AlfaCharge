using AlfaGrid.Source.Models;
using AlfaGrid.Source.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Timers;

namespace AlfaGrid.Source.ViewModel
{
    [QueryProperty(nameof(LocationName), "locationName")]
    [QueryProperty(nameof(StationName), "stationName")]
    [QueryProperty(nameof(ConnectorType), "connectorType")]
    [QueryProperty(nameof(EvseId), "evseId")]
    public partial class ChargingSessionPageViewModel : BaseViewModel
    {
        private readonly ILocalizationService _localizationService;
        private System.Timers.Timer? _chargingTimer;
        private DateTime _sessionStartTime;

        [ObservableProperty]
        private string locationName = "Unknown Location";

        [ObservableProperty]
        private string stationName = "Unknown Station";

        [ObservableProperty]
        private string connectorType = "Type 2";

        [ObservableProperty]
        private string evseId = "";

        [ObservableProperty]
        private string chargingStatus = "CHARGING";

        [ObservableProperty]
        private DateTime startTime;

        [ObservableProperty]
        private string duration = "00:00:00 Hrs";

        [ObservableProperty]
        private double energyConsumed = 0.0;

        [ObservableProperty]
        private double sessionCost = 0.00;

        [ObservableProperty]
        private DateTime lastUpdated;

        [ObservableProperty]
        private string connectorTypeImage = "type2_connector.png";

        [ObservableProperty]
        private BackButtonBehavior backButtonBehavior;

        [ObservableProperty]
        private bool isEndingSession = false;

        // Charging simulation parameters
        private double _powerRating = 22.0; // kW
        private double _tariffPerKWh = 0.50; // SAR per kWh
        private int _secondsElapsed = 0;

        public ChargingSessionPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            Title = _localizationService.GetString("ChargingSession_Title");

            // Disable back button
            BackButtonBehavior = new BackButtonBehavior
            {
                IsEnabled = false,
                IsVisible = false
            };
        }

        partial void OnConnectorTypeChanged(string value)
        {
            // Update connector image based on type
            ConnectorTypeImage = GetConnectorImage(value);
        }

        private string GetConnectorImage(string connectorType)
        {
            return connectorType?.ToLower() switch
            {
                "type 2" or "type2" or "ac type 2" => "type2_connector.png",
                "ccs2" or "ccs 2" or "dc ccs2" => "ccs2_connector.png",
                "chademo" or "dc chademo" => "chademo_connector.png",
                "type 1" or "type1" or "ac type 1" => "type1_connector.png",
                "gb/t" or "gbt" => "gbt_connector.png",
                _ => "type2_connector.png" // Default
            };
        }

        public void StartChargingSession()
        {
            // Initialize session
            _sessionStartTime = DateTime.Now;
            StartTime = _sessionStartTime;
            LastUpdated = DateTime.Now;
            _secondsElapsed = 0;
            EnergyConsumed = 0.0;
            SessionCost = 0.00;
            ChargingStatus = _localizationService.GetString("ChargingSession_Charging");

            // Start timer to simulate charging
            _chargingTimer = new System.Timers.Timer(1000); // Update every second
            _chargingTimer.Elapsed += OnChargingTimerElapsed;
            _chargingTimer.Start();
        }

        private void OnChargingTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            _secondsElapsed++;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Update duration
                var elapsed = TimeSpan.FromSeconds(_secondsElapsed);
                Duration = $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2} Hrs";

                // Simulate energy consumption (power rating / 3600 * seconds)
                // This simulates charging at the power rating
                EnergyConsumed = (_powerRating / 3600.0) * _secondsElapsed;

                // Calculate cost
                SessionCost = EnergyConsumed * _tariffPerKWh;

                // Update last updated time
                LastUpdated = DateTime.Now;
            });
        }

        public void StopTimer()
        {
            _chargingTimer?.Stop();
            _chargingTimer?.Dispose();
            _chargingTimer = null;
        }

        [RelayCommand]
        private async Task GoBack()
        {
            // Disabled - user must stop charging first
            await Shell.Current.DisplayAlertAsync(
                _localizationService.GetString("ChargingSession_Title"),
                _localizationService.GetString("ChargingSession_MustStopFirst"),
                _localizationService.GetString("OK"));
        }

        [RelayCommand]
        private async Task StopCharging()
        {
            var confirm = await Shell.Current.DisplayAlertAsync(
                _localizationService.GetString("ChargingSession_StopCharging"),
                _localizationService.GetString("ChargingSession_ConfirmStop"),
                _localizationService.GetString("ChargingSession_Yes"),
                _localizationService.GetString("Cancel"));

            if (confirm)
            {
                try
                {
                    // Show ending session overlay
                    IsEndingSession = true;
                    ChargingStatus = _localizationService.GetString("ChargingSession_Stopping");

                    // Stop the timer
                    StopTimer();

                    // Simulate API call to stop charging (2 seconds)
                    await Task.Delay(2000);

                    // Hide ending session overlay
                    IsEndingSession = false;

                    // Prepare session details data
                    var navigationParameters = new Dictionary<string, object>
                    {
                        { "stationName", StationName },
                        { "locationName", LocationName },
                        { "evseId", EvseId },
                        { "totalCost", SessionCost.ToString("F2") },
                        { "currentChargingCost", SessionCost.ToString("F2") },
                        { "energyCharges", (SessionCost * 0.8).ToString("F2") }, // 80% of total
                        { "timeCharges", (SessionCost * 0.1).ToString("F2") }, // 10% of total
                        { "parkingCharges", "0.00" },
                        { "fixedCharges", (SessionCost * 0.1).ToString("F2") }, // 10% of total
                        { "chargingDuration", Duration },
                        { "idleDuration", "00:00:00" },
                        { "estEndBatterySoC", "-" },
                        { "energyAdded", EnergyConsumed.ToString("F2") },
                        { "sessionDate", StartTime.ToString("O") }, // ISO 8601 format
                        { "sessionTime", StartTime.ToString("O") }
                    };

                    // Navigate to session details page
                    await Shell.Current.GoToAsync("SessionDetailsPage", navigationParameters);
                }
                catch (Exception ex)
                {
                    IsEndingSession = false;
                    System.Diagnostics.Debug.WriteLine($"Error stopping charging: {ex.Message}");
                    await Shell.Current.DisplayAlertAsync(
                        _localizationService.GetString("Error"),
                        _localizationService.GetString("ChargingSession_StopError"),
                        _localizationService.GetString("OK"));
                }
            }
        }

        [RelayCommand]
        private async Task ContactSupport()
        {
            await Shell.Current.DisplayAlertAsync(
                _localizationService.GetString("ChargingSession_Support"),
                _localizationService.GetString("ChargingSession_SupportMessage"),
                _localizationService.GetString("OK"));
        }
    }
}
