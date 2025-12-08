using System.Text;
using LichessSharp.Api.Options;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Users API.
/// </summary>
internal sealed class UsersApi : IUsersApi
{
    private readonly ILichessHttpClient _httpClient;

    public UsersApi(ILichessHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<UserExtended> GetAsync(string username, GetUserOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = BuildGetUserEndpoint(username, options);
        return await _httpClient.GetAsync<UserExtended>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<User>> GetManyAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userIds);

        var ids = userIds.ToList();
        if (ids.Count == 0)
        {
            return Array.Empty<User>();
        }

        if (ids.Count > 300)
        {
            throw new ArgumentException("Cannot request more than 300 users at once.", nameof(userIds));
        }

        var body = string.Join(",", ids);
        var users = await _httpClient.PostPlainTextAsync<List<User>>("/api/users", body, cancellationToken).ConfigureAwait(false);
        return users;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserStatus>> GetStatusAsync(IEnumerable<string> userIds, GetUserStatusOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userIds);

        var ids = userIds.ToList();
        if (ids.Count == 0)
        {
            return Array.Empty<UserStatus>();
        }

        if (ids.Count > 100)
        {
            throw new ArgumentException("Cannot request status for more than 100 users at once.", nameof(userIds));
        }

        var endpoint = BuildGetStatusEndpoint(ids, options);
        var statuses = await _httpClient.GetAsync<List<UserStatus>>(endpoint, cancellationToken).ConfigureAwait(false);
        return statuses;
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, List<User>>> GetAllTop10Async(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<Dictionary<string, List<User>>>("/api/player", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<User>> GetLeaderboardAsync(string perfType, int count = 100, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(perfType);

        if (count < 1 || count > 200)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be between 1 and 200.");
        }

        var endpoint = $"/api/player/top/{count}/{perfType}";
        var response = await _httpClient.GetAsync<LeaderboardResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return response.Users;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RatingHistory>> GetRatingHistoryAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = $"/api/user/{username}/rating-history";
        var history = await _httpClient.GetAsync<List<RatingHistory>>(endpoint, cancellationToken).ConfigureAwait(false);
        return history;
    }

    private static string BuildGetUserEndpoint(string username, GetUserOptions? options)
    {
        var sb = new StringBuilder($"/api/user/{username}");

        if (options == null)
        {
            return sb.ToString();
        }

        var queryParams = new List<string>();

        if (options.Trophies.HasValue)
        {
            queryParams.Add($"trophies={options.Trophies.Value.ToString().ToLowerInvariant()}");
        }

        if (options.Profile.HasValue)
        {
            queryParams.Add($"profile={options.Profile.Value.ToString().ToLowerInvariant()}");
        }

        if (options.Rank.HasValue)
        {
            queryParams.Add($"rank={options.Rank.Value.ToString().ToLowerInvariant()}");
        }

        if (queryParams.Count > 0)
        {
            sb.Append('?');
            sb.Append(string.Join("&", queryParams));
        }

        return sb.ToString();
    }

    private static string BuildGetStatusEndpoint(List<string> ids, GetUserStatusOptions? options)
    {
        var sb = new StringBuilder("/api/users/status?ids=");
        sb.Append(string.Join(",", ids));

        if (options == null)
        {
            return sb.ToString();
        }

        if (options.WithSignal.HasValue && options.WithSignal.Value)
        {
            sb.Append("&withSignal=true");
        }

        if (options.WithGameIds.HasValue && options.WithGameIds.Value)
        {
            sb.Append("&withGameIds=true");
        }

        if (options.WithGameMetas.HasValue && options.WithGameMetas.Value)
        {
            sb.Append("&withGameMetas=true");
        }

        return sb.ToString();
    }
}

/// <summary>
/// Response wrapper for the leaderboard endpoint.
/// </summary>
internal sealed class LeaderboardResponse
{
    /// <summary>
    /// The list of users in the leaderboard.
    /// </summary>
    public required List<User> Users { get; init; }
}
