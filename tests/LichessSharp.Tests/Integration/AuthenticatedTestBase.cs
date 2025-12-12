using LichessSharp.Exceptions;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Base class for integration tests that require authentication.
///     Creates a LichessClient with a token from the environment variable.
/// </summary>
/// <remarks>
///     <para>
///         Tests inheriting from this class will be skipped if no token is available.
///         Set the <c>LICHESS_TEST_TOKEN</c> environment variable to run these tests.
///     </para>
///     <para>
///         Example usage:
///         <code>
/// [AuthenticatedTest]
/// public class MyAuthenticatedTests : AuthenticatedTestBase
/// {
///     [Fact]
///     public async Task MyTest()
///     {
///         var profile = await Client.Account.GetProfileAsync();
///         // ...
///     }
/// }
/// </code>
///     </para>
/// </remarks>
public abstract class AuthenticatedTestBase : IDisposable
{
    /// <summary>
    ///     Default number of retry attempts for rate-limited requests.
    /// </summary>
    protected const int DefaultMaxRetries = 3;

    /// <summary>
    ///     Default base delay in milliseconds for exponential backoff.
    /// </summary>
    protected const int DefaultBaseDelayMs = 1000;
    /// <summary>
    ///     Gets the username of the authenticated user (cached after first call).
    /// </summary>
    private string? _username;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthenticatedTestBase" /> class.
    ///     Creates a LichessClient with a longer timeout suitable for integration tests.
    ///     The timeout is increased to 2 minutes to accommodate rate limit retry delays
    ///     (Lichess API can request up to 60 second waits).
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the test token environment variable is not set.
    /// </exception>
    protected AuthenticatedTestBase()
    {
        var token = TestConfiguration.LichessToken;

        if (string.IsNullOrWhiteSpace(token)) throw new InvalidOperationException(TestConfiguration.SkipReason);

        Client = new LichessClient(
            new HttpClient(),
            new LichessClientOptions
            {
                AccessToken = token,
                DefaultTimeout = TimeSpan.FromMinutes(2)
            });
    }

    /// <summary>
    ///     Gets the authenticated Lichess client.
    /// </summary>
    protected LichessClient Client { get; }

    /// <summary>
    ///     Disposes the client.
    /// </summary>
    public void Dispose()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Gets the username of the authenticated user.
    ///     Makes an API call on first access, then caches the result.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The username of the authenticated user.</returns>
    protected async Task<string> GetAuthenticatedUsernameAsync(CancellationToken cancellationToken = default)
    {
        if (_username == null)
        {
            var profile = await Client.Account.GetProfileAsync(cancellationToken);
            _username = profile.Username;
        }

        return _username;
    }

    /// <summary>
    ///     Executes an async action with retry logic for rate limiting.
    ///     Uses exponential backoff when a <see cref="LichessRateLimitException" /> is thrown.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The async action to execute.</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3).</param>
    /// <param name="baseDelayMs">Base delay in milliseconds for exponential backoff (default: 1000).</param>
    /// <returns>The result of the action.</returns>
    protected static async Task<T> WithRetryAsync<T>(
        Func<Task<T>> action,
        int maxRetries = DefaultMaxRetries,
        int baseDelayMs = DefaultBaseDelayMs)
    {
        var attempt = 0;
        while (true)
        {
            try
            {
                return await action();
            }
            catch (LichessRateLimitException ex)
            {
                attempt++;
                if (attempt > maxRetries)
                    throw;

                // Use RetryAfter if provided, otherwise use exponential backoff
                var delay = ex.RetryAfter ?? TimeSpan.FromMilliseconds(baseDelayMs * Math.Pow(2, attempt - 1));

                // Cap the delay at 60 seconds
                if (delay > TimeSpan.FromSeconds(60))
                    delay = TimeSpan.FromSeconds(60);

                await Task.Delay(delay);
            }
        }
    }

    /// <summary>
    ///     Executes an async action with retry logic for rate limiting.
    ///     Uses exponential backoff when a <see cref="LichessRateLimitException" /> is thrown.
    /// </summary>
    /// <param name="action">The async action to execute.</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3).</param>
    /// <param name="baseDelayMs">Base delay in milliseconds for exponential backoff (default: 1000).</param>
    protected static async Task WithRetryAsync(
        Func<Task> action,
        int maxRetries = DefaultMaxRetries,
        int baseDelayMs = DefaultBaseDelayMs)
    {
        await WithRetryAsync(async () =>
        {
            await action();
            return true;
        }, maxRetries, baseDelayMs);
    }
}