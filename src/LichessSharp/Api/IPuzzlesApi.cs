using LichessSharp.Models;

namespace LichessSharp.Api;

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
    /// Stream puzzle activity for the authenticated user.
    /// </summary>
    /// <param name="max">Maximum number of entries.</param>
    /// <param name="before">Only entries before this timestamp.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of puzzle activity.</returns>
    IAsyncEnumerable<PuzzleActivity> StreamActivityAsync(int? max = null, DateTimeOffset? before = null, CancellationToken cancellationToken = default);

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
}

/// <summary>
/// Puzzle activity entry.
/// </summary>
public class PuzzleActivity
{
    /// <summary>
    /// The puzzle ID.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// When the puzzle was played.
    /// </summary>
    public DateTimeOffset Date { get; init; }

    /// <summary>
    /// Whether the puzzle was solved correctly.
    /// </summary>
    public bool Win { get; init; }

    /// <summary>
    /// Rating before playing.
    /// </summary>
    public int RatingBefore { get; init; }

    /// <summary>
    /// Rating after playing.
    /// </summary>
    public int RatingAfter { get; init; }
}

/// <summary>
/// Puzzle dashboard.
/// </summary>
public class PuzzleDashboard
{
    /// <summary>
    /// The number of days covered.
    /// </summary>
    public int Days { get; init; }

    /// <summary>
    /// Global puzzle performance.
    /// </summary>
    public PuzzlePerformance? Global { get; init; }

    /// <summary>
    /// Performance by theme.
    /// </summary>
    public Dictionary<string, PuzzlePerformance>? Themes { get; init; }
}

/// <summary>
/// Puzzle performance statistics.
/// </summary>
public class PuzzlePerformance
{
    /// <summary>
    /// First attempt results.
    /// </summary>
    public PuzzleResults? FirstWins { get; init; }

    /// <summary>
    /// Replayable results.
    /// </summary>
    public PuzzleResults? ReplayWins { get; init; }
}

/// <summary>
/// Puzzle results.
/// </summary>
public class PuzzleResults
{
    /// <summary>
    /// Number of wins.
    /// </summary>
    public int Wins { get; init; }

    /// <summary>
    /// Number of losses.
    /// </summary>
    public int Losses { get; init; }

    /// <summary>
    /// Number of draws.
    /// </summary>
    public int Draws { get; init; }
}

/// <summary>
/// Storm dashboard.
/// </summary>
public class StormDashboard
{
    /// <summary>
    /// High scores.
    /// </summary>
    public StormHigh? High { get; init; }

    /// <summary>
    /// Daily history.
    /// </summary>
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
    public int AllTime { get; init; }

    /// <summary>
    /// Monthly high.
    /// </summary>
    public int Month { get; init; }

    /// <summary>
    /// Weekly high.
    /// </summary>
    public int Week { get; init; }

    /// <summary>
    /// Daily high.
    /// </summary>
    public int Day { get; init; }
}

/// <summary>
/// Storm daily record.
/// </summary>
public class StormDay
{
    /// <summary>
    /// The day identifier (YYYY/MM/DD format).
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// High score for this day.
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Number of runs.
    /// </summary>
    public int Runs { get; init; }

    /// <summary>
    /// Time spent in seconds.
    /// </summary>
    public int Time { get; init; }
}

/// <summary>
/// Puzzle race.
/// </summary>
public class PuzzleRace
{
    /// <summary>
    /// The race ID.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// URL to the race.
    /// </summary>
    public required string Url { get; init; }
}
