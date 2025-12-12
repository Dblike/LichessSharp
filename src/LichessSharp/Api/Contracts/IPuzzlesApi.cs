using System.Text.Json.Serialization;

using LichessSharp.Models.Puzzles;

// Required for PuzzleBatch, PuzzleBatchResult, PuzzleSolution, PuzzleReplay, PuzzleRaceResults

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Puzzles API - Access Lichess puzzle history and dashboard.
/// </summary>
public interface IPuzzlesApi
{
    /// <summary>
    /// Get the daily puzzle.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The daily puzzle with its game.</returns>
    Task<PuzzleWithGame> GetDailyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a puzzle by its ID.
    /// </summary>
    /// <param name="id">The puzzle ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The puzzle with its game.</returns>
    Task<PuzzleWithGame> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the next puzzle for the authenticated user.
    /// </summary>
    /// <param name="angle">The theme or opening to filter puzzles.</param>
    /// <param name="difficulty">The desired difficulty relative to user rating.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The next puzzle with its game.</returns>
    Task<PuzzleWithGame> GetNextAsync(string? angle = null, string? difficulty = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a batch of puzzles to solve offline.
    /// Requires OAuth with puzzle:read scope.
    /// </summary>
    /// <param name="angle">The theme or opening to filter puzzles. Use "mix" for random puzzles.</param>
    /// <param name="nb">Number of puzzles (1-50, default varies).</param>
    /// <param name="difficulty">Difficulty relative to user rating: "easiest", "easier", "normal", "harder", "hardest".</param>
    /// <param name="color">Color to play: "white" or "black" (only works when nb=1).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A batch of puzzles with Glicko rating info.</returns>
    Task<PuzzleBatch> GetBatchAsync(string angle, int? nb = null, string? difficulty = null, string? color = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Submit solutions for a batch of puzzles.
    /// Requires OAuth with puzzle:write scope.
    /// </summary>
    /// <param name="angle">The theme or opening of the solved puzzles.</param>
    /// <param name="solutions">The puzzle solutions to submit.</param>
    /// <param name="nb">When greater than 0, include a new batch of puzzles in the response.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Results of solving and optionally new puzzles.</returns>
    Task<PuzzleBatchResult> SolveBatchAsync(string angle, IEnumerable<PuzzleSolution> solutions, int? nb = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream puzzle activity for the authenticated user.
    /// </summary>
    /// <param name="max">Maximum number of entries.</param>
    /// <param name="before">Only entries before this timestamp.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of puzzle activity.</returns>
    IAsyncEnumerable<PuzzleActivity> StreamActivityAsync(int? max = null, DateTimeOffset? before = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get puzzles to replay (review incorrect puzzles).
    /// Requires OAuth with puzzle:read scope.
    /// </summary>
    /// <param name="days">Number of days to look back (e.g., 30).</param>
    /// <param name="theme">The theme or opening to filter puzzles.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Information about puzzles to replay.</returns>
    Task<PuzzleReplay> GetReplayAsync(int days, string theme, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the puzzle dashboard for the authenticated user.
    /// </summary>
    /// <param name="days">Number of days to look back (default 30).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The puzzle dashboard.</returns>
    Task<PuzzleDashboard> GetDashboardAsync(int days = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the storm dashboard for a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="days">Number of days of history (0-365, default 30).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The storm dashboard.</returns>
    Task<StormDashboard> GetStormDashboardAsync(string username, int days = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create and join a puzzle race.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created puzzle race.</returns>
    Task<PuzzleRace> CreateRaceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get information about a puzzle race.
    /// </summary>
    /// <param name="raceId">The puzzle race ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The puzzle race results.</returns>
    Task<PuzzleRaceResults> GetRaceAsync(string raceId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Puzzle activity entry.
/// </summary>
public class PuzzleActivity
{
    /// <summary>
    /// When the puzzle was played (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("date")]
    public long Date { get; init; }

    /// <summary>
    /// Whether the puzzle was solved correctly.
    /// </summary>
    [JsonPropertyName("win")]
    public bool Win { get; init; }

    /// <summary>
    /// The puzzle that was played.
    /// </summary>
    [JsonPropertyName("puzzle")]
    public PuzzleActivityPuzzle? Puzzle { get; init; }
}

/// <summary>
/// Puzzle details in activity.
/// </summary>
public class PuzzleActivityPuzzle
{
    /// <summary>
    /// The puzzle ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The puzzle rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; init; }

    /// <summary>
    /// Number of times played.
    /// </summary>
    [JsonPropertyName("plays")]
    public int Plays { get; init; }

    /// <summary>
    /// The FEN position.
    /// </summary>
    [JsonPropertyName("fen")]
    public string? Fen { get; init; }

    /// <summary>
    /// The last move before the puzzle.
    /// </summary>
    [JsonPropertyName("lastMove")]
    public string? LastMove { get; init; }

    /// <summary>
    /// Solution moves in UCI notation.
    /// </summary>
    [JsonPropertyName("solution")]
    public string[]? Solution { get; init; }

    /// <summary>
    /// Puzzle themes.
    /// </summary>
    [JsonPropertyName("themes")]
    public string[]? Themes { get; init; }
}

/// <summary>
/// Puzzle dashboard.
/// </summary>
public class PuzzleDashboard
{
    /// <summary>
    /// The number of days covered.
    /// </summary>
    [JsonPropertyName("days")]
    public int Days { get; init; }

    /// <summary>
    /// Global puzzle performance.
    /// </summary>
    [JsonPropertyName("global")]
    public PuzzlePerformance? Global { get; init; }

    /// <summary>
    /// Performance by theme.
    /// </summary>
    [JsonPropertyName("themes")]
    public Dictionary<string, PuzzleThemeResults>? Themes { get; init; }
}

/// <summary>
/// Puzzle theme results.
/// </summary>
public class PuzzleThemeResults
{
    /// <summary>
    /// The theme name.
    /// </summary>
    [JsonPropertyName("theme")]
    public string? Theme { get; init; }

    /// <summary>
    /// The results for this theme.
    /// </summary>
    [JsonPropertyName("results")]
    public PuzzlePerformance? Results { get; init; }
}

/// <summary>
/// Puzzle performance statistics.
/// </summary>
public class PuzzlePerformance
{
    /// <summary>
    /// Number of puzzles played.
    /// </summary>
    [JsonPropertyName("nb")]
    public int Count { get; init; }

    /// <summary>
    /// Number of first attempt wins.
    /// </summary>
    [JsonPropertyName("firstWins")]
    public int FirstWins { get; init; }

    /// <summary>
    /// Number of replay wins.
    /// </summary>
    [JsonPropertyName("replayWins")]
    public int ReplayWins { get; init; }

    /// <summary>
    /// Average puzzle rating.
    /// </summary>
    [JsonPropertyName("puzzleRatingAvg")]
    public int PuzzleRatingAvg { get; init; }

    /// <summary>
    /// Performance rating.
    /// </summary>
    [JsonPropertyName("performance")]
    public int Performance { get; init; }
}

/// <summary>
/// Storm dashboard.
/// </summary>
public class StormDashboard
{
    /// <summary>
    /// High scores.
    /// </summary>
    [JsonPropertyName("high")]
    public StormHigh? High { get; init; }

    /// <summary>
    /// Daily history.
    /// </summary>
    [JsonPropertyName("days")]
    public IReadOnlyList<StormDay>? Days { get; init; }
}

/// <summary>
/// Storm high scores.
/// </summary>
public class StormHigh
{
    /// <summary>
    /// All-time high.
    /// </summary>
    [JsonPropertyName("allTime")]
    public int AllTime { get; init; }

    /// <summary>
    /// Monthly high.
    /// </summary>
    [JsonPropertyName("month")]
    public int Month { get; init; }

    /// <summary>
    /// Weekly high.
    /// </summary>
    [JsonPropertyName("week")]
    public int Week { get; init; }

    /// <summary>
    /// Daily high.
    /// </summary>
    [JsonPropertyName("day")]
    public int Day { get; init; }
}

/// <summary>
/// Storm daily record.
/// </summary>
public class StormDay
{
    /// <summary>
    /// The day identifier (YYYY/M/D format).
    /// </summary>
    [JsonPropertyName("_id")]
    public required string Id { get; init; }

    /// <summary>
    /// High score for this day.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; init; }

    /// <summary>
    /// Number of runs.
    /// </summary>
    [JsonPropertyName("runs")]
    public int Runs { get; init; }

    /// <summary>
    /// Time spent in seconds.
    /// </summary>
    [JsonPropertyName("time")]
    public int Time { get; init; }

    /// <summary>
    /// Number of moves.
    /// </summary>
    [JsonPropertyName("moves")]
    public int Moves { get; init; }

    /// <summary>
    /// Number of errors.
    /// </summary>
    [JsonPropertyName("errors")]
    public int Errors { get; init; }

    /// <summary>
    /// Highest combo.
    /// </summary>
    [JsonPropertyName("combo")]
    public int Combo { get; init; }

    /// <summary>
    /// Highest puzzle rating reached.
    /// </summary>
    [JsonPropertyName("highest")]
    public int Highest { get; init; }
}

/// <summary>
/// Puzzle race.
/// </summary>
public class PuzzleRace
{
    /// <summary>
    /// The race ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// URL to the race.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }
}
