using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// TV API - Access Lichess TV channels and games.
/// </summary>
public interface ITvApi
{
    /// <summary>
    /// Get current TV games for all channels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>TV games by channel.</returns>
    Task<Dictionary<string, TvGame>> GetCurrentGamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream the current TV game.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of TV feed events.</returns>
    IAsyncEnumerable<TvFeedEvent> StreamCurrentGameAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream the current game of a TV channel.
    /// </summary>
    /// <param name="channel">The channel name (e.g., "bullet", "blitz").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of TV feed events.</returns>
    IAsyncEnumerable<TvFeedEvent> StreamChannelAsync(string channel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the best ongoing games of a TV channel.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    /// <param name="count">Number of games (1-30, default 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of games.</returns>
    IAsyncEnumerable<GameJson> GetChannelGamesAsync(string channel, int count = 10, CancellationToken cancellationToken = default);
}

/// <summary>
/// A TV game summary.
/// </summary>
public class TvGame
{
    /// <summary>
    /// The game ID.
    /// </summary>
    public required string GameId { get; init; }

    /// <summary>
    /// The game URL.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// The FEN position.
    /// </summary>
    public string? Fen { get; init; }

    /// <summary>
    /// The white player.
    /// </summary>
    public TvPlayer? White { get; init; }

    /// <summary>
    /// The black player.
    /// </summary>
    public TvPlayer? Black { get; init; }
}

/// <summary>
/// A TV game player.
/// </summary>
public class TvPlayer
{
    /// <summary>
    /// The user ID.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// The rating.
    /// </summary>
    public int? Rating { get; init; }
}

/// <summary>
/// TV feed event.
/// </summary>
public class TvFeedEvent
{
    /// <summary>
    /// The event type.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// The FEN position.
    /// </summary>
    public string? Fen { get; init; }

    /// <summary>
    /// The last move.
    /// </summary>
    public string? Lm { get; init; }

    /// <summary>
    /// White clock time in seconds.
    /// </summary>
    public int? Wc { get; init; }

    /// <summary>
    /// Black clock time in seconds.
    /// </summary>
    public int? Bc { get; init; }
}
