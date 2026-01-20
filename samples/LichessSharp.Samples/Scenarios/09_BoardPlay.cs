using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
///     Sample 08: Board Play
///     Demonstrates how to play games using the Board API.
///     This is for playing with physical boards or third-party clients.
///     Requires authentication with 'board:play' scope.
/// </summary>
public static class BoardPlay
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("08 - Board API (Playing Games)");

        var token = SampleRunner.GetToken();
        if (!SampleRunner.CheckAuthentication(token))
        {
            SampleRunner.PrintWarning("This sample requires authentication with 'board:play' scope.");
            SampleRunner.PrintInfo("Set LICHESS_TEST_TOKEN to run this sample.");
            return;
        }

        using var client = new LichessClient(token);

        // =====================================================================
        // Get Current Profile
        // =====================================================================
        SampleRunner.PrintSubHeader("Account Info");

        var profile = await client.Account.GetProfileAsync();
        Console.WriteLine($"Logged in as: {profile.Username}");

        // =====================================================================
        // Check for Ongoing Games
        // =====================================================================
        SampleRunner.PrintSubHeader("Ongoing Games");

        var ongoingGames = await client.Games.GetOngoingGamesAsync();
        if (ongoingGames.Count > 0)
        {
            Console.WriteLine($"You have {ongoingGames.Count} ongoing game(s):");
            foreach (var game in ongoingGames)
            {
                Console.WriteLine($"  - {game.GameId}: vs {game.Opponent?.Username ?? "Unknown"}");
                Console.WriteLine($"    Your turn: {game.IsMyTurn}, Speed: {game.Speed}");
            }
        }
        else
        {
            Console.WriteLine("No ongoing games.");
        }

        // =====================================================================
        // Stream Account Events
        // =====================================================================
        SampleRunner.PrintSubHeader("Account Event Stream");

        Console.WriteLine("Listening for account events (5 seconds)...");
        Console.WriteLine("(Events: gameStart, gameFinish, challenge, challengeCanceled)");
        Console.WriteLine();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var eventCount = 0;

        try
        {
            await foreach (var accountEvent in client.Board.StreamEventsAsync(cts.Token))
            {
                eventCount++;
                Console.WriteLine($"  Event: {accountEvent.Type}");

                if (accountEvent.Game != null)
                {
                    Console.WriteLine($"    Game: {accountEvent.Game.GameId}");
                    Console.WriteLine($"    Opponent: {accountEvent.Game.Opponent?.Username}");
                    Console.WriteLine($"    My turn: {accountEvent.Game.IsMyTurn}");
                }

                if (accountEvent.Challenge != null)
                {
                    Console.WriteLine($"    Challenge from: {accountEvent.Challenge.Challenger?.Name}");
                    Console.WriteLine($"    Time control: {accountEvent.Challenge.TimeControl?.Show}");
                    Console.WriteLine($"    Variant: {accountEvent.Challenge.Variant?.Key}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected after timeout
        }

        Console.WriteLine($"Received {eventCount} event(s).");

        // =====================================================================
        // Stream a Game (if we have one)
        // =====================================================================
        if (ongoingGames.Count > 0)
        {
            var gameId = ongoingGames[0].GameId;
            SampleRunner.PrintSubHeader($"Stream Game: {gameId}");

            Console.WriteLine("Streaming game state (3 updates or 10 seconds)...");

            using var gameCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var updateCount = 0;

            try
            {
                await foreach (var evt in client.Board.StreamGameAsync(gameId, gameCts.Token))
                {
                    updateCount++;
                    Console.WriteLine($"  Event {updateCount}: {evt.Type}");

                    // The event contains game state info based on type
                    // gameFull = initial state, gameState = updates
                    if (evt.Type == "gameFull")
                        Console.WriteLine("    Initial game state received");
                    else if (evt.Type == "gameState")
                        Console.WriteLine("    Game state update received");

                    if (updateCount >= 3)
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // Expected
            }

            Console.WriteLine($"Received {updateCount} game event(s).");

            // =====================================================================
            // Get Chat History
            // =====================================================================
            SampleRunner.PrintSubHeader("Game Chat");

            try
            {
                var chatMessages = await client.Board.GetChatAsync(gameId);
                if (chatMessages.Count > 0)
                {
                    Console.WriteLine($"Chat messages in game {gameId}:");
                    foreach (var msg in chatMessages.Take(10))
                        Console.WriteLine($"  {msg.User}: {msg.Text}");
                }
                else
                {
                    Console.WriteLine("No chat messages in this game.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not fetch chat: {ex.Message}");
            }
        }

        // =====================================================================
        // Available Board API Methods
        // =====================================================================
        SampleRunner.PrintSubHeader("Available Methods");

        Console.WriteLine("Event streaming:");
        Console.WriteLine("  StreamEventsAsync()     - Stream account events");
        Console.WriteLine("  StreamGameAsync()       - Stream specific game");
        Console.WriteLine();
        Console.WriteLine("Game actions:");
        Console.WriteLine("  MakeMoveAsync()         - Make a move (UCI format: e2e4)");
        Console.WriteLine("  HandleDrawAsync()       - Accept/decline draw offer");
        Console.WriteLine("  HandleTakebackAsync()   - Accept/decline takeback");
        Console.WriteLine("  ResignAsync()           - Resign game");
        Console.WriteLine("  AbortAsync()            - Abort game (first moves only)");
        Console.WriteLine("  ClaimVictoryAsync()     - Claim win if opponent left");
        Console.WriteLine();
        Console.WriteLine("Finding games:");
        Console.WriteLine("  SeekAsync()             - Find random opponent");
        Console.WriteLine();
        Console.WriteLine("Chat:");
        Console.WriteLine("  GetChatAsync()          - Get chat messages");
        Console.WriteLine("  WriteChatAsync()        - Send chat message");

        SampleRunner.PrintSuccess("Board Play sample completed!");
    }
}
