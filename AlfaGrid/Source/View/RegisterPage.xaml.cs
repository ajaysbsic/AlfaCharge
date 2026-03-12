using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _registerVM;
    public RegisterPage(RegisterViewModel registerVM)
    {
        InitializeComponent();
        BindingContext = registerVM;
        _registerVM = registerVM;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _registerVM.InitOnce();
    }
}