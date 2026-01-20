// LichessSharp.PuzzleSolver - Interactive puzzle solving CLI
//
// This sample demonstrates the Puzzles API:
// - Fetching the daily puzzle
// - Getting puzzles by ID
// - Streaming puzzle activity (authenticated)
// - Viewing puzzle dashboard (authenticated)
//
// Run: dotnet run --project samples/LichessSharp.PuzzleSolver

using LichessSharp;

Console.WriteLine("=== LichessSharp Puzzle Solver ===\n");

// Try to get token for authenticated features (optional)
var token = Environment.GetEnvironmentVariable("LICHESS_TOKEN");
using var client = string.IsNullOrEmpty(token)
    ? new LichessClient()
    : new LichessClient(token);

if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("Running in unauthenticated mode.");
    Console.WriteLine("Set LICHESS_TOKEN for additional features.\n");
}

while (true)
{
    Console.WriteLine("\nChoose an option:");
    Console.WriteLine("  1. Daily Puzzle");
    Console.WriteLine("  2. Get Puzzle by ID");
    Console.WriteLine("  3. Random Puzzle (requires auth)");
    Console.WriteLine("  4. View Dashboard (requires auth)");
    Console.WriteLine("  5. Recent Activity (requires auth)");
    Console.WriteLine("  6. Storm Stats");
    Console.WriteLine("  q. Quit");
    Console.Write("\n> ");

    var choice = Console.ReadLine()?.Trim().ToLowerInvariant();

    try
    {
        switch (choice)
        {
            case "1":
                await ShowDailyPuzzleAsync();
                break;
            case "2":
                await GetPuzzleByIdAsync();
                break;
            case "3":
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("This feature requires authentication.");
                    break;
                }
                await GetRandomPuzzleAsync();
                break;
            case "4":
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("This feature requires authentication.");
                    break;
                }
                await ShowDashboardAsync();
                break;
            case "5":
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("This feature requires authentication.");
                    break;
                }
                await ShowRecentActivityAsync();
                break;
            case "6":
                await ShowStormStatsAsync();
                break;
            case "q":
            case "quit":
            case "exit":
                Console.WriteLine("Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

// === Features ===

async Task ShowDailyPuzzleAsync()
{
    Console.WriteLine("\n--- Daily Puzzle ---\n");

    var daily = await client.Puzzles.GetDailyAsync();
    var puzzle = daily.Puzzle;
    var game = daily.Game;

    Console.WriteLine($"Puzzle ID: {puzzle.Id}");
    Console.WriteLine($"Rating: {puzzle.Rating}");
    Console.WriteLine($"Plays: {puzzle.Plays:N0}");
    Console.WriteLine($"Themes: {string.Join(", ", puzzle.Themes ?? [])}");
    Console.WriteLine();

    // Show the game info
    if (game.Players?.Length >= 2)
    {
        var white = game.Players.FirstOrDefault(p => p.Color == "white");
        var black = game.Players.FirstOrDefault(p => p.Color == "black");
        Console.WriteLine($"From game: {white?.Name ?? "?"} vs {black?.Name ?? "?"}");
    }
    Console.WriteLine($"Game ID: {game.Id}");
    Console.WriteLine();

    // Show the position
    Console.WriteLine("Starting position (PGN available in game.Pgn)");
    Console.WriteLine();

    // Interactive solving
    Console.WriteLine("Solution (spoiler alert!):");
    Console.WriteLine($"  {string.Join(" -> ", puzzle.Solution ?? [])}");
    Console.WriteLine();
    Console.WriteLine($"Play this puzzle: https://lichess.org/training/{puzzle.Id}");
}

async Task GetPuzzleByIdAsync()
{
    Console.Write("\nEnter puzzle ID (e.g., 5ryPB): ");
    var puzzleId = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(puzzleId))
    {
        Console.WriteLine("No puzzle ID provided.");
        return;
    }

    Console.WriteLine($"\n--- Puzzle {puzzleId} ---\n");

    var result = await client.Puzzles.GetAsync(puzzleId);
    var puzzle = result.Puzzle;

    Console.WriteLine($"Rating: {puzzle.Rating}");
    Console.WriteLine($"Plays: {puzzle.Plays:N0}");
    Console.WriteLine($"Themes: {string.Join(", ", puzzle.Themes ?? [])}");
    Console.WriteLine();
    Console.WriteLine("Solution:");
    Console.WriteLine($"  {string.Join(" -> ", puzzle.Solution ?? [])}");
    Console.WriteLine();
    Console.WriteLine($"Play: https://lichess.org/training/{puzzle.Id}");
}

async Task GetRandomPuzzleAsync()
{
    Console.WriteLine("\nDifficulty options: easiest, easier, normal, harder, hardest");
    Console.Write("Enter difficulty (or press Enter for normal): ");
    var difficulty = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(difficulty)) difficulty = "normal";

    Console.WriteLine("\nTheme options: mateIn1, mateIn2, fork, pin, skewer, endgame, opening, etc.");
    Console.Write("Enter theme (or press Enter for mix): ");
    var angle = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(angle)) angle = null;

    Console.WriteLine("\n--- Random Puzzle ---\n");

    var result = await client.Puzzles.GetNextAsync(angle: angle, difficulty: difficulty);
    var puzzle = result.Puzzle;

    Console.WriteLine($"Puzzle ID: {puzzle.Id}");
    Console.WriteLine($"Rating: {puzzle.Rating}");
    Console.WriteLine($"Themes: {string.Join(", ", puzzle.Themes ?? [])}");
    Console.WriteLine();
    Console.WriteLine($"Play: https://lichess.org/training/{puzzle.Id}");
    Console.WriteLine();

    Console.Write("Show solution? (y/n): ");
    if (Console.ReadLine()?.Trim().ToLowerInvariant() == "y")
    {
        Console.WriteLine($"Solution: {string.Join(" -> ", puzzle.Solution ?? [])}");
    }
}

async Task ShowDashboardAsync()
{
    Console.Write("\nDays to analyze (default 30): ");
    var daysStr = Console.ReadLine()?.Trim();
    var days = int.TryParse(daysStr, out var d) ? d : 30;

    Console.WriteLine($"\n--- Puzzle Dashboard ({days} days) ---\n");

    var dashboard = await client.Puzzles.GetDashboardAsync(days);

    Console.WriteLine($"Global Statistics:");
    Console.WriteLine($"  Puzzles attempted: {dashboard.Global?.Count}");
    Console.WriteLine($"  First-try wins: {dashboard.Global?.FirstWins}");
    Console.WriteLine($"  Replay wins: {dashboard.Global?.ReplayWins}");
    Console.WriteLine($"  Performance: {dashboard.Global?.Performance}");
    Console.WriteLine();

    if (dashboard.Themes?.Any() == true)
    {
        Console.WriteLine("By Theme:");
        foreach (var (theme, results) in dashboard.Themes.Take(10))
        {
            var perf = results.Results?.Performance ?? 0;
            var count = results.Results?.Count ?? 0;
            Console.WriteLine($"  {theme,-20} {count,4} puzzles, {perf,4} perf");
        }

        if (dashboard.Themes.Count > 10)
        {
            Console.WriteLine($"  ... and {dashboard.Themes.Count - 10} more themes");
        }
    }
}

async Task ShowRecentActivityAsync()
{
    Console.Write("\nNumber of recent puzzles (default 10): ");
    var countStr = Console.ReadLine()?.Trim();
    var count = int.TryParse(countStr, out var c) ? c : 10;

    Console.WriteLine($"\n--- Recent Activity ---\n");

    var activityCount = 0;
    await foreach (var activity in client.Puzzles.StreamActivityAsync(max: count))
    {
        activityCount++;
        var date = DateTimeOffset.FromUnixTimeMilliseconds(activity.Date);
        var result = activity.Win ? "Solved" : "Failed";
        var rating = activity.Puzzle?.Rating ?? 0;
        var themes = activity.Puzzle?.Themes != null
            ? string.Join(", ", activity.Puzzle.Themes.Take(2))
            : "";

        Console.WriteLine($"{date:yyyy-MM-dd HH:mm} | {activity.Puzzle?.Id,-6} | {rating,4} | {result,-6} | {themes}");
    }

    if (activityCount == 0)
    {
        Console.WriteLine("No recent puzzle activity found.");
    }
}

async Task ShowStormStatsAsync()
{
    Console.Write("\nEnter username (or press Enter for DrNykterstein): ");
    var username = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(username)) username = "DrNykterstein";

    Console.WriteLine($"\n--- Storm Dashboard: {username} ---\n");

    var storm = await client.Puzzles.GetStormDashboardAsync(username, days: 30);

    Console.WriteLine("High Scores:");
    Console.WriteLine($"  All-time: {storm.High?.AllTime}");
    Console.WriteLine($"  Month:    {storm.High?.Month}");
    Console.WriteLine($"  Week:     {storm.High?.Week}");
    Console.WriteLine($"  Day:      {storm.High?.Day}");
    Console.WriteLine();

    if (storm.Days?.Any() == true)
    {
        Console.WriteLine("Recent Days:");
        foreach (var day in storm.Days.Take(7))
        {
            Console.WriteLine($"  {day.Id}: Best {day.Score}, {day.Runs} runs, {day.Time}s total");
        }
    }
}
