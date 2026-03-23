using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Shared fixture for all Lichess integration tests.
///     Provides rate-limited clients that pace requests to avoid hitting Lichess API limits.
/// </summary>
/// <remarks>
///     <para>
///         All integration tests share this fixture via the <see cref="LichessApiCollection" />.
///         This forces sequential execution and provides request throttling.
///     </para>
///     <para>
///         Lichess rate limits are approximately 1 request/second for most endpoints.
///         The throttle adds a configurable delay between requests across all tests.
///     </para>
/// </remarks>
public sealed class LichessTestFixture : IDisposable
{
    private readonly SemaphoreSlim _throttle = new(1, 1);
    private DateTimeOffset _lastRequest = DateTimeOffset.MinValue;

    /// <summary>
    ///     Minimum delay between consecutive API requests.
    ///     Default is 1.5 seconds to stay safely under Lichess's ~1 req/sec limit.
    /// </summary>
    public TimeSpan RequestDelay { get; init; } = TimeSpan.FromMilliseconds(1500);

    /// <summary>
    ///     Default timeout per test (reduced from 10 minutes since throttling prevents rate limits).
    /// </summary>
    public TimeSpan DefaultTimeout { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>
    ///     Creates a LichessClient configured for integration testing (unauthenticated).
    /// </summary>
    public LichessClient CreateClient()
    {
        return new LichessClient(
            new HttpClient(),
            new LichessClientOptions
            {
                DefaultTimeout = DefaultTimeout,
                AutoRetryOnRateLimit = true,
                MaxRateLimitRetries = 3
            });
    }

    /// <summary>
    ///     Creates a LichessClient configured for authenticated integration testing.
    /// </summary>
    /// <param name="token">The OAuth access token.</param>
    public LichessClient CreateAuthenticatedClient(string token)
    {
        return new LichessClient(
            new HttpClient(),
            new LichessClientOptions
            {
                AccessToken = token,
                DefaultTimeout = DefaultTimeout,
                AutoRetryOnRateLimit = true,
                MaxRateLimitRetries = 3
            });
    }

    /// <summary>
    ///     Waits until it is safe to make another API request, based on the configured delay.
    ///     Call this before each API call in integration tests.
    /// </summary>
    public async Task ThrottleAsync(CancellationToken cancellationToken = default)
    {
        await _throttle.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var elapsed = DateTimeOffset.UtcNow - _lastRequest;
            if (elapsed < RequestDelay)
            {
                var remaining = RequestDelay - elapsed;
                await Task.Delay(remaining, cancellationToken).ConfigureAwait(false);
            }

            _lastRequest = DateTimeOffset.UtcNow;
        }
        finally
        {
            _throttle.Release();
        }
    }

    public void Dispose()
    {
        _throttle.Dispose();
    }
}

/// <summary>
///     Collection definition that groups all Lichess integration tests.
///     Tests in this collection run sequentially and share a <see cref="LichessTestFixture" />.
/// </summary>
[CollectionDefinition("Lichess API")]
public class LichessApiCollection : ICollectionFixture<LichessTestFixture>;
