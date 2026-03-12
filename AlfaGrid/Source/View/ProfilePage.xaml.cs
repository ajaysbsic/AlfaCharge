using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfilePageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}