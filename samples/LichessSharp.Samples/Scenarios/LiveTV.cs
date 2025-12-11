using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
/// Sample 04: Live TV
/// Demonstrates how to watch live games on Lichess TV.
/// </summary>
public static class LiveTV
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("04 - Live TV");

        using var client = new LichessClient();

        // =====================================================================
        // Get Current TV Games
        // =====================================================================
        SampleRunner.PrintSubHeader("Current TV Games (All Channels)");

        var tvGames = await client.Tv.GetCurrentGamesAsync();
        Console.WriteLine("Current featured games on each channel:");

        if (tvGames.Bullet != null)
            PrintTvGame("Bullet", tvGames.Bullet);
        if (tvGames.Blitz != null)
            PrintTvGame("Blitz", tvGames.Blitz);
        if (tvGames.Rapid != null)
            PrintTvGame("Rapid", tvGames.Rapid);
        if (tvGames.Classical != null)
            PrintTvGame("Classical", tvGames.Classical);
        if (tvGames.UltraBullet != null)
            PrintTvGame("UltraBullet", tvGames.UltraBullet);
        if (tvGames.Chess960 != null)
            PrintTvGame("Chess960", tvGames.Chess960);
        if (tvGames.Computer != null)
            PrintTvGame("Computer", tvGames.Computer);
        if (tvGames.Bot != null)
            PrintTvGame("Bot", tvGames.Bot);

        // =====================================================================
        // Stream Current TV Game
        // =====================================================================
        SampleRunner.PrintSubHeader("Stream Current TV Game");

        Console.WriteLine("Streaming the current featured game (5 updates)...");
        Console.WriteLine("(The stream shows FEN positions and last moves)");
        Console.WriteLine();

        var updateCount = 0;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            await foreach (var tvEvent in client.Tv.StreamCurrentGameAsync(cts.Token))
            {
                updateCount++;

                // Display the update
                if (!string.IsNullOrEmpty(tvEvent.Data?.Fen))
                {
                    Console.WriteLine($"  Update {updateCount}:");
                    Console.WriteLine($"    FEN: {tvEvent.Data.Fen}");
                    if (!string.IsNullOrEmpty(tvEvent.Data.LastMove))
                        Console.WriteLine($"    Last move: {tvEvent.Data.LastMove}");
                    if (tvEvent.Data.WhiteClock.HasValue && tvEvent.Data.BlackClock.HasValue)
                        Console.WriteLine($"    Time: W {tvEvent.Data.WhiteClock}s / B {tvEvent.Data.BlackClock}s");
                }

                // Featured game info (first event typically contains this)
                if (tvEvent.Data?.Players != null && updateCount == 1)
                {
                    Console.WriteLine("    Players:");
                    foreach (var player in tvEvent.Data.Players)
                    {
                        Console.WriteLine($"      {player.Color}: {player.User?.Name ?? "Unknown"} ({player.Rating})");
                    }
                }

                if (updateCount >= 5)
                {
                    Console.WriteLine();
                    Console.WriteLine("  (Stopping after 5 updates)");
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            SampleRunner.PrintInfo("Stream timed out (no updates within 30 seconds)");
        }

        Console.WriteLine($"Received {updateCount} updates.");

        // =====================================================================
        // Stream Specific Channel
        // =====================================================================
        SampleRunner.PrintSubHeader("Stream Specific TV Channel");

        Console.WriteLine("Streaming Blitz TV channel (3 updates)...");

        updateCount = 0;
        using var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            await foreach (var channelEvent in client.Tv.StreamChannelAsync("blitz", cts2.Token))
            {
                updateCount++;
                Console.WriteLine($"  Blitz update {updateCount}: {channelEvent.Data?.Fen?[..30] ?? "..."}...");

                if (updateCount >= 3)
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            SampleRunner.PrintInfo("Channel stream timed out");
        }

        // =====================================================================
        // Stream Channel Games
        // =====================================================================
        SampleRunner.PrintSubHeader("Stream Channel Games");

        Console.WriteLine("Getting recent games from the Bullet TV channel (3 games)...");

        var gameCount = 0;
        await foreach (var channelGame in client.Tv.StreamChannelGamesAsync("bullet"))
        {
            gameCount++;

            var white = channelGame.Players?.White?.User?.Name ?? "Unknown";
            var black = channelGame.Players?.Black?.User?.Name ?? "Unknown";
            Console.WriteLine($"  {gameCount}. {white} vs {black} ({channelGame.Status})");

            if (gameCount >= 3)
                break;
        }

        // =====================================================================
        // Tips for Live TV Integration
        // =====================================================================
        SampleRunner.PrintSubHeader("Integration Tips");

        Console.WriteLine("When building a live TV feature:");
        Console.WriteLine();
        Console.WriteLine("1. Use StreamCurrentGameAsync() for the main featured game");
        Console.WriteLine("2. Use GetCurrentGamesAsync() to show all channels");
        Console.WriteLine("3. Use StreamChannelAsync() for specific variant channels");
        Console.WriteLine("4. Always handle cancellation for long-running streams");
        Console.WriteLine("5. The FEN string can be rendered using any chess board library");
        Console.WriteLine("6. Use the LastMove field to highlight the most recent move");
        Console.WriteLine();
        Console.WriteLine("Available TV channels:");
        Console.WriteLine("  bullet, blitz, rapid, classical, ultraBullet,");
        Console.WriteLine("  chess960, crazyhouse, antichess, atomic, horde,");
        Console.WriteLine("  kingOfTheHill, racingKings, threeCheck, computer, bot");

        SampleRunner.PrintSuccess("Live TV sample completed!");
    }

    private static void PrintTvGame(string channel, LichessSharp.Api.Contracts.TvGame game)
    {
        var white = game.User?.Name ?? "Unknown";
        Console.WriteLine($"  {channel,-12}: {white} ({game.Rating}) - Game: {game.GameId}");
    }
}
