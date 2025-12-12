using System.Runtime.CompilerServices;
using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models.Games;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the TV API.
/// </summary>
internal sealed class TvApi(ILichessHttpClient httpClient) : ITvApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<TvChannels> GetCurrentGamesAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<TvChannels>("/api/tv/channels", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TvFeedEvent> StreamCurrentGameAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var feedEvent in _httpClient.StreamNdjsonAsync<TvFeedEvent>("/api/tv/feed", cancellationToken).ConfigureAwait(false))
        {
            yield return feedEvent;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TvFeedEvent> StreamChannelAsync(string channel, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);

        var endpoint = $"/api/tv/{Uri.EscapeDataString(channel)}/feed";
        await foreach (var feedEvent in _httpClient.StreamNdjsonAsync<TvFeedEvent>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return feedEvent;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<GameJson> StreamChannelGamesAsync(string channel, TvChannelGamesOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);

        if (options?.Count is < 1 or > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Count must be between 1 and 30.");
        }

        var endpoint = BuildChannelGamesEndpoint(channel, options);
        await foreach (var game in _httpClient.StreamNdjsonAsync<GameJson>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return game;
        }
    }

    private static string BuildChannelGamesEndpoint(string channel, TvChannelGamesOptions? options)
    {
        var sb = new StringBuilder();
        sb.Append("/api/tv/");
        sb.Append(Uri.EscapeDataString(channel));

        if (options == null)
        {
            return sb.ToString();
        }

        var hasQuery = false;

        if (options.Count.HasValue)
        {
            sb.Append('?');
            sb.Append("nb=");
            sb.Append(options.Count.Value);
            hasQuery = true;
        }

        AppendBoolParam(sb, "moves", options.Moves, ref hasQuery);
        AppendBoolParam(sb, "pgnInJson", options.PgnInJson, ref hasQuery);
        AppendBoolParam(sb, "tags", options.Tags, ref hasQuery);
        AppendBoolParam(sb, "clocks", options.Clocks, ref hasQuery);
        AppendBoolParam(sb, "evals", options.Evals, ref hasQuery);
        AppendBoolParam(sb, "opening", options.Opening, ref hasQuery);

        return sb.ToString();
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
}
