using System.Text.Json.Serialization;

namespace LichessSharp.Models.Users;

/// <summary>
///     Crosstable (head-to-head) statistics between two users.
/// </summary>
[ResponseOnly]
public class Crosstable
{
    /// <summary>
    ///     Scores for each user (user ID to score).
    ///     Score is number of points (wins = 1, draws = 0.5).
    /// </summary>
    [JsonPropertyName("users")]
    public Dictionary<string, double>? Users { get; init; }

    /// <summary>
    ///     Total number of games played between the users.
    /// </summary>
    [JsonPropertyName("nbGames")]
    public int NbGames { get; init; }

    /// <summary>
    ///     Current matchup information (if in an ongoing match).
    /// </summary>
    [JsonPropertyName("matchup")]
    public CrosstableMatchup? Matchup { get; init; }
}

/// <summary>
///     Current matchup information.
/// </summary>
[ResponseOnly]
public class CrosstableMatchup
{
    /// <summary>
    ///     Scores for each user in the current matchup.
    /// </summary>
    [JsonPropertyName("users")]
    public Dictionary<string, double>? Users { get; init; }

    /// <summary>
    ///     Number of games in the current matchup.
    /// </summary>
    [JsonPropertyName("nbGames")]
    public int NbGames { get; init; }
}