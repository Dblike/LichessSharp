using System.Runtime.CompilerServices;
using System.Text;

using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Arena Tournaments API.
/// </summary>
internal sealed class ArenaTournamentsApi : IArenaTournamentsApi
{
    private readonly ILichessHttpClient _httpClient;

    public ArenaTournamentsApi(ILichessHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<ArenaTournamentList> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<ArenaTournamentList>("/api/tournament", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ArenaTournament> GetAsync(string id, int page = 1, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = page > 1 ? $"/api/tournament/{Uri.EscapeDataString(id)}?page={page}" : $"/api/tournament/{Uri.EscapeDataString(id)}";
        return await _httpClient.GetAsync<ArenaTournament>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ArenaTournament> CreateAsync(ArenaCreateOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("clockTime", options.ClockTime.ToString()),
            new("clockIncrement", options.ClockIncrement.ToString()),
            new("minutes", options.Minutes.ToString())
        };

        if (!string.IsNullOrEmpty(options.Name))
        {
            parameters.Add(new("name", options.Name));
        }

        if (options.WaitMinutes.HasValue)
        {
            parameters.Add(new("waitMinutes", options.WaitMinutes.Value.ToString()));
        }

        if (options.StartDate.HasValue)
        {
            parameters.Add(new("startDate", options.StartDate.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(options.Variant))
        {
            parameters.Add(new("variant", options.Variant));
        }

        if (options.Rated.HasValue)
        {
            parameters.Add(new("rated", options.Rated.Value.ToString().ToLowerInvariant()));
        }

        if (!string.IsNullOrEmpty(options.Position))
        {
            parameters.Add(new("position", options.Position));
        }

        if (options.Berserkable.HasValue)
        {
            parameters.Add(new("berserkable", options.Berserkable.Value.ToString().ToLowerInvariant()));
        }

        if (options.Streakable.HasValue)
        {
            parameters.Add(new("streakable", options.Streakable.Value.ToString().ToLowerInvariant()));
        }

        if (options.HasChat.HasValue)
        {
            parameters.Add(new("hasChat", options.HasChat.Value.ToString().ToLowerInvariant()));
        }

        if (!string.IsNullOrEmpty(options.Description))
        {
            parameters.Add(new("description", options.Description));
        }

        if (!string.IsNullOrEmpty(options.Password))
        {
            parameters.Add(new("password", options.Password));
        }

        if (!string.IsNullOrEmpty(options.TeamId))
        {
            parameters.Add(new("conditions.teamMember.teamId", options.TeamId));
        }

        if (options.MinRating.HasValue)
        {
            parameters.Add(new("conditions.minRating.rating", options.MinRating.Value.ToString()));
        }

        if (options.MaxRating.HasValue)
        {
            parameters.Add(new("conditions.maxRating.rating", options.MaxRating.Value.ToString()));
        }

        if (options.MinRatedGames.HasValue)
        {
            parameters.Add(new("conditions.nbRatedGame.nb", options.MinRatedGames.Value.ToString()));
        }

        var content = new FormUrlEncodedContent(parameters);
        return await _httpClient.PostAsync<ArenaTournament>("/api/tournament", content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ArenaTournament> UpdateAsync(string id, ArenaUpdateOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(options);

        var parameters = new List<KeyValuePair<string, string>>();

        if (!string.IsNullOrEmpty(options.Name))
        {
            parameters.Add(new("name", options.Name));
        }

        if (options.ClockTime.HasValue)
        {
            parameters.Add(new("clockTime", options.ClockTime.Value.ToString()));
        }

        if (options.ClockIncrement.HasValue)
        {
            parameters.Add(new("clockIncrement", options.ClockIncrement.Value.ToString()));
        }

        if (options.Minutes.HasValue)
        {
            parameters.Add(new("minutes", options.Minutes.Value.ToString()));
        }

        if (options.StartDate.HasValue)
        {
            parameters.Add(new("startDate", options.StartDate.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(options.Variant))
        {
            parameters.Add(new("variant", options.Variant));
        }

        if (options.Rated.HasValue)
        {
            parameters.Add(new("rated", options.Rated.Value.ToString().ToLowerInvariant()));
        }

        if (!string.IsNullOrEmpty(options.Position))
        {
            parameters.Add(new("position", options.Position));
        }

        if (options.Berserkable.HasValue)
        {
            parameters.Add(new("berserkable", options.Berserkable.Value.ToString().ToLowerInvariant()));
        }

        if (options.Streakable.HasValue)
        {
            parameters.Add(new("streakable", options.Streakable.Value.ToString().ToLowerInvariant()));
        }

        if (options.HasChat.HasValue)
        {
            parameters.Add(new("hasChat", options.HasChat.Value.ToString().ToLowerInvariant()));
        }

        if (!string.IsNullOrEmpty(options.Description))
        {
            parameters.Add(new("description", options.Description));
        }

        if (!string.IsNullOrEmpty(options.Password))
        {
            parameters.Add(new("password", options.Password));
        }

        if (options.MinRating.HasValue)
        {
            parameters.Add(new("conditions.minRating.rating", options.MinRating.Value.ToString()));
        }

        if (options.MaxRating.HasValue)
        {
            parameters.Add(new("conditions.maxRating.rating", options.MaxRating.Value.ToString()));
        }

        if (options.MinRatedGames.HasValue)
        {
            parameters.Add(new("conditions.nbRatedGame.nb", options.MinRatedGames.Value.ToString()));
        }

        var content = new FormUrlEncodedContent(parameters);
        var endpoint = $"/api/tournament/{Uri.EscapeDataString(id)}";
        return await _httpClient.PostAsync<ArenaTournament>(endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> JoinAsync(string id, string? password = null, string? team = null, bool? pairMeAsap = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var parameters = new List<KeyValuePair<string, string>>();

        if (!string.IsNullOrEmpty(password))
        {
            parameters.Add(new("password", password));
        }

        if (!string.IsNullOrEmpty(team))
        {
            parameters.Add(new("team", team));
        }

        if (pairMeAsap.HasValue)
        {
            parameters.Add(new("pairMeAsap", pairMeAsap.Value.ToString().ToLowerInvariant()));
        }

        HttpContent? content = parameters.Count > 0 ? new FormUrlEncodedContent(parameters) : null;
        var endpoint = $"/api/tournament/{Uri.EscapeDataString(id)}/join";
        await _httpClient.PostAsync<OkResponse>(endpoint, content, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> WithdrawAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/tournament/{Uri.EscapeDataString(id)}/withdraw";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> TerminateAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/tournament/{Uri.EscapeDataString(id)}/terminate";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<ArenaTournament> UpdateTeamBattleAsync(string id, string teams, int? nbLeaders = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(teams);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("teams", teams)
        };

        if (nbLeaders.HasValue)
        {
            parameters.Add(new("nbLeaders", nbLeaders.Value.ToString()));
        }

        var content = new FormUrlEncodedContent(parameters);
        var endpoint = $"/api/tournament/team-battle/{Uri.EscapeDataString(id)}";
        return await _httpClient.PostAsync<ArenaTournament>(endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<GameJson> StreamGamesAsync(string id, ArenaGamesExportOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var sb = new StringBuilder($"/api/tournament/{Uri.EscapeDataString(id)}/games");
        var hasQuery = false;

        void AppendParam(string name, string value)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append(name);
            sb.Append('=');
            sb.Append(value);
            hasQuery = true;
        }

        if (options != null)
        {
            if (!string.IsNullOrEmpty(options.Player))
            {
                AppendParam("player", Uri.EscapeDataString(options.Player));
            }

            if (options.Moves.HasValue)
            {
                AppendParam("moves", options.Moves.Value.ToString().ToLowerInvariant());
            }

            if (options.PgnInJson.HasValue)
            {
                AppendParam("pgnInJson", options.PgnInJson.Value.ToString().ToLowerInvariant());
            }

            if (options.Tags.HasValue)
            {
                AppendParam("tags", options.Tags.Value.ToString().ToLowerInvariant());
            }

            if (options.Clocks.HasValue)
            {
                AppendParam("clocks", options.Clocks.Value.ToString().ToLowerInvariant());
            }

            if (options.Evals.HasValue)
            {
                AppendParam("evals", options.Evals.Value.ToString().ToLowerInvariant());
            }

            if (options.Opening.HasValue)
            {
                AppendParam("opening", options.Opening.Value.ToString().ToLowerInvariant());
            }
        }

        await foreach (var game in _httpClient.StreamNdjsonAsync<GameJson>(sb.ToString(), cancellationToken).ConfigureAwait(false))
        {
            yield return game;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ArenaPlayerResult> StreamResultsAsync(string id, int? nb = null, bool sheet = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var sb = new StringBuilder($"/api/tournament/{Uri.EscapeDataString(id)}/results");
        var hasQuery = false;

        if (nb.HasValue)
        {
            sb.Append('?');
            sb.Append("nb=");
            sb.Append(nb.Value);
            hasQuery = true;
        }

        if (sheet)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("sheet=true");
        }

        await foreach (var result in _httpClient.StreamNdjsonAsync<ArenaPlayerResult>(sb.ToString(), cancellationToken).ConfigureAwait(false))
        {
            yield return result;
        }
    }

    /// <inheritdoc />
    public async Task<ArenaTeamStanding> GetTeamStandingAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/tournament/{Uri.EscapeDataString(id)}/teams";
        return await _httpClient.GetAsync<ArenaTeamStanding>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ArenaTournamentSummary> StreamCreatedByAsync(string username, ArenaStatusFilter? status = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = $"/api/user/{Uri.EscapeDataString(username)}/tournament/created";
        if (status.HasValue)
        {
            var statusValue = status.Value switch
            {
                ArenaStatusFilter.Created => 10,
                ArenaStatusFilter.Started => 20,
                ArenaStatusFilter.Finished => 30,
                _ => throw new ArgumentOutOfRangeException(nameof(status))
            };
            endpoint += $"?status={statusValue}";
        }

        await foreach (var tournament in _httpClient.StreamNdjsonAsync<ArenaTournamentSummary>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return tournament;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ArenaPlayedTournament> StreamPlayedByAsync(string username, int? nb = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = $"/api/user/{Uri.EscapeDataString(username)}/tournament/played";
        if (nb.HasValue)
        {
            endpoint += $"?nb={nb.Value}";
        }

        await foreach (var tournament in _httpClient.StreamNdjsonAsync<ArenaPlayedTournament>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return tournament;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ArenaTournamentSummary> StreamTeamTournamentsAsync(string teamId, int? max = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var endpoint = $"/api/team/{Uri.EscapeDataString(teamId)}/arena";
        if (max.HasValue)
        {
            endpoint += $"?max={max.Value}";
        }

        await foreach (var tournament in _httpClient.StreamNdjsonAsync<ArenaTournamentSummary>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return tournament;
        }
    }
}
