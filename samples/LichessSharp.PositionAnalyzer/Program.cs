// LichessSharp.PositionAnalyzer - Chess position analysis CLI
//
// This sample demonstrates the Analysis, Opening Explorer, and Tablebase APIs:
// - Cloud evaluation lookup
// - Opening explorer (Masters, Lichess, Player databases)
// - Endgame tablebase lookup
//
// Run: dotnet run --project samples/LichessSharp.PositionAnalyzer

using LichessSharp;
using LichessSharp.Api.Contracts;

Console.WriteLine("=== LichessSharp Position Analyzer ===\n");

using var client = new LichessClient();

// Default starting position
var currentFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

while (true)
{
    Console.WriteLine($"\nCurrent FEN: {currentFen}\n");
    PrintBoard(currentFen);

    Console.WriteLine("\nOptions:");
    Console.WriteLine("  1. Cloud evaluation");
    Console.WriteLine("  2. Opening explorer (Masters)");
    Console.WriteLine("  3. Opening explorer (Lichess)");
    Console.WriteLine("  4. Opening explorer (Player)");
    Console.WriteLine("  5. Tablebase lookup");
    Console.WriteLine("  6. Enter new FEN");
    Console.WriteLine("  7. Use famous positions");
    Console.WriteLine("  q. Quit");
    Console.Write("\n> ");

    var choice = Console.ReadLine()?.Trim().ToLowerInvariant();

    try
    {
        switch (choice)
        {
            case "1":
                await ShowCloudEvaluationAsync();
                break;
            case "2":
                await ShowMastersExplorerAsync();
                break;
            case "3":
                await ShowLichessExplorerAsync();
                break;
            case "4":
                await ShowPlayerExplorerAsync();
                break;
            case "5":
                await ShowTablebaseAsync();
                break;
            case "6":
                EnterNewFen();
                break;
            case "7":
                SelectFamousPosition();
                break;
            case "q":
            case "quit":
            case "exit":
                Console.WriteLine("Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

// === Features ===

async Task ShowCloudEvaluationAsync()
{
    Console.WriteLine("\n--- Cloud Evaluation ---\n");

    Console.Write("Number of principal variations (1-5, default 3): ");
    var pvStr = Console.ReadLine()?.Trim();
    var multiPv = int.TryParse(pvStr, out var pv) ? Math.Clamp(pv, 1, 5) : 3;

    var result = await client.Analysis.GetCloudEvaluationAsync(currentFen, multiPv);

    if (result == null)
    {
        Console.WriteLine("Position not found in cloud database.");
        Console.WriteLine("Try a more common position or one that has been analyzed.");
        return;
    }

    Console.WriteLine($"Position: {result.Fen}");
    Console.WriteLine($"Depth: {result.Depth}");
    Console.WriteLine($"Nodes: {result.Knodes:N0}k");
    Console.WriteLine();

    if (result.Pvs?.Any() == true)
    {
        Console.WriteLine("Principal Variations:");
        var pvIndex = 1;
        foreach (var pvar in result.Pvs)
        {
            var evalStr = FormatEvaluation(pvar.Cp, pvar.Mate);
            var moves = pvar.Moves.Split(' ').Take(8);
            Console.WriteLine($"  {pvIndex}. [{evalStr}] {string.Join(" ", moves)}");
            pvIndex++;
        }
    }
}

async Task ShowMastersExplorerAsync()
{
    Console.WriteLine("\n--- Masters Database ---\n");

    var options = new ExplorerOptions
    {
        Moves = 10,
        TopGames = 5
    };

    var result = await client.OpeningExplorer.GetMastersAsync(currentFen, options);
    DisplayExplorerResult(result, "Masters");
}

async Task ShowLichessExplorerAsync()
{
    Console.WriteLine("\n--- Lichess Database ---\n");

    Console.WriteLine("Speed filters (comma-separated): bullet, blitz, rapid, classical");
    Console.Write("Speeds (or Enter for all): ");
    var speedsInput = Console.ReadLine()?.Trim();
    string[]? speeds = string.IsNullOrEmpty(speedsInput)
        ? null
        : speedsInput.Split(',').Select(s => s.Trim()).ToArray();

    Console.WriteLine("\nRating ranges: 400, 1000, 1200, 1400, 1600, 1800, 2000, 2200, 2500");
    Console.Write("Ratings (or Enter for all): ");
    var ratingsInput = Console.ReadLine()?.Trim();
    int[]? ratings = string.IsNullOrEmpty(ratingsInput)
        ? null
        : ratingsInput.Split(',').Select(s => int.TryParse(s.Trim(), out var r) ? r : 0).Where(r => r > 0).ToArray();

    var options = new ExplorerOptions
    {
        Moves = 10,
        RecentGames = 5,
        Speeds = speeds,
        Ratings = ratings
    };

    var result = await client.OpeningExplorer.GetLichessAsync(currentFen, options);
    DisplayExplorerResult(result, "Lichess");
}

async Task ShowPlayerExplorerAsync()
{
    Console.WriteLine("\n--- Player Database ---\n");

    Console.Write("Enter username: ");
    var username = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(username))
    {
        Console.WriteLine("Username required.");
        return;
    }

    Console.Write("Color (white/black): ");
    var color = Console.ReadLine()?.Trim().ToLowerInvariant();
    if (color != "white" && color != "black")
    {
        Console.WriteLine("Invalid color. Using 'white'.");
        color = "white";
    }

    var options = new ExplorerOptions
    {
        Color = color,
        Moves = 10,
        RecentGames = 5
    };

    var result = await client.OpeningExplorer.GetPlayerAsync(currentFen, username, options);
    DisplayExplorerResult(result, $"{username} ({color})");
}

async Task ShowTablebaseAsync()
{
    Console.WriteLine("\n--- Tablebase Lookup ---\n");

    // Count pieces to check if tablebase is applicable
    var pieceCount = CountPieces(currentFen);
    if (pieceCount > 7)
    {
        Console.WriteLine($"Position has {pieceCount} pieces. Tablebase only supports up to 7 pieces.");
        Console.WriteLine("Try an endgame position with fewer pieces.");
        return;
    }

    var result = await client.Tablebase.LookupAsync(currentFen);

    Console.WriteLine($"Category: {result.Category}");

    if (result.Checkmate)
        Console.WriteLine("Position is CHECKMATE");
    else if (result.Stalemate)
        Console.WriteLine("Position is STALEMATE");
    else if (result.InsufficientMaterial)
        Console.WriteLine("INSUFFICIENT MATERIAL");

    if (result.Dtz.HasValue)
        Console.WriteLine($"DTZ (Distance to Zero/conversion): {result.Dtz}");
    if (result.Dtm.HasValue)
        Console.WriteLine($"DTM (Distance to Mate): {result.Dtm}");

    if (result.Moves?.Any() == true)
    {
        Console.WriteLine("\nBest moves:");
        foreach (var move in result.Moves.Take(5))
        {
            var info = new List<string> { move.Category };
            if (move.Dtm.HasValue) info.Add($"mate in {Math.Abs(move.Dtm.Value)}");
            else if (move.Dtz.HasValue) info.Add($"dtz {move.Dtz}");

            Console.WriteLine($"  {move.San,-8} -> {string.Join(", ", info)}");
        }
    }
}

void EnterNewFen()
{
    Console.WriteLine("\n--- Enter FEN ---\n");
    Console.WriteLine("Paste a FEN string (or press Enter to keep current):");
    Console.Write("> ");

    var newFen = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(newFen))
    {
        // Basic validation - FEN should have at least the piece placement
        if (newFen.Contains('/'))
        {
            currentFen = newFen;
            Console.WriteLine("FEN updated.");
        }
        else
        {
            Console.WriteLine("Invalid FEN format.");
        }
    }
}

void SelectFamousPosition()
{
    Console.WriteLine("\n--- Famous Positions ---\n");
    Console.WriteLine("  1. Starting position");
    Console.WriteLine("  2. Italian Game");
    Console.WriteLine("  3. Sicilian Dragon");
    Console.WriteLine("  4. King's Indian Attack");
    Console.WriteLine("  5. Immortal Game position (Anderssen vs Kieseritzky)");
    Console.WriteLine("  6. Lucena position (rook endgame)");
    Console.WriteLine("  7. Philidor position (rook endgame)");
    Console.WriteLine("  8. KQvK (queen vs king)");
    Console.Write("\n> ");

    var choice = Console.ReadLine()?.Trim();
    currentFen = choice switch
    {
        "1" => "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
        "2" => "r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4",
        "3" => "rnbqkb1r/pp2pp1p/3p1np1/8/3NP3/2N5/PPP2PPP/R1BQKB1R w KQkq - 0 6",
        "4" => "r1bqkb1r/pppppppp/2n2n2/8/8/5NP1/PPPPPPBP/RNBQK2R b KQkq - 3 3",
        "5" => "r1b2k1r/p1p3pp/2N2n2/2P2b2/5p2/3QP3/PP3PPP/R1B2RK1 w - - 0 16",
        "6" => "1K1k4/1P6/8/8/8/8/r7/2R5 w - - 0 1",
        "7" => "8/8/8/4k3/R7/4K3/8/4r3 w - - 0 1",
        "8" => "8/8/8/4k3/8/8/6Q1/4K3 w - - 0 1",
        _ => currentFen
    };

    Console.WriteLine("Position loaded.");
}

// === Helpers ===

void DisplayExplorerResult(ExplorerResult result, string database)
{
    var total = result.White + result.Draws + result.Black;

    if (total == 0)
    {
        Console.WriteLine($"No games found in {database} database for this position.");
        return;
    }

    Console.WriteLine($"Database: {database}");
    Console.WriteLine($"Total games: {total:N0}");
    Console.WriteLine($"  White wins: {result.White:N0} ({100.0 * result.White / total:F1}%)");
    Console.WriteLine($"  Draws:      {result.Draws:N0} ({100.0 * result.Draws / total:F1}%)");
    Console.WriteLine($"  Black wins: {result.Black:N0} ({100.0 * result.Black / total:F1}%)");

    if (result.Opening != null)
    {
        Console.WriteLine($"\nOpening: {result.Opening.Eco} {result.Opening.Name}");
    }

    if (result.Moves?.Any() == true)
    {
        Console.WriteLine("\nMoves played:");
        Console.WriteLine("  {0,-8} {1,10} {2,8} {3,8} {4,8} {5,6}", "Move", "Games", "White%", "Draw%", "Black%", "AvgElo");
        Console.WriteLine("  " + new string('-', 55));

        foreach (var move in result.Moves.Take(10))
        {
            var moveTotal = move.White + move.Draws + move.Black;
            if (moveTotal == 0) continue;

            var whiteP = 100.0 * move.White / moveTotal;
            var drawP = 100.0 * move.Draws / moveTotal;
            var blackP = 100.0 * move.Black / moveTotal;
            var avgElo = move.AverageRating?.ToString() ?? "-";

            Console.WriteLine("  {0,-8} {1,10:N0} {2,7:F1}% {3,7:F1}% {4,7:F1}% {5,6}",
                move.San, moveTotal, whiteP, drawP, blackP, avgElo);
        }
    }

    if (result.TopGames?.Any() == true)
    {
        Console.WriteLine("\nTop games:");
        foreach (var game in result.TopGames.Take(5))
        {
            var winner = game.Winner ?? "draw";
            Console.WriteLine($"  {game.Id} ({game.Year}) - {winner}");
        }
    }
}

string FormatEvaluation(int? cp, int? mate)
{
    if (mate.HasValue)
    {
        return mate.Value > 0 ? $"M{mate.Value}" : $"-M{Math.Abs(mate.Value)}";
    }

    if (cp.HasValue)
    {
        var score = cp.Value / 100.0;
        return score >= 0 ? $"+{score:F2}" : $"{score:F2}";
    }

    return "?";
}

int CountPieces(string fen)
{
    var board = fen.Split(' ')[0];
    return board.Count(c => char.IsLetter(c));
}

void PrintBoard(string fen)
{
    var board = fen.Split(' ')[0];
    var rows = board.Split('/');

    Console.WriteLine("  +---+---+---+---+---+---+---+---+");

    for (var rank = 0; rank < 8; rank++)
    {
        Console.Write($"{8 - rank} |");
        foreach (var c in rows[rank])
        {
            if (char.IsDigit(c))
            {
                for (var i = 0; i < c - '0'; i++)
                    Console.Write("   |");
            }
            else
            {
                var piece = GetPieceChar(c);
                Console.Write($" {piece} |");
            }
        }
        Console.WriteLine();
        Console.WriteLine("  +---+---+---+---+---+---+---+---+");
    }

    Console.WriteLine("    a   b   c   d   e   f   g   h");
}

char GetPieceChar(char piece) => piece switch
{
    'K' => 'K', 'Q' => 'Q', 'R' => 'R', 'B' => 'B', 'N' => 'N', 'P' => 'P',
    'k' => 'k', 'q' => 'q', 'r' => 'r', 'b' => 'b', 'n' => 'n', 'p' => 'p',
    _ => ' '
};
