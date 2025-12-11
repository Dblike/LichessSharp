using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
/// Sample 01: Getting Started
/// Demonstrates basic client setup and configuration options.
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
        // Available API Surface
        // =====================================================================
        SampleRunner.PrintSubHeader("Available APIs");

        Console.WriteLine("The LichessClient exposes the following API areas:");
        Console.WriteLine();
        Console.WriteLine("  Public (No Auth Required):");
        Console.WriteLine("    - Users        : User profiles, status, leaderboards");
        Console.WriteLine("    - Games        : Export and stream games");
        Console.WriteLine("    - Tv           : Live TV channels");
        Console.WriteLine("    - Puzzles      : Daily puzzle, dashboard");
        Console.WriteLine("    - Analysis     : Cloud evaluations");
        Console.WriteLine("    - OpeningExplorer : Position lookups");
        Console.WriteLine("    - Tablebase    : Endgame tablebases");
        Console.WriteLine("    - ArenaTournaments : Tournament data");
        Console.WriteLine("    - SwissTournaments : Swiss tournament data");
        Console.WriteLine("    - Broadcasts   : Live event broadcasts");
        Console.WriteLine("    - Fide         : FIDE player data");
        Console.WriteLine();
        Console.WriteLine("  Authenticated:");
        Console.WriteLine("    - Account      : Your profile, email, preferences");
        Console.WriteLine("    - Relations    : Follow/block users");
        Console.WriteLine("    - Board        : Play games with physical boards");
        Console.WriteLine("    - Bot          : Bot account operations");
        Console.WriteLine("    - Challenges   : Send and receive challenges");
        Console.WriteLine("    - Teams        : Team management");
        Console.WriteLine("    - Studies      : Lichess studies");
        Console.WriteLine("    - Messaging    : Private messages");
        Console.WriteLine("    - BulkPairings : Tournament organizer tools");

        // =====================================================================
        // Best Practices
        // =====================================================================
        SampleRunner.PrintSubHeader("Best Practices");

        Console.WriteLine("1. Always dispose the client when done:");
        Console.WriteLine("   using var client = new LichessClient();");
        Console.WriteLine();
        Console.WriteLine("2. Use environment variables for tokens:");
        Console.WriteLine("   var token = Environment.GetEnvironmentVariable(\"LICHESS_TOKEN\");");
        Console.WriteLine();
        Console.WriteLine("3. Handle cancellation for long-running operations:");
        Console.WriteLine("   using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));");
        Console.WriteLine("   await client.Games.StreamUserGamesAsync(\"user\", cts.Token);");
        Console.WriteLine();
        Console.WriteLine("4. Enable rate limit retry for production apps:");
        Console.WriteLine("   options.AutoRetryOnRateLimit = true;");

        SampleRunner.PrintSuccess("Getting Started sample completed!");
    }
}
