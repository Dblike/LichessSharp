using System.Text.Json.Serialization;

namespace LichessSharp.Models.Puzzles;

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
