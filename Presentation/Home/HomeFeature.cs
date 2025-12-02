using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MicroWinMixology.App.Models;
using MicroWinMixology.App.Services;
using Uno.Extensions.Reactive;

namespace MicroWinMixology.App.Presentation.Home
{
    public partial class HomeFeature : IAsyncEnumerable<IState<HomeModel>>
    {
        private readonly MicroWinService _microWinService;

        public HomeFeature(MicroWinService microWinService)
        {
            _microWinService = microWinService;
        }

        // Messages
        public async IAsyncEnumerable<IState<HomeModel>> LoadHome(
            IState<HomeModel> state,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            // set IsBusy
            yield return state with
            {
                Value = state.Value with { IsBusy = true, ErrorMessage = string.Empty }
            };

            try
            {
                var featured = _microWinService.GetFeaturedDrinks();
                var category = state.Value.SelectedCategory;
                var categoryDrinks = _microWinService.GetDrinksByCategory(category);
                var tasks = await _microWinService.GetRandomMicroTasksAsync(ct);

                yield return state with
                {
                    Value = state.Value with
                    {
                        IsBusy = false,
                        FeaturedDrinks = featured,
                        CategoryDrinks = categoryDrinks,
                        SuggestedTasks = tasks
                    }
                };
            }
            catch (Exception ex)
            {
                yield return state with
                {
                    Value = state.Value with
                    {
                        IsBusy = false,
                        ErrorMessage = ex.Message
                    }
                };
            }
        }

        public async IAsyncEnumerable<IState<HomeModel>> ChangeCategory(
            IState<HomeModel> state,
            string newCategory,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            // Immediately update selected category and set busy flag
            yield return state with
            {
                Value = state.Value with
                {
                    IsBusy = true,
                    SelectedCategory = newCategory,
                    ErrorMessage = string.Empty
                }
            };

            try
            {
                var categoryDrinks = _microWinService.GetDrinksByCategory(newCategory);

                yield return state with
                {
                    Value = state.Value with
                    {
                        IsBusy = false,
                        CategoryDrinks = categoryDrinks
                    }
                };
            }
            catch (Exception ex)
            {
                yield return state with
                {
                    Value = state.Value with
                    {
                        IsBusy = false,
                        ErrorMessage = ex.Message
                    }
                };
            }
        }

        public async IAsyncEnumerable<IState<HomeModel>> RefreshTasks(
            IState<HomeModel> state,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            yield return state with
            {
                Value = state.Value with
                {
                    IsBusy = true,
                    ErrorMessage = string.Empty
                }
            };

            try
            {
                var tasks = await _microWinService.GetRandomMicroTasksAsync(ct);

                yield return state with
                {
                    Value = state.Value with
                    {
                        IsBusy = false,
                        SuggestedTasks = tasks
                    }
                };
            }
            catch (Exception ex)
            {
                yield return state with
                {
                    Value = state.Value with
                    {
                        IsBusy = false,
                        ErrorMessage = ex.Message
                    }
                };
            }
        }
    }
}
