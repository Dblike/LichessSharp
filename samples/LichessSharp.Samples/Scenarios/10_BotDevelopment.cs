using LichessSharp.Models.Enums;
using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
///     Sample 09: Bot Development
///     Demonstrates how to build a Lichess bot using the Bot API.
///     Requires a bot account with 'bot:play' scope.
/// </summary>
public static class BotDevelopment
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("09 - Bot Development");

        using var client = new LichessClient();

        // =====================================================================
        // Get Online Bots
        // =====================================================================
        SampleRunner.PrintSubHeader("Online Bots");

        Console.WriteLine("Currently online bots (top 15 by blitz rating)...");
        Console.WriteLine();

        var bots = new List<(string Username, int Rating)>();
        await foreach (var bot in client.Bot.GetOnlineBotsAsync(50))
        {
            var blitzRating = bot.Perfs?.Blitz?.Rating ?? 0;
            if (blitzRating > 0)
                bots.Add((bot.Username, blitzRating));
        }

        // Sort by rating and show top 15
        foreach (var bot in bots.OrderByDescending(b => b.Rating).Take(15))
            Console.WriteLine($"  {bot.Username,-20} {bot.Rating,4} blitz");

        Console.WriteLine();
        Console.WriteLine($"Found {bots.Count} online bots with blitz ratings.");

        // =====================================================================
        // Check Authentication
        // =====================================================================
        var token = SampleRunner.GetToken();
        if (string.IsNullOrEmpty(token))
        {
            SampleRunner.PrintInfo("Set LICHESS_TEST_TOKEN to test authenticated Bot API features.");
            ShowAvailableMethods();
            SampleRunner.PrintSuccess("Bot Development sample completed!");
            return;
        }

        using var authClient = new LichessClient(token);

        // =====================================================================
        // Check Account Type
        // =====================================================================
        SampleRunner.PrintSubHeader("Account Check");

        try
        {
            var profile = await authClient.Account.GetProfileAsync();
            Console.WriteLine($"Logged in as: {profile.Username}");

            if (profile.Title == Title.BOT)
            {
                SampleRunner.PrintSuccess("This is a bot account!");

                // Stream bot events briefly
                SampleRunner.PrintSubHeader("Bot Event Stream");
                Console.WriteLine("Listening for bot events (5 seconds)...");

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var eventCount = 0;

                try
                {
                    await foreach (var evt in authClient.Bot.StreamEventsAsync(cts.Token))
                    {
                        eventCount++;
                        Console.WriteLine($"  Event: {evt.Type}");

                        if (evt.Game != null)
                            Console.WriteLine($"    Game: {evt.Game.GameId} vs {evt.Game.Opponent?.Username}");

                        if (evt.Challenge != null)
                            Console.WriteLine($"    Challenge from: {evt.Challenge.Challenger?.Name}");
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected
                }

                Console.WriteLine($"Received {eventCount} event(s).");
            }
            else
            {
                Console.WriteLine("This is NOT a bot account.");
                Console.WriteLine();
                Console.WriteLine("To use the Bot API, you need a dedicated bot account:");
                Console.WriteLine("  1. Create a fresh Lichess account");
                Console.WriteLine("  2. The account must have played ZERO games");
                Console.WriteLine("  3. Call client.Bot.UpgradeAccountAsync()");
                Console.WriteLine("  4. This conversion is IRREVERSIBLE");
            }
        }
        catch (Exception ex)
        {
            SampleRunner.PrintError($"Could not check account: {ex.Message}");
        }

        ShowAvailableMethods();
        SampleRunner.PrintSuccess("Bot Development sample completed!");
    }

    private static void ShowAvailableMethods()
    {
        // =====================================================================
        // Available Bot API Methods
        // =====================================================================
        SampleRunner.PrintSubHeader("Bot API Methods");

        Console.WriteLine("Account:");
        Console.WriteLine("  GetOnlineBotsAsync()       - List online bots (public)");
        Console.WriteLine("  UpgradeAccountAsync()      - Convert to bot (irreversible!)");
        Console.WriteLine();
        Console.WriteLine("Event streaming:");
        Console.WriteLine("  StreamEventsAsync()        - Stream account events");
        Console.WriteLine("  StreamGameAsync(gameId)    - Stream specific game");
        Console.WriteLine();
        Console.WriteLine("Game actions:");
        Console.WriteLine("  MakeMoveAsync()            - Make a move (UCI format)");
        Console.WriteLine("  AbortAsync()               - Abort game");
        Console.WriteLine("  ResignAsync()              - Resign game");
        Console.WriteLine("  HandleDrawAsync()          - Accept/decline draw");
        Console.WriteLine("  HandleTakebackAsync()      - Accept/decline takeback");
        Console.WriteLine();
        Console.WriteLine("Chat:");
        Console.WriteLine("  GetChatAsync()             - Get chat messages");
        Console.WriteLine("  WriteChatAsync()           - Send chat message");
        Console.WriteLine();
        Console.WriteLine("For a complete bot implementation, see:");
        Console.WriteLine("  samples/LichessSharp.SimpleBot/");
    }
}
