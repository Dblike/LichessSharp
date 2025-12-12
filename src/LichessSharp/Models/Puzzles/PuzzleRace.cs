using System.Text.Json.Serialization;

namespace LichessSharp.Models.Puzzles;

/// <summary>
/// Information about puzzles to replay.
/// </summary>
public class PuzzleReplayInfo
{
    /// <summary>
    /// Number of days looked back.
    /// </summary>
    [JsonPropertyName("days")]
    public int Days { get; init; }

    /// <summary>
    /// The theme or opening filtered.
    /// </summary>
    [JsonPropertyName("theme")]
    public string? Theme { get; init; }

    /// <summary>
    /// Total number of puzzles to replay.
    /// </summary>
    [JsonPropertyName("nb")]
    public int Nb { get; init; }

    /// <summary>
    /// Remaining puzzle IDs to replay.
    /// </summary>
    [JsonPropertyName("remaining")]
    public IReadOnlyList<string>? Remaining { get; init; }
}

/// <summary>
/// Information about a puzzle angle (theme or opening).
/// </summary>
public class PuzzleAngle
{
    /// <summary>
    /// The angle key.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    /// <summary>
    /// The angle display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Description of the angle.
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; init; }
}

/// <summary>
/// Response for puzzle replay endpoint.
/// </summary>
public class PuzzleReplay
{
    /// <summary>
    /// Information about the replay.
    /// </summary>
    [JsonPropertyName("replay")]
    public PuzzleReplayInfo? Replay { get; init; }

    /// <summary>
    /// Information about the angle.
    /// </summary>
    [JsonPropertyName("angle")]
    public PuzzleAngle? Angle { get; init; }
}

/// <summary>
/// A player in a puzzle race.
/// </summary>
public class PuzzleRacePlayer
{
    /// <summary>
    /// The player's username.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// The player's user ID (missing if anonymous).
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// The player's flair.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }

    /// <summary>
    /// The player's score in the race.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; init; }
}

/// <summary>
/// Results of a puzzle race.
/// </summary>
public class PuzzleRaceResults
{
    /// <summary>
    /// The race ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The race owner username.
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; init; }

    /// <summary>
    /// Players participating in the race.
    /// </summary>
    [JsonPropertyName("players")]
    public IReadOnlyList<PuzzleRacePlayer>? Players { get; init; }
}
