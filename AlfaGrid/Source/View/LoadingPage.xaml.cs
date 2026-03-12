using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class LoadingPage : ContentPage
{
    public LoadingPage(LoadingPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LoadingPageViewModel vm)
            await vm.InitializeAsync();
    }
}