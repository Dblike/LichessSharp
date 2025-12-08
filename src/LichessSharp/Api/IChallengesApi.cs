namespace LichessSharp.Api;

/// <summary>
/// Challenges API - Send and receive challenges to play.
/// </summary>
public interface IChallengesApi
{
    /// <summary>
    /// Get pending challenges.
    /// </summary>
    Task<ChallengeList> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Challenge a user.
    /// </summary>
    Task<Challenge> CreateAsync(string username, ChallengeOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Accept a challenge.
    /// </summary>
    Task<bool> AcceptAsync(string challengeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decline a challenge.
    /// </summary>
    Task<bool> DeclineAsync(string challengeId, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a challenge.
    /// </summary>
    Task<bool> CancelAsync(string challengeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Challenge the AI.
    /// </summary>
    Task<Challenge> ChallengeAiAsync(ChallengeAiOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create an open challenge.
    /// </summary>
    Task<Challenge> CreateOpenAsync(ChallengeOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Start clocks of a game.
    /// </summary>
    Task<bool> StartClocksAsync(string gameId, string? token1 = null, string? token2 = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add time to opponent's clock.
    /// </summary>
    Task<bool> AddTimeAsync(string gameId, int seconds, CancellationToken cancellationToken = default);
}

/// <summary>
/// List of challenges.
/// </summary>
public class ChallengeList
{
    /// <summary>
    /// Incoming challenges.
    /// </summary>
    public IReadOnlyList<Challenge>? In { get; init; }

    /// <summary>
    /// Outgoing challenges.
    /// </summary>
    public IReadOnlyList<Challenge>? Out { get; init; }
}

/// <summary>
/// A challenge.
/// </summary>
public class Challenge
{
    /// <summary>
    /// Challenge ID.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Challenge URL.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Challenge status.
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Challenger info.
    /// </summary>
    public ChallengePlayer? Challenger { get; init; }

    /// <summary>
    /// Challenged player info.
    /// </summary>
    public ChallengePlayer? DestUser { get; init; }

    /// <summary>
    /// The variant.
    /// </summary>
    public ChallengeVariant? Variant { get; init; }

    /// <summary>
    /// Whether rated.
    /// </summary>
    public bool Rated { get; init; }

    /// <summary>
    /// Time control.
    /// </summary>
    public ChallengeTimeControl? TimeControl { get; init; }

    /// <summary>
    /// The color requested.
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// Speed of the game.
    /// </summary>
    public string? Speed { get; init; }
}

/// <summary>
/// Challenge player.
/// </summary>
public class ChallengePlayer
{
    /// <summary>
    /// User ID.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Username.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Rating.
    /// </summary>
    public int? Rating { get; init; }
}

/// <summary>
/// Challenge variant.
/// </summary>
public class ChallengeVariant
{
    /// <summary>
    /// Variant key.
    /// </summary>
    public string? Key { get; init; }

    /// <summary>
    /// Variant name.
    /// </summary>
    public string? Name { get; init; }
}

/// <summary>
/// Challenge time control.
/// </summary>
public class ChallengeTimeControl
{
    /// <summary>
    /// Time control type.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Initial time in seconds.
    /// </summary>
    public int? Limit { get; init; }

    /// <summary>
    /// Increment in seconds.
    /// </summary>
    public int? Increment { get; init; }

    /// <summary>
    /// Display string.
    /// </summary>
    public string? Show { get; init; }
}

/// <summary>
/// Options for creating a challenge.
/// </summary>
public class ChallengeOptions
{
    /// <summary>
    /// Whether the game should be rated.
    /// </summary>
    public bool Rated { get; set; }

    /// <summary>
    /// Clock limit in seconds.
    /// </summary>
    public int? ClockLimit { get; set; }

    /// <summary>
    /// Clock increment in seconds.
    /// </summary>
    public int? ClockIncrement { get; set; }

    /// <summary>
    /// Days per move (for correspondence).
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// Color preference.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Variant.
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// Starting FEN.
    /// </summary>
    public string? Fen { get; set; }
}

/// <summary>
/// Options for challenging AI.
/// </summary>
public class ChallengeAiOptions : ChallengeOptions
{
    /// <summary>
    /// AI level (1-8).
    /// </summary>
    public int Level { get; set; } = 1;
}
