using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class MyChargingProfilePage : ContentPage
{
    public MyChargingProfilePage(MyChargingProfilePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
