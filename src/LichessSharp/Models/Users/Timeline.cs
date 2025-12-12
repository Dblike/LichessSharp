using System.Text.Json.Serialization;
using LichessSharp.Models.Common;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models.Users;

/// <summary>
///     User timeline.
/// </summary>
[ResponseOnly]
public class Timeline
{
    /// <summary>
    ///     Timeline entries.
    /// </summary>
    [JsonPropertyName("entries")]
    public IReadOnlyList<TimelineEntry>? Entries { get; init; }

    /// <summary>
    ///     Users referenced in the timeline.
    /// </summary>
    [JsonPropertyName("users")]
    public Dictionary<string, LightUser>? Users { get; init; }
}

/// <summary>
///     A timeline entry.
/// </summary>
[ResponseOnly]
public class TimelineEntry
{
    /// <summary>
    ///     Entry type.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    ///     Entry data (varies by type).
    /// </summary>
    [JsonPropertyName("data")]
    public TimelineData? Data { get; init; }

    /// <summary>
    ///     When this entry occurred.
    /// </summary>
    [JsonPropertyName("date")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset Date { get; init; }
}

/// <summary>
///     Timeline entry data.
/// </summary>
[ResponseOnly]
public class TimelineData
{
    /// <summary>
    ///     User ID involved.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    /// <summary>
    ///     Game ID involved.
    /// </summary>
    [JsonPropertyName("gameId")]
    public string? GameId { get; init; }

    /// <summary>
    ///     Game full ID.
    /// </summary>
    [JsonPropertyName("fullId")]
    public string? FullId { get; init; }

    /// <summary>
    ///     Opponent info.
    /// </summary>
    [JsonPropertyName("opponent")]
    public string? Opponent { get; init; }

    /// <summary>
    ///     Study ID.
    /// </summary>
    [JsonPropertyName("studyId")]
    public string? StudyId { get; init; }

    /// <summary>
    ///     Study name.
    /// </summary>
    [JsonPropertyName("studyName")]
    public string? StudyName { get; init; }

    /// <summary>
    ///     Team ID.
    /// </summary>
    [JsonPropertyName("teamId")]
    public string? TeamId { get; init; }

    /// <summary>
    ///     Team name.
    /// </summary>
    [JsonPropertyName("teamName")]
    public string? TeamName { get; init; }

    /// <summary>
    ///     Forum topic ID.
    /// </summary>
    [JsonPropertyName("topicId")]
    public string? TopicId { get; init; }

    /// <summary>
    ///     Forum topic name.
    /// </summary>
    [JsonPropertyName("topicName")]
    public string? TopicName { get; init; }

    /// <summary>
    ///     Simul ID.
    /// </summary>
    [JsonPropertyName("simulId")]
    public string? SimulId { get; init; }

    /// <summary>
    ///     Simul name.
    /// </summary>
    [JsonPropertyName("simulName")]
    public string? SimulName { get; init; }

    /// <summary>
    ///     Tournament ID.
    /// </summary>
    [JsonPropertyName("tourId")]
    public string? TourId { get; init; }

    /// <summary>
    ///     Tournament name.
    /// </summary>
    [JsonPropertyName("tourName")]
    public string? TourName { get; init; }
}