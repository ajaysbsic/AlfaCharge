using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class SessionDetailsPage : ContentPage
{
    public SessionDetailsPage(SessionDetailsPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}
