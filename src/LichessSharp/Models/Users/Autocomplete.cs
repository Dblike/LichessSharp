using System.Text.Json.Serialization;

namespace LichessSharp.Models.Users;

/// <summary>
///     Player autocomplete result (object mode).
/// </summary>
public class AutocompletePlayer
{
    /// <summary>
    ///     User ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Username.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    ///     Whether the user is a patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }

    /// <summary>
    ///     Whether the user is online.
    /// </summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }
}

/// <summary>
///     Response wrapper for autocomplete with object mode.
/// </summary>
public class AutocompleteResponse
{
    /// <summary>
    ///     The result players.
    /// </summary>
    [JsonPropertyName("result")]
    public IReadOnlyList<AutocompletePlayer>? Result { get; init; }
}