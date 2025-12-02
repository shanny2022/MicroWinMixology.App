using System.Collections.Generic;
using MicroWinMixology.App.Models;

namespace MicroWinMixology.App.Presentation.Home
{
    public record HomeModel(
        bool IsBusy,
        string SelectedCategory,
        string ErrorMessage,
        IReadOnlyList<MicroWinDrink> FeaturedDrinks,
        IReadOnlyList<MicroWinDrink> CategoryDrinks,
        IReadOnlyList<MicroWinTask> SuggestedTasks
    )
    {
        public static HomeModel Default => new HomeModel(
            IsBusy: false,
            SelectedCategory: "Focus",
            ErrorMessage: string.Empty,
            FeaturedDrinks: new List<MicroWinDrink>(),
            CategoryDrinks: new List<MicroWinDrink>(),
            SuggestedTasks: new List<MicroWinTask>()
        );
    }
}
