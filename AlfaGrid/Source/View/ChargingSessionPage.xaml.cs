using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class ChargingSessionPage : ContentPage
{
    private readonly ChargingSessionPageViewModel _viewModel;

    public ChargingSessionPage(ChargingSessionPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.StartChargingSession();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopTimer();
    }
}
