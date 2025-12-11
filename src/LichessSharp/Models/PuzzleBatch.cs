using System.Text.Json.Serialization;

namespace LichessSharp.Models;

/// <summary>
/// Glicko rating information for puzzles.
/// </summary>
public class PuzzleGlicko
{
    /// <summary>
    /// Current puzzle rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public double Rating { get; init; }

    /// <summary>
    /// Rating deviation.
    /// </summary>
    [JsonPropertyName("deviation")]
    public double Deviation { get; init; }

    /// <summary>
    /// Whether the rating is provisional.
    /// </summary>
    [JsonPropertyName("provisional")]
    public bool? Provisional { get; init; }
}

/// <summary>
/// A batch of puzzles retrieved for offline solving.
/// </summary>
public class PuzzleBatch
{
    /// <summary>
    /// The puzzles in this batch.
    /// </summary>
    [JsonPropertyName("puzzles")]
    public IReadOnlyList<PuzzleWithGame>? Puzzles { get; init; }

    /// <summary>
    /// The user's puzzle Glicko rating.
    /// </summary>
    [JsonPropertyName("glicko")]
    public PuzzleGlicko? Glicko { get; init; }
}

/// <summary>
/// A solution submitted for a puzzle in a batch.
/// </summary>
public class PuzzleSolution
{
    /// <summary>
    /// The puzzle ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Whether the puzzle was solved correctly.
    /// </summary>
    [JsonPropertyName("win")]
    public bool Win { get; init; }

    /// <summary>
    /// Whether this solution should affect ratings.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool? Rated { get; init; }
}

/// <summary>
/// A round result from solving a puzzle.
/// </summary>
public class PuzzleRound
{
    /// <summary>
    /// The puzzle ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Whether the puzzle was solved correctly.
    /// </summary>
    [JsonPropertyName("win")]
    public bool Win { get; init; }

    /// <summary>
    /// Rating change from this puzzle.
    /// </summary>
    [JsonPropertyName("ratingDiff")]
    public int RatingDiff { get; init; }
}

/// <summary>
/// Result from solving a batch of puzzles.
/// </summary>
public class PuzzleBatchResult
{
    /// <summary>
    /// New puzzles to solve (if nb > 0 was requested).
    /// </summary>
    [JsonPropertyName("puzzles")]
    public IReadOnlyList<PuzzleWithGame>? Puzzles { get; init; }

    /// <summary>
    /// Updated Glicko rating.
    /// </summary>
    [JsonPropertyName("glicko")]
    public PuzzleGlicko? Glicko { get; init; }

    /// <summary>
    /// Results for each solved puzzle.
    /// </summary>
    [JsonPropertyName("rounds")]
    public IReadOnlyList<PuzzleRound>? Rounds { get; init; }
}

/// <summary>
/// Information about puzzles to replay.
/// </summary>
public class PuzzleReplayInfo
{
    /// <summary>
    /// Number of days looked back.
    /// </summary>
    [JsonPropertyName("days")]
    public int Days { get; init; }

    /// <summary>
    /// The theme or opening filtered.
    /// </summary>
    [JsonPropertyName("theme")]
    public string? Theme { get; init; }

    /// <summary>
    /// Total number of puzzles to replay.
    /// </summary>
    [JsonPropertyName("nb")]
    public int Nb { get; init; }

    /// <summary>
    /// Remaining puzzle IDs to replay.
    /// </summary>
    [JsonPropertyName("remaining")]
    public IReadOnlyList<string>? Remaining { get; init; }
}

/// <summary>
/// Information about a puzzle angle (theme or opening).
/// </summary>
public class PuzzleAngle
{
    /// <summary>
    /// The angle key.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    /// <summary>
    /// The angle display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Description of the angle.
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; init; }
}

/// <summary>
/// Response for puzzle replay endpoint.
/// </summary>
public class PuzzleReplay
{
    /// <summary>
    /// Information about the replay.
    /// </summary>
    [JsonPropertyName("replay")]
    public PuzzleReplayInfo? Replay { get; init; }

    /// <summary>
    /// Information about the angle.
    /// </summary>
    [JsonPropertyName("angle")]
    public PuzzleAngle? Angle { get; init; }
}

/// <summary>
/// A player in a puzzle race.
/// </summary>
public class PuzzleRacePlayer
{
    /// <summary>
    /// The player's username.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// The player's user ID (missing if anonymous).
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// The player's flair.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }

    /// <summary>
    /// The player's score in the race.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; init; }
}

/// <summary>
/// Results of a puzzle race.
/// </summary>
public class PuzzleRaceResults
{
    /// <summary>
    /// The race ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The race owner username.
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; init; }

    /// <summary>
    /// Players participating in the race.
    /// </summary>
    [JsonPropertyName("players")]
    public IReadOnlyList<PuzzleRacePlayer>? Players { get; init; }
}

/// <summary>
/// Request body for solving a batch of puzzles.
/// </summary>
internal class PuzzleBatchSolveRequest
{
    /// <summary>
    /// The solutions to submit.
    /// </summary>
    [JsonPropertyName("solutions")]
    public required IEnumerable<PuzzleSolution> Solutions { get; init; }
}
