// LichessSharp Samples
// This file demonstrates basic usage of the LichessSharp library.

using LichessSharp;

Console.WriteLine("LichessSharp Sample Application");
Console.WriteLine("==============================\n");

// Example 1: Create a client without authentication (public API only)
Console.WriteLine("1. Creating an unauthenticated client...");
using var publicClient = new LichessClient();
Console.WriteLine("   Client created successfully.\n");

// Example 2: Create a client with an access token
Console.WriteLine("2. Creating an authenticated client...");
var token = Environment.GetEnvironmentVariable("LICHESS_TOKEN");
if (!string.IsNullOrEmpty(token))
{
    using var authenticatedClient = new LichessClient(token);
    Console.WriteLine("   Authenticated client created.\n");
}
else
{
    Console.WriteLine("   Skipped - Set LICHESS_TOKEN environment variable to test authenticated access.\n");
}

// Example 3: Create a client with custom options
Console.WriteLine("3. Creating a client with custom options...");
var options = new LichessClientOptions
{
    AccessToken = token,
    DefaultTimeout = TimeSpan.FromSeconds(60),
    AutoRetryOnRateLimit = true,
    MaxRateLimitRetries = 5
};
using var customClient = new LichessClient(new HttpClient(), options);
Console.WriteLine("   Custom client created.\n");

// Example 4: Accessing API surface areas
Console.WriteLine("4. Available API surface areas:");
Console.WriteLine("   - Account API: Profile, email, preferences");
Console.WriteLine("   - Users API: User profiles, status, leaderboards");
Console.WriteLine("   - Games API: Export and stream games");
Console.WriteLine("   - Puzzles API: Daily puzzle, dashboard, storm");
Console.WriteLine("   - Teams API: Team management");
Console.WriteLine("   - Board API: Play with physical boards");
Console.WriteLine("   - Bot API: Bot account operations");
Console.WriteLine("   - Challenges API: Send and receive challenges");
Console.WriteLine("   - Tournaments API: Arena and Swiss tournaments");
Console.WriteLine("   - Studies API: Lichess studies");
Console.WriteLine("   - TV API: Lichess TV channels");
Console.WriteLine("   - Analysis API: Cloud evaluations");
Console.WriteLine("   - Opening Explorer API: Position lookups");
Console.WriteLine("   - Tablebase API: Endgame tablebases");
Console.WriteLine();

Console.WriteLine("Note: API implementations are being developed incrementally.");
Console.WriteLine("See the documentation for the current implementation status.\n");

Console.WriteLine("Sample completed.");
