using System.Text.Json.Serialization;
using LichessSharp.Models.Enums;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models.Games;

/// <summary>
///     Response from the ongoing games endpoint.
/// </summary>
[ResponseOnly]
public class OngoingGamesResponse
{
    /// <summary>
    ///     List of games currently being played.
    /// </summary>
    [JsonPropertyName("nowPlaying")]
    public IReadOnlyList<OngoingGame>? NowPlaying { get; init; }
}

/// <summary>
///     An ongoing game.
/// </summary>
[ResponseOnly]
public class OngoingGame
{
    /// <summary>
    ///     Full game ID (includes player color suffix).
    /// </summary>
    [JsonPropertyName("fullId")]
    public required string FullId { get; init; }

    /// <summary>
    ///     Game ID (8 characters).
    /// </summary>
    [JsonPropertyName("gameId")]
    public required string GameId { get; init; }

    /// <summary>
    ///     Current FEN position.
    /// </summary>
    [JsonPropertyName("fen")]
    public required string Fen { get; init; }

    /// <summary>
    ///     The color you are playing.
    /// </summary>
    [JsonPropertyName("color")]
    public Color Color { get; init; }

    /// <summary>
    ///     Last move in UCI notation.
    /// </summary>
    [JsonPropertyName("lastMove")]
    public string? LastMove { get; init; }

    /// <summary>
    ///     Game source.
    /// </summary>
    /// TODO: Can this be enum?
    [JsonPropertyName("source")]
    public string? Source { get; init; }

    /// <summary>
    ///     Game status.
    /// </summary>
    [JsonPropertyName("status")]
    public GameStatus Status { get; init; }

    /// <summary>
    ///     Variant.
    /// </summary>
    [JsonPropertyName("variant")]
    [JsonConverter(typeof(NullableVariantObjectConverter))]
    public Variant? Variant { get; init; }

    /// <summary>
    ///     Speed category.
    /// </summary>
    [JsonPropertyName("speed")]
    public Speed Speed { get; init; }

    /// <summary>
    ///     Whether the game is rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    ///     Whether the player has moved.
    /// </summary>
    [JsonPropertyName("hasMoved")]
    public bool HasMoved { get; init; }

    /// <summary>
    ///     Opponent information.
    /// </summary>
    [JsonPropertyName("opponent")]
    public OngoingGameOpponent? Opponent { get; init; }

    /// <summary>
    ///     Whether it's your turn.
    /// </summary>
    [JsonPropertyName("isMyTurn")]
    public bool IsMyTurn { get; init; }

    /// <summary>
    ///     Seconds left on your clock.
    /// </summary>
    [JsonPropertyName("secondsLeft")]
    public int? SecondsLeft { get; init; }
}

/// <summary>
///     Opponent in an ongoing game.
/// </summary>
[ResponseOnly]
public class OngoingGameOpponent
{
    /// <summary>
    ///     Opponent's user ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Opponent's username.
    /// </summary>
    [JsonPropertyName("username")]
    public required string Username { get; init; }

    /// <summary>
    ///     Opponent's rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    ///     Rating difference after the game.
    /// </summary>
    [JsonPropertyName("ratingDiff")]
    public int? RatingDiff { get; init; }

    /// <summary>
    ///     AI level if playing against AI.
    /// </summary>
    [JsonPropertyName("ai")]
    public int? Ai { get; init; }
}