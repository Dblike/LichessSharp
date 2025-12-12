using System.Text.Json.Serialization;
using LichessSharp.Models.Common;
using LichessSharp.Models.Enums;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models.Games;

/// <summary>
/// Basic game information.
/// </summary>
public class Game
{
    /// <summary>
    /// The game's unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Whether the game is rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    [JsonPropertyName("variant")]
    public Variant Variant { get; init; }

    /// <summary>
    /// The time control speed.
    /// </summary>
    [JsonPropertyName("speed")]
    public Speed Speed { get; init; }

    /// <summary>
    /// The performance type (game mode).
    /// </summary>
    /// TODO: Check if this is any different from Variant/Speed
    [JsonPropertyName("perf")]
    public string? Perf { get; init; }

    /// <summary>
    /// When the game was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the last move was played.
    /// </summary>
    [JsonPropertyName("lastMoveAt")]
    [JsonConverter(typeof(NullableUnixMillisecondsConverter))]
    public DateTimeOffset? LastMoveAt { get; init; }

    /// <summary>
    /// The game status.
    /// </summary>
    [JsonPropertyName("status")]
    public GameStatus Status { get; init; }

    /// <summary>
    /// The white player.
    /// </summary>
    [JsonPropertyName("players")]
    public GamePlayers? Players { get; init; }

    /// <summary>
    /// The winner color, if the game is finished.
    /// </summary>
    [JsonPropertyName("winner")]
    public Color? Winner { get; init; }

    /// <summary>
    /// The opening information.
    /// </summary>
    [JsonPropertyName("opening")]
    public Opening? Opening { get; init; }

    /// <summary>
    /// The moves in UCI notation, space-separated.
    /// </summary>
    [JsonPropertyName("moves")]
    public string? Moves { get; init; }

    /// <summary>
    /// Clock times in centiseconds after each move.
    /// </summary>
    [JsonPropertyName("clocks")]
    public int[]? Clocks { get; init; }
}

/// <summary>
/// Full game information in JSON format.
/// </summary>
public class GameJson : Game
{
    /// <summary>
    /// The clock settings.
    /// </summary>
    [JsonPropertyName("clock")]
    public Clock? Clock { get; init; }

    /// <summary>
    /// The full PGN of the game.
    /// </summary>
    [JsonPropertyName("pgn")]
    public string? Pgn { get; init; }

    /// <summary>
    /// Analysis evaluations.
    /// </summary>
    [JsonPropertyName("analysis")]
    public Analysis[]? Analysis { get; init; }
}

/// <summary>
/// Game players container.
/// </summary>
public class GamePlayers
{
    /// <summary>
    /// The white player.
    /// </summary>
    [JsonPropertyName("white")]
    public GamePlayer? White { get; init; }

    /// <summary>
    /// The black player.
    /// </summary>
    [JsonPropertyName("black")]
    public GamePlayer? Black { get; init; }
}

/// <summary>
/// A player in a game.
/// </summary>
public class GamePlayer
{
    /// <summary>
    /// The player's user information.
    /// </summary>
    [JsonPropertyName("user")]
    public LightUser? User { get; init; }

    /// <summary>
    /// The player's rating at the start of the game.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    /// Rating change after the game.
    /// </summary>
    [JsonPropertyName("ratingDiff")]
    public int? RatingDiff { get; init; }

    /// <summary>
    /// AI level if playing against AI.
    /// </summary>
    [JsonPropertyName("aiLevel")]
    public int? AiLevel { get; init; }

    /// <summary>
    /// Whether the player offered a draw.
    /// </summary>
    [JsonPropertyName("offeringDraw")]
    public bool? OfferingDraw { get; init; }

    /// <summary>
    /// Whether the player is proposing a takeback.
    /// </summary>
    [JsonPropertyName("proposingTakeback")]
    public bool? ProposingTakeback { get; init; }

    /// <summary>
    /// Analysis accuracy percentage.
    /// </summary>
    [JsonPropertyName("analysis")]
    public PlayerAnalysis? Analysis { get; init; }
}

/// <summary>
/// Variant information object containing key and display name.
/// Maps to the Variant schema in the Lichess API.
/// </summary>
public class VariantInfo
{
    /// <summary>
    /// Variant key (e.g., standard, chess960, crazyhouse).
    /// </summary>
    [JsonPropertyName("key")]
    public Variant Key { get; init; }

    /// <summary>
    /// Human-readable variant name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Short name for the variant.
    /// </summary>
    [JsonPropertyName("short")]
    public string? Short { get; init; }
}
