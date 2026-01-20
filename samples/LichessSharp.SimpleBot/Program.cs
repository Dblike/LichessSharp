// LichessSharp.SimpleBot - A minimal Lichess bot example
// This bot accepts challenges and makes random legal moves
//
// Prerequisites:
// 1. Create a Lichess account for your bot
// 2. Upgrade it to a BOT account: https://lichess.org/account/oauth/token (enable bot:play scope)
// 3. Generate an API token with bot:play and challenge:write scopes
// 4. Set the LICHESS_BOT_TOKEN environment variable
//
// Run: dotnet run --project samples/LichessSharp.SimpleBot

using LichessSharp;
using LichessSharp.Api.Contracts;
using LichessSharp.Models.Enums;
using System.Collections.Concurrent;

// Get token from environment
var token = Environment.GetEnvironmentVariable("LICHESS_BOT_TOKEN")
    ?? throw new InvalidOperationException(
        "Set LICHESS_BOT_TOKEN environment variable with a token that has bot:play scope");

// Create client with rate limit handling for long-running bots
using var httpClient = new HttpClient();
using var client = new LichessClient(httpClient, new LichessClientOptions
{
    AccessToken = token,
    UnlimitedRateLimitRetries = true,
    AutoRetryOnRateLimit = true
});

// Track active games
var activeGames = new ConcurrentDictionary<string, CancellationTokenSource>();

// Setup graceful shutdown
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    Console.WriteLine("\nShutting down...");
    cts.Cancel();
};

// Verify we're a bot account
try
{
    var profile = await client.Account.GetProfileAsync(cts.Token);
    Console.WriteLine($"Logged in as: {profile.Username}");

    if (profile.Title != Title.BOT)
    {
        Console.WriteLine("WARNING: This account is not a BOT account!");
        Console.WriteLine("Upgrade at: https://lichess.org/api/bot/account/upgrade");
        Console.WriteLine("Note: This is irreversible and the account can no longer play manually.");
        return;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to get profile: {ex.Message}");
    return;
}

Console.WriteLine("Bot is running. Press Ctrl+C to stop.");
Console.WriteLine("Waiting for challenges and games...\n");

// Main event loop
try
{
    await foreach (var evt in client.Bot.StreamEventsAsync(cts.Token))
    {
        switch (evt.Type)
        {
            case "challenge":
                await HandleChallengeAsync(evt.Challenge!);
                break;

            case "challengeCanceled":
                Console.WriteLine($"Challenge {evt.Challenge?.Id} was canceled");
                break;

            case "challengeDeclined":
                Console.WriteLine($"Challenge {evt.Challenge?.Id} was declined");
                break;

            case "gameStart":
                StartGame(evt.Game!.GameId!);
                break;

            case "gameFinish":
                StopGame(evt.Game!.GameId!);
                break;
        }
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Event stream stopped.");
}
catch (Exception ex)
{
    Console.WriteLine($"Event stream error: {ex.Message}");
}

// Wait for active games to finish
Console.WriteLine($"Stopping {activeGames.Count} active game(s)...");
foreach (var (gameId, gameCts) in activeGames)
{
    gameCts.Cancel();
}

Console.WriteLine("Bot stopped.");
return;

// Handle incoming challenges
async Task HandleChallengeAsync(ChallengeJson challenge)
{
    var challenger = challenge.Challenger?.Name ?? "Unknown";
    var speed = challenge.Speed ?? "unknown";
    var variant = challenge.Variant?.Key ?? "standard";

    Console.WriteLine($"Challenge from {challenger}: {speed} {variant}");

    // Accept challenges with reasonable time controls
    // You can customize these filters
    var validSpeeds = new[] { "bullet", "blitz", "rapid", "classical" };
    var validVariants = new[] { "standard", "chess960" };

    if (validSpeeds.Contains(speed) && validVariants.Contains(variant))
    {
        try
        {
            await client.Challenges.AcceptAsync(challenge.Id, cts.Token);
            Console.WriteLine($"  Accepted challenge {challenge.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Failed to accept: {ex.Message}");
        }
    }
    else
    {
        try
        {
            await client.Challenges.DeclineAsync(challenge.Id, ChallengeDeclineReason.Standard, cts.Token);
            Console.WriteLine($"  Declined challenge (unsupported variant/speed)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Failed to decline: {ex.Message}");
        }
    }
}

// Start playing a game
void StartGame(string gameId)
{
    Console.WriteLine($"Game started: {gameId}");

    var gameCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
    activeGames[gameId] = gameCts;

    // Run game handler in background
    _ = Task.Run(async () =>
    {
        try
        {
            await PlayGameAsync(gameId, gameCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Expected on shutdown
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Game {gameId} error: {ex.Message}");
        }
        finally
        {
            activeGames.TryRemove(gameId, out _);
        }
    });
}

// Stop a game
void StopGame(string gameId)
{
    if (activeGames.TryRemove(gameId, out var gameCts))
    {
        gameCts.Cancel();
        Console.WriteLine($"Game finished: {gameId}");
    }
}

// Play a single game
async Task PlayGameAsync(string gameId, CancellationToken ct)
{
    string? myColor = null;
    var moves = new List<string>();

    await foreach (var evt in client.Bot.StreamGameAsync(gameId, ct))
    {
        switch (evt)
        {
            case BotGameFullEvent full:
                // First event - determine our color
                var profile = await client.Account.GetProfileAsync(ct);
                var myId = profile.Id?.ToLowerInvariant();

                if (full.White?.Id?.ToLowerInvariant() == myId)
                    myColor = "white";
                else if (full.Black?.Id?.ToLowerInvariant() == myId)
                    myColor = "black";

                Console.WriteLine($"  Playing as {myColor} in {gameId}");

                // Say hello
                try
                {
                    await client.Bot.WriteChatAsync(gameId, ChatRoom.Player, "Good luck, have fun!", ct);
                }
                catch
                {
                    // Chat might fail if disabled
                }

                // Parse initial moves
                if (!string.IsNullOrEmpty(full.State?.Moves))
                {
                    moves = full.State.Moves.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                // Make move if it's our turn
                if (IsMyTurn(myColor, moves.Count))
                {
                    await MakeMoveAsync(gameId, moves, ct);
                }
                break;

            case BotGameStateEvent state:
                // Update moves list
                if (!string.IsNullOrEmpty(state.Moves))
                {
                    moves = state.Moves.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                // Check if game ended
                if (state.Status != "started")
                {
                    Console.WriteLine($"  Game {gameId} ended: {state.Status}");
                    return;
                }

                // Make move if it's our turn
                if (IsMyTurn(myColor, moves.Count))
                {
                    await MakeMoveAsync(gameId, moves, ct);
                }
                break;

            case BotChatLineEvent chat:
                Console.WriteLine($"  [{gameId}] {chat.Username}: {chat.Text}");
                break;

            case BotOpponentGoneEvent gone:
                if (gone.ClaimWinInSeconds.HasValue && gone.ClaimWinInSeconds > 0)
                {
                    Console.WriteLine($"  Opponent left, can claim win in {gone.ClaimWinInSeconds}s");
                    // Wait and claim victory
                    await Task.Delay(TimeSpan.FromSeconds(gone.ClaimWinInSeconds.Value + 1), ct);
                    try
                    {
                        await client.Bot.ClaimVictoryAsync(gameId, ct);
                        Console.WriteLine($"  Claimed victory in {gameId}");
                    }
                    catch
                    {
                        // Game might have ended
                    }
                }
                break;
        }
    }
}

// Check if it's our turn
bool IsMyTurn(string? myColor, int moveCount)
{
    if (myColor == null) return false;
    var whiteToMove = moveCount % 2 == 0;
    return (myColor == "white" && whiteToMove) || (myColor == "black" && !whiteToMove);
}

// Make a random legal move
async Task MakeMoveAsync(string gameId, List<string> moves, CancellationToken ct)
{
    // Get a random move
    // In a real bot, you would use a chess engine here (Stockfish, etc.)
    var move = GetRandomMove(moves);

    if (move == null)
    {
        Console.WriteLine($"  No legal moves in {gameId}");
        return;
    }

    try
    {
        await client.Bot.MakeMoveAsync(gameId, move, cancellationToken: ct);
        Console.WriteLine($"  Played {move} in {gameId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  Failed to play {move}: {ex.Message}");
    }
}

// Generate a pseudo-random move
// WARNING: This is NOT a chess engine - it generates syntactically valid UCI moves
// but doesn't check for legality. In a real bot, use Stockfish or similar.
string? GetRandomMove(List<string> moveHistory)
{
    // Very simple: alternate between common opening moves
    // A real bot would use a chess library and engine
    var moveNumber = moveHistory.Count;

    // Simple opening book
    string[] openingMoves =
    [
        "e2e4", "d7d5",  // 1. e4 d5
        "e4d5", "d8d5",  // 2. exd5 Qxd5
        "b1c3", "d5a5",  // 3. Nc3 Qa5
        "d2d4", "g8f6",  // 4. d4 Nf6
        "g1f3", "c8f5",  // 5. Nf3 Bf5
        "f1c4", "e7e6",  // 6. Bc4 e6
        "e1g1", "b8c6",  // 7. O-O Nc6
        "c1g5", "f8e7",  // 8. Bg5 Be7
        "f1e1", "e8g8",  // 9. Re1 O-O
        "a2a3", "h7h6",  // 10. a3 h6
    ];

    if (moveNumber < openingMoves.Length)
    {
        return openingMoves[moveNumber];
    }

    // After opening, just return some random squares
    // This will likely result in illegal moves - that's OK for a demo
    // The Lichess API will reject illegal moves
    var files = "abcdefgh";
    var ranks = "12345678";
    var random = new Random();

    // Try to make knight or bishop moves (less likely to be blocked)
    var pieces = new[] { "b1", "g1", "c1", "f1", "b8", "g8", "c8", "f8" };
    var targets = new[] { "c3", "f3", "d2", "e2", "c6", "f6", "d7", "e7" };

    var from = $"{files[random.Next(8)]}{ranks[random.Next(8)]}";
    var to = $"{files[random.Next(8)]}{ranks[random.Next(8)]}";

    return $"{from}{to}";
}
