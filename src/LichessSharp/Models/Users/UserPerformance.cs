using System.Text.Json.Serialization;
using LichessSharp.Models.Common;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models.Users;

/// <summary>
/// Performance statistics for a specific variant, including detailed stats.
/// </summary>
public class UserPerformance
{
    /// <summary>
    /// The user's performance rating data.
    /// </summary>
    [JsonPropertyName("perf")]
    public PerfStats? Perf { get; init; }

    /// <summary>
    /// The user's rank in this variant (if ranked).
    /// </summary>
    [JsonPropertyName("rank")]
    public int? Rank { get; init; }

    /// <summary>
    /// The user's percentile ranking.
    /// </summary>
    [JsonPropertyName("percentile")]
    public double? Percentile { get; init; }

    /// <summary>
    /// Detailed statistics for this performance type.
    /// </summary>
    [JsonPropertyName("stat")]
    public PerformanceStatistics? Stat { get; init; }
}

/// <summary>
/// Detailed performance statistics.
/// </summary>
public class PerformanceStatistics
{
    /// <summary>
    /// The performance type key.
    /// </summary>
    [JsonPropertyName("perfType")]
    public PerformanceType? PerfType { get; init; }

    /// <summary>
    /// Highest rating achieved.
    /// </summary>
    [JsonPropertyName("highest")]
    public RatingAtTime? Highest { get; init; }

    /// <summary>
    /// Lowest rating achieved.
    /// </summary>
    [JsonPropertyName("lowest")]
    public RatingAtTime? Lowest { get; init; }

    /// <summary>
    /// Best wins against higher-rated opponents.
    /// </summary>
    [JsonPropertyName("bestWins")]
    public ResultsVsOpponents? BestWins { get; init; }

    /// <summary>
    /// Worst losses against lower-rated opponents.
    /// </summary>
    [JsonPropertyName("worstLosses")]
    public ResultsVsOpponents? WorstLosses { get; init; }

    /// <summary>
    /// Game count statistics.
    /// </summary>
    [JsonPropertyName("count")]
    public PerformanceCount? Count { get; init; }

    /// <summary>
    /// Results against various rating ranges.
    /// </summary>
    [JsonPropertyName("resultStreak")]
    public ResultStreak? ResultStreak { get; init; }

    /// <summary>
    /// Play streak statistics.
    /// </summary>
    [JsonPropertyName("playStreak")]
    public PlayStreak? PlayStreak { get; init; }
}

/// <summary>
/// Rating at a specific time.
/// </summary>
public class RatingAtTime
{
    /// <summary>
    /// The rating value.
    /// </summary>
    [JsonPropertyName("int")]
    public int Value { get; init; }

    /// <summary>
    /// When this rating was achieved.
    /// </summary>
    [JsonPropertyName("at")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset At { get; init; }

    /// <summary>
    /// The game ID where this rating was achieved.
    /// </summary>
    [JsonPropertyName("gameId")]
    public string? GameId { get; init; }
}

/// <summary>
/// Results against opponents.
/// </summary>
public class ResultsVsOpponents
{
    /// <summary>
    /// List of results.
    /// </summary>
    [JsonPropertyName("results")]
    public IReadOnlyList<OpponentResult>? Results { get; init; }
}

/// <summary>
/// Result against a specific opponent.
/// </summary>
public class OpponentResult
{
    /// <summary>
    /// Opponent's rating.
    /// </summary>
    [JsonPropertyName("opInt")]
    public int OpponentRating { get; init; }

    /// <summary>
    /// Opponent's user information.
    /// </summary>
    [JsonPropertyName("opId")]
    public LightUser? Opponent { get; init; }

    /// <summary>
    /// When the game was played.
    /// </summary>
    [JsonPropertyName("at")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset At { get; init; }

    /// <summary>
    /// The game ID.
    /// </summary>
    [JsonPropertyName("gameId")]
    public string? GameId { get; init; }
}

/// <summary>
/// Performance game count.
/// </summary>
public class PerformanceCount
{
    /// <summary>
    /// Total games.
    /// </summary>
    [JsonPropertyName("all")]
    public int All { get; init; }

    /// <summary>
    /// Rated games.
    /// </summary>
    [JsonPropertyName("rated")]
    public int Rated { get; init; }

    /// <summary>
    /// Games won.
    /// </summary>
    [JsonPropertyName("win")]
    public int Win { get; init; }

    /// <summary>
    /// Games lost.
    /// </summary>
    [JsonPropertyName("loss")]
    public int Loss { get; init; }

    /// <summary>
    /// Games drawn.
    /// </summary>
    [JsonPropertyName("draw")]
    public int Draw { get; init; }

    /// <summary>
    /// Tournament games.
    /// </summary>
    [JsonPropertyName("tour")]
    public int Tour { get; init; }

    /// <summary>
    /// Berserk games.
    /// </summary>
    [JsonPropertyName("berserk")]
    public int Berserk { get; init; }

    /// <summary>
    /// Games where opponent disconnected.
    /// </summary>
    [JsonPropertyName("opDisc")]
    public int OpponentDisconnected { get; init; }

    /// <summary>
    /// Total seconds played.
    /// </summary>
    [JsonPropertyName("seconds")]
    public int Seconds { get; init; }
}

/// <summary>
/// Result streak statistics.
/// </summary>
public class ResultStreak
{
    /// <summary>
    /// Win streak.
    /// </summary>
    [JsonPropertyName("win")]
    public StreakInfo? Win { get; init; }

    /// <summary>
    /// Loss streak.
    /// </summary>
    [JsonPropertyName("loss")]
    public StreakInfo? Loss { get; init; }
}

/// <summary>
/// Streak information.
/// </summary>
public class StreakInfo
{
    /// <summary>
    /// Current streak.
    /// </summary>
    [JsonPropertyName("cur")]
    public StreakValue? Current { get; init; }

    /// <summary>
    /// Maximum streak.
    /// </summary>
    [JsonPropertyName("max")]
    public StreakValue? Max { get; init; }
}

/// <summary>
/// Streak value.
/// </summary>
public class StreakValue
{
    /// <summary>
    /// Streak count.
    /// </summary>
    [JsonPropertyName("v")]
    public int Value { get; init; }
}

/// <summary>
/// Play streak statistics.
/// </summary>
public class PlayStreak
{
    /// <summary>
    /// Number of games in current session.
    /// </summary>
    [JsonPropertyName("nb")]
    public StreakInfo? NbGames { get; init; }

    /// <summary>
    /// Time spent playing.
    /// </summary>
    [JsonPropertyName("time")]
    public StreakInfo? Time { get; init; }

    /// <summary>
    /// Last played time.
    /// </summary>
    [JsonPropertyName("lastDate")]
    [JsonConverter(typeof(NullableUnixMillisecondsConverter))]
    public DateTimeOffset? LastDate { get; init; }
}
