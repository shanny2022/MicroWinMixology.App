using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MicroWinMixology.App.Models;
using MicroWinMixology.App.Services;

namespace MicroWinMixology.App.Presentation.DrinkDetail
{
    public partial class DrinkDetailFeature
    {
        private readonly MicroWinService _service;

        public DrinkDetailFeature(MicroWinService service)
        {
            _service = service;
        }

        // Load a drink by id and prepare the detail model
        public async IAsyncEnumerable<DrinkDetailModel> LoadDrink(
            string drinkId,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            // Initial busy state
            var model = DrinkDetailModel.Default with { IsBusy = true };
            yield return model;

            // We use GetDrinksByCategory(null) to get all drinks
            var all = _service.GetDrinksByCategory(null);
            var selected = all.FirstOrDefault(d => d.Id == drinkId);

            if (selected is null)
            {
                // Not found: just stop being busy
                model = model with { IsBusy = false };
                yield return model;
                yield break;
            }

            model = model with
            {
                IsBusy = false,
                SelectedDrink = selected,
                IsRunning = false,
                RemainingSeconds = selected.DurationMinutes * 60,
                CurrentStepIndex = 0
            };

            yield return model;

            await Task.CompletedTask;
        }

        // Start the ritual timer
        public async IAsyncEnumerable<DrinkDetailModel> StartRitual(
            DrinkDetailModel current,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (current.SelectedDrink is null)
            {
                yield return current;
                yield break;
            }

            var model = current with { IsRunning = true };
            yield return model;

            for (int i = model.RemainingSeconds; i >= 0; i--)
            {
                if (ct.IsCancellationRequested)
                {
                    yield break;
                }

                model = model with { RemainingSeconds = i };
                yield return model;

                await Task.Delay(1000, ct);
            }

            model = model with { IsRunning = false, RemainingSeconds = 0 };
            yield return model;
        }

        // Move to the next step in the ritual
        public async IAsyncEnumerable<DrinkDetailModel> AdvanceStep(
            DrinkDetailModel current,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            var drink = current.SelectedDrink;
            if (drink is null || drink.Steps is null || drink.Steps.Count == 0)
            {
                yield return current;
                yield break;
            }

            var nextIndex = current.CurrentStepIndex + 1;
            if (nextIndex >= drink.Steps.Count)
            {
                nextIndex = drink.Steps.Count - 1;
            }

            var model = current with { CurrentStepIndex = nextIndex };
            yield return model;

            await Task.CompletedTask;
        }
    }
}
