using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}