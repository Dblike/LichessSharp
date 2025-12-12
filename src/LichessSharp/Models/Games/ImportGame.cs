using System.Text.Json.Serialization;

namespace LichessSharp.Models.Games;

/// <summary>
///     Response from the import game endpoint.
/// </summary>
public class ImportGameResponse
{
    /// <summary>
    ///     The imported game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     URL to the imported game.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}