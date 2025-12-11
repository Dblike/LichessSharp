using System.Text;
using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Opening Explorer API.
/// </summary>
internal sealed class OpeningExplorerApi(ILichessHttpClient httpClient, Uri explorerBaseAddress) : IOpeningExplorerApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly Uri _baseAddress = explorerBaseAddress ?? throw new ArgumentNullException(nameof(explorerBaseAddress));

    /// <inheritdoc />
    public async Task<ExplorerResult> GetMastersAsync(
        string fen,
        ExplorerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fen);

        var url = BuildMastersUrl(fen, options);
        return await _httpClient.GetAbsoluteAsync<ExplorerResult>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ExplorerResult> GetLichessAsync(
        string fen,
        ExplorerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fen);

        var url = BuildLichessUrl(fen, options);
        return await _httpClient.GetAbsoluteAsync<ExplorerResult>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ExplorerResult> GetPlayerAsync(
        string fen,
        string player,
        ExplorerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fen);
        ArgumentException.ThrowIfNullOrWhiteSpace(player);

        var url = BuildPlayerUrl(fen, player, options);
        // The player endpoint returns NDJSON with progressive updates.
        // We read all lines and return the last (most complete) result.
        return await _httpClient.GetAbsoluteNdjsonLastAsync<ExplorerResult>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> GetMasterGamePgnAsync(string gameId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var url = new Uri($"{_baseAddress.ToString().TrimEnd('/')}/masters/pgn/{Uri.EscapeDataString(gameId)}");
        return await _httpClient.GetAbsoluteStringAsync(url, "application/x-chess-pgn", cancellationToken).ConfigureAwait(false);
    }

    private Uri BuildMastersUrl(string fen, ExplorerOptions? options)
    {
        var sb = new StringBuilder();
        sb.Append(_baseAddress.ToString().TrimEnd('/'));
        sb.Append("/masters?fen=");
        sb.Append(Uri.EscapeDataString(fen));

        AppendCommonOptions(sb, options);
        AppendMastersOptions(sb, options);

        return new Uri(sb.ToString());
    }

    private Uri BuildLichessUrl(string fen, ExplorerOptions? options)
    {
        var sb = new StringBuilder();
        sb.Append(_baseAddress.ToString().TrimEnd('/'));
        sb.Append("/lichess?fen=");
        sb.Append(Uri.EscapeDataString(fen));

        AppendCommonOptions(sb, options);
        AppendLichessOptions(sb, options);

        return new Uri(sb.ToString());
    }

    private Uri BuildPlayerUrl(string fen, string player, ExplorerOptions? options)
    {
        var sb = new StringBuilder();
        sb.Append(_baseAddress.ToString().TrimEnd('/'));
        sb.Append("/player?fen=");
        sb.Append(Uri.EscapeDataString(fen));
        sb.Append("&player=");
        sb.Append(Uri.EscapeDataString(player));

        // Player endpoint requires a color - default to white if not specified
        sb.Append("&color=");
        sb.Append(options?.Color ?? "white");

        AppendCommonOptions(sb, options);
        AppendLichessOptions(sb, options);

        return new Uri(sb.ToString());
    }

    private static void AppendCommonOptions(StringBuilder sb, ExplorerOptions? options)
    {
        if (options == null)
        {
            return;
        }

        if (options.Moves.HasValue)
        {
            sb.Append("&moves=");
            sb.Append(options.Moves.Value);
        }

        if (options.TopGames.HasValue)
        {
            sb.Append("&topGames=");
            sb.Append(options.TopGames.Value);
        }

        if (options.RecentGames.HasValue)
        {
            sb.Append("&recentGames=");
            sb.Append(options.RecentGames.Value);
        }
    }

    private static void AppendMastersOptions(StringBuilder sb, ExplorerOptions? options)
    {
        if (options == null)
        {
            return;
        }

        if (options.Since.HasValue)
        {
            sb.Append("&since=");
            sb.Append(options.Since.Value);
        }

        if (options.Until.HasValue)
        {
            sb.Append("&until=");
            sb.Append(options.Until.Value);
        }
    }

    private static void AppendLichessOptions(StringBuilder sb, ExplorerOptions? options)
    {
        if (options == null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(options.Variant))
        {
            sb.Append("&variant=");
            sb.Append(Uri.EscapeDataString(options.Variant));
        }

        if (options.Speeds is { Length: > 0 })
        {
            sb.Append("&speeds=");
            sb.Append(string.Join(",", options.Speeds));
        }

        if (options.Ratings is { Length: > 0 })
        {
            sb.Append("&ratings=");
            sb.Append(string.Join(",", options.Ratings));
        }

        if (!string.IsNullOrWhiteSpace(options.SinceMonth))
        {
            sb.Append("&since=");
            sb.Append(Uri.EscapeDataString(options.SinceMonth));
        }

        if (!string.IsNullOrWhiteSpace(options.UntilMonth))
        {
            sb.Append("&until=");
            sb.Append(Uri.EscapeDataString(options.UntilMonth));
        }
    }
}
