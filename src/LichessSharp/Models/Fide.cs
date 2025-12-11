using System.Text.Json.Serialization;

namespace LichessSharp.Models;

/// <summary>
/// A FIDE chess player.
/// </summary>
public class FidePlayer
{
    /// <summary>
    /// FIDE player ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Player's name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Player's title (GM, IM, FM, WGM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Federation (3-letter country code).
    /// </summary>
    [JsonPropertyName("federation")]
    public string? Federation { get; init; }

    /// <summary>
    /// Year of birth.
    /// </summary>
    [JsonPropertyName("year")]
    public int? Year { get; init; }

    /// <summary>
    /// Whether the player is inactive.
    /// </summary>
    [JsonPropertyName("inactive")]
    public bool? Inactive { get; init; }

    /// <summary>
    /// Standard (classical) rating.
    /// </summary>
    [JsonPropertyName("standard")]
    public int? Standard { get; init; }

    /// <summary>
    /// Rapid rating.
    /// </summary>
    [JsonPropertyName("rapid")]
    public int? Rapid { get; init; }

    /// <summary>
    /// Blitz rating.
    /// </summary>
    [JsonPropertyName("blitz")]
    public int? Blitz { get; init; }
}
