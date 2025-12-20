using System.Text.Json.Serialization;
using LichessSharp.Models.Common;
using LichessSharp.Models.Enums;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models.Games;

/// <summary>
///     Basic game information.
/// </summary>
[ResponseOnly]
public class Game
{
    /// <summary>
    ///     The game's unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Whether the game is rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    ///     The chess variant.
    /// </summary>
    [JsonPropertyName("variant")]
    public Variant Variant { get; init; }

    /// <summary>
    ///     The time control speed.
    /// </summary>
    [JsonPropertyName("speed")]
    public Speed Speed { get; init; }

    /// <summary>
    ///     When the game was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    ///     When the last move was played.
    /// </summary>
    [JsonPropertyName("lastMoveAt")]
    [JsonConverter(typeof(NullableUnixMillisecondsConverter))]
    public DateTimeOffset? LastMoveAt { get; init; }

    /// <summary>
    ///     The game status.
    /// </summary>
    [JsonPropertyName("status")]
    public GameStatus Status { get; init; }

    /// <summary>
    ///     The white player.
    /// </summary>
    [JsonPropertyName("players")]
    public GamePlayers? Players { get; init; }

    /// <summary>
    ///     The winner color, if the game is finished.
    /// </summary>
    [JsonPropertyName("winner")]
    public Color? Winner { get; init; }

    /// <summary>
    ///     The opening information.
    /// </summary>
    [JsonPropertyName("opening")]
    public Opening? Opening { get; init; }

    /// <summary>
    ///     The moves in UCI notation, space-separated.
    /// </summary>
    [JsonPropertyName("moves")]
    public string? Moves { get; init; }

    /// <summary>
    ///     Clock times in centiseconds after each move.
    /// </summary>
    [JsonPropertyName("clocks")]
    public int[]? Clocks { get; init; }
}

/// <summary>
///     Full game information in JSON format.
/// </summary>
[ResponseOnly]
public class GameJson : Game
{
    /// <summary>
    ///     The clock settings.
    /// </summary>
    [JsonPropertyName("clock")]
    public Clock? Clock { get; init; }

    /// <summary>
    ///     The full PGN of the game.
    /// </summary>
    [JsonPropertyName("pgn")]
    public string? Pgn { get; init; }

    /// <summary>
    ///     Analysis evaluations.
    /// </summary>
    [JsonPropertyName("analysis")]
    public Analysis[]? Analysis { get; init; }

    /// <summary>
    ///     The source of the game (e.g., "pool", "friend", "ai", "arena", "swiss").
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; init; }

    /// <summary>
    ///     The initial FEN position, if not the standard starting position.
    /// </summary>
    [JsonPropertyName("initialFen")]
    public string? InitialFen { get; init; }

    /// <summary>
    ///     Days per turn for correspondence games.
    /// </summary>
    [JsonPropertyName("daysPerTurn")]
    public int? DaysPerTurn { get; init; }

    /// <summary>
    ///     The tournament ID if this game is part of an arena tournament.
    /// </summary>
    [JsonPropertyName("tournament")]
    public string? Tournament { get; init; }

    /// <summary>
    ///     The Swiss tournament ID if this game is part of a Swiss tournament.
    /// </summary>
    [JsonPropertyName("swiss")]
    public string? Swiss { get; init; }

    /// <summary>
    ///     Game division information (middle game and endgame ply markers).
    /// </summary>
    [JsonPropertyName("division")]
    public GameDivision? Division { get; init; }
}

/// <summary>
///     Game division information indicating where middle game and endgame begin.
/// </summary>
[ResponseOnly]
public class GameDivision
{
    /// <summary>
    ///     The ply at which the middle game starts.
    /// </summary>
    [JsonPropertyName("middle")]
    public int? Middle { get; init; }

    /// <summary>
    ///     The ply at which the endgame starts.
    /// </summary>
    [JsonPropertyName("end")]
    public int? End { get; init; }
}

/// <summary>
///     Game players container.
/// </summary>
[ResponseOnly]
public class GamePlayers
{
    /// <summary>
    ///     The white player.
    /// </summary>
    [JsonPropertyName("white")]
    public GamePlayer? White { get; init; }

    /// <summary>
    ///     The black player.
    /// </summary>
    [JsonPropertyName("black")]
    public GamePlayer? Black { get; init; }
}

/// <summary>
///     A player in a game.
/// </summary>
[ResponseOnly]
public class GamePlayer
{
    /// <summary>
    ///     The player's user information.
    /// </summary>
    [JsonPropertyName("user")]
    public LightUser? User { get; init; }

    /// <summary>
    ///     The player's rating at the start of the game.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    ///     Rating change after the game.
    /// </summary>
    [JsonPropertyName("ratingDiff")]
    public int? RatingDiff { get; init; }

    /// <summary>
    ///     AI level if playing against AI.
    /// </summary>
    [JsonPropertyName("aiLevel")]
    public int? AiLevel { get; init; }

    /// <summary>
    ///     Whether the player offered a draw.
    /// </summary>
    [JsonPropertyName("offeringDraw")]
    public bool? OfferingDraw { get; init; }

    /// <summary>
    ///     Whether the player is proposing a takeback.
    /// </summary>
    [JsonPropertyName("proposingTakeback")]
    public bool? ProposingTakeback { get; init; }

    /// <summary>
    ///     Analysis accuracy percentage.
    /// </summary>
    [JsonPropertyName("analysis")]
    public PlayerAnalysis? Analysis { get; init; }
}