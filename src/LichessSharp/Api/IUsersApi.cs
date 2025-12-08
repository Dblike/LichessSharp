using LichessSharp.Api.Options;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Users API - Access registered users on Lichess.
/// </summary>
public interface IUsersApi
{
    /// <summary>
    /// Get the public profile of a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="options">Optional request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's public profile.</returns>
    Task<UserExtended> GetAsync(string username, GetUserOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get multiple users by their IDs.
    /// </summary>
    /// <param name="userIds">The user IDs (up to 300).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The users' public profiles.</returns>
    Task<IReadOnlyList<User>> GetManyAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get real-time status of users.
    /// </summary>
    /// <param name="userIds">The user IDs (up to 100).</param>
    /// <param name="options">Optional request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The users' real-time statuses.</returns>
    Task<IReadOnlyList<UserStatus>> GetStatusAsync(IEnumerable<string> userIds, GetUserStatusOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the top 10 players for each speed and variant.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary of perf type to list of top players.</returns>
    Task<Dictionary<string, List<User>>> GetAllTop10Async(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the leaderboard for a single speed or variant.
    /// </summary>
    /// <param name="perfType">The performance type.</param>
    /// <param name="count">Number of players to fetch (1-200).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The leaderboard.</returns>
    Task<IReadOnlyList<User>> GetLeaderboardAsync(string perfType, int count = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get rating history of a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Rating history by perf type.</returns>
    Task<IReadOnlyList<RatingHistory>> GetRatingHistoryAsync(string username, CancellationToken cancellationToken = default);
}

/// <summary>
/// Rating history for a specific performance type.
/// </summary>
public class RatingHistory
{
    /// <summary>
    /// The performance type name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The rating data points.
    /// </summary>
    public required IReadOnlyList<RatingDataPoint> Points { get; init; }
}

/// <summary>
/// A single rating data point.
/// </summary>
public class RatingDataPoint
{
    /// <summary>
    /// The year.
    /// </summary>
    public int Year { get; init; }

    /// <summary>
    /// The month (0-11).
    /// </summary>
    public int Month { get; init; }

    /// <summary>
    /// The day of month.
    /// </summary>
    public int Day { get; init; }

    /// <summary>
    /// The rating.
    /// </summary>
    public int Rating { get; init; }
}
