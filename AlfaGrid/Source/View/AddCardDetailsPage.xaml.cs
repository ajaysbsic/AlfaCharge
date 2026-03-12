using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class AddCardDetailsPage : ContentPage
{
    public AddCardDetailsPage(AddCardDetailsPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}
