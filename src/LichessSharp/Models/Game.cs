using System.Text.Json.Serialization;
using LichessSharp.Models.Enums;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models;

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
/// Game players.
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
    public User? User { get; init; }

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
/// Player analysis statistics.
/// </summary>
public class PlayerAnalysis
{
    /// <summary>
    /// Number of inaccuracies.
    /// </summary>
    [JsonPropertyName("inaccuracy")]
    public int Inaccuracy { get; init; }

    /// <summary>
    /// Number of mistakes.
    /// </summary>
    [JsonPropertyName("mistake")]
    public int Mistake { get; init; }

    /// <summary>
    /// Number of blunders.
    /// </summary>
    [JsonPropertyName("blunder")]
    public int Blunder { get; init; }

    /// <summary>
    /// Average centipawn loss.
    /// </summary>
    [JsonPropertyName("acpl")]
    public int Acpl { get; init; }

    /// <summary>
    /// Accuracy percentage.
    /// </summary>
    [JsonPropertyName("accuracy")]
    public int? Accuracy { get; init; }
}

/// <summary>
/// Opening information.
/// </summary>
public class Opening
{
    /// <summary>
    /// ECO code.
    /// </summary>
    [JsonPropertyName("eco")]
    public string? Eco { get; init; }

    /// <summary>
    /// Opening name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Number of plies in the opening.
    /// </summary>
    [JsonPropertyName("ply")]
    public int? Ply { get; init; }
}

/// <summary>
/// Clock settings.
/// </summary>
public class Clock
{
    /// <summary>
    /// Initial time in seconds.
    /// </summary>
    [JsonPropertyName("initial")]
    public int Initial { get; init; }

    /// <summary>
    /// Increment per move in seconds.
    /// </summary>
    [JsonPropertyName("increment")]
    public int Increment { get; init; }

    /// <summary>
    /// Total time in seconds (for display).
    /// </summary>
    [JsonPropertyName("totalTime")]
    public int? TotalTime { get; init; }
}

/// <summary>
/// Analysis evaluation for a position.
/// </summary>
public class Analysis
{
    /// <summary>
    /// Evaluation in centipawns.
    /// </summary>
    [JsonPropertyName("eval")]
    public int? Eval { get; init; }

    /// <summary>
    /// Mate in N moves (positive for white, negative for black).
    /// </summary>
    [JsonPropertyName("mate")]
    public int? Mate { get; init; }

    /// <summary>
    /// Best move in UCI notation.
    /// </summary>
    [JsonPropertyName("best")]
    public string? Best { get; init; }

    /// <summary>
    /// Principal variation.
    /// </summary>
    [JsonPropertyName("variation")]
    public string? Variation { get; init; }

    /// <summary>
    /// Judgment on the move quality.
    /// </summary>
    [JsonPropertyName("judgment")]
    public Judgment? Judgment { get; init; }
}

/// <summary>
/// Move quality judgment.
/// </summary>
public class Judgment
{
    /// <summary>
    /// Judgment name (Inaccuracy, Mistake, Blunder).
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Comment about the judgment.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; init; }
}
