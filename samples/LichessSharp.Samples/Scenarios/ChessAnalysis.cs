using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
/// Sample 06: Chess Analysis
/// Demonstrates cloud evaluations, opening explorer, and tablebase lookups.
/// </summary>
public static class ChessAnalysis
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("06 - Chess Analysis Tools");

        using var client = new LichessClient();

        // =====================================================================
        // Cloud Evaluation
        // =====================================================================
        SampleRunner.PrintSubHeader("Cloud Evaluation");

        // Starting position
        var startingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        Console.WriteLine("Querying cloud evaluation for starting position...");

        var cloudEval = await client.Analysis.GetCloudEvaluationAsync(startingFen, multiPv: 3);
        if (cloudEval != null)
        {
            SampleRunner.PrintKeyValue("Depth", cloudEval.Depth);
            SampleRunner.PrintKeyValue("Nodes (thousands)", cloudEval.Knodes);
            Console.WriteLine("  Principal variations:");
            if (cloudEval.Pvs != null)
            {
                foreach (var pv in cloudEval.Pvs.Take(3))
                {
                    var score = pv.Mate.HasValue
                        ? $"Mate in {pv.Mate}"
                        : $"{(pv.Cp ?? 0) / 100.0:+0.00;-0.00}";
                    Console.WriteLine($"    {score}: {pv.Moves}");
                }
            }
        }
        else
        {
            SampleRunner.PrintInfo("No cloud evaluation available for this position");
        }

        // Italian Game position
        var italianFen = "r1bqkbnr/pppp1ppp/2n5/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3";
        Console.WriteLine();
        Console.WriteLine("Querying evaluation for Italian Game position...");

        cloudEval = await client.Analysis.GetCloudEvaluationAsync(italianFen);
        if (cloudEval != null)
        {
            var eval = cloudEval.Pvs?.FirstOrDefault();
            var score = eval?.Mate.HasValue == true
                ? $"Mate in {eval.Mate}"
                : $"{(eval?.Cp ?? 0) / 100.0:+0.00;-0.00}";
            Console.WriteLine($"  Evaluation: {score} at depth {cloudEval.Depth}");
            Console.WriteLine($"  Best line: {eval?.Moves}");
        }

        // =====================================================================
        // Opening Explorer - Masters Database
        // =====================================================================
        SampleRunner.PrintSubHeader("Opening Explorer - Masters Database");

        Console.WriteLine("Querying Masters database for starting position...");

        var mastersExplorer = await client.OpeningExplorer.GetMastersAsync(startingFen);
        if (mastersExplorer != null)
        {
            Console.WriteLine($"  Total games: {mastersExplorer.White + mastersExplorer.Draws + mastersExplorer.Black:N0}");
            Console.WriteLine($"  White wins: {mastersExplorer.White:N0}, Draws: {mastersExplorer.Draws:N0}, Black wins: {mastersExplorer.Black:N0}");

            if (mastersExplorer.Opening != null)
            {
                Console.WriteLine($"  Opening: {mastersExplorer.Opening.Name} ({mastersExplorer.Opening.Eco})");
            }

            Console.WriteLine("  Top moves:");
            if (mastersExplorer.Moves != null)
            {
                foreach (var move in mastersExplorer.Moves.Take(5))
                {
                    var total = move.White + move.Draws + move.Black;
                    var whitePerc = total > 0 ? move.White * 100.0 / total : 0;
                    Console.WriteLine($"    {move.Uci,-6} ({move.San,-6}): {total:N0} games, White: {whitePerc:F1}%");
                }
            }
        }

        // =====================================================================
        // Opening Explorer - Lichess Database
        // =====================================================================
        SampleRunner.PrintSubHeader("Opening Explorer - Lichess Database");

        // Sicilian Defense position
        var sicilianFen = "rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2";
        Console.WriteLine("Querying Lichess database for Sicilian Defense...");

        var lichessOptions = new LichessSharp.Api.Contracts.ExplorerOptions
        {
            Speeds = ["blitz", "rapid"],
            Ratings = [2000, 2200, 2500]
        };
        var lichessExplorer = await client.OpeningExplorer.GetLichessAsync(sicilianFen, lichessOptions);

        if (lichessExplorer != null)
        {
            Console.WriteLine($"  Total games: {lichessExplorer.White + lichessExplorer.Draws + lichessExplorer.Black:N0}");

            if (lichessExplorer.Opening != null)
            {
                Console.WriteLine($"  Opening: {lichessExplorer.Opening.Name} ({lichessExplorer.Opening.Eco})");
            }

            Console.WriteLine("  Top moves:");
            if (lichessExplorer.Moves != null)
            {
                foreach (var move in lichessExplorer.Moves.Take(5))
                {
                    var total = move.White + move.Draws + move.Black;
                    Console.WriteLine($"    {move.San,-6}: {total:N0} games");
                }
            }
        }

        // =====================================================================
        // Opening Explorer - Player Database
        // =====================================================================
        SampleRunner.PrintSubHeader("Opening Explorer - Player Database");

        Console.WriteLine("Querying DrNykterstein's games from starting position...");

        var playerOptions = new LichessSharp.Api.Contracts.ExplorerOptions
        {
            Color = "white",
            Speeds = ["blitz", "rapid"]
        };
        var playerExplorer = await client.OpeningExplorer.GetPlayerAsync(startingFen, "DrNykterstein", playerOptions);

        if (playerExplorer != null)
        {
            var totalGames = playerExplorer.White + playerExplorer.Draws + playerExplorer.Black;
            Console.WriteLine($"  Total games as White: {totalGames:N0}");

            Console.WriteLine("  Favorite openings:");
            if (playerExplorer.Moves != null)
            {
                foreach (var move in playerExplorer.Moves.Take(5))
                {
                    var moveTotal = move.White + move.Draws + move.Black;
                    Console.WriteLine($"    {move.San}: {moveTotal} games");
                }
            }
        }

        // =====================================================================
        // Tablebase Lookup
        // =====================================================================
        SampleRunner.PrintSubHeader("Tablebase Lookup");

        // King and Rook vs King (basic endgame)
        var krkFen = "8/8/8/4k3/8/8/8/4K2R w - - 0 1";
        Console.WriteLine("Querying tablebase for K+R vs K endgame...");
        Console.WriteLine($"  FEN: {krkFen}");

        var tablebase = await client.Tablebase.LookupAsync(krkFen);
        if (tablebase != null)
        {
            SampleRunner.PrintKeyValue("Category", tablebase.Category);
            SampleRunner.PrintKeyValue("DTZ (Distance to Zeroing)", tablebase.Dtz);
            if (tablebase.Dtm.HasValue)
                SampleRunner.PrintKeyValue("DTM (Distance to Mate)", tablebase.Dtm);

            Console.WriteLine("  Best moves:");
            if (tablebase.Moves != null)
            {
                foreach (var move in tablebase.Moves.Take(3))
                {
                    var dtz = move.Dtz.HasValue ? $"dtz={move.Dtz}" : "";
                    Console.WriteLine($"    {move.San}: {move.Category} {dtz}");
                }
            }
        }

        // Queen vs Rook (trickier endgame)
        var qvrFen = "8/8/8/4k3/8/8/4r3/4KQ2 w - - 0 1";
        Console.WriteLine();
        Console.WriteLine("Querying tablebase for Q vs R endgame...");

        tablebase = await client.Tablebase.LookupAsync(qvrFen);
        if (tablebase != null)
        {
            SampleRunner.PrintKeyValue("Category", tablebase.Category);
            SampleRunner.PrintKeyValue("DTZ", tablebase.Dtz);
            Console.WriteLine($"  Best move: {tablebase.Moves?.FirstOrDefault()?.San}");
        }

        // =====================================================================
        // Atomic Tablebase
        // =====================================================================
        SampleRunner.PrintSubHeader("Variant Tablebases");

        Console.WriteLine("Tablebase lookups are also available for variants:");
        Console.WriteLine("  - Tablebase.LookupAtomicAsync() - Atomic chess");
        Console.WriteLine("  - Tablebase.LookupAntichessAsync() - Antichess");

        // Quick atomic example
        var atomicFen = "8/8/8/3k4/8/8/8/4K2R w - - 0 1";
        Console.WriteLine();
        Console.WriteLine("Atomic tablebase query (same position)...");

        var atomicTablebase = await client.Tablebase.LookupAtomicAsync(atomicFen);
        if (atomicTablebase != null)
        {
            Console.WriteLine($"  Category: {atomicTablebase.Category}");
            Console.WriteLine($"  Best move: {atomicTablebase.Moves?.FirstOrDefault()?.San}");
        }

        // =====================================================================
        // Integration Tips
        // =====================================================================
        SampleRunner.PrintSubHeader("Integration Tips");

        Console.WriteLine("Building a chess analysis tool:");
        Console.WriteLine();
        Console.WriteLine("1. Cloud Evaluations");
        Console.WriteLine("   - Use multiPv parameter for multiple lines");
        Console.WriteLine("   - Evaluations are cached; popular positions are available");
        Console.WriteLine("   - Returns null if position not in cloud database");
        Console.WriteLine();
        Console.WriteLine("2. Opening Explorer");
        Console.WriteLine("   - Masters: OTB games from titled players");
        Console.WriteLine("   - Lichess: Online games, filterable by rating/speed");
        Console.WriteLine("   - Player: Specific player's games");
        Console.WriteLine();
        Console.WriteLine("3. Tablebases");
        Console.WriteLine("   - Up to 7 pieces (including kings)");
        Console.WriteLine("   - DTZ = Distance To Zeroing (50-move rule reset)");
        Console.WriteLine("   - DTM = Distance To Mate (if available)");
        Console.WriteLine("   - Categories: win, draw, loss, maybe-win, maybe-loss");

        SampleRunner.PrintSuccess("Chess Analysis sample completed!");
    }
}
