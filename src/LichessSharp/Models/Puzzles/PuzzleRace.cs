using System.Text.Json.Serialization;

namespace LichessSharp.Models.Puzzles;

/// <summary>
///     Information about puzzles to replay.
/// </summary>
[ResponseOnly]
public class PuzzleReplayInfo
{
    /// <summary>
    ///     Number of days looked back.
    /// </summary>
    [JsonPropertyName("days")]
    public int Days { get; init; }

    /// <summary>
    ///     The theme or opening filtered.
    /// </summary>
    [JsonPropertyName("theme")]
    public string? Theme { get; init; }

    /// <summary>
    ///     Total number of puzzles to replay.
    /// </summary>
    [JsonPropertyName("nb")]
    public int Nb { get; init; }

    /// <summary>
    ///     Remaining puzzle IDs to replay.
    /// </summary>
    [JsonPropertyName("remaining")]
    public IReadOnlyList<string>? Remaining { get; init; }
}

/// <summary>
///     Information about a puzzle angle (theme or opening).
/// </summary>
[ResponseOnly]
public class PuzzleAngle
{
    /// <summary>
    ///     The angle key.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    /// <summary>
    ///     The angle display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     Description of the angle.
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; init; }
}

/// <summary>
///     Response for puzzle replay endpoint.
/// </summary>
[ResponseOnly]
public class PuzzleReplay
{
    /// <summary>
    ///     Information about the replay.
    /// </summary>
    [JsonPropertyName("replay")]
    public PuzzleReplayInfo? Replay { get; init; }

    /// <summary>
    ///     Information about the angle.
    /// </summary>
    [JsonPropertyName("angle")]
    public PuzzleAngle? Angle { get; init; }
}

/// <summary>
///     A player in a puzzle race.
/// </summary>
[ResponseOnly]
public class PuzzleRacePlayer
{
    /// <summary>
    ///     The player's username.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     The player's user ID (missing if anonymous).
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    ///     The player's flair.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }

    /// <summary>
    ///     The player's score in the race.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; init; }
}

/// <summary>
///     Results of a puzzle race.
/// </summary>
[ResponseOnly]
public class PuzzleRaceResults
{
    /// <summary>
    ///     The race ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     The race owner username.
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; init; }

    /// <summary>
    ///     Players participating in the race.
    /// </summary>
    [JsonPropertyName("players")]
    public IReadOnlyList<PuzzleRacePlayer>? Players { get; init; }

    /// <summary>
    ///     The puzzle IDs in this race.
    /// </summary>
    [JsonPropertyName("puzzles")]
    public IReadOnlyList<string>? Puzzles { get; init; }

    /// <summary>
    ///     When the race starts (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("startsAt")]
    public long? StartsAt { get; init; }

    /// <summary>
    ///     When the race finishes (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("finishesAt")]
    public long? FinishesAt { get; init; }
}