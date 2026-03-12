using AlfaGrid.Source.Models;
using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class LocationListPage : ContentPage
{
    public LocationListPage(LocationListPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnLocationTapped(object sender, EventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is ChargingLocation selectedLocation)
        {
            // Navigate to LocationDetailsPage with the selected location
            var navigationParameter = new Dictionary<string, object>
            {
                { "location", selectedLocation }
            };

            await Shell.Current.GoToAsync(nameof(LocationDetailsPage), navigationParameter);
        }
    }
}