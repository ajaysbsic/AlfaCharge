using AlfaGrid.Source.ViewModel;

namespace AlfaGrid.Source.View;

public partial class QRScannerPage : ContentPage
{
    public QRScannerPage(QRScannerPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
