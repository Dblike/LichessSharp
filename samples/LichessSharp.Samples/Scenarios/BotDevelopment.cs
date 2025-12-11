using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
/// Sample 09: Bot Development
/// Demonstrates how to build a Lichess bot using the Bot API.
/// Requires a bot account with 'bot:play' scope.
/// </summary>
public static class BotDevelopment
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("09 - Bot Development");

        var token = SampleRunner.GetToken();
        if (!SampleRunner.CheckAuthentication(token))
        {
            SampleRunner.PrintWarning("This sample requires authentication with 'bot:play' scope.");
            SampleRunner.PrintInfo("Set LICHESS_TEST_TOKEN to run this sample.");
            SampleRunner.PrintInfo("Note: Bot API requires a bot account, not a regular account.");
            ShowBotDevelopmentGuide();
            return;
        }

        using var client = new LichessClient(token);

        // =====================================================================
        // Important: Bot vs Regular Account
        // =====================================================================
        SampleRunner.PrintSubHeader("Bot Accounts");

        Console.WriteLine("IMPORTANT: The Bot API only works with bot accounts!");
        Console.WriteLine();
        Console.WriteLine("To convert your account to a bot:");
        Console.WriteLine("  1. Create a fresh Lichess account for your bot");
        Console.WriteLine("  2. The account must have played ZERO games");
        Console.WriteLine("  3. Call client.Bot.UpgradeAccountAsync()");
        Console.WriteLine("  4. This is IRREVERSIBLE - the account becomes a bot forever");
        Console.WriteLine();

        // Check if this is a bot account
        try
        {
            var profile = await client.Account.GetProfileAsync();
            if (profile.Title == LichessSharp.Models.Enums.Title.BOT)
            {
                SampleRunner.PrintSuccess($"Logged in as bot: {profile.Username}");
            }
            else
            {
                SampleRunner.PrintWarning($"Account '{profile.Username}' is not a bot account.");
                SampleRunner.PrintInfo("The Bot API requires a dedicated bot account.");
            }
        }
        catch (Exception ex)
        {
            SampleRunner.PrintError($"Could not check account: {ex.Message}");
        }

        // =====================================================================
        // Get Online Bots
        // =====================================================================
        SampleRunner.PrintSubHeader("Online Bots");

        Console.WriteLine("Currently online bots (limit 10)...");

        var botCount = 0;
        await foreach (var bot in client.Bot.GetOnlineBotsAsync(count: 10))
        {
            botCount++;
            Console.WriteLine($"  {botCount}. {bot.Username} ({bot.Perfs?.Blitz?.Rating ?? 0} blitz)");
        }

        Console.WriteLine($"Found {botCount} online bots.");

        // Show the development guide
        ShowBotDevelopmentGuide();

        SampleRunner.PrintSuccess("Bot Development sample completed!");
    }

    private static void ShowBotDevelopmentGuide()
    {
        // =====================================================================
        // Bot Architecture
        // =====================================================================
        SampleRunner.PrintSubHeader("Bot Architecture");

        Console.WriteLine("A typical Lichess bot has these components:");
        Console.WriteLine();
        Console.WriteLine("  1. EVENT LOOP - Stream and handle account events");
        Console.WriteLine("     - Receives challenges, game starts/ends");
        Console.WriteLine("     - Main coordination point");
        Console.WriteLine();
        Console.WriteLine("  2. CHALLENGE HANDLER - Accept/decline incoming challenges");
        Console.WriteLine("     - Filter by time control, variant, rating");
        Console.WriteLine("     - Avoid playing too many games at once");
        Console.WriteLine();
        Console.WriteLine("  3. GAME MANAGER - Handle active games");
        Console.WriteLine("     - One streaming connection per game");
        Console.WriteLine("     - Track game state and clocks");
        Console.WriteLine();
        Console.WriteLine("  4. CHESS ENGINE - Calculate moves");
        Console.WriteLine("     - Integrate Stockfish or other UCI engine");
        Console.WriteLine("     - Or implement your own logic");

        // =====================================================================
        // Bot Event Loop
        // =====================================================================
        SampleRunner.PrintSubHeader("Bot Event Loop (Code Example)");

        Console.WriteLine("async Task RunBotAsync(LichessClient client)");
        Console.WriteLine("{");
        Console.WriteLine("    var activeGames = new Dictionary<string, Task>();");
        Console.WriteLine();
        Console.WriteLine("    await foreach (var evt in client.Bot.StreamEventsAsync())");
        Console.WriteLine("    {");
        Console.WriteLine("        switch (evt.Type)");
        Console.WriteLine("        {");
        Console.WriteLine("            case \"challenge\":");
        Console.WriteLine("                await HandleChallengeAsync(client, evt.Challenge);");
        Console.WriteLine("                break;");
        Console.WriteLine();
        Console.WriteLine("            case \"gameStart\":");
        Console.WriteLine("                var gameId = evt.Game.GameId;");
        Console.WriteLine("                activeGames[gameId] = PlayGameAsync(client, gameId);");
        Console.WriteLine("                break;");
        Console.WriteLine();
        Console.WriteLine("            case \"gameFinish\":");
        Console.WriteLine("                activeGames.Remove(evt.Game.GameId);");
        Console.WriteLine("                break;");
        Console.WriteLine("        }");
        Console.WriteLine("    }");
        Console.WriteLine("}");

        // =====================================================================
        // Challenge Handling
        // =====================================================================
        SampleRunner.PrintSubHeader("Challenge Handling");

        Console.WriteLine("async Task HandleChallengeAsync(LichessClient client, ChallengeJson challenge)");
        Console.WriteLine("{");
        Console.WriteLine("    // Accept criteria");
        Console.WriteLine("    var validVariant = challenge.Variant.Key == \"standard\";");
        Console.WriteLine("    var validSpeed = challenge.Speed is \"blitz\" or \"rapid\";");
        Console.WriteLine("    var validRating = challenge.Challenger.Rating is > 1000 and < 2500;");
        Console.WriteLine();
        Console.WriteLine("    if (validVariant && validSpeed && validRating)");
        Console.WriteLine("    {");
        Console.WriteLine("        await client.Challenges.AcceptAsync(challenge.Id);");
        Console.WriteLine("        Console.WriteLine($\"Accepted challenge from {challenge.Challenger.Name}\");");
        Console.WriteLine("    }");
        Console.WriteLine("    else");
        Console.WriteLine("    {");
        Console.WriteLine("        await client.Challenges.DeclineAsync(challenge.Id, \"generic\");");
        Console.WriteLine("        Console.WriteLine($\"Declined challenge from {challenge.Challenger.Name}\");");
        Console.WriteLine("    }");
        Console.WriteLine("}");

        // =====================================================================
        // Playing Games
        // =====================================================================
        SampleRunner.PrintSubHeader("Playing Games as Bot");

        Console.WriteLine("async Task PlayGameAsync(LichessClient client, string gameId)");
        Console.WriteLine("{");
        Console.WriteLine("    string? myColor = null;");
        Console.WriteLine();
        Console.WriteLine("    await foreach (var evt in client.Bot.StreamGameAsync(gameId))");
        Console.WriteLine("    {");
        Console.WriteLine("        if (evt.Type == \"gameFull\")");
        Console.WriteLine("        {");
        Console.WriteLine("            // Determine our color from gameFull event");
        Console.WriteLine("            var gameFull = (GameFullEvent)evt;");
        Console.WriteLine("            myColor = DetermineMyColor(gameFull, botUsername);");
        Console.WriteLine();
        Console.WriteLine("            // Check if we move first (white)");
        Console.WriteLine("            if (myColor == \"white\")");
        Console.WriteLine("                await MakeBotMoveAsync(client, gameId, gameFull.State);");
        Console.WriteLine("        }");
        Console.WriteLine("        else if (evt.Type == \"gameState\")");
        Console.WriteLine("        {");
        Console.WriteLine("            var state = (GameStateEvent)evt;");
        Console.WriteLine();
        Console.WriteLine("            // Game over?");
        Console.WriteLine("            if (state.Status != \"started\")");
        Console.WriteLine("            {");
        Console.WriteLine("                Console.WriteLine($\"Game {gameId} ended: {state.Status}\");");
        Console.WriteLine("                break;");
        Console.WriteLine("            }");
        Console.WriteLine();
        Console.WriteLine("            // Is it our turn?");
        Console.WriteLine("            if (IsMyTurn(state.Moves, myColor))");
        Console.WriteLine("                await MakeBotMoveAsync(client, gameId, state);");
        Console.WriteLine("        }");
        Console.WriteLine("    }");
        Console.WriteLine("}");

        // =====================================================================
        // Engine Integration
        // =====================================================================
        SampleRunner.PrintSubHeader("Chess Engine Integration");

        Console.WriteLine("Most bots integrate a UCI chess engine like Stockfish:");
        Console.WriteLine();
        Console.WriteLine("  1. Start the engine as a subprocess");
        Console.WriteLine("  2. Send 'uci' command and wait for 'uciok'");
        Console.WriteLine("  3. For each position:");
        Console.WriteLine("     - Send 'position startpos moves e2e4 e7e5 ...'");
        Console.WriteLine("     - Send 'go movetime 1000' (think for 1 second)");
        Console.WriteLine("     - Read 'bestmove e2e4' response");
        Console.WriteLine("  4. Send move via client.Bot.MakeMoveAsync()");
        Console.WriteLine();
        Console.WriteLine("Popular .NET UCI libraries:");
        Console.WriteLine("  - Gera.Chess.Core");
        Console.WriteLine("  - ChessDotNet");
        Console.WriteLine("  - Or write your own UCI protocol handler");

        // =====================================================================
        // Bot API Methods
        // =====================================================================
        SampleRunner.PrintSubHeader("Bot API Methods");

        Console.WriteLine("Account management:");
        Console.WriteLine("  Bot.UpgradeAccountAsync()      - Convert account to bot (irreversible!)");
        Console.WriteLine("  Bot.GetOnlineBotsAsync()       - List online bots");
        Console.WriteLine();
        Console.WriteLine("Event streaming:");
        Console.WriteLine("  Bot.StreamEventsAsync()        - Stream account events");
        Console.WriteLine("  Bot.StreamGameAsync(gameId)    - Stream specific game");
        Console.WriteLine();
        Console.WriteLine("Game actions:");
        Console.WriteLine("  Bot.MakeMoveAsync()            - Make a move");
        Console.WriteLine("  Bot.GetChatAsync()             - Get chat messages");
        Console.WriteLine("  Bot.WriteChatAsync()           - Send chat message");
        Console.WriteLine("  Bot.AbortAsync()               - Abort game");
        Console.WriteLine("  Bot.ResignAsync()              - Resign game");
        Console.WriteLine("  Bot.HandleDrawAsync()          - Accept/decline draw");
        Console.WriteLine("  Bot.HandleTakebackAsync()      - Accept/decline takeback");

        // =====================================================================
        // Best Practices
        // =====================================================================
        SampleRunner.PrintSubHeader("Bot Best Practices");

        Console.WriteLine("1. RESOURCE MANAGEMENT");
        Console.WriteLine("   - Limit concurrent games (e.g., max 5-10)");
        Console.WriteLine("   - Use separate tasks/threads per game");
        Console.WriteLine("   - Handle disconnections and reconnect");
        Console.WriteLine();
        Console.WriteLine("2. FAIR PLAY");
        Console.WriteLine("   - Respect opponent's time (don't premove too fast)");
        Console.WriteLine("   - Add small delays to seem more human-like");
        Console.WriteLine("   - Say 'Good luck!' at start, 'Good game!' at end");
        Console.WriteLine();
        Console.WriteLine("3. CHALLENGE FILTERING");
        Console.WriteLine("   - Decline variants you don't support");
        Console.WriteLine("   - Decline if too many games active");
        Console.WriteLine("   - Consider rating range limits");
        Console.WriteLine();
        Console.WriteLine("4. ERROR HANDLING");
        Console.WriteLine("   - Catch and log all exceptions");
        Console.WriteLine("   - Reconnect on connection loss");
        Console.WriteLine("   - Resign gracefully on fatal errors");
    }
}
