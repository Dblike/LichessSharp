using System.Runtime.CompilerServices;
using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Api.Options;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Games API.
/// </summary>
internal sealed class GamesApi(ILichessHttpClient httpClient) : IGamesApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<GameJson> ExportAsync(string gameId, ExportGameOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = BuildExportGameEndpoint(gameId, options);
        return await _httpClient.GetAsync<GameJson>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> GetPgnAsync(string gameId, ExportGameOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = BuildExportGameEndpoint(gameId, options);
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<GameJson> GetCurrentGameByUserAsync(string username, ExportGameOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = BuildCurrentGameEndpoint(username, options);
        return await _httpClient.GetAsync<GameJson>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<GameJson> StreamUserGamesAsync(string username, ExportUserGamesOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = BuildUserGamesEndpoint(username, options);
        await foreach (var game in _httpClient.StreamNdjsonAsync<GameJson>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return game;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<GameJson> StreamByIdsAsync(IEnumerable<string> gameIds, ExportGameOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(gameIds);

        var idsList = gameIds.ToList();
        if (idsList.Count == 0)
        {
            yield break;
        }

        if (idsList.Count > 300)
        {
            throw new ArgumentException("Cannot export more than 300 games at once.", nameof(gameIds));
        }

        var endpoint = BuildExportByIdsEndpoint(options);
        var body = string.Join(",", idsList);

        await foreach (var game in _httpClient.StreamNdjsonPostAsync<GameJson>(endpoint, new StringContent(body, Encoding.UTF8, "text/plain"), cancellationToken).ConfigureAwait(false))
        {
            yield return game;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<GameJson> StreamByUsersAsync(IEnumerable<string> userIds, bool withCurrentGames = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userIds);

        var idsList = userIds.ToList();
        if (idsList.Count == 0)
        {
            yield break;
        }

        if (idsList.Count > 300)
        {
            throw new ArgumentException("Cannot stream games for more than 300 users at once.", nameof(userIds));
        }

        var endpoint = $"/api/stream/games-by-users{(withCurrentGames ? "?withCurrentGames=true" : "")}";
        var body = string.Join(",", idsList);

        await foreach (var game in _httpClient.StreamNdjsonPostAsync<GameJson>(endpoint, new StringContent(body, Encoding.UTF8, "text/plain"), cancellationToken).ConfigureAwait(false))
        {
            yield return game;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OngoingGame>> GetOngoingGamesAsync(int count = 9, CancellationToken cancellationToken = default)
    {
        if (count < 1 || count > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be between 1 and 50.");
        }

        var endpoint = $"/api/account/playing?nb={count}";
        var response = await _httpClient.GetAsync<OngoingGamesResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return response.NowPlaying ?? [];
    }

    /// <inheritdoc />
    public async Task<ImportGameResponse> ImportAsync(string pgn, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pgn);

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["pgn"] = pgn
        });

        return await _httpClient.PostAsync<ImportGameResponse>("/api/import", content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> ExportImportedGamesAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetStringWithAcceptAsync(
            "/api/games/export/imports",
            "application/x-chess-pgn",
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<GameJson> StreamBookmarkedGamesAsync(
        ExportBookmarksOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var endpoint = BuildExportBookmarksEndpoint(options);
        await foreach (var game in _httpClient.StreamNdjsonAsync<GameJson>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return game;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<MoveStreamEvent> StreamGameMovesAsync(
        string gameId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/stream/game/{Uri.EscapeDataString(gameId)}";
        await foreach (var evt in _httpClient.StreamNdjsonAsync<MoveStreamEvent>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return evt;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<GameStreamEvent> StreamByIdsAsync(
        string streamId,
        IEnumerable<string> gameIds,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);
        ArgumentNullException.ThrowIfNull(gameIds);

        var idsList = gameIds.ToList();
        if (idsList.Count == 0)
        {
            yield break;
        }

        if (idsList.Count > 500)
        {
            throw new ArgumentException("Cannot stream more than 500 games at once.", nameof(gameIds));
        }

        var endpoint = $"/api/stream/games/{Uri.EscapeDataString(streamId)}";
        var body = string.Join(",", idsList);

        await foreach (var evt in _httpClient.StreamNdjsonPostAsync<GameStreamEvent>(
            endpoint,
            new StringContent(body, Encoding.UTF8, "text/plain"),
            cancellationToken).ConfigureAwait(false))
        {
            yield return evt;
        }
    }

    /// <inheritdoc />
    public async Task AddGameIdsToStreamAsync(
        string streamId,
        IEnumerable<string> gameIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);
        ArgumentNullException.ThrowIfNull(gameIds);

        var idsList = gameIds.ToList();
        if (idsList.Count == 0)
        {
            return;
        }

        if (idsList.Count > 500)
        {
            throw new ArgumentException("Cannot add more than 500 games at once.", nameof(gameIds));
        }

        var endpoint = $"/api/stream/games/{Uri.EscapeDataString(streamId)}/add";
        var body = string.Join(",", idsList);

        await _httpClient.PostAsync<OkResponse>(
            endpoint,
            new StringContent(body, Encoding.UTF8, "text/plain"),
            cancellationToken).ConfigureAwait(false);
    }

    private static string BuildExportGameEndpoint(string gameId, ExportGameOptions? options)
    {
        var sb = new StringBuilder();
        sb.Append("/game/export/");
        sb.Append(Uri.EscapeDataString(gameId));

        AppendExportGameOptions(sb, options);

        return sb.ToString();
    }

    private static string BuildCurrentGameEndpoint(string username, ExportGameOptions? options)
    {
        var sb = new StringBuilder();
        sb.Append("/api/user/");
        sb.Append(Uri.EscapeDataString(username));
        sb.Append("/current-game");

        AppendExportGameOptions(sb, options);

        return sb.ToString();
    }

    private static string BuildUserGamesEndpoint(string username, ExportUserGamesOptions? options)
    {
        var sb = new StringBuilder();
        sb.Append("/api/games/user/");
        sb.Append(Uri.EscapeDataString(username));

        var hasQuery = false;
        AppendExportGameOptions(sb, options, ref hasQuery);
        AppendUserGamesOptions(sb, options, ref hasQuery);

        return sb.ToString();
    }

    private static string BuildExportByIdsEndpoint(ExportGameOptions? options)
    {
        var sb = new StringBuilder();
        sb.Append("/api/games/export/_ids");

        AppendExportGameOptions(sb, options);

        return sb.ToString();
    }

    private static void AppendExportGameOptions(StringBuilder sb, ExportGameOptions? options)
    {
        var hasQuery = false;
        AppendExportGameOptions(sb, options, ref hasQuery);
    }

    private static void AppendExportGameOptions(StringBuilder sb, ExportGameOptions? options, ref bool hasQuery)
    {
        if (options == null)
        {
            return;
        }

        AppendBoolParam(sb, "moves", options.Moves, ref hasQuery);
        AppendBoolParam(sb, "pgnInJson", options.PgnInJson, ref hasQuery);
        AppendBoolParam(sb, "tags", options.Tags, ref hasQuery);
        AppendBoolParam(sb, "clocks", options.Clocks, ref hasQuery);
        AppendBoolParam(sb, "evals", options.Evals, ref hasQuery);
        AppendBoolParam(sb, "accuracy", options.Accuracy, ref hasQuery);
        AppendBoolParam(sb, "opening", options.Opening, ref hasQuery);
        AppendBoolParam(sb, "division", options.Division, ref hasQuery);
        AppendBoolParam(sb, "literate", options.Literate, ref hasQuery);

        if (!string.IsNullOrWhiteSpace(options.Players))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("players=");
            sb.Append(Uri.EscapeDataString(options.Players));
            hasQuery = true;
        }
    }

    private static void AppendUserGamesOptions(StringBuilder sb, ExportUserGamesOptions? options, ref bool hasQuery)
    {
        if (options == null)
        {
            return;
        }

        if (options.Since.HasValue)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("since=");
            sb.Append(options.Since.Value.ToUnixTimeMilliseconds());
            hasQuery = true;
        }

        if (options.Until.HasValue)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("until=");
            sb.Append(options.Until.Value.ToUnixTimeMilliseconds());
            hasQuery = true;
        }

        if (options.Max.HasValue)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("max=");
            sb.Append(options.Max.Value);
            hasQuery = true;
        }

        if (!string.IsNullOrWhiteSpace(options.Vs))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("vs=");
            sb.Append(Uri.EscapeDataString(options.Vs));
            hasQuery = true;
        }

        AppendBoolParam(sb, "rated", options.Rated, ref hasQuery);

        if (!string.IsNullOrWhiteSpace(options.PerfType))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("perfType=");
            sb.Append(Uri.EscapeDataString(options.PerfType));
            hasQuery = true;
        }

        if (!string.IsNullOrWhiteSpace(options.Color))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("color=");
            sb.Append(Uri.EscapeDataString(options.Color));
            hasQuery = true;
        }

        AppendBoolParam(sb, "analysed", options.Analysed, ref hasQuery);
        AppendBoolParam(sb, "ongoing", options.Ongoing, ref hasQuery);
        AppendBoolParam(sb, "finished", options.Finished, ref hasQuery);

        if (!string.IsNullOrWhiteSpace(options.Sort))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("sort=");
            sb.Append(Uri.EscapeDataString(options.Sort));
            hasQuery = true;
        }
    }

    private static void AppendBoolParam(StringBuilder sb, string name, bool? value, ref bool hasQuery)
    {
        if (value.HasValue)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append(name);
            sb.Append('=');
            sb.Append(value.Value ? "true" : "false");
            hasQuery = true;
        }
    }

    private static string BuildExportBookmarksEndpoint(ExportBookmarksOptions? options)
    {
        var sb = new StringBuilder("/api/games/export/bookmarks");
        var hasQuery = false;
        AppendExportGameOptions(sb, options, ref hasQuery);
        AppendBookmarksOptions(sb, options, ref hasQuery);
        return sb.ToString();
    }

    private static void AppendBookmarksOptions(StringBuilder sb, ExportBookmarksOptions? options, ref bool hasQuery)
    {
        if (options == null)
        {
            return;
        }

        if (options.Since.HasValue)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("since=");
            sb.Append(options.Since.Value.ToUnixTimeMilliseconds());
            hasQuery = true;
        }

        if (options.Until.HasValue)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("until=");
            sb.Append(options.Until.Value.ToUnixTimeMilliseconds());
            hasQuery = true;
        }

        if (options.Max.HasValue)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("max=");
            sb.Append(options.Max.Value);
            hasQuery = true;
        }

        AppendBoolParam(sb, "lastFen", options.LastFen, ref hasQuery);

        if (!string.IsNullOrWhiteSpace(options.Sort))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("sort=");
            sb.Append(Uri.EscapeDataString(options.Sort));
            hasQuery = true;
        }
    }
}
