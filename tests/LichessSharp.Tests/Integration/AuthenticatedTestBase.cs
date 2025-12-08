namespace LichessSharp.Tests.Integration;

/// <summary>
/// Base class for integration tests that require authentication.
/// Creates a LichessClient with a token from the environment variable.
/// </summary>
/// <remarks>
/// <para>
/// Tests inheriting from this class will be skipped if no token is available.
/// Set the <c>LICHESS_TEST_TOKEN</c> environment variable to run these tests.
/// </para>
/// <para>
/// Example usage:
/// <code>
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
/// </para>
/// </remarks>
public abstract class AuthenticatedTestBase : IDisposable
{
    /// <summary>
    /// Gets the authenticated Lichess client.
    /// </summary>
    protected LichessClient Client { get; }

    /// <summary>
    /// Gets the username of the authenticated user (cached after first call).
    /// </summary>
    private string? _username;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticatedTestBase"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the test token environment variable is not set.
    /// </exception>
    protected AuthenticatedTestBase()
    {
        var token = TestConfiguration.LichessToken;

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(TestConfiguration.SkipReason);
        }

        Client = new LichessClient(token);
    }

    /// <summary>
    /// Gets the username of the authenticated user.
    /// Makes an API call on first access, then caches the result.
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
    /// Disposes the client.
    /// </summary>
    public void Dispose()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }
}
