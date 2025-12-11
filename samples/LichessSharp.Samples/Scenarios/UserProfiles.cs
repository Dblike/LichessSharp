using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
/// Sample 02: User Profiles
/// Demonstrates how to fetch and explore user data from Lichess.
/// </summary>
public static class UserProfiles
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("02 - User Profiles");

        using var client = new LichessClient();

        // =====================================================================
        // Get a Single User by Username
        // =====================================================================
        SampleRunner.PrintSubHeader("Get User by Username");

        var magnus = await client.Users.GetAsync("DrNykterstein");
        SampleRunner.PrintKeyValue("Username", magnus.Username);
        SampleRunner.PrintKeyValue("Title", magnus.Title?.ToString() ?? "None");
        SampleRunner.PrintKeyValue("Created", magnus.CreatedAt);
        SampleRunner.PrintKeyValue("Last seen", magnus.SeenAt?.ToString() ?? "Unknown");

        // Show performance ratings if available
        if (magnus.Perfs != null)
        {
            Console.WriteLine("  Ratings:");
            if (magnus.Perfs.TryGetValue("bullet", out var bullet))
                Console.WriteLine($"    Bullet: {bullet.Rating} ({bullet.Games} games)");
            if (magnus.Perfs.TryGetValue("blitz", out var blitz))
                Console.WriteLine($"    Blitz: {blitz.Rating} ({blitz.Games} games)");
            if (magnus.Perfs.TryGetValue("rapid", out var rapid))
                Console.WriteLine($"    Rapid: {rapid.Rating} ({rapid.Games} games)");
            if (magnus.Perfs.TryGetValue("classical", out var classical))
                Console.WriteLine($"    Classical: {classical.Rating} ({classical.Games} games)");
        }

        // =====================================================================
        // Get Multiple Users at Once
        // =====================================================================
        SampleRunner.PrintSubHeader("Get Multiple Users");

        var usernames = new[] { "DrNykterstein", "Hikaru", "nihalsarin2004", "GMWSO" };
        var users = await client.Users.GetManyAsync(usernames);

        Console.WriteLine($"Found {users.Count} users:");
        foreach (var user in users)
        {
            Console.WriteLine($"  - {user.Username} (ID: {user.Id})");
        }

        // =====================================================================
        // Check Real-Time User Status
        // =====================================================================
        SampleRunner.PrintSubHeader("Real-Time User Status");

        var statuses = await client.Users.GetRealTimeStatusAsync(usernames);

        Console.WriteLine("Current status:");
        foreach (var status in statuses)
        {
            var statusText = status.Online == true
                ? (status.Playing == true ? "Playing" : "Online")
                : "Offline";
            Console.WriteLine($"  - {status.Name}: {statusText}");
        }

        // =====================================================================
        // Get Leaderboards
        // =====================================================================
        SampleRunner.PrintSubHeader("Leaderboards");

        // Get top 10 across all variants
        var allTop10 = await client.Users.GetAllTop10Async();
        Console.WriteLine("Top 10 leaders (sample):");
        if (allTop10.TryGetValue("bullet", out var bulletTop))
        {
            Console.WriteLine("  Bullet top 3:");
            foreach (var player in bulletTop.Take(3))
            {
                Console.WriteLine($"    - {player.Username}");
            }
        }

        // Get specific leaderboard
        var blitzLeaders = await client.Users.GetLeaderboardAsync("blitz", count: 5);
        Console.WriteLine("  Blitz top 5:");
        foreach (var player in blitzLeaders)
        {
            Console.WriteLine($"    - {player.Username}");
        }

        // =====================================================================
        // Get Rating History
        // =====================================================================
        SampleRunner.PrintSubHeader("Rating History");

        var ratingHistory = await client.Users.GetRatingHistoryAsync("DrNykterstein");
        Console.WriteLine($"Rating history categories: {ratingHistory.Count}");
        foreach (var category in ratingHistory.Take(3))
        {
            Console.WriteLine($"  - {category.Name}: {category.Points?.Count ?? 0} data points");
        }

        // =====================================================================
        // Username Autocomplete
        // =====================================================================
        SampleRunner.PrintSubHeader("Username Autocomplete");

        // Simple string autocomplete
        var suggestions = await client.Users.AutocompleteAsync("Magnus");
        Console.WriteLine($"Autocomplete for 'Magnus': {string.Join(", ", suggestions.Take(5))}");

        // Object autocomplete with more details
        var playerSuggestions = await client.Users.AutocompletePlayersAsync("Hikaru");
        Console.WriteLine("Player autocomplete for 'Hikaru':");
        foreach (var player in playerSuggestions.Take(3))
        {
            Console.WriteLine($"  - {player.Name} ({player.Id})");
        }

        // =====================================================================
        // Get Live Streamers
        // =====================================================================
        SampleRunner.PrintSubHeader("Live Streamers");

        var streamers = await client.Users.GetLiveStreamersAsync();
        if (streamers.Count > 0)
        {
            Console.WriteLine($"Currently streaming ({streamers.Count} total):");
            foreach (var streamer in streamers.Take(5))
            {
                Console.WriteLine($"  - {streamer.Name}: {streamer.Stream?.Status ?? "Live"}");
            }
        }
        else
        {
            Console.WriteLine("No live streamers at the moment.");
        }

        // =====================================================================
        // Get Crosstable
        // =====================================================================
        SampleRunner.PrintSubHeader("Head-to-Head (Crosstable)");

        var crosstable = await client.Users.GetCrosstableAsync("DrNykterstein", "Hikaru");
        if (crosstable != null)
        {
            Console.WriteLine("DrNykterstein vs Hikaru:");
            SampleRunner.PrintKeyValue("Total games", crosstable.NbGames);
            if (crosstable.Users != null)
            {
                foreach (var (username, score) in crosstable.Users)
                {
                    Console.WriteLine($"  {username}: {score} points");
                }
            }
        }

        SampleRunner.PrintSuccess("User Profiles sample completed!");
    }
}
