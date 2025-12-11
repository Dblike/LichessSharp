using System.Runtime.CompilerServices;
using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Broadcasts API.
/// </summary>
internal sealed class BroadcastsApi(ILichessHttpClient httpClient) : IBroadcastsApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    #region Viewing Broadcasts

    /// <inheritdoc />
    public async IAsyncEnumerable<BroadcastWithRounds> StreamOfficialBroadcastsAsync(int? nb = null, bool? html = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder("/api/broadcast");
        var hasParams = false;

        if (nb.HasValue)
        {
            sb.Append(hasParams ? '&' : '?');
            sb.Append("nb=");
            sb.Append(nb.Value);
            hasParams = true;
        }

        if (html.HasValue)
        {
            sb.Append(hasParams ? '&' : '?');
            sb.Append("html=");
            sb.Append(html.Value.ToString().ToLowerInvariant());
        }

        await foreach (var broadcast in _httpClient.StreamNdjsonAsync<BroadcastWithRounds>(sb.ToString(), cancellationToken).ConfigureAwait(false))
        {
            yield return broadcast;
        }
    }

    /// <inheritdoc />
    public async Task<BroadcastTopPage> GetTopBroadcastsAsync(int? page = null, CancellationToken cancellationToken = default)
    {
        var endpoint = page.HasValue ? $"/api/broadcast/top?page={page.Value}" : "/api/broadcast/top";
        return await _httpClient.GetAsync<BroadcastTopPage>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BroadcastByUser> StreamUserBroadcastsAsync(string username, int? nb = null, bool? html = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var sb = new StringBuilder($"/api/broadcast/by/{Uri.EscapeDataString(username)}");
        var hasParams = false;

        if (nb.HasValue)
        {
            sb.Append(hasParams ? '&' : '?');
            sb.Append("nb=");
            sb.Append(nb.Value);
            hasParams = true;
        }

        if (html.HasValue)
        {
            sb.Append(hasParams ? '&' : '?');
            sb.Append("html=");
            sb.Append(html.Value.ToString().ToLowerInvariant());
        }

        await foreach (var broadcast in _httpClient.StreamNdjsonAsync<BroadcastByUser>(sb.ToString(), cancellationToken).ConfigureAwait(false))
        {
            yield return broadcast;
        }
    }

    /// <inheritdoc />
    public async Task<BroadcastSearchPage> SearchBroadcastsAsync(string query, int? page = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var sb = new StringBuilder("/api/broadcast/search?q=");
        sb.Append(Uri.EscapeDataString(query));

        if (page.HasValue)
        {
            sb.Append("&page=");
            sb.Append(page.Value);
        }

        return await _httpClient.GetAsync<BroadcastSearchPage>(sb.ToString(), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<BroadcastWithRounds> GetTournamentAsync(string broadcastTournamentId, bool? html = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastTournamentId);

        var endpoint = html.HasValue && html.Value
            ? $"/api/broadcast/{Uri.EscapeDataString(broadcastTournamentId)}?html=1"
            : $"/api/broadcast/{Uri.EscapeDataString(broadcastTournamentId)}";

        return await _httpClient.GetAsync<BroadcastWithRounds>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<BroadcastRound> GetRoundAsync(string broadcastTournamentSlug, string broadcastRoundSlug, string broadcastRoundId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastTournamentSlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastRoundSlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastRoundId);

        var endpoint = $"/api/broadcast/{Uri.EscapeDataString(broadcastTournamentSlug)}/{Uri.EscapeDataString(broadcastRoundSlug)}/{Uri.EscapeDataString(broadcastRoundId)}";
        return await _httpClient.GetAsync<BroadcastRound>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BroadcastMyRound> StreamMyRoundsAsync(int? nb = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var endpoint = nb.HasValue ? $"/api/broadcast/my-rounds?nb={nb.Value}" : "/api/broadcast/my-rounds";

        await foreach (var round in _httpClient.StreamNdjsonAsync<BroadcastMyRound>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return round;
        }
    }

    #endregion

    #region Creating and Managing Broadcasts

    /// <inheritdoc />
    public async Task<BroadcastWithRounds> CreateTournamentAsync(BroadcastTournamentOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Name);

        var content = new FormUrlEncodedContent(BuildTournamentParameters(options));
        return await _httpClient.PostAsync<BroadcastWithRounds>("/api/broadcast/new", content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<BroadcastWithRounds> UpdateTournamentAsync(string broadcastTournamentId, BroadcastTournamentOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastTournamentId);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Name);

        var content = new FormUrlEncodedContent(BuildTournamentParameters(options));
        var endpoint = $"/broadcast/{Uri.EscapeDataString(broadcastTournamentId)}/edit";
        return await _httpClient.PostAsync<BroadcastWithRounds>(endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<BroadcastRoundNew> CreateRoundAsync(string broadcastTournamentId, BroadcastRoundOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastTournamentId);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Name);

        var content = new FormUrlEncodedContent(BuildRoundParameters(options));
        var endpoint = $"/broadcast/{Uri.EscapeDataString(broadcastTournamentId)}/new";
        return await _httpClient.PostAsync<BroadcastRoundNew>(endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<BroadcastRoundNew> UpdateRoundAsync(string broadcastRoundId, BroadcastRoundOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastRoundId);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Name);

        var content = new FormUrlEncodedContent(BuildRoundParameters(options));
        var endpoint = $"/broadcast/round/{Uri.EscapeDataString(broadcastRoundId)}/edit";
        return await _httpClient.PostAsync<BroadcastRoundNew>(endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> ResetRoundAsync(string broadcastRoundId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastRoundId);

        var endpoint = $"/api/broadcast/round/{Uri.EscapeDataString(broadcastRoundId)}/reset";
        await _httpClient.PostNoContentAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<BroadcastPgnPushResult> PushPgnAsync(string broadcastRoundId, string pgn, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastRoundId);
        ArgumentException.ThrowIfNullOrWhiteSpace(pgn);

        var endpoint = $"/api/broadcast/round/{Uri.EscapeDataString(broadcastRoundId)}/push";
        return await _httpClient.PostPlainTextAsync<BroadcastPgnPushResult>(endpoint, pgn, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Export PGN

    /// <inheritdoc />
    public async Task<string> ExportRoundPgnAsync(string broadcastRoundId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastRoundId);

        var endpoint = $"/api/broadcast/round/{Uri.EscapeDataString(broadcastRoundId)}.pgn";
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> ExportAllRoundsPgnAsync(string broadcastTournamentId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastTournamentId);

        var endpoint = $"/api/broadcast/{Uri.EscapeDataString(broadcastTournamentId)}.pgn";
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> StreamRoundPgnAsync(string broadcastRoundId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(broadcastRoundId);

        // This endpoint streams raw PGN text, not NDJSON
        // Each update is a complete PGN of the round
        // We need to implement a special streaming method for this
        var endpoint = $"/api/stream/broadcast/round/{Uri.EscapeDataString(broadcastRoundId)}.pgn";

        // For now, yield a single PGN export
        // A proper implementation would require streaming support for plain text
        var pgn = await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken).ConfigureAwait(false);
        yield return pgn;
    }

    #endregion

    #region Players

    /// <inheritdoc />
    public async Task<IReadOnlyList<BroadcastPlayerEntry>> GetPlayersAsync(string tournamentId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tournamentId);

        var endpoint = $"/broadcast/{Uri.EscapeDataString(tournamentId)}/players";
        return await _httpClient.GetAsync<List<BroadcastPlayerEntry>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<BroadcastPlayerWithGames> GetPlayerAsync(string tournamentId, string playerId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tournamentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(playerId);

        var endpoint = $"/broadcast/{Uri.EscapeDataString(tournamentId)}/players/{Uri.EscapeDataString(playerId)}";
        return await _httpClient.GetAsync<BroadcastPlayerWithGames>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Helper Methods

    private static List<KeyValuePair<string, string>> BuildTournamentParameters(BroadcastTournamentOptions options)
    {
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("name", options.Name)
        };

        if (!string.IsNullOrWhiteSpace(options.ShortDescription))
        {
            parameters.Add(new("shortDescription", options.ShortDescription));
        }

        if (!string.IsNullOrWhiteSpace(options.FullDescription))
        {
            parameters.Add(new("fullDescription", options.FullDescription));
        }

        if (options.AutoLeaderboard.HasValue)
        {
            parameters.Add(new("autoLeaderboard", options.AutoLeaderboard.Value.ToString().ToLowerInvariant()));
        }

        if (options.TeamTable.HasValue)
        {
            parameters.Add(new("teamTable", options.TeamTable.Value.ToString().ToLowerInvariant()));
        }

        if (!string.IsNullOrWhiteSpace(options.Players))
        {
            parameters.Add(new("players", options.Players));
        }

        if (!string.IsNullOrWhiteSpace(options.Website))
        {
            parameters.Add(new("info.website", options.Website));
        }

        if (!string.IsNullOrWhiteSpace(options.Standings))
        {
            parameters.Add(new("info.standings", options.Standings));
        }

        if (!string.IsNullOrWhiteSpace(options.Location))
        {
            parameters.Add(new("info.location", options.Location));
        }

        if (!string.IsNullOrWhiteSpace(options.Format))
        {
            parameters.Add(new("info.format", options.Format));
        }

        if (!string.IsNullOrWhiteSpace(options.TimeControl))
        {
            parameters.Add(new("info.tc", options.TimeControl));
        }

        if (!string.IsNullOrWhiteSpace(options.FideTimeControl))
        {
            parameters.Add(new("info.fideTc", options.FideTimeControl));
        }

        if (!string.IsNullOrWhiteSpace(options.TimeZone))
        {
            parameters.Add(new("info.timeZone", options.TimeZone));
        }

        return parameters;
    }

    private static List<KeyValuePair<string, string>> BuildRoundParameters(BroadcastRoundOptions options)
    {
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("name", options.Name)
        };

        if (!string.IsNullOrWhiteSpace(options.SyncUrl))
        {
            parameters.Add(new("syncUrl", options.SyncUrl));
        }

        if (options.StartsAt.HasValue)
        {
            parameters.Add(new("startsAt", options.StartsAt.Value.ToString()));
        }

        if (options.StartsAfterPrevious.HasValue)
        {
            parameters.Add(new("startsAfterPrevious", options.StartsAfterPrevious.Value.ToString().ToLowerInvariant()));
        }

        if (options.Rated.HasValue)
        {
            parameters.Add(new("rated", options.Rated.Value.ToString().ToLowerInvariant()));
        }

        if (options.Delay.HasValue)
        {
            parameters.Add(new("delay", options.Delay.Value.ToString()));
        }

        if (options.SyncPeriod.HasValue)
        {
            parameters.Add(new("period", options.SyncPeriod.Value.ToString()));
        }

        return parameters;
    }

    #endregion
}
