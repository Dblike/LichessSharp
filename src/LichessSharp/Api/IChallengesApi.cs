using System.Text.Json.Serialization;

namespace LichessSharp.Api;

/// <summary>
/// Challenges API - Send and receive challenges to play.
/// <see href="https://lichess.org/api#tag/Challenges"/>
/// </summary>
public interface IChallengesApi
{
    /// <summary>
    /// Get a list of challenges created by or targeted at you.
    /// Requires OAuth scope: challenge:read
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Lists of incoming and outgoing challenges.</returns>
    Task<ChallengeList> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Show details of a single challenge.
    /// Requires OAuth scope: challenge:read
    /// </summary>
    /// <param name="challengeId">The challenge ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The challenge details.</returns>
    Task<ChallengeJson> ShowAsync(string challengeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Challenge another player to a game.
    /// Requires OAuth scope: challenge:write, bot:play, or board:play
    /// </summary>
    /// <param name="username">The username to challenge.</param>
    /// <param name="options">Challenge options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created challenge.</returns>
    Task<ChallengeJson> CreateAsync(string username, ChallengeCreateOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Accept an incoming challenge.
    /// Requires OAuth scope: challenge:write, bot:play, or board:play
    /// </summary>
    /// <param name="challengeId">The challenge ID to accept.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> AcceptAsync(string challengeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decline an incoming challenge.
    /// Requires OAuth scope: challenge:write, bot:play, or board:play
    /// </summary>
    /// <param name="challengeId">The challenge ID to decline.</param>
    /// <param name="reason">Optional reason for declining.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> DeclineAsync(string challengeId, ChallengeDeclineReason? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a challenge you created.
    /// Requires OAuth scope: challenge:write, bot:play, or board:play
    /// </summary>
    /// <param name="challengeId">The challenge ID to cancel.</param>
    /// <param name="opponentToken">Optional opponent token for open challenges.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> CancelAsync(string challengeId, string? opponentToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Challenge the Lichess AI (Stockfish).
    /// Requires OAuth scope: challenge:write, bot:play, or board:play
    /// </summary>
    /// <param name="options">AI challenge options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created game against AI.</returns>
    Task<ChallengeAiResponse> ChallengeAiAsync(ChallengeAiOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create an open challenge that any two players can join.
    /// Requires OAuth scope: challenge:write
    /// </summary>
    /// <param name="options">Open challenge options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created open challenge with URLs for both players.</returns>
    Task<ChallengeOpenJson> CreateOpenAsync(ChallengeOpenOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Start the clocks of a game immediately, even if a player has not yet made a move.
    /// Requires OAuth scopes from both players.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="token1">OAuth token of the first player (or current user if omitted).</param>
    /// <param name="token2">OAuth token of the second player (or current user if omitted).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> StartClocksAsync(string gameId, string? token1 = null, string? token2 = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add seconds to the opponent's clock.
    /// Requires OAuth scope: challenge:write, bot:play, or board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="seconds">Number of seconds to add (1-60).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> AddTimeAsync(string gameId, int seconds, CancellationToken cancellationToken = default);
}

#region Enums

/// <summary>
/// Challenge status.
/// </summary>
public enum ChallengeStatus
{
    /// <summary>
    /// Challenge was just created.
    /// </summary>
    Created,

    /// <summary>
    /// Challenge target is offline.
    /// </summary>
    Offline,

    /// <summary>
    /// Challenge was canceled.
    /// </summary>
    Canceled,

    /// <summary>
    /// Challenge was declined.
    /// </summary>
    Declined,

    /// <summary>
    /// Challenge was accepted.
    /// </summary>
    Accepted
}

/// <summary>
/// Reason for declining a challenge.
/// </summary>
public enum ChallengeDeclineReason
{
    /// <summary>
    /// Generic decline.
    /// </summary>
    Generic,

    /// <summary>
    /// Later.
    /// </summary>
    Later,

    /// <summary>
    /// Too fast.
    /// </summary>
    TooFast,

    /// <summary>
    /// Too slow.
    /// </summary>
    TooSlow,

    /// <summary>
    /// Time control not acceptable.
    /// </summary>
    TimeControl,

    /// <summary>
    /// Rated game required.
    /// </summary>
    Rated,

    /// <summary>
    /// Casual game required.
    /// </summary>
    Casual,

    /// <summary>
    /// Standard chess required.
    /// </summary>
    Standard,

    /// <summary>
    /// Variant not acceptable.
    /// </summary>
    Variant,

    /// <summary>
    /// Not accepting challenges from this user.
    /// </summary>
    NoBot,

    /// <summary>
    /// Only accepting challenges from bots.
    /// </summary>
    OnlyBot
}

/// <summary>
/// Color preference for challenges.
/// </summary>
public enum ChallengeColor
{
    /// <summary>
    /// Random color assignment.
    /// </summary>
    Random,

    /// <summary>
    /// Play as white.
    /// </summary>
    White,

    /// <summary>
    /// Play as black.
    /// </summary>
    Black
}

/// <summary>
/// Extra game rules.
/// </summary>
[Flags]
public enum ChallengeRules
{
    /// <summary>
    /// No special rules.
    /// </summary>
    None = 0,

    /// <summary>
    /// Cannot abort the game.
    /// </summary>
    NoAbort = 1,

    /// <summary>
    /// Cannot offer rematch.
    /// </summary>
    NoRematch = 2,

    /// <summary>
    /// Cannot give extra time.
    /// </summary>
    NoGiveTime = 4,

    /// <summary>
    /// Cannot claim win on time.
    /// </summary>
    NoClaimWin = 8,

    /// <summary>
    /// Cannot draw before move 30.
    /// </summary>
    NoEarlyDraw = 16
}

#endregion

#region Models

/// <summary>
/// List of challenges.
/// </summary>
public class ChallengeList
{
    /// <summary>
    /// Incoming challenges (targeted at you).
    /// </summary>
    [JsonPropertyName("in")]
    public IReadOnlyList<ChallengeJson>? In { get; init; }

    /// <summary>
    /// Outgoing challenges (created by you).
    /// </summary>
    [JsonPropertyName("out")]
    public IReadOnlyList<ChallengeJson>? Out { get; init; }
}

/// <summary>
/// A challenge.
/// </summary>
public class ChallengeJson
{
    /// <summary>
    /// Challenge ID (same as game ID if accepted).
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Challenge URL.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    /// <summary>
    /// Challenge status.
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    /// <summary>
    /// The user who created the challenge.
    /// </summary>
    [JsonPropertyName("challenger")]
    public ChallengeUser? Challenger { get; init; }

    /// <summary>
    /// The user who is challenged (null for open challenges).
    /// </summary>
    [JsonPropertyName("destUser")]
    public ChallengeUser? DestUser { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    [JsonPropertyName("variant")]
    public ChallengeVariant? Variant { get; init; }

    /// <summary>
    /// Whether the game is rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    /// Game speed category.
    /// </summary>
    [JsonPropertyName("speed")]
    public string? Speed { get; init; }

    /// <summary>
    /// Time control settings.
    /// </summary>
    [JsonPropertyName("timeControl")]
    public ChallengeTimeControl? TimeControl { get; init; }

    /// <summary>
    /// Color preference requested.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    /// The actual color assigned (after acceptance).
    /// </summary>
    [JsonPropertyName("finalColor")]
    public string? FinalColor { get; init; }

    /// <summary>
    /// Performance type information.
    /// </summary>
    [JsonPropertyName("perf")]
    public ChallengePerf? Perf { get; init; }

    /// <summary>
    /// Direction of the challenge (in/out) relative to authenticated user.
    /// </summary>
    [JsonPropertyName("direction")]
    public string? Direction { get; init; }

    /// <summary>
    /// Initial FEN for custom positions.
    /// </summary>
    [JsonPropertyName("initialFen")]
    public string? InitialFen { get; init; }

    /// <summary>
    /// ID of the game this is a rematch of.
    /// </summary>
    [JsonPropertyName("rematchOf")]
    public string? RematchOf { get; init; }

    /// <summary>
    /// Decline reason if declined.
    /// </summary>
    [JsonPropertyName("declineReason")]
    public string? DeclineReason { get; init; }

    /// <summary>
    /// Localized decline reason key.
    /// </summary>
    [JsonPropertyName("declineReasonKey")]
    public string? DeclineReasonKey { get; init; }
}

/// <summary>
/// Challenge user information.
/// </summary>
public class ChallengeUser
{
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// User rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    /// User title (GM, IM, BOT, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// User flair.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }

    /// <summary>
    /// Whether the rating is provisional.
    /// </summary>
    [JsonPropertyName("provisional")]
    public bool? Provisional { get; init; }

    /// <summary>
    /// Whether the user is online.
    /// </summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>
    /// User's connection lag in ms.
    /// </summary>
    [JsonPropertyName("lag")]
    public int? Lag { get; init; }

    /// <summary>
    /// Whether the user is a patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }
}

/// <summary>
/// Challenge variant information.
/// </summary>
public class ChallengeVariant
{
    /// <summary>
    /// Variant key (e.g., "standard", "chess960").
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    /// <summary>
    /// Variant display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Short variant name.
    /// </summary>
    [JsonPropertyName("short")]
    public string? Short { get; init; }
}

/// <summary>
/// Challenge time control.
/// </summary>
public class ChallengeTimeControl
{
    /// <summary>
    /// Time control type ("clock", "correspondence", "unlimited").
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Initial time in seconds (for clock).
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; init; }

    /// <summary>
    /// Increment per move in seconds (for clock).
    /// </summary>
    [JsonPropertyName("increment")]
    public int? Increment { get; init; }

    /// <summary>
    /// Days per move (for correspondence).
    /// </summary>
    [JsonPropertyName("daysPerTurn")]
    public int? DaysPerTurn { get; init; }

    /// <summary>
    /// Display string (e.g., "5+3").
    /// </summary>
    [JsonPropertyName("show")]
    public string? Show { get; init; }
}

/// <summary>
/// Challenge performance type.
/// </summary>
public class ChallengePerf
{
    /// <summary>
    /// Performance icon.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; init; }

    /// <summary>
    /// Performance name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Open challenge response.
/// </summary>
public class ChallengeOpenJson : ChallengeJson
{
    /// <summary>
    /// URL for the first player (white if color is set).
    /// </summary>
    [JsonPropertyName("urlWhite")]
    public string? UrlWhite { get; init; }

    /// <summary>
    /// URL for the second player (black if color is set).
    /// </summary>
    [JsonPropertyName("urlBlack")]
    public string? UrlBlack { get; init; }
}

/// <summary>
/// Response from challenging the AI.
/// </summary>
public class ChallengeAiResponse
{
    /// <summary>
    /// The created game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The variant.
    /// </summary>
    [JsonPropertyName("variant")]
    public ChallengeVariant? Variant { get; init; }

    /// <summary>
    /// Game speed.
    /// </summary>
    [JsonPropertyName("speed")]
    public string? Speed { get; init; }

    /// <summary>
    /// Performance type.
    /// </summary>
    [JsonPropertyName("perf")]
    public string? Perf { get; init; }

    /// <summary>
    /// Whether the game is rated (always false for AI).
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    /// Initial FEN if custom position.
    /// </summary>
    [JsonPropertyName("fen")]
    public string? Fen { get; init; }

    /// <summary>
    /// The player's information.
    /// </summary>
    [JsonPropertyName("player")]
    public string? Player { get; init; }

    /// <summary>
    /// Number of turns played.
    /// </summary>
    [JsonPropertyName("turns")]
    public int Turns { get; init; }

    /// <summary>
    /// When the game started.
    /// </summary>
    [JsonPropertyName("startedAtTurn")]
    public int StartedAtTurn { get; init; }

    /// <summary>
    /// Game source.
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; init; }

    /// <summary>
    /// Game status.
    /// </summary>
    [JsonPropertyName("status")]
    public ChallengeAiGameStatus? Status { get; init; }

    /// <summary>
    /// Timestamp when created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public long? CreatedAt { get; init; }
}

/// <summary>
/// Game status in AI challenge response.
/// </summary>
public class ChallengeAiGameStatus
{
    /// <summary>
    /// Status ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Status name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

#endregion

#region Options

/// <summary>
/// Options for creating a challenge.
/// </summary>
public class ChallengeCreateOptions
{
    /// <summary>
    /// Whether the game should be rated (default: false).
    /// </summary>
    public bool? Rated { get; set; }

    /// <summary>
    /// Clock initial time in seconds (required for real-time games).
    /// Must be between 0 and 10800 (3 hours).
    /// </summary>
    public int? ClockLimit { get; set; }

    /// <summary>
    /// Clock increment in seconds (required for real-time games).
    /// Must be between 0 and 180.
    /// </summary>
    public int? ClockIncrement { get; set; }

    /// <summary>
    /// Days per turn for correspondence games.
    /// Must be 1, 2, 3, 5, 7, 10, or 14.
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// Color preference.
    /// </summary>
    public ChallengeColor? Color { get; set; }

    /// <summary>
    /// Chess variant (e.g., "standard", "chess960", "crazyhouse").
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// Starting FEN for custom positions.
    /// Variant must be "standard", "fromPosition", or "chess960".
    /// </summary>
    public string? Fen { get; set; }

    /// <summary>
    /// Keep the challenge alive in a stream.
    /// </summary>
    public bool? KeepAliveStream { get; set; }

    /// <summary>
    /// Extra game rules.
    /// </summary>
    public ChallengeRules? Rules { get; set; }

    /// <summary>
    /// Message to send with the challenge.
    /// </summary>
    public string? Message { get; set; }
}

/// <summary>
/// Options for creating an open challenge.
/// </summary>
public class ChallengeOpenOptions : ChallengeCreateOptions
{
    /// <summary>
    /// Optional name for the challenge.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Lichess usernames to send the challenge notification to.
    /// </summary>
    public IReadOnlyList<string>? Users { get; set; }

    /// <summary>
    /// Expiration time in minutes for the challenge (default: 24 hours).
    /// </summary>
    public int? ExpiresAt { get; set; }
}

/// <summary>
/// Options for challenging the AI.
/// </summary>
public class ChallengeAiOptions
{
    /// <summary>
    /// AI level (1-8). Default is 8.
    /// </summary>
    public int Level { get; set; } = 8;

    /// <summary>
    /// Clock initial time in seconds.
    /// </summary>
    public int? ClockLimit { get; set; }

    /// <summary>
    /// Clock increment in seconds.
    /// </summary>
    public int? ClockIncrement { get; set; }

    /// <summary>
    /// Days per turn for correspondence.
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// Color preference.
    /// </summary>
    public ChallengeColor? Color { get; set; }

    /// <summary>
    /// Chess variant.
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// Starting FEN for custom positions.
    /// </summary>
    public string? Fen { get; set; }
}

#endregion
