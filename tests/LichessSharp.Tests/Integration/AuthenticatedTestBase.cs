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
///         The client is configured with <see cref="LichessClientOptions.UnlimitedRateLimitRetries" /> enabled,
///         which means it will automatically wait and retry when rate limited by Lichess.
///         This ensures tests eventually complete rather than failing due to rate limits.
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
    ///     Gets the username of the authenticated user (cached after first call).
    /// </summary>
    private string? _username;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthenticatedTestBase" /> class.
    ///     Creates a LichessClient configured for integration testing with unlimited rate limit retries
    ///     and an extended timeout to handle long waits when Lichess rate limits requests.
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
                DefaultTimeout = TimeSpan.FromMinutes(10),
                UnlimitedRateLimitRetries = true
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
}