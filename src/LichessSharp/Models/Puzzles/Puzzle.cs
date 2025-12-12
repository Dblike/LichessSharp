using System.Text.Json.Serialization;
using LichessSharp.Models.Enums;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models.Puzzles;

/// <summary>
///     A Lichess puzzle.
/// </summary>
public class Puzzle
{
    /// <summary>
    ///     The puzzle's unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     The puzzle rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; init; }

    /// <summary>
    ///     Number of times the puzzle has been played.
    /// </summary>
    [JsonPropertyName("plays")]
    public int Plays { get; init; }

    /// <summary>
    ///     The solution moves in UCI notation.
    /// </summary>
    [JsonPropertyName("solution")]
    public string[]? Solution { get; init; }

    /// <summary>
    ///     The puzzle themes.
    /// </summary>
    [JsonPropertyName("themes")]
    public string[]? Themes { get; init; }

    /// <summary>
    ///     The initial position in FEN notation.
    /// </summary>
    [JsonPropertyName("initialPly")]
    public int InitialPly { get; init; }
}

/// <summary>
///     A puzzle with its associated game.
/// </summary>
public class PuzzleWithGame
{
    /// <summary>
    ///     The puzzle.
    /// </summary>
    [JsonPropertyName("puzzle")]
    public required Puzzle Puzzle { get; init; }

    /// <summary>
    ///     The game the puzzle is from.
    /// </summary>
    [JsonPropertyName("game")]
    public required PuzzleGame Game { get; init; }
}

/// <summary>
///     The game a puzzle is derived from.
/// </summary>
[JsonConverter(typeof(PuzzleGameConverter))]
public class PuzzleGame
{
    /// <summary>
    ///     The game's unique identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     The game in PGN format.
    /// </summary>
    public string? Pgn { get; init; }

    /// <summary>
    ///     The speed (time control) if this is a standard chess game.
    ///     Parsed from the perf field in the API response.
    /// </summary>
    public Speed? Speed { get; init; }

    /// <summary>
    ///     The variant if this is a variant chess game.
    ///     Parsed from the perf field in the API response.
    /// </summary>
    public Variant? Variant { get; init; }

    /// <summary>
    ///     The players in the puzzle game.
    /// </summary>
    public PuzzlePlayer[]? Players { get; init; }
}

/// <summary>
///     A player in a puzzle game.
/// </summary>
public class PuzzlePlayer
{
    /// <summary>
    ///     The player's user ID.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    /// <summary>
    ///     The player's username.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     The player's color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }
}