using System.Text.Json.Serialization;

using LichessSharp.Models;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// TV API - Access Lichess TV channels and games.
/// </summary>
public interface ITvApi
{
    /// <summary>
    /// Get current TV games for all channels.
    /// See <see href="https://lichess.org/tv"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>TV games by channel name (e.g., "bullet", "blitz", "rapid", "best").</returns>
    Task<TvChannels> GetCurrentGamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream positions and moves of the current TV game in real-time.
    /// The stream sends a "featured" event when a new game starts, followed by "fen" events for each move.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of TV feed events.</returns>
    IAsyncEnumerable<TvFeedEvent> StreamCurrentGameAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream positions and moves of a specific TV channel's current game in real-time.
    /// </summary>
    /// <param name="channel">The channel name in camelCase (e.g., "bullet", "blitz", "rapid", "classical", "ultraBullet", "chess960", "crazyhouse", "antichess", "atomic", "horde", "kingOfTheHill", "racingKings", "threeCheck", "bot", "computer").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of TV feed events.</returns>
    IAsyncEnumerable<TvFeedEvent> StreamChannelAsync(string channel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the best ongoing games of a TV channel.
    /// </summary>
    /// <param name="channel">The channel name in camelCase.</param>
    /// <param name="options">Export options for the games.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of games in ndjson format.</returns>
    IAsyncEnumerable<GameJson> StreamChannelGamesAsync(string channel, TvChannelGamesOptions? options = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Options for getting TV channel games.
/// </summary>
public class TvChannelGamesOptions
{
    /// <summary>
    /// Number of games to fetch (1-30, default 10).
    /// </summary>
    public int? Count { get; init; }

    /// <summary>
    /// Include the PGN moves.
    /// </summary>
    public bool? Moves { get; init; }

    /// <summary>
    /// Include the full PGN within the JSON response, in a pgn field.
    /// </summary>
    public bool? PgnInJson { get; init; }

    /// <summary>
    /// Include the PGN tags.
    /// </summary>
    public bool? Tags { get; init; }

    /// <summary>
    /// Include clock status when available.
    /// </summary>
    public bool? Clocks { get; init; }

    /// <summary>
    /// Include analysis evaluations when available.
    /// </summary>
    public bool? Evals { get; init; }

    /// <summary>
    /// Include opening information.
    /// </summary>
    public bool? Opening { get; init; }
}

/// <summary>
/// Contains current TV games for all channels.
/// </summary>
public class TvChannels
{
    /// <summary>
    /// The best game currently being played (featured game).
    /// </summary>
    [JsonPropertyName("best")]
    public TvGame? Best { get; init; }

    /// <summary>
    /// Best Ultra Bullet game.
    /// </summary>
    [JsonPropertyName("ultraBullet")]
    public TvGame? UltraBullet { get; init; }

    /// <summary>
    /// Best Bullet game.
    /// </summary>
    [JsonPropertyName("bullet")]
    public TvGame? Bullet { get; init; }

    /// <summary>
    /// Best Blitz game.
    /// </summary>
    [JsonPropertyName("blitz")]
    public TvGame? Blitz { get; init; }

    /// <summary>
    /// Best Rapid game.
    /// </summary>
    [JsonPropertyName("rapid")]
    public TvGame? Rapid { get; init; }

    /// <summary>
    /// Best Classical game.
    /// </summary>
    [JsonPropertyName("classical")]
    public TvGame? Classical { get; init; }

    /// <summary>
    /// Best Chess960 game.
    /// </summary>
    [JsonPropertyName("chess960")]
    public TvGame? Chess960 { get; init; }

    /// <summary>
    /// Best Crazyhouse game.
    /// </summary>
    [JsonPropertyName("crazyhouse")]
    public TvGame? Crazyhouse { get; init; }

    /// <summary>
    /// Best Antichess game.
    /// </summary>
    [JsonPropertyName("antichess")]
    public TvGame? Antichess { get; init; }

    /// <summary>
    /// Best Atomic game.
    /// </summary>
    [JsonPropertyName("atomic")]
    public TvGame? Atomic { get; init; }

    /// <summary>
    /// Best Horde game.
    /// </summary>
    [JsonPropertyName("horde")]
    public TvGame? Horde { get; init; }

    /// <summary>
    /// Best King of the Hill game.
    /// </summary>
    [JsonPropertyName("kingOfTheHill")]
    public TvGame? KingOfTheHill { get; init; }

    /// <summary>
    /// Best Racing Kings game.
    /// </summary>
    [JsonPropertyName("racingKings")]
    public TvGame? RacingKings { get; init; }

    /// <summary>
    /// Best Three-check game.
    /// </summary>
    [JsonPropertyName("threeCheck")]
    public TvGame? ThreeCheck { get; init; }

    /// <summary>
    /// Best bot game.
    /// </summary>
    [JsonPropertyName("bot")]
    public TvGame? Bot { get; init; }

    /// <summary>
    /// Best computer analysis game.
    /// </summary>
    [JsonPropertyName("computer")]
    public TvGame? Computer { get; init; }
}

/// <summary>
/// A TV game summary for a channel.
/// </summary>
public class TvGame
{
    /// <summary>
    /// The featured player's user info.
    /// </summary>
    [JsonPropertyName("user")]
    public TvUser? User { get; init; }

    /// <summary>
    /// The featured player's rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    /// The game ID.
    /// </summary>
    [JsonPropertyName("gameId")]
    public string? GameId { get; init; }

    /// <summary>
    /// The featured player's color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }
}

/// <summary>
/// A user in TV game context.
/// </summary>
public class TvUser
{
    /// <summary>
    /// The user ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// The username.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// The user's title (GM, IM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }
}

/// <summary>
/// TV feed event - represents updates from a TV game stream.
/// Can be either a "featured" event (new game) or a "fen" event (position update).
/// </summary>
public class TvFeedEvent
{
    /// <summary>
    /// The event type: "featured" for new game, "fen" for position update.
    /// </summary>
    [JsonPropertyName("t")]
    public string? Type { get; init; }

    /// <summary>
    /// The event data.
    /// </summary>
    [JsonPropertyName("d")]
    public TvFeedData? Data { get; init; }
}

/// <summary>
/// Data payload for TV feed events.
/// For "featured" events, contains full game info.
/// For "fen" events, contains position update.
/// </summary>
public class TvFeedData
{
    /// <summary>
    /// The game ID (featured events only).
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Board orientation color (featured events only).
    /// </summary>
    [JsonPropertyName("orientation")]
    public string? Orientation { get; init; }

    /// <summary>
    /// The players in the game (featured events only).
    /// </summary>
    [JsonPropertyName("players")]
    public IReadOnlyList<TvFeedPlayer>? Players { get; init; }

    /// <summary>
    /// The current FEN position.
    /// </summary>
    [JsonPropertyName("fen")]
    public string? Fen { get; init; }

    /// <summary>
    /// The last move in UCI format (fen events only).
    /// </summary>
    [JsonPropertyName("lm")]
    public string? LastMove { get; init; }

    /// <summary>
    /// White's clock time in seconds (fen events only).
    /// </summary>
    [JsonPropertyName("wc")]
    public int? WhiteClock { get; init; }

    /// <summary>
    /// Black's clock time in seconds (fen events only).
    /// </summary>
    [JsonPropertyName("bc")]
    public int? BlackClock { get; init; }
}

/// <summary>
/// A player in a TV feed featured event.
/// </summary>
public class TvFeedPlayer
{
    /// <summary>
    /// The player's color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    /// The player's user info.
    /// </summary>
    [JsonPropertyName("user")]
    public TvUser? User { get; init; }

    /// <summary>
    /// The player's rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    /// The player's remaining time in seconds.
    /// </summary>
    [JsonPropertyName("seconds")]
    public int? Seconds { get; init; }
}
