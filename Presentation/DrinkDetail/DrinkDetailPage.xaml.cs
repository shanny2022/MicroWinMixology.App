using Microsoft.UI.Xaml.Controls;

namespace MicroWinMixology.App.Presentation.DrinkDetail;

public sealed partial class DrinkDetailPage : Page
{
    public DrinkDetailModel ViewModel { get; }

    public DrinkDetailPage()
    {
        this.InitializeComponent();

        // For now use the default model so the page has a valid DataContext.
        ViewModel = DrinkDetailModel.Default;
        DataContext = ViewModel;
    }
}
