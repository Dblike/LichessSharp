using System.Runtime.CompilerServices;
using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Board API.
/// </summary>
internal sealed class BoardApi(ILichessHttpClient httpClient) : IBoardApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async IAsyncEnumerable<BoardAccountEvent> StreamEventsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var evt in _httpClient.StreamNdjsonAsync<BoardAccountEvent>("/api/stream/event", cancellationToken).ConfigureAwait(false))
        {
            yield return evt;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BoardGameEvent> StreamGameAsync(string gameId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/board/game/stream/{Uri.EscapeDataString(gameId)}";
        await foreach (var evt in _httpClient.StreamNdjsonAsync<BoardGameEvent>(endpoint, cancellationToken).ConfigureAwait(false))
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
        sb.Append("/api/board/game/");
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

        var endpoint = $"/api/board/game/{Uri.EscapeDataString(gameId)}/chat";
        return await _httpClient.GetAsync<List<ChatMessage>>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> WriteChatAsync(string gameId, ChatRoom room, string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var endpoint = $"/api/board/game/{Uri.EscapeDataString(gameId)}/chat";
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("room", room == ChatRoom.Spectator ? "spectator" : "player"),
            new KeyValuePair<string, string>("text", text)
        ]);

        await _httpClient.PostAsync<OkResponse>(endpoint, content, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> AbortAsync(string gameId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/board/game/{Uri.EscapeDataString(gameId)}/abort";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ResignAsync(string gameId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/board/game/{Uri.EscapeDataString(gameId)}/resign";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> HandleDrawAsync(string gameId, bool accept, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/board/game/{Uri.EscapeDataString(gameId)}/draw/{(accept ? "yes" : "no")}";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> HandleTakebackAsync(string gameId, bool accept, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/board/game/{Uri.EscapeDataString(gameId)}/takeback/{(accept ? "yes" : "no")}";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ClaimVictoryAsync(string gameId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/board/game/{Uri.EscapeDataString(gameId)}/claim-victory";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ClaimDrawAsync(string gameId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/board/game/{Uri.EscapeDataString(gameId)}/claim-draw";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> BerserkAsync(string gameId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var endpoint = $"/api/board/game/{Uri.EscapeDataString(gameId)}/berserk";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<SeekResult> SeekAsync(SeekOptions options, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var content = BuildSeekContent(options);
        await foreach (var result in _httpClient.StreamNdjsonPostAsync<SeekResult>("/api/board/seek", content, cancellationToken).ConfigureAwait(false))
        {
            yield return result;
        }
    }


    private static FormUrlEncodedContent BuildSeekContent(SeekOptions options)
    {
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("rated", options.Rated.ToString().ToLowerInvariant()),
            new("time", options.Time.ToString()),
            new("increment", options.Increment.ToString())
        };

        if (options.Days.HasValue)
        {
            parameters.Add(new("days", options.Days.Value.ToString()));
        }
        if (!string.IsNullOrEmpty(options.Variant))
        {
            parameters.Add(new("variant", options.Variant));
        }
        if (options.Color.HasValue)
        {
            parameters.Add(new("color", GetColorString(options.Color.Value)));
        }
        if (options.RatingMin.HasValue)
        {
            parameters.Add(new("ratingRange", $"{options.RatingMin.Value}-{options.RatingMax ?? 9999}"));
        }
        else if (options.RatingMax.HasValue)
        {
            parameters.Add(new("ratingRange", $"0-{options.RatingMax.Value}"));
        }

        return new FormUrlEncodedContent(parameters);
    }

    private static string GetColorString(ChallengeColor color) => color switch
    {
        ChallengeColor.White => "white",
        ChallengeColor.Black => "black",
        _ => "random"
    };

}
