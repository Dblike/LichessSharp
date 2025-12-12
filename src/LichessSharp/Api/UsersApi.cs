using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Api.Options;
using LichessSharp.Http;
using LichessSharp.Models.Common;
using LichessSharp.Models.Users;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Users API.
/// </summary>
internal sealed class UsersApi(ILichessHttpClient httpClient) : IUsersApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

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
            return [];
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
    public async Task<IReadOnlyList<UserStatus>> GetRealTimeStatusAsync(IEnumerable<string> userIds, GetUserStatusOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userIds);

        var ids = userIds.ToList();
        if (ids.Count == 0)
        {
            return [];
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

    /// <inheritdoc />
    public async Task<UserPerformance> GetPerformanceAsync(string username, string perfType, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(perfType);

        var endpoint = $"/api/user/{Uri.EscapeDataString(username)}/perf/{Uri.EscapeDataString(perfType)}";
        return await _httpClient.GetAsync<UserPerformance>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserActivity>> GetActivityAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = $"/api/user/{Uri.EscapeDataString(username)}/activity";
        var activity = await _httpClient.GetAsync<List<UserActivity>>(endpoint, cancellationToken).ConfigureAwait(false);
        return activity ?? [];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> AutocompleteAsync(string term, bool asObject = false, string? friend = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(term);

        var sb = new StringBuilder($"/api/player/autocomplete?term={Uri.EscapeDataString(term)}");
        if (asObject)
        {
            sb.Append("&object=true");
        }
        if (!string.IsNullOrEmpty(friend))
        {
            sb.Append($"&friend={Uri.EscapeDataString(friend)}");
        }

        var result = await _httpClient.GetAsync<List<string>>(sb.ToString(), cancellationToken).ConfigureAwait(false);
        return result ?? [];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AutocompletePlayer>> AutocompletePlayersAsync(string term, string? friend = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(term);

        var sb = new StringBuilder($"/api/player/autocomplete?term={Uri.EscapeDataString(term)}&object=true");
        if (!string.IsNullOrEmpty(friend))
        {
            sb.Append($"&friend={Uri.EscapeDataString(friend)}");
        }

        var response = await _httpClient.GetAsync<AutocompleteResponse>(sb.ToString(), cancellationToken).ConfigureAwait(false);
        return response?.Result ?? [];
    }

    /// <inheritdoc />
    public async Task<Crosstable> GetCrosstableAsync(string user1, string user2, bool matchup = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user1);
        ArgumentException.ThrowIfNullOrWhiteSpace(user2);

        var endpoint = $"/api/crosstable/{Uri.EscapeDataString(user1)}/{Uri.EscapeDataString(user2)}";
        if (matchup)
        {
            endpoint += "?matchup=true";
        }

        return await _httpClient.GetAsync<Crosstable>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Streamer>> GetLiveStreamersAsync(CancellationToken cancellationToken = default)
    {
        var streamers = await _httpClient.GetAsync<List<Streamer>>("/api/streamer/live", cancellationToken).ConfigureAwait(false);
        return streamers ?? [];
    }

    /// <inheritdoc />
    public async Task<string?> GetNoteAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = $"/api/user/{Uri.EscapeDataString(username)}/note";
        var response = await _httpClient.GetAsync<NoteResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return response?.Text;
    }

    /// <inheritdoc />
    public async Task<bool> WriteNoteAsync(string username, string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentNullException.ThrowIfNull(text);

        var endpoint = $"/api/user/{Uri.EscapeDataString(username)}/note";
        var formData = new Dictionary<string, string> { ["text"] = text };
        var response = await _httpClient.PostFormAsync<OkResponse>(endpoint, formData, cancellationToken).ConfigureAwait(false);
        return response?.Ok == true;
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
