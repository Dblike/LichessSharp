using System.Text.Json.Serialization;

namespace LichessSharp.Models.Common;

/// <summary>
///     Opening information for a chess game.
/// </summary>
public class Opening
{
    /// <summary>
    ///     ECO code (e.g., "C50", "B12").
    /// </summary>
    [JsonPropertyName("eco")]
    public string? Eco { get; init; }

    /// <summary>
    ///     Opening name (e.g., "Italian Game", "Caro-Kann Defense").
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     Number of half-moves (plies) in the opening.
    /// </summary>
    [JsonPropertyName("ply")]
    public int? Ply { get; init; }
}