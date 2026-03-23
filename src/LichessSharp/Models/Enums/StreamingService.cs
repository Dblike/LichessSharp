using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
///     Streaming platforms supported by Lichess.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<StreamingService>))]
public enum StreamingService
{
    /// <summary>Twitch.tv.</summary>
    [JsonStringEnumMemberName("twitch")] Twitch,

    /// <summary>YouTube.</summary>
    [JsonStringEnumMemberName("youtube")] YouTube
}
