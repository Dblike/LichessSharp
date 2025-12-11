using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the FIDE API.
/// </summary>
internal sealed class FideApi : IFideApi
{
    private readonly ILichessHttpClient _httpClient;

    public FideApi(ILichessHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<FidePlayer> GetPlayerAsync(int playerId, CancellationToken cancellationToken = default)
    {
        if (playerId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(playerId), playerId, "FIDE player ID must be positive.");
        }

        return await _httpClient.GetAsync<FidePlayer>($"/api/fide/player/{playerId}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FidePlayer>> SearchPlayersAsync(string query, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var endpoint = $"/api/fide/player?q={Uri.EscapeDataString(query)}";
        var players = await _httpClient.GetAsync<List<FidePlayer>>(endpoint, cancellationToken).ConfigureAwait(false);
        return players ?? [];
    }
}
