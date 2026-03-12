using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class LoginPage : ContentPage
{
    private readonly LoginPageViewModel _viewModel;

    public LoginPage(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Ensure loader is hidden when page appears
        _viewModel.IsBusy = false;
    }
}