using System.Text.Json.Serialization;

namespace LichessSharp.Models.Common;

/// <summary>
///     Standard OK response from Lichess API.
/// </summary>
public class OkResponse
{
    /// <summary>
    ///     Whether the operation was successful.
    /// </summary>
    [JsonPropertyName("ok")]
    public bool Ok { get; init; }
}

/// <summary>
///     Error response from Lichess API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    ///     The error message.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }
}

/// <summary>
///     Email response from Lichess API.
/// </summary>
public class EmailResponse
{
    /// <summary>
    ///     The email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }
}

/// <summary>
///     Kid mode status response from Lichess API.
/// </summary>
public class KidModeResponse
{
    /// <summary>
    ///     Whether kid mode is enabled.
    /// </summary>
    [JsonPropertyName("kid")]
    public bool Kid { get; init; }
}

/// <summary>
///     User note response from Lichess API.
/// </summary>
public class NoteResponse
{
    /// <summary>
    ///     The note text.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}