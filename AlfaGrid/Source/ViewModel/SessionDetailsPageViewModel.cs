using AlfaGrid.Source.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace AlfaGrid.Source.ViewModel
{
    [QueryProperty(nameof(StationName), "stationName")]
    [QueryProperty(nameof(LocationName), "locationName")]
    [QueryProperty(nameof(EvseId), "evseId")]
    [QueryProperty(nameof(TotalCostString), "totalCost")]
    [QueryProperty(nameof(CurrentChargingCostString), "currentChargingCost")]
    [QueryProperty(nameof(EnergyChargesString), "energyCharges")]
    [QueryProperty(nameof(TimeChargesString), "timeCharges")]
    [QueryProperty(nameof(ParkingChargesString), "parkingCharges")]
    [QueryProperty(nameof(FixedChargesString), "fixedCharges")]
    [QueryProperty(nameof(ChargingDuration), "chargingDuration")]
    [QueryProperty(nameof(IdleDuration), "idleDuration")]
    [QueryProperty(nameof(EstEndBatterySoC), "estEndBatterySoC")]
    [QueryProperty(nameof(EnergyAddedString), "energyAdded")]
    [QueryProperty(nameof(SessionDateString), "sessionDate")]
    [QueryProperty(nameof(SessionTimeString), "sessionTime")]
    public partial class SessionDetailsPageViewModel : BaseViewModel
    {
        private readonly ILocalizationService _localizationService;

        [ObservableProperty]
        private string stationName = "Unknown Station";

        [ObservableProperty]
        private string locationName = "Unknown Location";

        [ObservableProperty]
        private string evseId = "";

        [ObservableProperty]
        private double totalCost = 0.00;

        [ObservableProperty]
        private double currentChargingCost = 0.00;

        [ObservableProperty]
        private double energyCharges = 0.00;

        [ObservableProperty]
        private double timeCharges = 0.00;

        [ObservableProperty]
        private double parkingCharges = 0.00;

        [ObservableProperty]
        private double fixedCharges = 0.00;

        [ObservableProperty]
        private string chargingDuration = "00:00:00";

        [ObservableProperty]
        private string idleDuration = "00:00:00";

        [ObservableProperty]
        private string estEndBatterySoC = "-";

        [ObservableProperty]
        private double energyAdded = 0.0;

        [ObservableProperty]
        private DateTime sessionDate = DateTime.Now;

        [ObservableProperty]
        private DateTime sessionTime = DateTime.Now;

        // String properties for QueryProperty (will be parsed)
        private string totalCostString = "0.00";
        public string TotalCostString
        {
            get => totalCostString;
            set
            {
                totalCostString = value;
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    TotalCost = result;
                }
            }
        }

        private string currentChargingCostString = "0.00";
        public string CurrentChargingCostString
        {
            get => currentChargingCostString;
            set
            {
                currentChargingCostString = value;
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    CurrentChargingCost = result;
                }
            }
        }

        private string energyChargesString = "0.00";
        public string EnergyChargesString
        {
            get => energyChargesString;
            set
            {
                energyChargesString = value;
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    EnergyCharges = result;
                }
            }
        }

        private string timeChargesString = "0.00";
        public string TimeChargesString
        {
            get => timeChargesString;
            set
            {
                timeChargesString = value;
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    TimeCharges = result;
                }
            }
        }

        private string parkingChargesString = "0.00";
        public string ParkingChargesString
        {
            get => parkingChargesString;
            set
            {
                parkingChargesString = value;
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    ParkingCharges = result;
                }
            }
        }

        private string fixedChargesString = "0.00";
        public string FixedChargesString
        {
            get => fixedChargesString;
            set
            {
                fixedChargesString = value;
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    FixedCharges = result;
                }
            }
        }

        private string energyAddedString = "0.00";
        public string EnergyAddedString
        {
            get => energyAddedString;
            set
            {
                energyAddedString = value;
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    EnergyAdded = result;
                }
            }
        }

        private string sessionDateString = "";
        public string SessionDateString
        {
            get => sessionDateString;
            set
            {
                sessionDateString = value;
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
                {
                    SessionDate = result;
                }
            }
        }

        private string sessionTimeString = "";
        public string SessionTimeString
        {
            get => sessionTimeString;
            set
            {
                sessionTimeString = value;
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
                {
                    SessionTime = result;
                }
            }
        }

        public SessionDetailsPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            Title = _localizationService.GetString("SessionDetails_Title");
            
            System.Diagnostics.Debug.WriteLine("SessionDetailsPageViewModel created");
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("//home");
        }

        [RelayCommand]
        private async Task Continue()
        {
            // Navigate to home page
            await Shell.Current.GoToAsync("//home");
        }
    }
}
