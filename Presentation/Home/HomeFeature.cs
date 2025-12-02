using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MicroWinMixology.App.Models;
using MicroWinMixology.App.Services;

namespace MicroWinMixology.App.Presentation.Home
{
    public partial class HomeFeature
    {
        private readonly MicroWinService _service;

        public HomeFeature(MicroWinService service)
        {
            _service = service;
        }

        // Called to load the initial home data
        public async IAsyncEnumerable<HomeModel> Load(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            // Start in a busy state
            var model = HomeModel.Empty with { IsBusy = true };
            yield return model;

            // Get data from your service
            var featured = _service.GetFeaturedDrinks();
            var categoryDrinks = _service.GetDrinksByCategory(model.SelectedCategory);
            var tasks = await _service.GetRandomMicroTasksAsync(ct);

            // Updated, ready state
            model = model with
            {
                IsBusy = false,
                Featured = featured,
                CategoryDrinks = categoryDrinks,
                Tasks = tasks,
                Error = string.Empty
            };

            yield return model;
        }

        // Called when the user changes category (Focus / Calm / Social)
        public async IAsyncEnumerable<HomeModel> ChangeCategory(
            HomeModel current,
            string category,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            // Show loading state
            var model = current with
            {
                IsBusy = true,
                SelectedCategory = category
            };

            yield return model;

            // Get drinks for the selected category
            var drinks = _service.GetDrinksByCategory(category);

            model = model with
            {
                IsBusy = false,
                CategoryDrinks = drinks,
                Error = string.Empty
            };

            yield return model;

            // Just to satisfy async
            await Task.CompletedTask;
        }
    }
}

