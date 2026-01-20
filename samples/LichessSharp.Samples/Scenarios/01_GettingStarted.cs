using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
///     Sample 01: Getting Started
///     Demonstrates basic client setup and configuration options.
/// </summary>
public static class GettingStarted
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("01 - Getting Started with LichessSharp");

        // =====================================================================
        // Creating an Unauthenticated Client
        // =====================================================================
        SampleRunner.PrintSubHeader("Creating an Unauthenticated Client");

        // The simplest way to create a client - no authentication required
        // This gives access to all public/read-only endpoints
        using (var client = new LichessClient())
        {
            SampleRunner.PrintSuccess("Created unauthenticated client");
            SampleRunner.PrintInfo("This client can access all public endpoints");

            // Quick test - get a user profile (public data)
            var user = await client.Users.GetAsync("DrNykterstein");
            SampleRunner.PrintKeyValue("Test call - User found", user.Username);
        }

        // =====================================================================
        // Creating an Authenticated Client
        // =====================================================================
        SampleRunner.PrintSubHeader("Creating an Authenticated Client");

        var token = SampleRunner.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            // Pass the token directly to the constructor
            using var authenticatedClient = new LichessClient(token);
            SampleRunner.PrintSuccess("Created authenticated client");

            // Test authenticated endpoint
            try
            {
                var profile = await authenticatedClient.Account.GetProfileAsync();
                SampleRunner.PrintKeyValue("Logged in as", profile.Username);
            }
            catch (Exception ex)
            {
                SampleRunner.PrintError($"Authentication test failed: {ex.Message}");
            }
        }
        else
        {
            SampleRunner.PrintWarning("Skipping authenticated client demo");
            SampleRunner.PrintInfo("Set LICHESS_TEST_TOKEN environment variable to test");
        }

        // =====================================================================
        // Custom Configuration Options
        // =====================================================================
        SampleRunner.PrintSubHeader("Custom Configuration Options");

        var options = new LichessClientOptions
        {
            // Authentication
            AccessToken = token,

            // Timeout settings
            DefaultTimeout = TimeSpan.FromSeconds(60),

            // Rate limit handling - automatically retry when rate limited
            AutoRetryOnRateLimit = true,
            MaxRateLimitRetries = 5,

            // Transient error handling - retry on network errors
            EnableTransientRetry = true,
            MaxTransientRetries = 3,
            TransientRetryBaseDelay = TimeSpan.FromSeconds(1),
            TransientRetryMaxDelay = TimeSpan.FromSeconds(30)
        };

        using (var customClient = new LichessClient(new HttpClient(), options))
        {
            SampleRunner.PrintSuccess("Created client with custom options");
            SampleRunner.PrintKeyValue("Timeout", options.DefaultTimeout);
            SampleRunner.PrintKeyValue("Auto-retry on rate limit", options.AutoRetryOnRateLimit);
            SampleRunner.PrintKeyValue("Max rate limit retries", options.MaxRateLimitRetries);
            SampleRunner.PrintKeyValue("Transient retry enabled", options.EnableTransientRetry);
        }

        // =====================================================================
        // Quick API Demos
        // =====================================================================
        SampleRunner.PrintSubHeader("Quick API Demos");

        using (var client = new LichessClient())
        {
            Console.WriteLine("Demonstrating various public APIs:");
            Console.WriteLine();

            // Users API
            var leaderboard = await client.Users.GetLeaderboardAsync("bullet", 3);
            Console.WriteLine($"  Users API - Top 3 bullet: {string.Join(", ", leaderboard.Select(u => u.Username))}");

            // Puzzles API
            var daily = await client.Puzzles.GetDailyAsync();
            Console.WriteLine($"  Puzzles API - Daily puzzle: {daily.Puzzle?.Id} (rating {daily.Puzzle?.Rating})");

            // TV API
            var tvGames = await client.Tv.GetCurrentGamesAsync();
            Console.WriteLine($"  TV API - Blitz TV: {tvGames.Blitz?.User?.Name ?? "N/A"}");

            // Analysis API
            var eval = await client.Analysis.GetCloudEvaluationAsync("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            Console.WriteLine($"  Analysis API - Starting position depth: {eval?.Depth ?? 0}");

            // Tablebase API
            var tablebase = await client.Tablebase.LookupAsync("8/8/8/4k3/8/8/8/4K2R w - - 0 1");
            Console.WriteLine($"  Tablebase API - K+R vs K: {tablebase.Category}");

            // Tournaments API
            var tournaments = await client.ArenaTournaments.GetCurrentAsync();
            var activeCount = tournaments.Started?.Count ?? 0;
            Console.WriteLine($"  Tournaments API - Active arenas: {activeCount}");
        }

        SampleRunner.PrintSuccess("Getting Started sample completed!");
    }
}