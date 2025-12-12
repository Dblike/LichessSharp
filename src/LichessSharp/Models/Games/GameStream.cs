using System.Text.Json.Serialization;
using LichessSharp.Models.Common;
using LichessSharp.Models.Enums;

namespace LichessSharp.Models.Games;

/// <summary>
/// Event from streaming a single game's moves via /api/stream/game/{id}.
/// The first event contains full game info, subsequent events contain move updates.
/// </summary>
public class MoveStreamEvent
{
    /// <summary>
    /// Game ID (first event only).
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Variant information (first event only).
    /// </summary>
    [JsonPropertyName("variant")]
    public VariantInfo? Variant { get; init; }

    /// <summary>
    /// Game speed (first event only).
    /// </summary>
    [JsonPropertyName("speed")]
    public string? Speed { get; init; }

    /// <summary>
    /// Performance type (first event only).
    /// </summary>
    [JsonPropertyName("perf")]
    public string? Perf { get; init; }

    /// <summary>
    /// Whether the game is rated (first event only).
    /// </summary>
    [JsonPropertyName("rated")]
    public bool? Rated { get; init; }

    /// <summary>
    /// Initial FEN position (first event only).
    /// </summary>
    [JsonPropertyName("initialFen")]
    public string? InitialFen { get; init; }

    /// <summary>
    /// Current FEN position.
    /// </summary>
    [JsonPropertyName("fen")]
    public string? Fen { get; init; }

    /// <summary>
    /// Color to play: "white" or "black" (first event only).
    /// </summary>
    [JsonPropertyName("player")]
    public string? Player { get; init; }

    /// <summary>
    /// Number of half-moves played (first event only).
    /// </summary>
    [JsonPropertyName("turns")]
    public int? Turns { get; init; }

    /// <summary>
    /// Ply at which the game started (first event only).
    /// </summary>
    [JsonPropertyName("startedAtTurn")]
    public int? StartedAtTurn { get; init; }

    /// <summary>
    /// Game source (first event only).
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; init; }

    /// <summary>
    /// Game status (first event only).
    /// </summary>
    [JsonPropertyName("status")]
    public GameStatus? Status { get; init; }

    /// <summary>
    /// Timestamp when the game was created (first event only).
    /// </summary>
    [JsonPropertyName("createdAt")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Last move in UCI format (first event only).
    /// </summary>
    [JsonPropertyName("lastMove")]
    public string? LastMove { get; init; }

    /// <summary>
    /// Player information (first event only).
    /// </summary>
    [JsonPropertyName("players")]
    public GamePlayers? Players { get; init; }

    /// <summary>
    /// Last move in UCI format (move update events).
    /// </summary>
    [JsonPropertyName("lm")]
    public string? Lm { get; init; }

    /// <summary>
    /// White clock time in seconds (move update events).
    /// </summary>
    [JsonPropertyName("wc")]
    public int? Wc { get; init; }

    /// <summary>
    /// Black clock time in seconds (move update events).
    /// </summary>
    [JsonPropertyName("bc")]
    public int? Bc { get; init; }
}

/// <summary>
/// Event from streaming multiple games via /api/stream/games/{streamId}.
/// Events are emitted when watched games start or finish.
/// </summary>
public class GameStreamEvent
{
    /// <summary>
    /// Game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Whether the game is rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool? Rated { get; init; }

    /// <summary>
    /// Variant key.
    /// </summary>
    [JsonPropertyName("variant")]
    public string? Variant { get; init; }

    /// <summary>
    /// Game speed.
    /// </summary>
    [JsonPropertyName("speed")]
    public string? Speed { get; init; }

    /// <summary>
    /// Performance type.
    /// </summary>
    [JsonPropertyName("perf")]
    public string? Perf { get; init; }

    /// <summary>
    /// Timestamp when the game was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Game status ID.
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; init; }

    /// <summary>
    /// Game status name (e.g., "started", "resign", "mate").
    /// </summary>
    [JsonPropertyName("statusName")]
    public string? StatusName { get; init; }

    /// <summary>
    /// Clock information.
    /// </summary>
    [JsonPropertyName("clock")]
    public Clock? Clock { get; init; }

    /// <summary>
    /// Player information.
    /// </summary>
    [JsonPropertyName("players")]
    public GameStreamPlayers? Players { get; init; }

    /// <summary>
    /// Winner color ("white" or "black") if the game has ended.
    /// </summary>
    [JsonPropertyName("winner")]
    public string? Winner { get; init; }
}

/// <summary>
/// Player information for a streamed game.
/// </summary>
public class GameStreamPlayers
{
    /// <summary>
    /// White player.
    /// </summary>
    [JsonPropertyName("white")]
    public GameStreamPlayer? White { get; init; }

    /// <summary>
    /// Black player.
    /// </summary>
    [JsonPropertyName("black")]
    public GameStreamPlayer? Black { get; init; }
}

/// <summary>
/// Player information for a streamed game.
/// </summary>
public class GameStreamPlayer
{
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    /// <summary>
    /// Rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }
}
