using System.Text.Json.Serialization;
using LichessSharp.Models.Enums;

namespace LichessSharp.Models.Common;

/// <summary>
/// Minimal user information as defined by the Lichess API LightUser schema.
/// This is the canonical representation for user references in game results,
/// player lists, and other contexts where full user details aren't needed.
/// </summary>
/// <remarks>
/// Note: The Lichess API uses "name" for LightUser and "username" for full User objects.
/// This class uses "name" to match the LightUser schema exactly.
/// </remarks>
public class LightUser
{
    /// <summary>
    /// The user's unique identifier (lowercase username).
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The user's display name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The user's FIDE/Lichess title, if any.
    /// </summary>
    [JsonPropertyName("title")]
    public Title? Title { get; init; }

    /// <summary>
    /// Whether the user is a Lichess patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }

    /// <summary>
    /// The user's flair emoji.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }
}
