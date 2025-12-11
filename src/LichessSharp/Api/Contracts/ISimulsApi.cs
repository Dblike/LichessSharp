using System.Text.Json.Serialization;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Simuls API - Access simultaneous exhibitions played on Lichess.
/// </summary>
public interface ISimulsApi
{
    /// <summary>
    /// Get recently created, started, finished simuls.
    /// Created and finished simul lists are not exhaustive, only those with
    /// strong enough host will be listed, the same filter is used to display simuls on https://lichess.org/simul.
    /// When authenticated with OAuth2, the pending list will be populated with your created, but unstarted simuls.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Lists of current simuls grouped by status.</returns>
    Task<SimulList> GetCurrentAsync(CancellationToken cancellationToken = default);
}

#region Response Models

/// <summary>
/// Response containing simuls grouped by status.
/// </summary>
public class SimulList
{
    /// <summary>
    /// Your pending (created but not started) simuls.
    /// Only populated when authenticated with OAuth2.
    /// </summary>
    [JsonPropertyName("pending")]
    public IReadOnlyList<Simul> Pending { get; init; } = [];

    /// <summary>
    /// Recently created simuls (not yet started).
    /// </summary>
    [JsonPropertyName("created")]
    public IReadOnlyList<Simul> Created { get; init; } = [];

    /// <summary>
    /// Currently running simuls.
    /// </summary>
    [JsonPropertyName("started")]
    public IReadOnlyList<Simul> Started { get; init; } = [];

    /// <summary>
    /// Recently finished simuls.
    /// </summary>
    [JsonPropertyName("finished")]
    public IReadOnlyList<Simul> Finished { get; init; } = [];
}

/// <summary>
/// A simultaneous exhibition (simul) on Lichess.
/// </summary>
public class Simul
{
    /// <summary>
    /// Simul ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The host of the simul.
    /// </summary>
    [JsonPropertyName("host")]
    public required SimulHost Host { get; init; }

    /// <summary>
    /// Short name of the simul.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Full name of the simul including host information.
    /// </summary>
    [JsonPropertyName("fullName")]
    public required string FullName { get; init; }

    /// <summary>
    /// Chess variants included in this simul.
    /// </summary>
    [JsonPropertyName("variants")]
    public required IReadOnlyList<SimulVariant> Variants { get; init; }

    /// <summary>
    /// Whether the simul has been created but not yet started.
    /// </summary>
    [JsonPropertyName("isCreated")]
    public bool IsCreated { get; init; }

    /// <summary>
    /// Whether the simul has finished.
    /// </summary>
    [JsonPropertyName("isFinished")]
    public bool IsFinished { get; init; }

    /// <summary>
    /// Whether the simul is currently running.
    /// </summary>
    [JsonPropertyName("isRunning")]
    public bool IsRunning { get; init; }

    /// <summary>
    /// Optional description text for the simul.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// Estimated start time (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("estimatedStartAt")]
    public long? EstimatedStartAt { get; init; }

    /// <summary>
    /// Actual start time (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("startedAt")]
    public long? StartedAt { get; init; }

    /// <summary>
    /// Finish time (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("finishedAt")]
    public long? FinishedAt { get; init; }

    /// <summary>
    /// Number of applicants wanting to join the simul.
    /// </summary>
    [JsonPropertyName("nbApplicants")]
    public int NbApplicants { get; init; }

    /// <summary>
    /// Number of active pairings (games being played).
    /// </summary>
    [JsonPropertyName("nbPairings")]
    public int NbPairings { get; init; }
}

/// <summary>
/// The host of a simul with rating and game info.
/// </summary>
public class SimulHost
{
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Username (display name with original casing).
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Chess title if any (GM, IM, FM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Patron status (true if Lichess supporter).
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }

    /// <summary>
    /// Custom flair emoji.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }

    /// <summary>
    /// Host's rating in the simul variant.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    /// Whether the rating is provisional.
    /// </summary>
    [JsonPropertyName("provisional")]
    public bool? Provisional { get; init; }

    /// <summary>
    /// Current game ID if the host is playing.
    /// </summary>
    [JsonPropertyName("gameId")]
    public string? GameId { get; init; }

    /// <summary>
    /// Whether the host is currently online.
    /// </summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }
}

/// <summary>
/// A chess variant available in a simul.
/// </summary>
public class SimulVariant
{
    /// <summary>
    /// Variant key (e.g., "standard", "chess960", "crazyhouse").
    /// </summary>
    [JsonPropertyName("key")]
    public required string Key { get; init; }

    /// <summary>
    /// Icon character for the variant.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; init; }

    /// <summary>
    /// Display name of the variant.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

#endregion
