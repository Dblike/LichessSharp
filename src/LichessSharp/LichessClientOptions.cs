namespace LichessSharp;

/// <summary>
/// Configuration options for the Lichess API client.
/// </summary>
public sealed class LichessClientOptions
{
    /// <summary>
    /// The personal access token for authenticated API requests.
    /// Generate one at https://lichess.org/account/oauth/token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// The base address for the Lichess API.
    /// Defaults to https://lichess.org
    /// </summary>
    public Uri BaseAddress { get; set; } = new("https://lichess.org");

    /// <summary>
    /// The base address for the Opening Explorer API.
    /// Defaults to https://explorer.lichess.ovh
    /// </summary>
    public Uri ExplorerBaseAddress { get; set; } = new("https://explorer.lichess.ovh");

    /// <summary>
    /// The base address for the Tablebase API.
    /// Defaults to https://tablebase.lichess.ovh
    /// </summary>
    public Uri TablebaseBaseAddress { get; set; } = new("https://tablebase.lichess.ovh");

    /// <summary>
    /// The default timeout for HTTP requests.
    /// Defaults to 30 seconds.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// The timeout for streaming requests.
    /// Defaults to infinite (no timeout).
    /// </summary>
    public TimeSpan StreamingTimeout { get; set; } = Timeout.InfiniteTimeSpan;

    /// <summary>
    /// Whether to automatically handle rate limiting by waiting and retrying.
    /// Defaults to true.
    /// </summary>
    public bool AutoRetryOnRateLimit { get; set; } = true;

    /// <summary>
    /// Maximum number of retries when rate limited.
    /// Defaults to 3.
    /// </summary>
    public int MaxRateLimitRetries { get; set; } = 3;
}
