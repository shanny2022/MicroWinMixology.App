using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MicroWinMixology.App.Models;
using MicroWinMixology.App.Services;
using Uno.Extensions.Reactive;

namespace MicroWinMixology.App.Presentation.DrinkDetail
{
    public partial class DrinkDetailFeature
    {
        private readonly MicroWinService _microWinService;

        public DrinkDetailFeature(MicroWinService microWinService)
        {
            _microWinService = microWinService;
        }

        public async IAsyncEnumerable<IState<DrinkDetailModel>> LoadDrink(
            IState<DrinkDetailModel> state,
            string drinkId,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            yield return state with
            {
                Value = state.Value with
                {
                    IsBusy = true
                }
            };

            try
            {
                // MicroWinService doesn't have a direct GetById, so use GetDrinksByCategory(null)
                var all = _microWinService.GetDrinksByCategory(null);
                var selected = all.FirstOrDefault(d => d.Id == drinkId);

                yield return state with
                {
                    Value = state.Value with
                    {
                        IsBusy = false,
                        SelectedDrink = selected,
                        IsRunning = false,
                        RemainingSeconds = (selected?.DurationMinutes ?? 5) * 60,
                        CurrentStepIndex = 0
                    }
                };
            }
            catch (Exception ex)
            {
                // You might add error state to DrinkDetailModel if needed.
                yield return state;
            }
        }

        public async IAsyncEnumerable<IState<DrinkDetailModel>> StartRitual(
            IState<DrinkDetailModel> state,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (state.Value.SelectedDrink is null)
            {
                yield return state;
                yield break;
            }

            yield return state with
            {
                Value = state.Value with
                {
                    IsRunning = true
                }
            };

            int totalSeconds = state.Value.RemainingSeconds;
            for (int i = totalSeconds; i >= 0; i--)
            {
                if (ct.IsCancellationRequested)
                {
                    yield break;
                }

                yield return state with
                {
                    Value = state.Value with
                    {
                        RemainingSeconds = i
                    }
                };

                await Task.Delay(1000, ct);
            }

            // Ritual complete
            yield return state with
            {
                Value = state.Value with
                {
                    IsRunning = false,
                    RemainingSeconds = 0
                }
            };
        }

        public async IAsyncEnumerable<IState<DrinkDetailModel>> AdvanceStep(
            IState<DrinkDetailModel> state,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            var drink = state.Value.SelectedDrink;
            if (drink is null || drink.Steps is null || drink.Steps.Count == 0)
            {
                yield return state;
                yield break;
            }

            var nextIndex = state.Value.CurrentStepIndex + 1;
            if (nextIndex >= drink.Steps.Count)
            {
                nextIndex = drink.Steps.Count - 1;
            }

            yield return state with
            {
                Value = state.Value with
                {
                    CurrentStepIndex = nextIndex
                }
            };

            await Task.CompletedTask;
        }
    }
}
