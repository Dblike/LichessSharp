using System.Text.Json.Serialization;

namespace LichessSharp.Models;

/// <summary>
/// Standard OK response from Lichess API.
/// </summary>
public class OkResponse
{
    /// <summary>
    /// Whether the operation was successful.
    /// </summary>
    [JsonPropertyName("ok")]
    public bool Ok { get; init; }
}

/// <summary>
/// Error response from Lichess API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// The error message.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
