using System.Text.Json.Serialization;

namespace LichessSharp.Models.Users;

/// <summary>
///     A live streamer on Lichess.
/// </summary>
[ResponseOnly]
public class Streamer
{
    /// <summary>
    ///     The user's unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     The user's display name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     The user's title, if any.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    ///     Whether the user is online.
    /// </summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>
    ///     Whether the user is a patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }

    /// <summary>
    ///     Stream information.
    /// </summary>
    [JsonPropertyName("stream")]
    public StreamInfo? Stream { get; init; }

    /// <summary>
    ///     Streamer-specific information.
    /// </summary>
    [JsonPropertyName("streamer")]
    public StreamerInfo? StreamerDetails { get; init; }
}

/// <summary>
///     Stream information.
/// </summary>
[ResponseOnly]
public class StreamInfo
{
    /// <summary>
    ///     Streaming service (twitch, youtube).
    /// </summary>
    [JsonPropertyName("service")]
    public string? Service { get; init; }

    /// <summary>
    ///     Stream status.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    ///     Stream language.
    /// </summary>
    [JsonPropertyName("lang")]
    public string? Lang { get; init; }
}

/// <summary>
///     Streamer-specific information.
/// </summary>
[ResponseOnly]
public class StreamerInfo
{
    /// <summary>
    ///     Streamer name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     Streamer headline.
    /// </summary>
    [JsonPropertyName("headline")]
    public string? Headline { get; init; }

    /// <summary>
    ///     Streamer description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    ///     Twitch channel URL.
    /// </summary>
    [JsonPropertyName("twitch")]
    public string? Twitch { get; init; }

    /// <summary>
    ///     YouTube channel URL.
    /// </summary>
    [JsonPropertyName("youTube")]
    public string? YouTube { get; init; }

    /// <summary>
    ///     Stream image URL.
    /// </summary>
    [JsonPropertyName("image")]
    public string? Image { get; init; }
}

/// <summary>
///     Stream channel information.
/// </summary>
[ResponseOnly]
public class StreamChannel
{
    /// <summary>
    ///     Channel name/ID.
    /// </summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; init; }
}