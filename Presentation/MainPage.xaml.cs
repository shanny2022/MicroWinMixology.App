using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace MicroWinMixology.App.Presentation.Home;

public sealed partial class MainPage : Page
{
    private CancellationTokenSource _cts = new();

    public MainPage()
    {
        this.InitializeComponent();

        this.WhenChanged(x => x.ViewModel).Execute(async vm =>
        {
            if (vm?.Feature is not null)
                await vm.Feature.LoadAsync(_cts.Token);
        });
    }
}
