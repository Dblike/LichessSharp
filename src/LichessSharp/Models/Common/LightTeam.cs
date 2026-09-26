using System.Text.Json.Serialization;

namespace LichessSharp.Models.Common;

/// <summary>
///     Minimal team information, as embedded in other responses (for example team updates).
/// </summary>
[ResponseOnly]
public class LightTeam
{
    /// <summary>
    ///     The team ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     The team name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     The team flair emoji, if any.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }
}
