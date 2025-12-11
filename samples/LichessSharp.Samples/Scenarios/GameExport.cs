using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
/// Sample 03: Game Export
/// Demonstrates how to export and stream games from Lichess.
/// </summary>
public static class GameExport
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("03 - Game Export");

        using var client = new LichessClient();

        // =====================================================================
        // Export a Single Game by ID
        // =====================================================================
        SampleRunner.PrintSubHeader("Export Single Game (JSON)");

        // This is a famous game - use any valid game ID
        const string gameId = "q7ZvsdUF";

        try
        {
            var game = await client.Games.ExportAsync(gameId);
            SampleRunner.PrintKeyValue("Game ID", game.Id);
            SampleRunner.PrintKeyValue("Rated", game.Rated);
            SampleRunner.PrintKeyValue("Variant", game.Variant);
            SampleRunner.PrintKeyValue("Speed", game.Speed);
            SampleRunner.PrintKeyValue("Status", game.Status);
            SampleRunner.PrintKeyValue("Winner", game.Winner?.ToString() ?? "Draw/Ongoing");

            if (game.Players != null)
            {
                Console.WriteLine("  Players:");
                if (game.Players.White?.User != null)
                    Console.WriteLine($"    White: {game.Players.White.User.Name} ({game.Players.White.Rating})");
                if (game.Players.Black?.User != null)
                    Console.WriteLine($"    Black: {game.Players.Black.User.Name} ({game.Players.Black.Rating})");
            }

            if (!string.IsNullOrEmpty(game.Moves))
            {
                var moveCount = game.Moves.Split(' ').Length;
                Console.WriteLine($"  Moves: {moveCount} half-moves");
            }
        }
        catch (Exception ex)
        {
            SampleRunner.PrintError($"Could not fetch game: {ex.Message}");
        }

        // =====================================================================
        // Export Game as PGN
        // =====================================================================
        SampleRunner.PrintSubHeader("Export Game as PGN");

        try
        {
            var pgn = await client.Games.GetPgnAsync(gameId);
            Console.WriteLine("PGN (first 500 chars):");
            Console.WriteLine(pgn.Length > 500 ? pgn[..500] + "..." : pgn);
        }
        catch (Exception ex)
        {
            SampleRunner.PrintError($"Could not fetch PGN: {ex.Message}");
        }

        // =====================================================================
        // Stream User Games
        // =====================================================================
        SampleRunner.PrintSubHeader("Stream User Games");

        Console.WriteLine("Streaming recent games for DrNykterstein (limit 5)...");

        var gameCount = 0;
        var streamOptions = new LichessSharp.Api.Options.ExportUserGamesOptions { Max = 5 };
        await foreach (var userGame in client.Games.StreamUserGamesAsync("DrNykterstein", streamOptions))
        {
            gameCount++;
            Console.WriteLine($"  Game {gameCount}: {userGame.Id} - {userGame.Variant} {userGame.Speed} ({userGame.Status})");
        }
        Console.WriteLine($"Streamed {gameCount} games.");

        // =====================================================================
        // Stream Games with Filters
        // =====================================================================
        SampleRunner.PrintSubHeader("Stream Games with Filters");

        Console.WriteLine("Streaming rated blitz games from last month (limit 3)...");

        var oneMonthAgo = DateTimeOffset.UtcNow.AddMonths(-1);
        gameCount = 0;
        var filteredOptions = new LichessSharp.Api.Options.ExportUserGamesOptions
        {
            Rated = true,
            PerfType = "blitz",
            Since = oneMonthAgo,
            Max = 3
        };
        await foreach (var filteredGame in client.Games.StreamUserGamesAsync("DrNykterstein", filteredOptions))
        {
            gameCount++;
            var opponent = filteredGame.Players?.Black?.User?.Name == "DrNykterstein"
                ? filteredGame.Players?.White?.User?.Name
                : filteredGame.Players?.Black?.User?.Name;
            Console.WriteLine($"  {gameCount}. vs {opponent ?? "Unknown"} - {filteredGame.Status}");
        }
        Console.WriteLine($"Found {gameCount} matching games.");

        // =====================================================================
        // Export Games by Multiple IDs
        // =====================================================================
        SampleRunner.PrintSubHeader("Export Multiple Games by ID");

        // Stream games by providing multiple IDs
        var gameIds = new[] { "q7ZvsdUF", "TJxUmbWK" };
        Console.WriteLine($"Fetching games: {string.Join(", ", gameIds)}");

        gameCount = 0;
        await foreach (var multiGame in client.Games.StreamByIdsAsync(gameIds))
        {
            gameCount++;
            Console.WriteLine($"  {multiGame.Id}: {multiGame.Variant} - {multiGame.Status}");
        }
        Console.WriteLine($"Retrieved {gameCount} games.");

        // =====================================================================
        // Stream with Cancellation
        // =====================================================================
        SampleRunner.PrintSubHeader("Streaming with Cancellation");

        Console.WriteLine("Starting a stream that we'll cancel after 3 games...");

        using var cts = new CancellationTokenSource();
        gameCount = 0;

        try
        {
            await foreach (var cancelGame in client.Games.StreamUserGamesAsync("Hikaru", cancellationToken: cts.Token))
            {
                gameCount++;
                Console.WriteLine($"  Game {gameCount}: {cancelGame.Id}");

                if (gameCount >= 3)
                {
                    Console.WriteLine("  Cancelling stream...");
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException)
        {
            SampleRunner.PrintSuccess("Stream cancelled successfully after 3 games.");
        }

        // =====================================================================
        // Get Ongoing Games (Authenticated)
        // =====================================================================
        SampleRunner.PrintSubHeader("Ongoing Games");

        var token = SampleRunner.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            using var authClient = new LichessClient(token);
            try
            {
                var ongoingGames = await authClient.Games.GetOngoingGamesAsync();
                if (ongoingGames.Count > 0)
                {
                    Console.WriteLine($"You have {ongoingGames.Count} ongoing game(s):");
                    foreach (var ongoing in ongoingGames)
                    {
                        Console.WriteLine($"  - {ongoing.GameId}: vs {ongoing.Opponent?.Username ?? "Unknown"}");
                    }
                }
                else
                {
                    Console.WriteLine("No ongoing games.");
                }
            }
            catch (Exception ex)
            {
                SampleRunner.PrintError($"Could not fetch ongoing games: {ex.Message}");
            }
        }
        else
        {
            SampleRunner.PrintInfo("Skipping ongoing games (requires authentication)");
        }

        SampleRunner.PrintSuccess("Game Export sample completed!");
    }
}
