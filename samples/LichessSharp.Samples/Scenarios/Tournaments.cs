using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
/// Sample 07: Tournaments
/// Demonstrates how to access arena and Swiss tournament data.
/// </summary>
public static class Tournaments
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("07 - Tournaments");

        using var client = new LichessClient();

        // =====================================================================
        // Get Current Arena Tournaments
        // =====================================================================
        SampleRunner.PrintSubHeader("Current Arena Tournaments");

        var currentTournaments = await client.ArenaTournaments.GetCurrentAsync();

        Console.WriteLine("Started tournaments:");
        if (currentTournaments.Started != null)
        {
            foreach (var tournament in currentTournaments.Started.Take(5))
            {
                Console.WriteLine($"  - {tournament.FullName}");
                Console.WriteLine($"      ID: {tournament.Id}, Players: {tournament.NbPlayers}, Minutes: {tournament.Minutes}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Created (upcoming) tournaments:");
        if (currentTournaments.Created != null)
        {
            foreach (var tournament in currentTournaments.Created.Take(3))
            {
                Console.WriteLine($"  - {tournament.FullName} (starts: {tournament.StartsAt})");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Finished tournaments:");
        if (currentTournaments.Finished != null)
        {
            foreach (var tournament in currentTournaments.Finished.Take(3))
            {
                Console.WriteLine($"  - {tournament.FullName}");
            }
        }

        // =====================================================================
        // Get Tournament Details
        // =====================================================================
        SampleRunner.PrintSubHeader("Tournament Details");

        // Get details for a current tournament
        var tournamentId = currentTournaments.Started?.FirstOrDefault()?.Id
            ?? currentTournaments.Finished?.FirstOrDefault()?.Id;

        if (tournamentId != null)
        {
            Console.WriteLine($"Fetching details for tournament: {tournamentId}");

            var tournament = await client.ArenaTournaments.GetAsync(tournamentId);
            SampleRunner.PrintKeyValue("Name", tournament.FullName);
            SampleRunner.PrintKeyValue("Variant", tournament.Variant);
            SampleRunner.PrintKeyValue("Rated", tournament.Rated);
            SampleRunner.PrintKeyValue("Status", tournament.Status);
            SampleRunner.PrintKeyValue("Players", tournament.NbPlayers);
            SampleRunner.PrintKeyValue("Duration (min)", tournament.Minutes);
            SampleRunner.PrintKeyValue("Created by", tournament.CreatedBy);

            if (tournament.Clock != null)
            {
                Console.WriteLine($"  Time control: {tournament.Clock.Limit / 60}+{tournament.Clock.Increment}");
            }
        }
        else
        {
            SampleRunner.PrintInfo("No tournaments available to show details");
        }

        // =====================================================================
        // Stream Tournament Results
        // =====================================================================
        SampleRunner.PrintSubHeader("Tournament Results/Standings");

        if (tournamentId != null)
        {
            Console.WriteLine($"Top 10 players in tournament {tournamentId}:");

            var rank = 0;
            await foreach (var result in client.ArenaTournaments.StreamResultsAsync(tournamentId, nb: 10))
            {
                rank++;
                var performance = result.Performance > 0 ? $"perf: {result.Performance}" : "";
                Console.WriteLine($"  {rank,2}. {result.Username,-20} Score: {result.Score,4} {performance}");
            }
        }

        // =====================================================================
        // Stream Tournament Games
        // =====================================================================
        SampleRunner.PrintSubHeader("Tournament Games");

        if (tournamentId != null)
        {
            Console.WriteLine($"Recent games from tournament {tournamentId} (limit 3):");

            var gameCount = 0;
            await foreach (var game in client.ArenaTournaments.StreamGamesAsync(tournamentId))
            {
                gameCount++;
                var white = game.Players?.White?.User?.Name ?? "Unknown";
                var black = game.Players?.Black?.User?.Name ?? "Unknown";
                Console.WriteLine($"  {gameCount}. {white} vs {black} ({game.Status})");

                if (gameCount >= 3)
                    break;
            }
        }

        // =====================================================================
        // User's Tournament History
        // =====================================================================
        SampleRunner.PrintSubHeader("User's Tournament History");

        Console.WriteLine("Tournaments played by DrNykterstein (last 5)...");

        var count = 0;
        await foreach (var played in client.ArenaTournaments.StreamPlayedByAsync("DrNykterstein", nb: 5))
        {
            count++;
            if (played.Tournament != null)
            {
                var result = played.Player?.Rank.HasValue == true ? $"Rank: {played.Player.Rank}" : "";
                Console.WriteLine($"  {count}. {played.Tournament.FullName} {result}");
            }
        }

        // =====================================================================
        // Swiss Tournaments
        // =====================================================================
        SampleRunner.PrintSubHeader("Swiss Tournaments");

        Console.WriteLine("Swiss tournaments for a team (lichess-swiss sample)...");

        // Stream Swiss tournaments for a team
        var swissCount = 0;
        await foreach (var swiss in client.SwissTournaments.StreamTeamTournamentsAsync("lichess-swiss", max: 3))
        {
            swissCount++;
            Console.WriteLine($"  {swissCount}. {swiss.Name} - Rounds: {swiss.NbRounds}, Players: {swiss.NbPlayers}");
        }

        if (swissCount == 0)
        {
            Console.WriteLine("  No Swiss tournaments found for this team.");
        }

        // =====================================================================
        // Team Battles
        // =====================================================================
        SampleRunner.PrintSubHeader("Team Battles");

        // Team battles are special arena tournaments between teams
        if (tournamentId != null)
        {
            try
            {
                var teamStanding = await client.ArenaTournaments.GetTeamStandingAsync(tournamentId);
                if (teamStanding.Teams != null && teamStanding.Teams.Count > 0)
                {
                    Console.WriteLine("Team standings:");
                    var teamRank = 0;
                    foreach (var team in teamStanding.Teams.Take(5))
                    {
                        teamRank++;
                        Console.WriteLine($"  {teamRank}. {team.Id}: {team.Score} points");
                    }
                }
                else
                {
                    Console.WriteLine("This tournament is not a team battle.");
                }
            }
            catch
            {
                Console.WriteLine("Team standings not available (not a team battle).");
            }
        }

        // =====================================================================
        // Tournament Management (Authenticated)
        // =====================================================================
        SampleRunner.PrintSubHeader("Tournament Management");

        var token = SampleRunner.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine("With authentication, you can:");
            Console.WriteLine("  - ArenaTournaments.CreateAsync() - Create tournaments");
            Console.WriteLine("  - ArenaTournaments.JoinAsync() - Join tournaments");
            Console.WriteLine("  - ArenaTournaments.WithdrawAsync() - Leave tournaments");
            Console.WriteLine("  - ArenaTournaments.TerminateAsync() - End your tournaments");
            Console.WriteLine("  - SwissTournaments.CreateAsync() - Create Swiss tournaments");
            Console.WriteLine();
            Console.WriteLine("(Skipping creation demo to avoid creating real tournaments)");
        }
        else
        {
            SampleRunner.PrintInfo("Tournament management requires authentication");
        }

        // =====================================================================
        // Simuls
        // =====================================================================
        SampleRunner.PrintSubHeader("Simultaneous Exhibitions (Simuls)");

        var simuls = await client.Simuls.GetCurrentAsync();
        Console.WriteLine("Current simuls:");

        if (simuls.Pending?.Count > 0)
        {
            Console.WriteLine("  Pending:");
            foreach (var simul in simuls.Pending.Take(3))
            {
                Console.WriteLine($"    - {simul.Name} by {simul.Host?.Name}");
            }
        }

        if (simuls.Started?.Count > 0)
        {
            Console.WriteLine("  In progress:");
            foreach (var simul in simuls.Started.Take(3))
            {
                Console.WriteLine($"    - {simul.Name} by {simul.Host?.Name}");
            }
        }

        if (simuls.Finished?.Count > 0)
        {
            Console.WriteLine("  Recently finished:");
            foreach (var simul in simuls.Finished.Take(3))
            {
                Console.WriteLine($"    - {simul.Name}");
            }
        }

        if ((simuls.Pending?.Count ?? 0) + (simuls.Started?.Count ?? 0) + (simuls.Finished?.Count ?? 0) == 0)
        {
            Console.WriteLine("  No simuls currently available.");
        }

        SampleRunner.PrintSuccess("Tournaments sample completed!");
    }
}
