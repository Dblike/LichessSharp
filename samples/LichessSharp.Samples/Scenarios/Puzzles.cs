using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
/// Sample 05: Puzzles
/// Demonstrates how to work with Lichess puzzles.
/// </summary>
public static class Puzzles
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("05 - Puzzles");

        using var client = new LichessClient();
        var token = SampleRunner.GetToken();

        // =====================================================================
        // Get Daily Puzzle
        // =====================================================================
        SampleRunner.PrintSubHeader("Daily Puzzle");

        var daily = await client.Puzzles.GetDailyAsync();
        Console.WriteLine("Today's daily puzzle:");
        SampleRunner.PrintKeyValue("Puzzle ID", daily.Puzzle?.Id);
        SampleRunner.PrintKeyValue("Rating", daily.Puzzle?.Rating);
        SampleRunner.PrintKeyValue("Plays", daily.Puzzle?.Plays);
        SampleRunner.PrintKeyValue("Themes", string.Join(", ", daily.Puzzle?.Themes ?? []));
        Console.WriteLine();
        Console.WriteLine($"  Game ID: {daily.Game?.Id}");
        Console.WriteLine($"  Solution: {string.Join(" ", daily.Puzzle?.Solution ?? [])}");

        // =====================================================================
        // Get Puzzle by ID
        // =====================================================================
        SampleRunner.PrintSubHeader("Get Puzzle by ID");

        // Use the daily puzzle ID or a known puzzle ID
        var puzzleId = daily.Puzzle?.Id ?? "K69di";

        try
        {
            var puzzle = await client.Puzzles.GetAsync(puzzleId);
            Console.WriteLine($"Puzzle {puzzleId}:");
            SampleRunner.PrintKeyValue("Rating", puzzle.Puzzle?.Rating);
            SampleRunner.PrintKeyValue("Plays", puzzle.Puzzle?.Plays);
            SampleRunner.PrintKeyValue("Themes", string.Join(", ", puzzle.Puzzle?.Themes ?? []));

            if (puzzle.Game != null)
            {
                Console.WriteLine("  From game:");
                SampleRunner.PrintKeyValue("Game ID", puzzle.Game.Id);
                if (puzzle.Game.Players != null)
                {
                    var white = puzzle.Game.Players.FirstOrDefault(p => p.Color == "white");
                    var black = puzzle.Game.Players.FirstOrDefault(p => p.Color == "black");
                    Console.WriteLine($"    White: {white?.Name ?? "Unknown"}");
                    Console.WriteLine($"    Black: {black?.Name ?? "Unknown"}");
                }
            }
        }
        catch (Exception ex)
        {
            SampleRunner.PrintError($"Could not fetch puzzle: {ex.Message}");
        }

        // =====================================================================
        // Get Puzzles in Batch (Authenticated)
        // =====================================================================
        SampleRunner.PrintSubHeader("Get Puzzles in Batch");

        if (!string.IsNullOrEmpty(token))
        {
            using var authClient = new LichessClient(token);
            Console.WriteLine("Fetching a batch of puzzles with 'fork' theme...");

            try
            {
                // GetBatchAsync requires authentication and fetches puzzles by theme
                var batch = await authClient.Puzzles.GetBatchAsync("fork", nb: 3);
                Console.WriteLine($"Retrieved {batch.Puzzles?.Count ?? 0} puzzles:");
                if (batch.Puzzles != null)
                {
                    foreach (var p in batch.Puzzles)
                    {
                        Console.WriteLine($"  - {p.Puzzle?.Id}: Rating {p.Puzzle?.Rating}, Themes: {string.Join(", ", p.Puzzle?.Themes?.Take(3) ?? [])}");
                    }
                }
            }
            catch (Exception ex)
            {
                SampleRunner.PrintError($"Could not fetch batch: {ex.Message}");
            }
        }
        else
        {
            SampleRunner.PrintInfo("Skipping batch fetch (requires authentication)");
        }

        // =====================================================================
        // Puzzle Dashboard (Authenticated)
        // =====================================================================
        SampleRunner.PrintSubHeader("Puzzle Dashboard");
        if (!string.IsNullOrEmpty(token))
        {
            using var authClient = new LichessClient(token);
            try
            {
                var dashboard = await authClient.Puzzles.GetDashboardAsync(days: 30);
                Console.WriteLine("Your puzzle stats (last 30 days):");
                if (dashboard.Global != null)
                {
                    SampleRunner.PrintKeyValue("Total puzzles", dashboard.Global.Count);
                    SampleRunner.PrintKeyValue("First tries", dashboard.Global.FirstWins);
                    SampleRunner.PrintKeyValue("Performance", dashboard.Global.Performance);
                }

                // Show theme performance
                if (dashboard.Themes != null && dashboard.Themes.Count > 0)
                {
                    Console.WriteLine("  Theme performance (sample):");
                    foreach (var (theme, stats) in dashboard.Themes.Take(5))
                    {
                        Console.WriteLine($"    {theme}: {stats.Results?.Count ?? 0} puzzles");
                    }
                }
            }
            catch (Exception ex)
            {
                SampleRunner.PrintError($"Could not fetch dashboard: {ex.Message}");
            }
        }
        else
        {
            SampleRunner.PrintInfo("Skipping dashboard (requires authentication)");
        }

        // =====================================================================
        // Stream Puzzle Activity (Authenticated)
        // =====================================================================
        SampleRunner.PrintSubHeader("Puzzle Activity Stream");

        if (!string.IsNullOrEmpty(token))
        {
            using var authClient = new LichessClient(token);
            Console.WriteLine("Streaming your recent puzzle activity (last 5)...");

            var activityCount = 0;
            try
            {
                await foreach (var activity in authClient.Puzzles.StreamActivityAsync(max: 5))
                {
                    activityCount++;
                    var result = activity.Win ? "Solved" : "Failed";
                    Console.WriteLine($"  {activityCount}. Puzzle {activity.Puzzle?.Id}: {result} (Rating: {activity.Puzzle?.Rating})");
                }
            }
            catch (Exception ex)
            {
                SampleRunner.PrintError($"Could not stream activity: {ex.Message}");
            }

            if (activityCount == 0)
            {
                Console.WriteLine("  No recent puzzle activity found.");
            }
        }
        else
        {
            SampleRunner.PrintInfo("Skipping activity stream (requires authentication)");
        }

        // =====================================================================
        // Storm Dashboard (Authenticated)
        // =====================================================================
        SampleRunner.PrintSubHeader("Puzzle Storm Dashboard");

        if (!string.IsNullOrEmpty(token))
        {
            using var authClient = new LichessClient(token);
            try
            {
                var storm = await authClient.Puzzles.GetStormDashboardAsync("me", days: 30);
                Console.WriteLine("Your Puzzle Storm stats:");
                if (storm.High != null)
                {
                    SampleRunner.PrintKeyValue("All-time high", storm.High.AllTime);
                    SampleRunner.PrintKeyValue("Month high", storm.High.Month);
                    SampleRunner.PrintKeyValue("Week high", storm.High.Week);
                    SampleRunner.PrintKeyValue("Day high", storm.High.Day);
                }
                SampleRunner.PrintKeyValue("Days played", storm.Days?.Count ?? 0);
            }
            catch (Exception ex)
            {
                SampleRunner.PrintError($"Could not fetch storm dashboard: {ex.Message}");
            }
        }
        else
        {
            SampleRunner.PrintInfo("Skipping storm dashboard (requires authentication)");
        }

        // =====================================================================
        // Puzzle Themes
        // =====================================================================
        SampleRunner.PrintSubHeader("Common Puzzle Themes");

        Console.WriteLine("Common puzzle themes you can filter by:");
        Console.WriteLine();
        Console.WriteLine("  Tactical:");
        Console.WriteLine("    fork, pin, skewer, discoveredAttack, doubleCheck");
        Console.WriteLine("    sacrifice, deflection, decoy, interference");
        Console.WriteLine();
        Console.WriteLine("  Mating Patterns:");
        Console.WriteLine("    mate, mateIn1, mateIn2, mateIn3, mateIn4, mateIn5");
        Console.WriteLine("    backRankMate, smotheredMate, anastasiaMate, arabianMate");
        Console.WriteLine();
        Console.WriteLine("  Endgame:");
        Console.WriteLine("    endgame, pawnEndgame, rookEndgame, bishopEndgame");
        Console.WriteLine("    knightEndgame, queenEndgame, queenRookEndgame");
        Console.WriteLine();
        Console.WriteLine("  Game Phase:");
        Console.WriteLine("    opening, middlegame, endgame, veryLong, long, short");

        SampleRunner.PrintSuccess("Puzzles sample completed!");
    }
}
