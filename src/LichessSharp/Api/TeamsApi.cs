using System.Runtime.CompilerServices;
using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Teams API.
/// </summary>
internal sealed class TeamsApi(ILichessHttpClient httpClient) : ITeamsApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<Team> GetAsync(string teamId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var endpoint = $"/api/team/{Uri.EscapeDataString(teamId)}";
        return await _httpClient.GetAsync<Team>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TeamPaginator> GetPopularAsync(int page = 1, CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be at least 1.");
        }

        var endpoint = page > 1 ? $"/api/team/all?page={page}" : "/api/team/all";
        return await _httpClient.GetAsync<TeamPaginator>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Team>> GetUserTeamsAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = $"/api/team/of/{Uri.EscapeDataString(username)}";
        return await _httpClient.GetAsync<List<Team>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TeamPaginator> SearchAsync(string text, int page = 1, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be at least 1.");
        }

        var sb = new StringBuilder("/api/team/search?text=");
        sb.Append(Uri.EscapeDataString(text));
        if (page > 1)
        {
            sb.Append("&page=");
            sb.Append(page);
        }

        return await _httpClient.GetAsync<TeamPaginator>(sb.ToString(), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TeamMember> StreamMembersAsync(string teamId, bool full = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var endpoint = $"/api/team/{Uri.EscapeDataString(teamId)}/users";
        if (full)
        {
            endpoint += "?full=true";
        }

        await foreach (var member in _httpClient.StreamNdjsonAsync<TeamMember>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return member;
        }
    }

    /// <inheritdoc />
    public async Task<bool> JoinAsync(string teamId, string? message = null, string? password = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var endpoint = $"/team/{Uri.EscapeDataString(teamId)}/join";

        HttpContent? content = null;
        if (!string.IsNullOrEmpty(message) || !string.IsNullOrEmpty(password))
        {
            var parameters = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(message))
            {
                parameters.Add(new KeyValuePair<string, string>("message", message));
            }
            if (!string.IsNullOrEmpty(password))
            {
                parameters.Add(new KeyValuePair<string, string>("password", password));
            }
            content = new FormUrlEncodedContent(parameters);
        }

        await _httpClient.PostAsync<OkResponse>(endpoint, content, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> LeaveAsync(string teamId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var endpoint = $"/team/{Uri.EscapeDataString(teamId)}/quit";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TeamRequestWithUser>> GetJoinRequestsAsync(string teamId, bool declined = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var endpoint = $"/api/team/{Uri.EscapeDataString(teamId)}/requests";
        if (declined)
        {
            endpoint += "?declined=true";
        }

        return await _httpClient.GetAsync<List<TeamRequestWithUser>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> AcceptJoinRequestAsync(string teamId, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var endpoint = $"/api/team/{Uri.EscapeDataString(teamId)}/request/{Uri.EscapeDataString(userId)}/accept";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeclineJoinRequestAsync(string teamId, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var endpoint = $"/api/team/{Uri.EscapeDataString(teamId)}/request/{Uri.EscapeDataString(userId)}/decline";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> KickMemberAsync(string teamId, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var endpoint = $"/api/team/{Uri.EscapeDataString(teamId)}/kick/{Uri.EscapeDataString(userId)}";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> MessageAllMembersAsync(string teamId, string message, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        var endpoint = $"/team/{Uri.EscapeDataString(teamId)}/pm-all";
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("message", message)
        ]);

        await _httpClient.PostAsync<OkResponse>(endpoint, content, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
