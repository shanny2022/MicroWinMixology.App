using System.Linq;
using System.Threading.Tasks;
using MicroWinMixology.App.Models;
using MicroWinMixology.App.Presentation.Home;
using MicroWinMixology.App.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MicroWinMixology.App.Views
{
    public sealed partial class MainPage : Page
    {
        private readonly HomeFeature _homeFeature;
        private HomeModel _state = HomeModel.Default;

        public MainPage()
        {
            this.InitializeComponent();

            // Resolve HomeFeature via DI manually.
            var host = ((App)Application.Current).Host; // This depends on how App exposes IHost
            _homeFeature = host.Services.GetService(typeof(HomeFeature)) as HomeFeature
                           ?? throw new System.Exception("HomeFeature not registered");

            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadHome();
        }

        private async Task LoadHome()
        {
            // Show busy ring
            BusyRing.Visibility = Visibility.Visible;

            await foreach (var newState in _homeFeature.LoadHome(new AsyncState<HomeModel>(_state)))
            {
                _state = newState.Value;
                RenderState();
            }

            BusyRing.Visibility = Visibility.Collapsed;
        }

        private void RenderState()
        {
            // Errors
            if (string.IsNullOrWhiteSpace(_state.ErrorMessage))
            {
                ErrorText.Visibility = Visibility.Collapsed;
            }
            else
            {
                ErrorText.Text = _state.ErrorMessage;
                ErrorText.Visibility = Visibility.Visible;
            }

            // Featured drinks
            FeaturedDrinksList.ItemsSource = _state.FeaturedDrinks.Select(d => d.Name).ToList();

            // Category drinks
            CategoryDrinksList.ItemsSource = _state.CategoryDrinks.Select(d => d.Name).ToList();

            // Suggested tasks
            SuggestedTasksList.ItemsSource = _state.SuggestedTasks.Select(t => t.Title).ToList();
        }
    }

    // Helper implementation of IState to plug into MVUX feature.
    // In a real project you'd use Uno.Extensions.Reactive.State, but this is simplified.
    public class AsyncState<T> : Uno.Extensions.Reactive.IState<T>
    {
        public T Value { get; set; }

        public AsyncState(T initial)
        {
            Value = initial;
        }
    }
}
