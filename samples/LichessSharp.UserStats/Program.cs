// LichessSharp.UserStats - Player statistics and comparison tool
//
// Analyze player profiles, ratings, and head-to-head statistics.
//
// Run: dotnet run --project samples/LichessSharp.UserStats [username]

using LichessSharp;

Console.WriteLine("=== LichessSharp User Stats ===\n");

using var client = new LichessClient();

// Get username from args or prompt
var username = args.Length > 0 ? args[0] : null;

if (string.IsNullOrEmpty(username))
{
    Console.Write("Enter username (or press Enter for DrNykterstein): ");
    username = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(username))
        username = "DrNykterstein";
}

await ShowUserStatsAsync(username);

async Task ShowUserStatsAsync(string user)
{
    try
    {
        var profile = await client.Users.GetAsync(user);

        // Header
        var title = profile.Title != null ? $"{profile.Title} " : "";
        Console.WriteLine($"\n=== {title}{profile.Username} ===\n");

        // Basic info
        Console.WriteLine("Profile:");
        Console.WriteLine($"  ID: {profile.Id}");
        Console.WriteLine($"  Created: {profile.CreatedAt:yyyy-MM-dd}");
        if (profile.SeenAt.HasValue)
            Console.WriteLine($"  Last seen: {profile.SeenAt:yyyy-MM-dd HH:mm}");
        if (profile.PlayTime != null)
        {
            var totalHours = profile.PlayTime.Total / 3600;
            Console.WriteLine($"  Play time: {totalHours:N0} hours");
        }
        Console.WriteLine();

        // Ratings
        if (profile.Perfs != null && profile.Perfs.Count > 0)
        {
            Console.WriteLine("Ratings:");
            var ratingOrder = new[] { "ultraBullet", "bullet", "blitz", "rapid", "classical", "correspondence", "chess960", "crazyhouse" };

            foreach (var key in ratingOrder)
            {
                if (profile.Perfs.TryGetValue(key, out var perf) && perf.Games > 0)
                {
                    var rd = perf.Rd > 0 ? $" (RD: {perf.Rd})" : "";
                    var prov = perf.Prov == true ? "?" : "";
                    Console.WriteLine($"  {key,-14}: {perf.Rating}{prov,-1} - {perf.Games:N0} games{rd}");
                }
            }
            Console.WriteLine();
        }

        // Game counts
        if (profile.Count != null)
        {
            Console.WriteLine("Game Statistics:");
            Console.WriteLine($"  Total games: {profile.Count.All:N0}");
            Console.WriteLine($"  Wins: {profile.Count.Win:N0}");
            Console.WriteLine($"  Losses: {profile.Count.Loss:N0}");
            Console.WriteLine($"  Draws: {profile.Count.Draw:N0}");

            if (profile.Count.All > 0)
            {
                var winRate = 100.0 * profile.Count.Win / profile.Count.All;
                Console.WriteLine($"  Win rate: {winRate:F1}%");
            }

            Console.WriteLine();
            Console.WriteLine("Game Types:");
            Console.WriteLine($"  Rated: {profile.Count.Rated:N0}");
            Console.WriteLine($"  AI: {profile.Count.Ai:N0}");
            Console.WriteLine();
        }

        // Profile info
        if (profile.Profile != null)
        {
            var hasProfileInfo = false;
            var realName = GetRealName(profile.Profile);

            if (!string.IsNullOrEmpty(realName) ||
                !string.IsNullOrEmpty(profile.Profile.Country) ||
                !string.IsNullOrEmpty(profile.Profile.Bio))
            {
                Console.WriteLine("About:");
                hasProfileInfo = true;
            }

            if (!string.IsNullOrEmpty(realName))
                Console.WriteLine($"  Name: {realName}");
            if (!string.IsNullOrEmpty(profile.Profile.Country))
                Console.WriteLine($"  Country: {profile.Profile.Country}");
            if (profile.Profile.FideRating > 0)
                Console.WriteLine($"  FIDE Rating: {profile.Profile.FideRating}");
            if (!string.IsNullOrEmpty(profile.Profile.Bio))
            {
                var bio = profile.Profile.Bio.Length > 100
                    ? profile.Profile.Bio[..100] + "..."
                    : profile.Profile.Bio;
                Console.WriteLine($"  Bio: {bio}");
            }

            if (hasProfileInfo)
                Console.WriteLine();
        }

        // Status indicators
        var indicators = new List<string>();
        if (profile.Patron == true) indicators.Add("Patron");
        if (profile.Verified == true) indicators.Add("Verified");
        if (profile.Disabled == true) indicators.Add("DISABLED");
        if (profile.TosViolation == true) indicators.Add("TOS Violation");

        if (indicators.Count > 0)
        {
            Console.WriteLine($"Status: {string.Join(", ", indicators)}");
            Console.WriteLine();
        }

        // Interactive options
        await ShowOptionsAsync(user);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

async Task ShowOptionsAsync(string user)
{
    while (true)
    {
        Console.WriteLine("Options:");
        Console.WriteLine("  1. View rating history");
        Console.WriteLine("  2. Compare with another player");
        Console.WriteLine("  3. Check online status");
        Console.WriteLine("  4. Look up another user");
        Console.WriteLine("  q. Quit");
        Console.Write("\n> ");

        var choice = Console.ReadLine()?.Trim().ToLowerInvariant();

        switch (choice)
        {
            case "1":
                await ShowRatingHistoryAsync(user);
                break;
            case "2":
                await ComparePlayersAsync(user);
                break;
            case "3":
                await CheckStatusAsync(user);
                break;
            case "4":
                Console.Write("Enter username: ");
                var newUser = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(newUser))
                {
                    user = newUser;
                    await ShowUserStatsAsync(user);
                }
                break;
            case "q":
            case "quit":
                return;
            default:
                Console.WriteLine("Invalid option.\n");
                break;
        }
    }
}

async Task ShowRatingHistoryAsync(string user)
{
    Console.WriteLine($"\n--- Rating History for {user} ---\n");

    var history = await client.Users.GetRatingHistoryAsync(user);

    foreach (var category in history.Where(h => h.Points?.Count > 0))
    {
        var points = category.Points!;
        var latest = points.Last();
        var earliest = points.First();

        if (points.Count > 1)
        {
            var change = latest.Rating - earliest.Rating;
            var changeStr = change >= 0 ? $"+{change}" : change.ToString();
            Console.WriteLine($"  {category.Name,-14}: {latest.Rating} ({changeStr} over {points.Count} data points)");
        }
        else
        {
            Console.WriteLine($"  {category.Name,-14}: {latest.Rating}");
        }
    }

    Console.WriteLine();
}

async Task ComparePlayersAsync(string user1)
{
    Console.Write("Enter username to compare: ");
    var user2 = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(user2))
    {
        Console.WriteLine("No username entered.\n");
        return;
    }

    Console.WriteLine($"\n--- {user1} vs {user2} ---\n");

    try
    {
        // Get crosstable (head-to-head)
        var crosstable = await client.Users.GetCrosstableAsync(user1, user2);

        if (crosstable != null && crosstable.NbGames > 0)
        {
            Console.WriteLine($"Head-to-head: {crosstable.NbGames} games");
            if (crosstable.Users != null)
            {
                foreach (var (name, score) in crosstable.Users)
                {
                    Console.WriteLine($"  {name}: {score} points");
                }
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("No head-to-head games found.\n");
        }

        // Compare ratings - need to fetch full profiles for Perfs data
        var p1 = await client.Users.GetAsync(user1);
        var p2 = await client.Users.GetAsync(user2);

        Console.WriteLine("Rating Comparison:");
        var categories = new[] { "bullet", "blitz", "rapid", "classical" };

        foreach (var cat in categories)
        {
            var r1 = p1.Perfs?.GetValueOrDefault(cat)?.Rating ?? 0;
            var r2 = p2.Perfs?.GetValueOrDefault(cat)?.Rating ?? 0;

            if (r1 > 0 || r2 > 0)
            {
                var diff = r1 - r2;
                var diffStr = diff > 0 ? $"+{diff}" : diff.ToString();
                Console.WriteLine($"  {cat,-10}: {r1,4} vs {r2,4} ({diffStr})");
            }
        }
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}\n");
    }
}

async Task CheckStatusAsync(string user)
{
    Console.WriteLine($"\n--- Status Check ---\n");

    var statuses = await client.Users.GetRealTimeStatusAsync([user]);
    var status = statuses.FirstOrDefault();

    if (status != null)
    {
        var state = status.Online == true
            ? status.Playing == true ? "Playing a game" : "Online"
            : "Offline";

        Console.WriteLine($"{status.Name}: {state}");

        if (!string.IsNullOrEmpty(status.PlayingId))
            Console.WriteLine($"  Current game: https://lichess.org/{status.PlayingId}");
    }

    Console.WriteLine();
}

string? GetRealName(LichessSharp.Models.Users.UserProfile profile)
{
    var parts = new List<string>();
    if (!string.IsNullOrEmpty(profile.FirstName))
        parts.Add(profile.FirstName);
    if (!string.IsNullOrEmpty(profile.LastName))
        parts.Add(profile.LastName);
    return parts.Count > 0 ? string.Join(" ", parts) : null;
}
