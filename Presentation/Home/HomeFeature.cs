using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MicroWinMixology.App.Models;
using MicroWinMixology.App.Services;
using Uno.Extensions.Reactive;
using Uno.Extensions.Reactive.Commands;

namespace MicroWinMixology.App.Presentation.Home;

public sealed partial class HomeFeature
{
    private readonly MicroWinService _service;

    public HomeFeature(MicroWinService service)
    {
        _service = service;
    }

    public IState<HomeModel> State => State<HomeModel>.Value(HomeModel.Empty);

    public async Task LoadAsync(CancellationToken ct)
    {
        try
        {
            await State.UpdateAsync(
                model => model with { IsBusy = true, Error = "" },
                ct
            );

            var featured = _service.GetFeaturedDrinks();
            var cats = _service.GetDrinksByCategory("Focus");
            var tasks = await _service.GetRandomMicroTasksAsync(ct);

            await State.UpdateAsync(
                model => model with
                {
                    Featured = featured,
                    CategoryDrinks = cats,
                    Tasks = tasks,
                    SelectedCategory = "Focus",
                    IsBusy = false
                },
                ct
            );
        }
        catch (Exception ex)
        {
            await State.UpdateAsync(
                model => model with
                {
                    IsBusy = false,
                    Error = ex.Message
                },
                ct
            );
        }
    }

    public async Task SelectCategoryAsync(string category, CancellationToken ct)
    {
        await State.UpdateAsync(m => m with
        {
            IsBusy = true
        }, ct);

        var drinks = _service.GetDrinksByCategory(category);

        await State.UpdateAsync(m => m with
        {
            SelectedCategory = category,
            CategoryDrinks = drinks,
            IsBusy = false
        }, ct);
    }
}

