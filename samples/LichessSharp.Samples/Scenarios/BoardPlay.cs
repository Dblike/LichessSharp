using LichessSharp.Api.Contracts;
using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
/// Sample 08: Board Play
/// Demonstrates how to play games using the Board API.
/// This is for playing with physical boards or third-party clients.
/// Requires authentication with 'board:play' scope.
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
            ShowConceptualExamples();
            return;
        }

        using var client = new LichessClient(token);

        // =====================================================================
        // Stream Account Events
        // =====================================================================
        SampleRunner.PrintSubHeader("Account Event Stream");

        Console.WriteLine("The Board API works by streaming events from your account.");
        Console.WriteLine("Events include: gameStart, gameFinish, challenge, challengeCanceled");
        Console.WriteLine();
        Console.WriteLine("Listening for account events (5 seconds)...");

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
                    Console.WriteLine($"    Time: {accountEvent.Challenge.TimeControl?.Show}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected after timeout
        }

        Console.WriteLine($"Received {eventCount} events.");

        // =====================================================================
        // Conceptual Game Flow
        // =====================================================================
        ShowConceptualExamples();

        // =====================================================================
        // Create a Seek (Find Random Opponent)
        // =====================================================================
        SampleRunner.PrintSubHeader("Creating Seeks");

        Console.WriteLine("To find a random opponent, create a seek:");
        Console.WriteLine();
        Console.WriteLine("  var options = new SeekOptions");
        Console.WriteLine("  {");
        Console.WriteLine("      Time = 5,        // 5 minutes");
        Console.WriteLine("      Increment = 3,   // 3 second increment");
        Console.WriteLine("      Rated = false,   // Casual game");
        Console.WriteLine("      Variant = \"standard\"");
        Console.WriteLine("  };");
        Console.WriteLine();
        Console.WriteLine("  await foreach (var result in client.Board.SeekAsync(options))");
        Console.WriteLine("  {");
        Console.WriteLine("      Console.WriteLine($\"Game found: {result.Id}\");");
        Console.WriteLine("      // Now stream the game and start playing");
        Console.WriteLine("  }");

        // =====================================================================
        // Chat Functions
        // =====================================================================
        SampleRunner.PrintSubHeader("In-Game Chat");

        Console.WriteLine("During a game, you can read and write chat messages:");
        Console.WriteLine();
        Console.WriteLine("  // Read chat history");
        Console.WriteLine("  var messages = await client.Board.GetChatAsync(gameId);");
        Console.WriteLine("  foreach (var msg in messages)");
        Console.WriteLine("      Console.WriteLine($\"{msg.User}: {msg.Text}\");");
        Console.WriteLine();
        Console.WriteLine("  // Send a message to player chat");
        Console.WriteLine("  await client.Board.WriteChatAsync(gameId, ChatRoom.Player, \"Good game!\");");
        Console.WriteLine();
        Console.WriteLine("  // Send a message to spectator chat");
        Console.WriteLine("  await client.Board.WriteChatAsync(gameId, ChatRoom.Spectator, \"Hello spectators!\");");

        SampleRunner.PrintSuccess("Board Play sample completed!");
    }

    private static void ShowConceptualExamples()
    {
        // =====================================================================
        // Game Lifecycle
        // =====================================================================
        SampleRunner.PrintSubHeader("Game Lifecycle");

        Console.WriteLine("A typical Board API game flow:");
        Console.WriteLine();
        Console.WriteLine("1. RECEIVE CHALLENGE OR CREATE SEEK");
        Console.WriteLine("   - StreamEventsAsync() receives 'challenge' event");
        Console.WriteLine("   - Or use SeekAsync() to find random opponent");
        Console.WriteLine();
        Console.WriteLine("2. STREAM THE GAME");
        Console.WriteLine("   - When game starts, call StreamGameAsync(gameId)");
        Console.WriteLine("   - First event is 'gameFull' with complete game state");
        Console.WriteLine("   - Subsequent events are 'gameState' with moves and clocks");
        Console.WriteLine();
        Console.WriteLine("3. MAKE MOVES");
        Console.WriteLine("   - When it's your turn, call MakeMoveAsync(gameId, \"e2e4\")");
        Console.WriteLine("   - Moves are in UCI format (e2e4, e7e8q for promotion)");
        Console.WriteLine();
        Console.WriteLine("4. HANDLE GAME END");
        Console.WriteLine("   - 'gameState' with status != 'started' means game over");
        Console.WriteLine("   - StreamEventsAsync() will emit 'gameFinish' event");

        // =====================================================================
        // Playing a Game Example
        // =====================================================================
        SampleRunner.PrintSubHeader("Playing a Game (Code Example)");

        Console.WriteLine("async Task PlayGameAsync(LichessClient client, string gameId)");
        Console.WriteLine("{");
        Console.WriteLine("    await foreach (var evt in client.Board.StreamGameAsync(gameId))");
        Console.WriteLine("    {");
        Console.WriteLine("        if (evt.Type == \"gameFull\")");
        Console.WriteLine("        {");
        Console.WriteLine("            // Initial state - determine our color");
        Console.WriteLine("            Console.WriteLine(\"Game started!\");");
        Console.WriteLine("        }");
        Console.WriteLine("        else if (evt.Type == \"gameState\")");
        Console.WriteLine("        {");
        Console.WriteLine("            // Game update - check if it's our turn");
        Console.WriteLine("            var moves = evt.Moves?.Split(' ') ?? [];");
        Console.WriteLine("            var isWhiteTurn = moves.Length % 2 == 0;");
        Console.WriteLine();
        Console.WriteLine("            if (IsMyTurn(isWhiteTurn, myColor))");
        Console.WriteLine("            {");
        Console.WriteLine("                var move = CalculateBestMove(evt.Moves);");
        Console.WriteLine("                await client.Board.MakeMoveAsync(gameId, move);");
        Console.WriteLine("            }");
        Console.WriteLine();
        Console.WriteLine("            if (evt.Status != \"started\")");
        Console.WriteLine("            {");
        Console.WriteLine("                Console.WriteLine($\"Game over: {evt.Status}\");");
        Console.WriteLine("                break;");
        Console.WriteLine("            }");
        Console.WriteLine("        }");
        Console.WriteLine("    }");
        Console.WriteLine("}");

        // =====================================================================
        // Game Actions
        // =====================================================================
        SampleRunner.PrintSubHeader("Game Actions");

        Console.WriteLine("Available actions during a game:");
        Console.WriteLine();
        Console.WriteLine("  Making moves:");
        Console.WriteLine("    await client.Board.MakeMoveAsync(gameId, \"e2e4\");");
        Console.WriteLine("    await client.Board.MakeMoveAsync(gameId, \"e7e8q\");  // Promotion");
        Console.WriteLine("    await client.Board.MakeMoveAsync(gameId, \"e2e4\", offeringDraw: true);");
        Console.WriteLine();
        Console.WriteLine("  Draw handling:");
        Console.WriteLine("    await client.Board.HandleDrawAsync(gameId, accept: true);   // Accept");
        Console.WriteLine("    await client.Board.HandleDrawAsync(gameId, accept: false);  // Decline");
        Console.WriteLine();
        Console.WriteLine("  Takeback handling:");
        Console.WriteLine("    await client.Board.HandleTakebackAsync(gameId, accept: true);");
        Console.WriteLine();
        Console.WriteLine("  Ending the game:");
        Console.WriteLine("    await client.Board.ResignAsync(gameId);");
        Console.WriteLine("    await client.Board.AbortAsync(gameId);  // First few moves only");
        Console.WriteLine();
        Console.WriteLine("  Special actions:");
        Console.WriteLine("    await client.Board.ClaimVictoryAsync(gameId);  // Opponent abandoned");
        Console.WriteLine("    await client.Board.BerserkAsync(gameId);       // Tournament berserk");

        // =====================================================================
        // Tips
        // =====================================================================
        SampleRunner.PrintSubHeader("Tips for Board API Integration");

        Console.WriteLine("1. Always maintain a streaming connection to StreamEventsAsync()");
        Console.WriteLine("   to receive challenges and game notifications.");
        Console.WriteLine();
        Console.WriteLine("2. When a game starts, open a separate StreamGameAsync() connection");
        Console.WriteLine("   for that specific game.");
        Console.WriteLine();
        Console.WriteLine("3. Handle disconnections gracefully - reconnect and resume.");
        Console.WriteLine();
        Console.WriteLine("4. UCI move format: source + destination + optional promotion");
        Console.WriteLine("   Examples: e2e4, g1f3, e7e8q, a7a1r");
        Console.WriteLine();
        Console.WriteLine("5. Time is reported in centiseconds (100 = 1 second).");
        Console.WriteLine();
        Console.WriteLine("6. The 'board:play' OAuth scope is required for all operations.");
    }
}
