using System.Runtime.CompilerServices;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Relations API.
/// </summary>
internal sealed class RelationsApi(ILichessHttpClient httpClient) : IRelationsApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<bool> FollowUserAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var response = await _httpClient.PostAsync<OkResponse>($"/api/rel/follow/{Uri.EscapeDataString(username)}", null, cancellationToken).ConfigureAwait(false);
        return response.Ok;
    }

    /// <inheritdoc />
    public async Task<bool> UnfollowUserAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var response = await _httpClient.PostAsync<OkResponse>($"/api/rel/unfollow/{Uri.EscapeDataString(username)}", null, cancellationToken).ConfigureAwait(false);
        return response.Ok;
    }

    /// <inheritdoc />
    public async Task<bool> BlockUserAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var response = await _httpClient.PostAsync<OkResponse>($"/api/rel/block/{Uri.EscapeDataString(username)}", null, cancellationToken).ConfigureAwait(false);
        return response.Ok;
    }

    /// <inheritdoc />
    public async Task<bool> UnblockUserAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var response = await _httpClient.PostAsync<OkResponse>($"/api/rel/unblock/{Uri.EscapeDataString(username)}", null, cancellationToken).ConfigureAwait(false);
        return response.Ok;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<UserExtended> StreamFollowingUsersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var user in _httpClient.StreamNdjsonAsync<UserExtended>("/api/rel/following", cancellationToken).ConfigureAwait(false))
        {
            yield return user;
        }
    }
}
