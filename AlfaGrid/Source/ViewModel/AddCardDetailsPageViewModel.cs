using AlfaGrid.Source.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlfaGrid.Source.ViewModel
{
    [QueryProperty(nameof(LocationName), "locationName")]
    [QueryProperty(nameof(StationName), "stationName")]
    [QueryProperty(nameof(ConnectorType), "connectorType")]
    [QueryProperty(nameof(EvseId), "evseId")]
    public partial class AddCardDetailsPageViewModel : BaseViewModel
    {
        private readonly ILocalizationService _localizationService;

        [ObservableProperty]
        private string _cardNumber;

        [ObservableProperty]
        private string _expiryDate;

        [ObservableProperty]
        private string _cvv;

        [ObservableProperty]
        private string _cardholderName;

        [ObservableProperty]
        private bool _saveCard;

        [ObservableProperty]
        private string locationName = "";

        [ObservableProperty]
        private string stationName = "";

        [ObservableProperty]
        private string connectorType = "";

        [ObservableProperty]
        private string evseId = "";

        public AddCardDetailsPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            Title = _localizationService.GetString("Payment_Title");
        }

        [RelayCommand]
        private async Task Continue()
        {
            // Validate card details
            if (string.IsNullOrWhiteSpace(CardNumber))
            {
                await Shell.Current.DisplayAlertAsync(
                    _localizationService.GetString("Error"),
                    "Please enter card number",
                    _localizationService.GetString("OK"));
                return;
            }

            if (string.IsNullOrWhiteSpace(ExpiryDate))
            {
                await Shell.Current.DisplayAlertAsync(
                    _localizationService.GetString("Error"),
                    "Please enter expiry date",
                    _localizationService.GetString("OK"));
                return;
            }

            if (string.IsNullOrWhiteSpace(Cvv))
            {
                await Shell.Current.DisplayAlertAsync(
                    _localizationService.GetString("Error"),
                    "Please enter CVV",
                    _localizationService.GetString("OK"));
                return;
            }

            if (string.IsNullOrWhiteSpace(CardholderName))
            {
                await Shell.Current.DisplayAlertAsync(
                    _localizationService.GetString("Error"),
                    "Please enter cardholder name",
                    _localizationService.GetString("OK"));
                return;
            }

            try
            {
                IsBusy = true;

                // Simulate API call to process payment
                await Task.Delay(1500);

                // Simulate checking if charger is online (placeholder logic)
                bool isChargerOnline = await CheckChargerOnlineAsync();

                if (!isChargerOnline)
                {
                    await Shell.Current.DisplayAlertAsync(
                        _localizationService.GetString("Error"),
                        "Charger is currently offline. Please try another charger.",
                        _localizationService.GetString("OK"));
                    return;
                }

                // Navigate to charging session page
                var navigationParameters = new Dictionary<string, object>
                {
                    { "locationName", LocationName },
                    { "stationName", StationName },
                    { "connectorType", ConnectorType },
                    { "evseId", EvseId }
                };

                await Shell.Current.GoToAsync("ChargingSessionPage", navigationParameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing payment: {ex.Message}");
                await Shell.Current.DisplayAlertAsync(
                    _localizationService.GetString("Error"),
                    "Unable to process payment. Please try again.",
                    _localizationService.GetString("OK"));
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Placeholder logic to check if charger is online
        /// In production, this would call an actual API
        /// </summary>
        private async Task<bool> CheckChargerOnlineAsync()
        {
            // Simulate API call
            await Task.Delay(500);
            
            // For demo purposes, always return true
            // In production, this would check actual charger status via API
            return true;
        }
    }
}
