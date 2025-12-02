using System.Collections.Generic;
using MicroWinMixology.App.Models;


namespace MicroWinMixology.App.Presentation.Home;

public record HomeModel(
    bool IsBusy,
    string SelectedCategory,
    List<MicroWinDrink> Featured,
    List<MicroWinDrink> CategoryDrinks,
    List<MicroWinTask> Tasks,
    string Error
)
{
    public static HomeModel Empty =>
        new(
            IsBusy: false,
            SelectedCategory: "Focus",
            Featured: new List<MicroWinDrink>(),
            CategoryDrinks: new List<MicroWinDrink>(),
            Tasks: new List<MicroWinTask>(),
            Error: string.Empty
        );
}
