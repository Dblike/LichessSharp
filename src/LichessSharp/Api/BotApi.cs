using System.Runtime.CompilerServices;
using System.Text;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Bot API.
/// </summary>
internal sealed class BotApi : IBotApi
{
    private readonly ILichessHttpClient _httpClient;

    public BotApi(ILichessHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<bool> UpgradeAccountAsync(CancellationToken cancellationToken = default)
    {
        await _httpClient.PostAsync<OkResponse>("/api/bot/account/upgrade", null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BotAccountEvent> StreamEventsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var evt in _httpClient.StreamNdjsonAsync<BotAccountEvent>("/api/stream/event", cancellationToken).ConfigureAwait(false))
        {
            yield return evt;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BotGameEvent> StreamGameAsync(string gameId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/bot/game/stream/{Uri.EscapeDataString(gameId)}";
        await foreach (var evt in _httpClient.StreamNdjsonAsync<BotGameEvent>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return evt;
        }
    }

    /// <inheritdoc />
    public async Task<bool> MakeMoveAsync(string gameId, string move, bool? offeringDraw = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);
        ArgumentException.ThrowIfNullOrWhiteSpace(move);

        var sb = new StringBuilder();
        sb.Append("/api/bot/game/");
        sb.Append(Uri.EscapeDataString(gameId));
        sb.Append("/move/");
        sb.Append(Uri.EscapeDataString(move));

        if (offeringDraw.HasValue)
        {
            sb.Append("?offeringDraw=");
            sb.Append(offeringDraw.Value.ToString().ToLowerInvariant());
        }

        await _httpClient.PostAsync<OkResponse>(sb.ToString(), null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChatMessage>> GetChatAsync(string gameId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/bot/game/{Uri.EscapeDataString(gameId)}/chat";
        return await _httpClient.GetAsync<List<ChatMessage>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> WriteChatAsync(string gameId, ChatRoom room, string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var endpoint = $"/api/bot/game/{Uri.EscapeDataString(gameId)}/chat";
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("room", room == ChatRoom.Spectator ? "spectator" : "player"),
            new KeyValuePair<string, string>("text", text)
        });

        await _httpClient.PostAsync<OkResponse>(endpoint, content, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> AbortAsync(string gameId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/bot/game/{Uri.EscapeDataString(gameId)}/abort";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ResignAsync(string gameId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/bot/game/{Uri.EscapeDataString(gameId)}/resign";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> HandleDrawAsync(string gameId, bool accept, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/bot/game/{Uri.EscapeDataString(gameId)}/draw/{(accept ? "yes" : "no")}";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> HandleTakebackAsync(string gameId, bool accept, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/bot/game/{Uri.EscapeDataString(gameId)}/takeback/{(accept ? "yes" : "no")}";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BotUser> GetOnlineBotsAsync(int? count = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (count is < 1 or > 300)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be between 1 and 300.");
        }

        var endpoint = "/api/bot/online";
        if (count.HasValue)
        {
            endpoint += $"?nb={count.Value}";
        }

        await foreach (var bot in _httpClient.StreamNdjsonAsync<BotUser>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return bot;
        }
    }
}
