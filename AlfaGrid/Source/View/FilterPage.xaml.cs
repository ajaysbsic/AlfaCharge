using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class FilterPage : ContentPage
{
    public FilterPage(FilterPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}
