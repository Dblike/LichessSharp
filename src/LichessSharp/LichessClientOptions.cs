namespace LichessSharp;

/// <summary>
///     Configuration options for the Lichess API client.
/// </summary>
public sealed class LichessClientOptions
{
    /// <summary>
    ///     The personal access token for authenticated API requests.
    ///     Generate one at https://lichess.org/account/oauth/token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    ///     The default timeout for HTTP requests.
    ///     Defaults to 30 seconds.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     The timeout for streaming requests.
    ///     Defaults to infinite (no timeout).
    /// </summary>
    public TimeSpan StreamingTimeout { get; set; } = Timeout.InfiniteTimeSpan;

    /// <summary>
    ///     Whether to automatically handle rate limiting by waiting and retrying.
    ///     Defaults to true.
    /// </summary>
    public bool AutoRetryOnRateLimit { get; set; } = true;

    /// <summary>
    ///     Maximum number of retries when rate limited.
    ///     Defaults to 3. Ignored when <see cref="UnlimitedRateLimitRetries" /> is true.
    /// </summary>
    public int MaxRateLimitRetries { get; set; } = 3;

    /// <summary>
    ///     Whether to retry indefinitely when rate limited, respecting the Retry-After header.
    ///     This is useful for integration tests that need to wait for rate limits to clear.
    ///     Defaults to false. When true, <see cref="MaxRateLimitRetries" /> is ignored.
    /// </summary>
    /// <remarks>
    ///     When enabled, the client will continue retrying 429 responses until the request succeeds
    ///     or the cancellation token is triggered. The Retry-After header from Lichess is always
    ///     respected to avoid premature retry attempts.
    /// </remarks>
    public bool UnlimitedRateLimitRetries { get; set; }

    /// <summary>
    ///     Whether to automatically retry on transient network failures (DNS errors, connection timeouts, etc.).
    ///     Defaults to true.
    /// </summary>
    public bool EnableTransientRetry { get; set; } = true;

    /// <summary>
    ///     Maximum number of retries for transient network failures.
    ///     Defaults to 3.
    /// </summary>
    public int MaxTransientRetries { get; set; } = 3;

    /// <summary>
    ///     Base delay for exponential backoff when retrying transient failures.
    ///     The actual delay will be: baseDelay * 2^(retryAttempt-1) + random jitter.
    ///     Defaults to 1 second.
    /// </summary>
    public TimeSpan TransientRetryBaseDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    ///     Maximum delay between transient retry attempts.
    ///     Defaults to 30 seconds.
    /// </summary>
    public TimeSpan TransientRetryMaxDelay { get; set; } = TimeSpan.FromSeconds(30);
}