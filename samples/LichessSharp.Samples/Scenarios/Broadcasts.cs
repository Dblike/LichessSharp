using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
///     Sample 10: Broadcasts
///     Demonstrates how to access and stream live chess broadcasts.
/// </summary>
public static class Broadcasts
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("10 - Live Broadcasts");

        using var client = new LichessClient();

        // =====================================================================
        // Get Top Broadcasts
        // =====================================================================
        SampleRunner.PrintSubHeader("Top Broadcasts");

        var topBroadcasts = await client.Broadcasts.GetTopBroadcastsAsync();
        Console.WriteLine("Featured broadcasts:");

        if (topBroadcasts.Active?.Count > 0)
        {
            Console.WriteLine("\n  Currently active:");
            foreach (var broadcast in topBroadcasts.Active.Take(5))
            {
                Console.WriteLine($"    - {broadcast.Tour.Name}");
                if (broadcast.Round != null)
                    Console.WriteLine($"      Round: {broadcast.Round.Name}");
            }
        }

        if (topBroadcasts.Past?.CurrentPageResults?.Count > 0)
        {
            Console.WriteLine("\n  Past broadcasts:");
            foreach (var broadcast in topBroadcasts.Past.CurrentPageResults.Take(3))
                Console.WriteLine($"    - {broadcast.Tour.Name}");
        }

        // =====================================================================
        // Stream Official Broadcasts
        // =====================================================================
        SampleRunner.PrintSubHeader("Stream Official Broadcasts");

        Console.WriteLine("Streaming official broadcasts (limit 5)...");

        var broadcastCount = 0;
        await foreach (var broadcast in client.Broadcasts.StreamOfficialBroadcastsAsync(5))
        {
            broadcastCount++;
            Console.WriteLine($"  {broadcastCount}. {broadcast.Tour?.Name}");
            if (broadcast.Tour?.Description != null)
            {
                var desc = broadcast.Tour.Description;
                if (desc.Length > 60)
                    desc = desc[..60] + "...";
                Console.WriteLine($"      {desc}");
            }
        }

        // =====================================================================
        // Search Broadcasts
        // =====================================================================
        SampleRunner.PrintSubHeader("Search Broadcasts");

        Console.WriteLine("Searching for 'candidates' broadcasts...");

        var searchResults = await client.Broadcasts.SearchBroadcastsAsync("candidates");
        Console.WriteLine("Found broadcasts:");
        if (searchResults.CurrentPageResults != null)
            foreach (var result in searchResults.CurrentPageResults.Take(5))
                Console.WriteLine($"  - {result.Tour.Name}");

        // =====================================================================
        // Get Broadcast Details
        // =====================================================================
        SampleRunner.PrintSubHeader("Broadcast Details");

        // Try to get details for a broadcast from our earlier results
        var broadcastTour = topBroadcasts.Active?.FirstOrDefault()?.Tour
                            ?? topBroadcasts.Past?.CurrentPageResults?.FirstOrDefault()?.Tour;

        if (broadcastTour?.Id != null)
        {
            Console.WriteLine($"Getting details for: {broadcastTour.Name}");

            try
            {
                var tourDetails = await client.Broadcasts.GetTournamentAsync(broadcastTour.Id);
                SampleRunner.PrintKeyValue("Name", tourDetails.Tour?.Name);
                var desc = tourDetails.Tour?.Description;
                SampleRunner.PrintKeyValue("Description",
                    desc != null ? desc[..Math.Min(80, desc.Length)] + "..." : "(none)");
                SampleRunner.PrintKeyValue("Tier", tourDetails.Tour?.Tier);

                if (tourDetails.Rounds?.Count > 0)
                {
                    Console.WriteLine($"  Rounds ({tourDetails.Rounds.Count} total):");
                    foreach (var round in tourDetails.Rounds.Take(5))
                    {
                        var status = round.Finished == true ? "Finished"
                            : round.Ongoing == true ? "Live"
                            : "Upcoming";
                        Console.WriteLine($"    - {round.Name} [{status}]");
                    }
                }
            }
            catch (Exception ex)
            {
                SampleRunner.PrintError($"Could not fetch broadcast details: {ex.Message}");
            }
        }
        else
        {
            SampleRunner.PrintInfo("No broadcast available to show details");
        }

        // =====================================================================
        // Get Round Details
        // =====================================================================
        SampleRunner.PrintSubHeader("Round Details");

        var activeBroadcast = topBroadcasts.Active?.FirstOrDefault();
        var activeRound = activeBroadcast?.Round;
        if (activeRound?.Id != null && activeBroadcast?.Tour?.Slug != null)
        {
            Console.WriteLine($"Getting round details: {activeRound.Name}");

            try
            {
                var roundDetails = await client.Broadcasts.GetRoundAsync(
                    activeBroadcast.Tour.Slug,
                    activeRound.Slug,
                    activeRound.Id);

                if (roundDetails.Games?.Count > 0)
                {
                    Console.WriteLine($"  Games in this round ({roundDetails.Games.Count} total):");
                    foreach (var game in roundDetails.Games.Take(5))
                    {
                        // Players array: index 0 = white, index 1 = black
                        var white = game.Players?.Count > 0 ? game.Players[0] : null;
                        var black = game.Players?.Count > 1 ? game.Players[1] : null;
                        Console.WriteLine($"    {white?.Name ?? "?"} vs {black?.Name ?? "?"}");
                    }
                }
            }
            catch (Exception ex)
            {
                SampleRunner.PrintError($"Could not fetch round: {ex.Message}");
            }
        }

        // =====================================================================
        // Stream Round PGN
        // =====================================================================
        SampleRunner.PrintSubHeader("Stream Round PGN");

        if (activeRound?.Id != null)
        {
            Console.WriteLine($"Streaming PGN for round: {activeRound.Name}");
            Console.WriteLine("(This streams real-time updates of all games)");
            Console.WriteLine();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            try
            {
                await foreach (var pgn in client.Broadcasts.StreamRoundPgnAsync(activeRound.Id, cts.Token))
                {
                    // Show first 200 chars of PGN
                    var preview = pgn.Length > 200 ? pgn[..200] + "..." : pgn;
                    Console.WriteLine($"  PGN update received ({pgn.Length} chars):");
                    Console.WriteLine($"  {preview}");
                    Console.WriteLine();
                    break; // Just show one update
                }
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }
        else
        {
            SampleRunner.PrintInfo("No active round to stream");
        }

        // =====================================================================
        // Get Broadcast Players
        // =====================================================================
        SampleRunner.PrintSubHeader("Broadcast Players");

        if (broadcastTour?.Id != null)
            try
            {
                var players = await client.Broadcasts.GetPlayersAsync(broadcastTour.Id);
                if (players.Count > 0)
                {
                    Console.WriteLine($"Players in {broadcastTour.Name}:");
                    foreach (var player in players.Take(10))
                    {
                        var rating = player.Rating > 0 ? $"({player.Rating})" : "";
                        var title = !string.IsNullOrEmpty(player.Title) ? $"{player.Title} " : "";
                        Console.WriteLine($"  - {title}{player.Name} {rating}");
                    }
                }
            }
            catch (Exception ex)
            {
                SampleRunner.PrintError($"Could not fetch players: {ex.Message}");
            }

        // =====================================================================
        // Broadcast Management (Authenticated)
        // =====================================================================
        SampleRunner.PrintSubHeader("Broadcast Management");

        var token = SampleRunner.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            using var authClient = new LichessClient(token);

            Console.WriteLine("With authentication, you can create and manage broadcasts:");
            Console.WriteLine();
            Console.WriteLine("  Creating broadcasts:");
            Console.WriteLine("    client.Broadcasts.CreateTournamentAsync()  - Create broadcast tournament");
            Console.WriteLine("    client.Broadcasts.CreateRoundAsync()       - Add a round");
            Console.WriteLine();
            Console.WriteLine("  Updating:");
            Console.WriteLine("    client.Broadcasts.UpdateTournamentAsync()  - Update tournament info");
            Console.WriteLine("    client.Broadcasts.UpdateRoundAsync()       - Update round info");
            Console.WriteLine("    client.Broadcasts.PushPgnAsync()           - Push PGN to a round");
            Console.WriteLine("    client.Broadcasts.ResetRoundAsync()        - Reset round games");
            Console.WriteLine();
            Console.WriteLine("  Exporting:");
            Console.WriteLine("    client.Broadcasts.ExportRoundPgnAsync()    - Export round as PGN");
            Console.WriteLine("    client.Broadcasts.ExportAllRoundsPgnAsync() - Export all rounds");
            Console.WriteLine();

            // Stream my rounds
            Console.WriteLine("Your broadcast rounds:");
            var myRoundCount = 0;
            await foreach (var myRound in authClient.Broadcasts.StreamMyRoundsAsync(5))
            {
                myRoundCount++;
                Console.WriteLine(
                    $"  {myRoundCount}. {myRound.Round?.Name ?? "Unknown"} - {myRound.Tour?.Name ?? "Unknown"}");
            }

            if (myRoundCount == 0) Console.WriteLine("  No broadcast rounds found for your account.");
        }
        else
        {
            SampleRunner.PrintInfo("Broadcast management requires authentication");
        }

        // =====================================================================
        // Tips
        // =====================================================================
        SampleRunner.PrintSubHeader("Broadcast Integration Tips");

        Console.WriteLine("1. Use StreamOfficialBroadcastsAsync() for a list of all broadcasts");
        Console.WriteLine();
        Console.WriteLine("2. StreamRoundPgnAsync() provides real-time game updates");
        Console.WriteLine("   - Parse the PGN to display live games");
        Console.WriteLine("   - Updates include moves, clocks, and results");
        Console.WriteLine();
        Console.WriteLine("3. For creating your own broadcasts:");
        Console.WriteLine("   - Create a tournament (event/competition)");
        Console.WriteLine("   - Add rounds (individual match days)");
        Console.WriteLine("   - Push PGN updates as games progress");
        Console.WriteLine();
        Console.WriteLine("4. Broadcast tiers indicate official status:");
        Console.WriteLine("   - Higher tiers get more visibility on Lichess");

        SampleRunner.PrintSuccess("Broadcasts sample completed!");
    }
}