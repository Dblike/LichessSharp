// LichessSharp.TvViewer - Live TV streaming viewer
//
// Watch live games from Lichess TV channels in your terminal.
// Displays real-time position updates, moves, and clock times.
//
// Run: dotnet run --project samples/LichessSharp.TvViewer [channel]
// Channels: bullet, blitz, rapid, classical, ultraBullet, chess960, computer, bot

using LichessSharp;

Console.WriteLine("=== LichessSharp TV Viewer ===\n");

using var client = new LichessClient();
using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

// Get channel from args or show menu
var channel = args.Length > 0 ? args[0].ToLowerInvariant() : null;

if (string.IsNullOrEmpty(channel))
{
    await ShowChannelMenuAsync();
}
else
{
    await StreamChannelAsync(channel);
}

async Task ShowChannelMenuAsync()
{
    // Get current TV games to show what's playing
    var tvGames = await client.Tv.GetCurrentGamesAsync();

    Console.WriteLine("Current TV Games:\n");

    var channels = new List<(string Key, string Name, string? Player, int? Rating)>();

    if (tvGames.Bullet != null)
        channels.Add(("bullet", "Bullet", tvGames.Bullet.User?.Name, tvGames.Bullet.Rating));
    if (tvGames.Blitz != null)
        channels.Add(("blitz", "Blitz", tvGames.Blitz.User?.Name, tvGames.Blitz.Rating));
    if (tvGames.Rapid != null)
        channels.Add(("rapid", "Rapid", tvGames.Rapid.User?.Name, tvGames.Rapid.Rating));
    if (tvGames.Classical != null)
        channels.Add(("classical", "Classical", tvGames.Classical.User?.Name, tvGames.Classical.Rating));
    if (tvGames.UltraBullet != null)
        channels.Add(("ultraBullet", "UltraBullet", tvGames.UltraBullet.User?.Name, tvGames.UltraBullet.Rating));
    if (tvGames.Chess960 != null)
        channels.Add(("chess960", "Chess960", tvGames.Chess960.User?.Name, tvGames.Chess960.Rating));
    if (tvGames.Computer != null)
        channels.Add(("computer", "Computer", tvGames.Computer.User?.Name, tvGames.Computer.Rating));
    if (tvGames.Bot != null)
        channels.Add(("bot", "Bot", tvGames.Bot.User?.Name, tvGames.Bot.Rating));

    for (int i = 0; i < channels.Count; i++)
    {
        var (key, name, player, rating) = channels[i];
        var playerInfo = player != null ? $" - {player} ({rating})" : "";
        Console.WriteLine($"  {i + 1}. {name,-12}{playerInfo}");
    }

    Console.WriteLine($"\n  0. Featured Game (all channels)");
    Console.WriteLine($"  q. Quit\n");

    Console.Write("Select channel: ");
    var input = Console.ReadLine()?.Trim().ToLowerInvariant();

    if (input == "q" || input == "quit")
        return;

    if (input == "0" || input == "featured")
    {
        await StreamFeaturedGameAsync();
        return;
    }

    if (int.TryParse(input, out var index) && index > 0 && index <= channels.Count)
    {
        await StreamChannelAsync(channels[index - 1].Key);
        return;
    }

    // Try as channel name
    var matchedChannel = channels.FirstOrDefault(c =>
        c.Key.Equals(input, StringComparison.OrdinalIgnoreCase) ||
        c.Name.Equals(input, StringComparison.OrdinalIgnoreCase));

    if (matchedChannel.Key != null)
    {
        await StreamChannelAsync(matchedChannel.Key);
    }
    else
    {
        Console.WriteLine("Invalid selection.");
    }
}

async Task StreamFeaturedGameAsync()
{
    Console.Clear();
    Console.WriteLine("=== Featured Game ===");
    Console.WriteLine("Streaming the current featured game. Press Ctrl+C to stop.\n");

    var moveCount = 0;
    string? lastFen = null;

    try
    {
        await foreach (var evt in client.Tv.StreamCurrentGameAsync(cts.Token))
        {
            if (evt.Data == null) continue;

            // Only update on new position
            if (evt.Data.Fen != lastFen)
            {
                lastFen = evt.Data.Fen;
                moveCount++;

                // Clear and redraw
                Console.SetCursorPosition(0, 3);

                // Show players (on first update with player info)
                if (evt.Data.Players != null && moveCount == 1)
                {
                    foreach (var player in evt.Data.Players)
                    {
                        var color = player.Color == "white" ? "White" : "Black";
                        Console.WriteLine($"{color}: {player.User?.Name ?? "Unknown"} ({player.Rating})");
                    }
                    Console.WriteLine();
                }

                // Show position
                if (!string.IsNullOrEmpty(evt.Data.Fen))
                    PrintBoard(evt.Data.Fen);

                // Show move info
                Console.WriteLine();
                if (!string.IsNullOrEmpty(evt.Data.LastMove))
                    Console.WriteLine($"Last move: {evt.Data.LastMove}");

                // Show clocks
                if (evt.Data.WhiteClock.HasValue && evt.Data.BlackClock.HasValue)
                {
                    var wTime = FormatTime(evt.Data.WhiteClock.Value);
                    var bTime = FormatTime(evt.Data.BlackClock.Value);
                    Console.WriteLine($"Time: White {wTime} | Black {bTime}");
                }

                Console.WriteLine($"\nUpdates received: {moveCount}");
            }
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("\nStream stopped.");
    }
}

async Task StreamChannelAsync(string channelName)
{
    Console.Clear();
    Console.WriteLine($"=== {channelName.ToUpper()} TV ===");
    Console.WriteLine("Streaming live games. Press Ctrl+C to stop.\n");

    var updateCount = 0;
    string? lastFen = null;

    try
    {
        await foreach (var evt in client.Tv.StreamChannelAsync(channelName, cts.Token))
        {
            if (evt.Data == null) continue;

            // Only update on new position
            if (evt.Data.Fen != lastFen)
            {
                lastFen = evt.Data.Fen;
                updateCount++;

                // Clear and redraw
                Console.SetCursorPosition(0, 3);

                // Show players
                if (evt.Data.Players != null)
                {
                    foreach (var player in evt.Data.Players)
                    {
                        var color = player.Color == "white" ? "White" : "Black";
                        Console.WriteLine($"{color}: {player.User?.Name ?? "Unknown"} ({player.Rating})   ");
                    }
                    Console.WriteLine();
                }

                // Show position
                if (!string.IsNullOrEmpty(evt.Data.Fen))
                    PrintBoard(evt.Data.Fen);

                // Show move info
                Console.WriteLine();
                if (!string.IsNullOrEmpty(evt.Data.LastMove))
                    Console.WriteLine($"Last move: {evt.Data.LastMove}      ");

                // Show clocks
                if (evt.Data.WhiteClock.HasValue && evt.Data.BlackClock.HasValue)
                {
                    var wTime = FormatTime(evt.Data.WhiteClock.Value);
                    var bTime = FormatTime(evt.Data.BlackClock.Value);
                    Console.WriteLine($"Time: White {wTime} | Black {bTime}      ");
                }

                Console.WriteLine($"\nUpdates: {updateCount}      ");
            }
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("\nStream stopped.");
    }
}

void PrintBoard(string fen)
{
    // Parse FEN to display a simple ASCII board
    var position = fen.Split(' ')[0];
    var ranks = position.Split('/');

    Console.WriteLine("  +---+---+---+---+---+---+---+---+");

    for (int r = 0; r < 8; r++)
    {
        Console.Write($"{8 - r} |");
        var rank = ranks[r];
        foreach (char c in rank)
        {
            if (char.IsDigit(c))
            {
                for (int i = 0; i < c - '0'; i++)
                    Console.Write("   |");
            }
            else
            {
                var piece = GetPieceSymbol(c);
                Console.Write($" {piece} |");
            }
        }
        Console.WriteLine();
        Console.WriteLine("  +---+---+---+---+---+---+---+---+");
    }
    Console.WriteLine("    a   b   c   d   e   f   g   h");
}

string GetPieceSymbol(char piece)
{
    return piece switch
    {
        'K' => "K", 'Q' => "Q", 'R' => "R", 'B' => "B", 'N' => "N", 'P' => "P",
        'k' => "k", 'q' => "q", 'r' => "r", 'b' => "b", 'n' => "n", 'p' => "p",
        _ => " "
    };
}

string FormatTime(int seconds)
{
    var ts = TimeSpan.FromSeconds(seconds);
    return ts.TotalHours >= 1
        ? $"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}"
        : $"{ts.Minutes}:{ts.Seconds:D2}";
}
