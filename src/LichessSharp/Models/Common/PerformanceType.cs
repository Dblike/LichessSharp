using System.Text.Json.Serialization;

namespace LichessSharp.Models.Common;

/// <summary>
///     Performance type information (game mode/variant identifier).
///     Used across games and puzzles to identify the type of chess played.
/// </summary>
public class PerformanceType
{
    /// <summary>
    ///     The performance type key (e.g., "bullet", "blitz", "rapid", "classical").
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    /// <summary>
    ///     The display name (e.g., "Bullet", "Blitz", "Rapid", "Classical").
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}