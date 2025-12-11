using System.Runtime.CompilerServices;
using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Swiss Tournaments API.
/// </summary>
internal sealed class SwissTournamentsApi(ILichessHttpClient httpClient) : ISwissTournamentsApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<SwissTournament> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/swiss/{Uri.EscapeDataString(id)}";
        return await _httpClient.GetAsync<SwissTournament>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<SwissTournament> CreateAsync(string teamId, SwissCreateOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);
        ArgumentNullException.ThrowIfNull(options);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("name", options.Name),
            new("clock.limit", options.ClockLimit.ToString()),
            new("clock.increment", options.ClockIncrement.ToString()),
            new("nbRounds", options.NbRounds.ToString())
        };

        if (options.StartsAt.HasValue)
        {
            parameters.Add(new("startsAt", options.StartsAt.Value.ToString()));
        }

        if (options.RoundInterval.HasValue)
        {
            parameters.Add(new("roundInterval", options.RoundInterval.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(options.Variant))
        {
            parameters.Add(new("variant", options.Variant));
        }

        if (!string.IsNullOrEmpty(options.Description))
        {
            parameters.Add(new("description", options.Description));
        }

        if (options.Rated.HasValue)
        {
            parameters.Add(new("rated", options.Rated.Value.ToString().ToLowerInvariant()));
        }

        if (!string.IsNullOrEmpty(options.Password))
        {
            parameters.Add(new("password", options.Password));
        }

        if (options.ForbiddenPairings.HasValue)
        {
            parameters.Add(new("forbiddenPairings", options.ForbiddenPairings.Value.ToString().ToLowerInvariant()));
        }

        if (options.ManualPairings.HasValue)
        {
            parameters.Add(new("manualPairings", options.ManualPairings.Value.ToString().ToLowerInvariant()));
        }

        if (options.ChatFor.HasValue)
        {
            parameters.Add(new("chatFor", options.ChatFor.Value ? "20" : "0"));
        }

        if (!string.IsNullOrEmpty(options.Position))
        {
            parameters.Add(new("position", options.Position));
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

        if (options.OnlyLeaders.HasValue)
        {
            parameters.Add(new("conditions.allowList", options.OnlyLeaders.Value.ToString().ToLowerInvariant()));
        }

        if (options.MinAccountAge.HasValue)
        {
            parameters.Add(new("conditions.minAccountAge.days", options.MinAccountAge.Value.ToString()));
        }

        if (options.OnlyTitled.HasValue)
        {
            parameters.Add(new("conditions.titled", options.OnlyTitled.Value.ToString().ToLowerInvariant()));
        }

        var content = new FormUrlEncodedContent(parameters);
        var endpoint = $"/api/swiss/new/{Uri.EscapeDataString(teamId)}";
        return await _httpClient.PostAsync<SwissTournament>(endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<SwissTournament> UpdateAsync(string id, SwissUpdateOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(options);

        var parameters = new List<KeyValuePair<string, string>>();

        if (!string.IsNullOrEmpty(options.Name))
        {
            parameters.Add(new("name", options.Name));
        }

        if (options.ClockLimit.HasValue)
        {
            parameters.Add(new("clock.limit", options.ClockLimit.Value.ToString()));
        }

        if (options.ClockIncrement.HasValue)
        {
            parameters.Add(new("clock.increment", options.ClockIncrement.Value.ToString()));
        }

        if (options.NbRounds.HasValue)
        {
            parameters.Add(new("nbRounds", options.NbRounds.Value.ToString()));
        }

        if (options.StartsAt.HasValue)
        {
            parameters.Add(new("startsAt", options.StartsAt.Value.ToString()));
        }

        if (options.RoundInterval.HasValue)
        {
            parameters.Add(new("roundInterval", options.RoundInterval.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(options.Variant))
        {
            parameters.Add(new("variant", options.Variant));
        }

        if (!string.IsNullOrEmpty(options.Description))
        {
            parameters.Add(new("description", options.Description));
        }

        if (options.Rated.HasValue)
        {
            parameters.Add(new("rated", options.Rated.Value.ToString().ToLowerInvariant()));
        }

        if (!string.IsNullOrEmpty(options.Password))
        {
            parameters.Add(new("password", options.Password));
        }

        if (options.ForbiddenPairings.HasValue)
        {
            parameters.Add(new("forbiddenPairings", options.ForbiddenPairings.Value.ToString().ToLowerInvariant()));
        }

        if (options.ManualPairings.HasValue)
        {
            parameters.Add(new("manualPairings", options.ManualPairings.Value.ToString().ToLowerInvariant()));
        }

        if (options.ChatFor.HasValue)
        {
            parameters.Add(new("chatFor", options.ChatFor.Value ? "20" : "0"));
        }

        if (!string.IsNullOrEmpty(options.Position))
        {
            parameters.Add(new("position", options.Position));
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
        var endpoint = $"/api/swiss/{Uri.EscapeDataString(id)}/edit";
        return await _httpClient.PostAsync<SwissTournament>(endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> ScheduleNextRoundAsync(string id, long date, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("date", date.ToString())
        };

        var content = new FormUrlEncodedContent(parameters);
        var endpoint = $"/api/swiss/{Uri.EscapeDataString(id)}/schedule-next-round";
        await _httpClient.PostAsync<OkResponse>(endpoint, content, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> JoinAsync(string id, string? password = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        HttpContent? content = null;
        if (!string.IsNullOrEmpty(password))
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new("password", password)
            };
            content = new FormUrlEncodedContent(parameters);
        }

        var endpoint = $"/api/swiss/{Uri.EscapeDataString(id)}/join";
        await _httpClient.PostAsync<OkResponse>(endpoint, content, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> PauseOrWithdrawAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/swiss/{Uri.EscapeDataString(id)}/withdraw";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> TerminateAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/swiss/{Uri.EscapeDataString(id)}/terminate";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<string> ExportTrfAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/swiss/{Uri.EscapeDataString(id)}.trf";
        return await _httpClient.GetStringAsync(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<GameJson> StreamGamesAsync(string id, SwissGamesExportOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var sb = new StringBuilder($"/api/swiss/{Uri.EscapeDataString(id)}/games");
        var hasQuery = false;

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

        yield break;

        void AppendParam(string name, string value)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append(name);
            sb.Append('=');
            sb.Append(value);
            hasQuery = true;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<SwissPlayerResult> StreamResultsAsync(string id, int? nb = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/swiss/{Uri.EscapeDataString(id)}/results";
        if (nb.HasValue)
        {
            endpoint += $"?nb={nb.Value}";
        }

        await foreach (var result in _httpClient.StreamNdjsonAsync<SwissPlayerResult>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return result;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<SwissTournament> StreamTeamTournamentsAsync(string teamId, int? max = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var endpoint = $"/api/team/{Uri.EscapeDataString(teamId)}/swiss";
        if (max.HasValue)
        {
            endpoint += $"?max={max.Value}";
        }

        await foreach (var tournament in _httpClient.StreamNdjsonAsync<SwissTournament>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return tournament;
        }
    }
}
