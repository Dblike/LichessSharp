using System.Text.Json.Serialization;

namespace LichessSharp.Models.Common;

/// <summary>
/// Clock settings for a game.
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
