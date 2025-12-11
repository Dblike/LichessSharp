// LichessSharp Samples
// Interactive sample application demonstrating common API usage patterns.

using LichessSharp.Samples.Scenarios;

Console.WriteLine();
Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║           LichessSharp Sample Application                  ║");
Console.WriteLine("║     Interactive examples for the Lichess .NET API          ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Check for authentication token
var token = Environment.GetEnvironmentVariable("LICHESS_TEST_TOKEN");
if (!string.IsNullOrEmpty(token))
{
    Console.WriteLine("[OK] LICHESS_TEST_TOKEN found - authenticated samples enabled");
}
else
{
    Console.WriteLine("[INFO] LICHESS_TEST_TOKEN not set - some samples will be limited");
    Console.WriteLine("       Set the environment variable to enable all features.");
}
Console.WriteLine();

while (true)
{
    ShowMenu();
    Console.Write("Enter choice (0-11): ");
    var input = Console.ReadLine()?.Trim();

    if (input == "0" || string.IsNullOrEmpty(input))
    {
        Console.WriteLine();
        Console.WriteLine("Goodbye!");
        break;
    }

    Console.Clear();

    try
    {
        await RunSampleAsync(input);
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine($"[ERROR] Sample failed: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"        Inner: {ex.InnerException.Message}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("Press any key to return to menu...");
    Console.ReadKey(intercept: true);
    Console.Clear();
}

static void ShowMenu()
{
    Console.WriteLine("Available Samples:");
    Console.WriteLine("──────────────────────────────────────────────────────────────");
    Console.WriteLine();
    Console.WriteLine("  Getting Started & Basics:");
    Console.WriteLine("    1. Getting Started      - Client setup and configuration");
    Console.WriteLine("    2. User Profiles        - Fetch user data and leaderboards");
    Console.WriteLine("    3. Game Export          - Export and stream games");
    Console.WriteLine();
    Console.WriteLine("  Live Content:");
    Console.WriteLine("    4. Live TV              - Watch live games on Lichess TV");
    Console.WriteLine("    5. Puzzles              - Daily puzzle, dashboard, activity");
    Console.WriteLine("    6. Tournaments          - Arena and Swiss tournaments");
    Console.WriteLine("    7. Broadcasts           - Live event broadcasts");
    Console.WriteLine();
    Console.WriteLine("  Analysis & Data:");
    Console.WriteLine("    8. Chess Analysis       - Cloud eval, opening explorer, tablebase");
    Console.WriteLine();
    Console.WriteLine("  Playing Games (Authenticated):");
    Console.WriteLine("    9. Board Play           - Play with Board API");
    Console.WriteLine("   10. Bot Development      - Build a Lichess bot");
    Console.WriteLine();
    Console.WriteLine("  Best Practices:");
    Console.WriteLine("   11. Error Handling       - Handle errors and edge cases");
    Console.WriteLine();
    Console.WriteLine("    0. Exit");
    Console.WriteLine();
}

static async Task RunSampleAsync(string choice)
{
    switch (choice)
    {
        case "1":
            await GettingStarted.RunAsync();
            break;
        case "2":
            await UserProfiles.RunAsync();
            break;
        case "3":
            await GameExport.RunAsync();
            break;
        case "4":
            await LiveTV.RunAsync();
            break;
        case "5":
            await Puzzles.RunAsync();
            break;
        case "6":
            await Tournaments.RunAsync();
            break;
        case "7":
            await Broadcasts.RunAsync();
            break;
        case "8":
            await ChessAnalysis.RunAsync();
            break;
        case "9":
            await BoardPlay.RunAsync();
            break;
        case "10":
            await BotDevelopment.RunAsync();
            break;
        case "11":
            await ErrorHandling.RunAsync();
            break;
        default:
            Console.WriteLine($"Unknown option: {choice}");
            break;
    }
}
