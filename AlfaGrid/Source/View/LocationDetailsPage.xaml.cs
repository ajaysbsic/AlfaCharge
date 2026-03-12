using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class LocationDetailsPage : ContentPage
{
    public LocationDetailsPage(LocationDetailsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
