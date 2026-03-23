namespace LichessSharp.Tests.Integration;

/// <summary>
///     Base class for integration tests that require authentication.
///     Uses <see cref="LichessTestFixture" /> for request throttling to avoid rate limits.
/// </summary>
/// <remarks>
///     <para>
///         Tests inheriting from this class will be skipped if no token is available.
///         Set the <c>LICHESS_TEST_TOKEN</c> environment variable to run these tests.
///     </para>
///     <para>
///         All subclasses must be annotated with <c>[Collection("Lichess API")]</c> to share
///         the fixture and run sequentially. Call <see cref="ThrottleAsync" /> before each API call.
///     </para>
/// </remarks>
public abstract class AuthenticatedTestBase : IDisposable
{
    private string? _username;

    /// <exception cref="InvalidOperationException">
    ///     Thrown when the test token environment variable is not set.
    /// </exception>
    protected AuthenticatedTestBase(LichessTestFixture fixture)
    {
        var token = TestConfiguration.LichessToken;

        if (string.IsNullOrWhiteSpace(token)) throw new InvalidOperationException(TestConfiguration.SkipReason);

        Fixture = fixture;
        Client = fixture.CreateAuthenticatedClient(token);
    }

    /// <summary>
    ///     The shared test fixture providing throttling and client creation.
    /// </summary>
    protected LichessTestFixture Fixture { get; }

    /// <summary>
    ///     Gets the authenticated Lichess client.
    /// </summary>
    protected LichessClient Client { get; }

    /// <summary>
    ///     Throttles the next API request to respect Lichess rate limits.
    ///     Call this before each API call.
    /// </summary>
    protected Task ThrottleAsync(CancellationToken cancellationToken = default) =>
        Fixture.ThrottleAsync(cancellationToken);

    public void Dispose()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Gets the username of the authenticated user.
    ///     Makes an API call on first access, then caches the result.
    /// </summary>
    protected async Task<string> GetAuthenticatedUsernameAsync(CancellationToken cancellationToken = default)
    {
        if (_username == null)
        {
            await ThrottleAsync(cancellationToken);
            var profile = await Client.Account.GetProfileAsync(cancellationToken);
            _username = profile.Username;
        }

        return _username;
    }
}