using System.Collections.Generic;

namespace MicroWinMixology.App.Models
{
    // One step in a micro ritual
    public class MicroWinStep
    {
        public int StepNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Instruction { get; set; } = string.Empty;
    }

    // The ritual drink
    public class MicroWinDrink
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Tagline { get; set; } = string.Empty;

        // Example: Focus, Calm, Social
        public string Category { get; set; } = string.Empty;

        // 1 to 3, where 3 = most intense
        public int Intensity { get; set; }

        // Duration in minutes for this ritual
        public int DurationMinutes { get; set; }

        // Image file name, example deep_focus_nitro.png
        public string ImageName { get; set; } = string.Empty;

        // Ordered list of steps
        public List<MicroWinStep> Steps { get; set; } = new();
    }

    // A micro task suggestion
    public class MicroWinTask
    {
        public int Id { get; set; }

        // The text of the task
        public string Title { get; set; } = string.Empty;

        public bool Completed { get; set; }

        // For debugging or UI: where it came from
        public string Source { get; set; } = string.Empty;
    }
}
