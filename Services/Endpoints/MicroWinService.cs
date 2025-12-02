using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MicroWinMixology.App.Models;

namespace MicroWinMixology.App.Services
{
    public class MicroWinService
    {
        private readonly HttpClient _httpClient;

        // We'll inject HttpClient via constructor later.
        public MicroWinService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<MicroWinDrink> GetFeaturedDrinks()
        {
            // Hard-coded demo drinks for now.
            var allDrinks = GetAllDrinks();
            // Let's say featured drinks are just the first 3 for demo.
            return allDrinks.Take(3).ToList();
        }

        public List<MicroWinDrink> GetDrinksByCategory(string category)
        {
            var allDrinks = GetAllDrinks();
            if (string.IsNullOrWhiteSpace(category))
            {
                return allDrinks;
            }

            return allDrinks.Where(d => d.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<List<MicroWinTask>> GetRandomMicroTasksAsync(CancellationToken ct = default)
        {
            // Use JSONPlaceholder todos as fake tasks.
            // Endpoint: https://jsonplaceholder.typicode.com/todos
            var requestUri = "https://jsonplaceholder.typicode.com/todos";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            using var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            // This matches JSONPlaceholder todo structure
            var todos = JsonSerializer.Deserialize<List<JsonPlaceholderTodo>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<JsonPlaceholderTodo>();

            // Take a handful of not completed tasks as suggestions
            var selected = todos
                .Where(t => !t.Completed)
                .Take(5)
                .ToList();

            var result = selected.Select(t => new MicroWinTask
            {
                Id = t.Id,
                Title = t.Title,
                Completed = t.Completed,
                Source = "JSONPlaceholder"
            }).ToList();

            return result;
        }

        // Private: all drinks definition
        private List<MicroWinDrink> GetAllDrinks()
        {
            return new List<MicroWinDrink>
            {
                new MicroWinDrink
                {
                    Id = "deep-focus-nitro",
                    Name = "Deep Focus Nitro",
                    Tagline = "Reset your brain in 5 minutes",
                    Category = "Focus",
                    Intensity = 3,
                    DurationMinutes = 5,
                    ImageName = "deep_focus_nitro.png",
                    Steps = new List<MicroWinStep>
                    {
                        new MicroWinStep { StepNumber = 1, Title = "Mute the noise", Instruction = "Silence notifications on your phone and computer." },
                        new MicroWinStep { StepNumber = 2, Title = "Micro-cleanup", Instruction = "Clear your desk surface except for what you’re working on." },
                        new MicroWinStep { StepNumber = 3, Title = "10-minute sprint", Instruction = "Set a 10-minute timer and work on a single small task." }
                    }
                },
                new MicroWinDrink
                {
                    Id = "calm-cappuccino",
                    Name = "Calm Cappuccino",
                    Tagline = "Melt tension gently",
                    Category = "Calm",
                    Intensity = 1,
                    DurationMinutes = 5,
                    ImageName = "calm_cappuccino.png",
                    Steps = new List<MicroWinStep>
                    {
                        new MicroWinStep { StepNumber = 1, Title = "Soft breathing", Instruction = "Inhale for 4 seconds, exhale for 6, repeat 10 times." },
                        new MicroWinStep { StepNumber = 2, Title = "Neck release", Instruction = "Slowly roll your shoulders and stretch your neck side to side." },
                        new MicroWinStep { StepNumber = 3, Title = "Hydrate", Instruction = "Sip a glass of water slowly, focusing on each swallow." }
                    }
                },
                new MicroWinDrink
                {
                    Id = "social-espresso-shot",
                    Name = "Social Espresso Shot",
                    Tagline = "A quick connection boost",
                    Category = "Social",
                    Intensity = 2,
                    DurationMinutes = 3,
                    ImageName = "social_espresso_shot.png",
                    Steps = new List<MicroWinStep>
                    {
                        new MicroWinStep { StepNumber = 1, Title = "Send a note", Instruction = "Message someone you appreciate and tell them one specific thing you value about them." },
                        new MicroWinStep { StepNumber = 2, Title = "Engage", Instruction = "Leave a thoughtful, kind comment on someone’s recent work or post." },
                        new MicroWinStep { StepNumber = 3, Title = "Plan a micro-meet", Instruction = "Invite someone to a 10–15 minute catch-up call sometime this week." }
                    }
                }
            };
        }

        // Helper class that matches JSONPlaceholder todo structure
        private class JsonPlaceholderTodo
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public bool Completed { get; set; }
        }
    }
}
