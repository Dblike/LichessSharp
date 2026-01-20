// LichessSharp.GameArchiver - Export games to PGN files
//
// This sample demonstrates the Games API:
// - Streaming user games with filters
// - Exporting to PGN format
// - Progress tracking and cancellation
//
// Run: dotnet run --project samples/LichessSharp.GameArchiver -- <username> [options]

using LichessSharp;
using LichessSharp.Api.Options;
using LichessSharp.Models.Enums;

// Parse command line arguments
if (args.Length == 0)
{
    PrintUsage();
    return;
}

var username = args[0];
var options = ParseOptions(args.Skip(1).ToArray());

Console.WriteLine("=== LichessSharp Game Archiver ===\n");
Console.WriteLine($"Exporting games for: {username}");

// Setup client
using var client = new LichessClient();

// Setup cancellation
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    Console.WriteLine("\nCancelling...");
    cts.Cancel();
};

// Build export options
var exportOptions = new ExportUserGamesOptions
{
    Max = options.MaxGames,
    Moves = true,
    Clocks = options.IncludeClocks,
    Evals = options.IncludeEvals,
    Opening = true,
    Tags = true,
    PgnInJson = true,
    Sort = "dateDesc"
};

// Apply filters
if (!string.IsNullOrEmpty(options.Variant))
{
    exportOptions.PerfType = options.Variant;
    Console.WriteLine($"Variant filter: {options.Variant}");
}

if (options.RatedOnly)
{
    exportOptions.Rated = true;
    Console.WriteLine("Rated games only");
}

if (options.SinceMonths.HasValue)
{
    exportOptions.Since = DateTimeOffset.UtcNow.AddMonths(-options.SinceMonths.Value);
    Console.WriteLine($"Since: {exportOptions.Since:yyyy-MM-dd}");
}

Console.WriteLine($"Maximum games: {options.MaxGames ?? -1} (-1 = unlimited)");
Console.WriteLine($"Output file: {options.OutputFile}");
Console.WriteLine();

// Track progress
var gameCount = 0;
var wins = 0;
var losses = 0;
var draws = 0;
var startTime = DateTime.UtcNow;

// Export games
try
{
    await using var writer = new StreamWriter(options.OutputFile);

    await foreach (var game in client.Games.StreamUserGamesAsync(username, exportOptions, cts.Token))
    {
        gameCount++;

        // Track statistics
        var isWhite = game.Players?.White?.User?.Id?.Equals(username, StringComparison.OrdinalIgnoreCase) == true;
        var won = (isWhite && game.Winner == Color.White) || (!isWhite && game.Winner == Color.Black);
        var lost = (isWhite && game.Winner == Color.Black) || (!isWhite && game.Winner == Color.White);

        if (won) wins++;
        else if (lost) losses++;
        else draws++;

        // Write PGN
        if (!string.IsNullOrEmpty(game.Pgn))
        {
            await writer.WriteLineAsync(game.Pgn);
            await writer.WriteLineAsync();
        }

        // Progress output
        if (gameCount % 10 == 0 || gameCount == 1)
        {
            var elapsed = DateTime.UtcNow - startTime;
            var rate = elapsed.TotalSeconds > 0 ? gameCount / elapsed.TotalSeconds : 0;
            Console.Write($"\rExported {gameCount} games ({rate:F1}/s) - W:{wins} L:{losses} D:{draws}     ");
        }

        // Check if we've hit the limit
        if (options.MaxGames.HasValue && gameCount >= options.MaxGames.Value)
        {
            break;
        }
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("=== Export Complete ===");
    Console.WriteLine($"Total games: {gameCount}");
    Console.WriteLine($"Wins: {wins} ({(gameCount > 0 ? 100.0 * wins / gameCount : 0):F1}%)");
    Console.WriteLine($"Losses: {losses} ({(gameCount > 0 ? 100.0 * losses / gameCount : 0):F1}%)");
    Console.WriteLine($"Draws: {draws} ({(gameCount > 0 ? 100.0 * draws / gameCount : 0):F1}%)");
    Console.WriteLine($"Output: {Path.GetFullPath(options.OutputFile)}");
}
catch (OperationCanceledException)
{
    Console.WriteLine($"\nExport cancelled. {gameCount} games written to {options.OutputFile}");
}
catch (Exception ex)
{
    Console.WriteLine($"\nError: {ex.Message}");
}

// === Helper Methods ===

void PrintUsage()
{
    Console.WriteLine("LichessSharp Game Archiver");
    Console.WriteLine();
    Console.WriteLine("Usage: dotnet run --project samples/LichessSharp.GameArchiver -- <username> [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -o, --output <file>      Output PGN file (default: <username>_games.pgn)");
    Console.WriteLine("  -n, --max <count>        Maximum number of games to export");
    Console.WriteLine("  -v, --variant <type>     Filter by variant: bullet, blitz, rapid, classical, etc.");
    Console.WriteLine("  -r, --rated              Only rated games");
    Console.WriteLine("  -m, --months <count>     Only games from the last N months");
    Console.WriteLine("  --clocks                 Include clock times in PGN");
    Console.WriteLine("  --evals                  Include engine evaluations");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run -- DrNykterstein -n 100");
    Console.WriteLine("  dotnet run -- DrNykterstein -v blitz -r -m 3");
    Console.WriteLine("  dotnet run -- DrNykterstein -o magnus_games.pgn --clocks --evals");
}

ExportArgs ParseOptions(string[] args)
{
    var result = new ExportArgs
    {
        OutputFile = $"{username}_games.pgn"
    };

    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i].ToLowerInvariant())
        {
            case "-o":
            case "--output":
                if (i + 1 < args.Length)
                    result.OutputFile = args[++i];
                break;
            case "-n":
            case "--max":
                if (i + 1 < args.Length && int.TryParse(args[++i], out var max))
                    result.MaxGames = max;
                break;
            case "-v":
            case "--variant":
                if (i + 1 < args.Length)
                    result.Variant = args[++i];
                break;
            case "-r":
            case "--rated":
                result.RatedOnly = true;
                break;
            case "-m":
            case "--months":
                if (i + 1 < args.Length && int.TryParse(args[++i], out var months))
                    result.SinceMonths = months;
                break;
            case "--clocks":
                result.IncludeClocks = true;
                break;
            case "--evals":
                result.IncludeEvals = true;
                break;
        }
    }

    return result;
}

class ExportArgs
{
    public string OutputFile { get; set; } = "games.pgn";
    public int? MaxGames { get; set; }
    public string? Variant { get; set; }
    public bool RatedOnly { get; set; }
    public int? SinceMonths { get; set; }
    public bool IncludeClocks { get; set; }
    public bool IncludeEvals { get; set; }
}
