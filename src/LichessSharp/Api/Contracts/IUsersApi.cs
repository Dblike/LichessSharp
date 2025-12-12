using System.Text.Json.Serialization;

using LichessSharp.Api.Options;
using LichessSharp.Models.Users;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Users API - Access registered users on Lichess.
/// </summary>
public interface IUsersApi
{
    /// <summary>
    /// Get real-time status of users.
    /// </summary>
    /// <param name="userIds">The user IDs (up to 100).</param>
    /// <param name="options">Optional request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The users' real-time statuses.</returns>
    Task<IReadOnlyList<UserStatus>> GetRealTimeStatusAsync(IEnumerable<string> userIds, GetUserStatusOptions? options = null, CancellationToken cancellationToken = default);

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
    /// Get the public profile of a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="options">Optional request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's public profile.</returns>
    Task<UserExtended> GetAsync(string username, GetUserOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get rating history of a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Rating history by perf type.</returns>
    Task<IReadOnlyList<RatingHistory>> GetRatingHistoryAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get performance statistics for a user in a specific variant.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="perfType">The performance type (e.g., bullet, blitz, rapid, classical, chess960, etc.).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's performance statistics for the specified variant.</returns>
    Task<UserPerformance> GetPerformanceAsync(string username, string perfType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the activity feed of a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's activity feed.</returns>
    Task<IReadOnlyList<UserActivity>> GetActivityAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get multiple users by their IDs.
    /// </summary>
    /// <param name="userIds">The user IDs (up to 300).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The users' public profiles.</returns>
    Task<IReadOnlyList<User>> GetManyAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get currently live streamers on Lichess.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of live streamers.</returns>
    Task<IReadOnlyList<Streamer>> GetLiveStreamersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the crosstable (head-to-head) statistics between two users.
    /// </summary>
    /// <param name="user1">First username.</param>
    /// <param name="user2">Second username.</param>
    /// <param name="matchup">Include current match data if users are playing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The crosstable statistics.</returns>
    Task<Crosstable> GetCrosstableAsync(string user1, string user2, bool matchup = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Autocomplete usernames.
    /// </summary>
    /// <param name="term">Search term (at least 3 characters).</param>
    /// <param name="asObject">If true, returns player objects; if false, returns string usernames.</param>
    /// <param name="friend">Filter to only friends of the specified user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching usernames.</returns>
    Task<IReadOnlyList<string>> AutocompleteAsync(string term, bool asObject = false, string? friend = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Autocomplete usernames and return player objects.
    /// </summary>
    /// <param name="term">Search term (at least 3 characters).</param>
    /// <param name="friend">Filter to only friends of the specified user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching players with details.</returns>
    Task<IReadOnlyList<AutocompletePlayer>> AutocompletePlayersAsync(string term, string? friend = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Write or update a note about another user.
    /// Requires OAuth with follow:write scope.
    /// </summary>
    /// <param name="username">The username to write a note for.</param>
    /// <param name="text">The note text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> WriteNoteAsync(string username, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a note you have written about another user.
    /// Requires OAuth with follow:read scope.
    /// </summary>
    /// <param name="username">The username to read the note for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The note text, or null if no note exists.</returns>
    Task<string?> GetNoteAsync(string username, CancellationToken cancellationToken = default);
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
    [JsonConverter(typeof(RatingDataPointArrayConverter))]
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
