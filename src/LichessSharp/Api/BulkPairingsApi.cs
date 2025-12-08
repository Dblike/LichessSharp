using System.Runtime.CompilerServices;
using System.Text;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Bulk Pairings API.
/// </summary>
internal sealed class BulkPairingsApi(ILichessHttpClient httpClient) : IBulkPairingsApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<IReadOnlyList<BulkPairing>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Lichess returns {"bulks": [...]} wrapper object
        var response = await _httpClient.GetAsync<BulkPairingListResponse>("/api/bulk-pairing", cancellationToken).ConfigureAwait(false);
        return response.Bulks ?? (IReadOnlyList<BulkPairing>)Array.Empty<BulkPairing>();
    }

    /// <inheritdoc />
    public async Task<BulkPairing> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/bulk-pairing/{Uri.EscapeDataString(id)}";
        return await _httpClient.GetAsync<BulkPairing>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<BulkPairing> CreateAsync(BulkPairingCreateOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Players);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("players", options.Players)
        };

        if (options.ClockLimit.HasValue)
        {
            parameters.Add(new("clock.limit", options.ClockLimit.Value.ToString()));
        }

        if (options.ClockIncrement.HasValue)
        {
            parameters.Add(new("clock.increment", options.ClockIncrement.Value.ToString()));
        }

        if (options.Days.HasValue)
        {
            parameters.Add(new("days", options.Days.Value.ToString()));
        }

        if (options.PairAt.HasValue)
        {
            parameters.Add(new("pairAt", options.PairAt.Value.ToString()));
        }

        if (options.StartClocksAt.HasValue)
        {
            parameters.Add(new("startClocksAt", options.StartClocksAt.Value.ToString()));
        }

        if (options.Rated.HasValue)
        {
            parameters.Add(new("rated", options.Rated.Value.ToString().ToLowerInvariant()));
        }

        if (!string.IsNullOrWhiteSpace(options.Variant))
        {
            parameters.Add(new("variant", options.Variant));
        }

        if (!string.IsNullOrWhiteSpace(options.Fen))
        {
            parameters.Add(new("fen", options.Fen));
        }

        if (!string.IsNullOrWhiteSpace(options.Message))
        {
            parameters.Add(new("message", options.Message));
        }

        if (!string.IsNullOrWhiteSpace(options.Rules))
        {
            parameters.Add(new("rules", options.Rules));
        }

        var content = new FormUrlEncodedContent(parameters);
        return await _httpClient.PostAsync<BulkPairing>("/api/bulk-pairing", content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> StartClocksAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/bulk-pairing/{Uri.EscapeDataString(id)}/start-clocks";
        await _httpClient.PostNoContentAsync(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CancelAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = $"/api/bulk-pairing/{Uri.EscapeDataString(id)}";
        await _httpClient.DeleteNoContentAsync(endpoint, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<string> ExportGamesPgnAsync(string id, BulkPairingExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = BuildExportEndpoint($"/api/bulk-pairing/{Uri.EscapeDataString(id)}/games", options);
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<GameJson> StreamGamesAsync(string id, BulkPairingExportOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var endpoint = BuildExportEndpoint($"/api/bulk-pairing/{Uri.EscapeDataString(id)}/games", options);

        await foreach (var game in _httpClient.StreamNdjsonAsync<GameJson>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return game;
        }
    }

    private static string BuildExportEndpoint(string baseEndpoint, BulkPairingExportOptions? options)
    {
        if (options == null)
        {
            return baseEndpoint;
        }

        var sb = new StringBuilder(baseEndpoint);
        var hasParams = false;

        void AppendParam(string name, bool? value)
        {
            if (value.HasValue)
            {
                sb.Append(hasParams ? '&' : '?');
                sb.Append(name);
                sb.Append('=');
                sb.Append(value.Value.ToString().ToLowerInvariant());
                hasParams = true;
            }
        }

        AppendParam("moves", options.Moves);
        AppendParam("pgnInJson", options.PgnInJson);
        AppendParam("tags", options.Tags);
        AppendParam("clocks", options.Clocks);
        AppendParam("opening", options.Opening);

        return sb.ToString();
    }
}
