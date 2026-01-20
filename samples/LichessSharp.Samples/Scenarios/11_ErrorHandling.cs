using LichessSharp.Exceptions;
using LichessSharp.Samples.Helpers;

namespace LichessSharp.Samples.Scenarios;

/// <summary>
///     Sample 11: Error Handling
///     Demonstrates how to handle various error scenarios when using the Lichess API.
/// </summary>
public static class ErrorHandling
{
    public static async Task RunAsync()
    {
        SampleRunner.PrintHeader("11 - Error Handling");

        using var client = new LichessClient();

        // =====================================================================
        // Not Found Errors
        // =====================================================================
        SampleRunner.PrintSubHeader("Not Found Errors (404)");

        Console.WriteLine("Attempting to fetch a non-existent user...");
        try
        {
            await client.Users.GetAsync("ThisUserDoesNotExist12345XYZ");
            SampleRunner.PrintError("Expected exception was not thrown!");
        }
        catch (LichessNotFoundException ex)
        {
            SampleRunner.PrintSuccess("Caught LichessNotFoundException");
            SampleRunner.PrintKeyValue("Message", ex.Message);
            SampleRunner.PrintKeyValue("Lichess Error", ex.LichessError ?? "(none)");
        }

        Console.WriteLine();
        Console.WriteLine("Attempting to fetch a non-existent game...");
        try
        {
            await client.Games.ExportAsync("XXXXXXXXX");
        }
        catch (LichessNotFoundException)
        {
            SampleRunner.PrintSuccess("Caught LichessNotFoundException for game");
        }

        // =====================================================================
        // Authentication Errors
        // =====================================================================
        SampleRunner.PrintSubHeader("Authentication Errors (401)");

        Console.WriteLine("Attempting to access authenticated endpoint without token...");
        try
        {
            // Account API requires authentication
            await client.Account.GetProfileAsync();
            SampleRunner.PrintError("Expected exception was not thrown!");
        }
        catch (LichessAuthenticationException ex)
        {
            SampleRunner.PrintSuccess("Caught LichessAuthenticationException");
            SampleRunner.PrintKeyValue("Message", ex.Message);
        }

        Console.WriteLine();
        Console.WriteLine("Attempting with an invalid token...");
        using (var badClient = new LichessClient("invalid_token_12345"))
        {
            try
            {
                await badClient.Account.GetProfileAsync();
            }
            catch (LichessAuthenticationException)
            {
                SampleRunner.PrintSuccess("Caught LichessAuthenticationException for invalid token");
            }
        }

        // =====================================================================
        // Authorization Errors (Missing Scope)
        // =====================================================================
        SampleRunner.PrintSubHeader("Authorization Errors (403)");

        Console.WriteLine("Authorization errors occur when your token lacks required scopes.");
        Console.WriteLine("Common scenarios: Board API without 'board:play', Bot API without 'bot:play'");
        Console.WriteLine();
        Console.WriteLine("Handle with LichessAuthorizationException (has RequiredScope property).");

        // =====================================================================
        // Rate Limiting
        // =====================================================================
        SampleRunner.PrintSubHeader("Rate Limiting (429)");

        Console.WriteLine("Rate limits protect the API from abuse.");
        Console.WriteLine("Use LichessClientOptions.AutoRetryOnRateLimit = true for automatic handling.");
        Console.WriteLine("Or catch LichessRateLimitException and use its RetryAfter property.");

        // =====================================================================
        // Cancellation
        // =====================================================================
        SampleRunner.PrintSubHeader("Cancellation Handling");

        Console.WriteLine("Cancelling a streaming operation...");

        using var cts = new CancellationTokenSource();

        // Start a streaming task
        var streamTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var game in client.Games.StreamUserGamesAsync("DrNykterstein",
                                   cancellationToken: cts.Token))
                {
                    Console.WriteLine($"  Received game: {game.Id}");
                    await Task.Delay(100); // Simulate processing
                }
            }
            catch (OperationCanceledException)
            {
                SampleRunner.PrintSuccess("Stream was cancelled successfully");
            }
        });

        // Cancel after a short delay
        await Task.Delay(500);
        Console.WriteLine("  Requesting cancellation...");
        cts.Cancel();

        await streamTask;

        // =====================================================================
        // Timeout Handling
        // =====================================================================
        SampleRunner.PrintSubHeader("Timeout Handling");

        Console.WriteLine("Setting a short timeout...");

        var timeoutOptions = new LichessClientOptions
        {
            DefaultTimeout = TimeSpan.FromMilliseconds(1) // Extremely short for demo
        };

        using (var timeoutClient = new LichessClient(new HttpClient(), timeoutOptions))
        {
            try
            {
                await timeoutClient.Users.GetAsync("DrNykterstein");
            }
            catch (TaskCanceledException)
            {
                SampleRunner.PrintSuccess("Request timed out as expected");
            }
            catch (Exception ex)
            {
                // May be wrapped differently depending on .NET version
                Console.WriteLine($"  Caught: {ex.GetType().Name}");
            }
        }

        // =====================================================================
        // Transient Errors
        // =====================================================================
        SampleRunner.PrintSubHeader("Transient Error Handling");

        Console.WriteLine("Transient errors include network issues, DNS failures, etc.");
        Console.WriteLine("Use LichessClientOptions.EnableTransientRetry = true for automatic handling.");
        Console.WriteLine("Retries use exponential backoff with jitter.");

        // =====================================================================
        // Exception Hierarchy
        // =====================================================================
        SampleRunner.PrintSubHeader("Exception Hierarchy");

        Console.WriteLine("LichessException (base)");
        Console.WriteLine("  ├── LichessNotFoundException         (404)");
        Console.WriteLine("  ├── LichessAuthenticationException   (401)");
        Console.WriteLine("  ├── LichessAuthorizationException    (403)");
        Console.WriteLine("  └── LichessRateLimitException        (429)");
        Console.WriteLine();
        Console.WriteLine("Properties available on LichessException:");
        Console.WriteLine("  - Message       : Error description");
        Console.WriteLine("  - LichessError  : Error message from Lichess API");
        Console.WriteLine("  - StatusCode    : HTTP status code (if available)");
        Console.WriteLine();
        Console.WriteLine("Additional on LichessRateLimitException:");
        Console.WriteLine("  - RetryAfter    : TimeSpan? indicating when to retry");
        Console.WriteLine();
        Console.WriteLine("Additional on LichessAuthorizationException:");
        Console.WriteLine("  - RequiredScope : The OAuth scope that was missing");

        SampleRunner.PrintSuccess("Error Handling sample completed!");
    }
}