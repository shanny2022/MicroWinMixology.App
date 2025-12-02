using MicroWinMixology.App.Models;

namespace MicroWinMixology.App.Presentation.DrinkDetail;

public record DrinkDetailModel(
    bool IsBusy,
    MicroWinDrink? SelectedDrink,
    bool IsRunning,
    int RemainingSeconds,
    int CurrentStepIndex
)
{
    public static DrinkDetailModel Default => new(
        IsBusy: false,
        SelectedDrink: null,
        IsRunning: false,
        RemainingSeconds: 0,
        CurrentStepIndex: 0
    );
}
