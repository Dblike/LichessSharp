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
    ///     Defaults to 3.
    /// </summary>
    public int MaxRateLimitRetries { get; set; } = 3;

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