using System.Text.Json.Serialization;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// External Engine API - Register and manage external analysis engines.
/// WARNING: This API is in alpha and subject to change.
/// See <see href="https://lichess.org/api#tag/External-engine"/>.
/// </summary>
public interface IExternalEngineApi
{
    /// <summary>
    /// List all external engines registered for the authenticated user.
    /// Requires OAuth with engine:read scope.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of registered external engines.</returns>
    Task<IReadOnlyList<ExternalEngine>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Register a new external engine.
    /// Requires OAuth with engine:write scope.
    /// </summary>
    /// <param name="registration">The engine registration details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The registered engine with its assigned ID and client secret.</returns>
    Task<ExternalEngine> CreateAsync(ExternalEngineRegistration registration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get details of a specific external engine.
    /// Requires OAuth with engine:read scope.
    /// </summary>
    /// <param name="engineId">The engine ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The engine details.</returns>
    Task<ExternalEngine> GetAsync(string engineId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing external engine.
    /// Requires OAuth with engine:write scope.
    /// </summary>
    /// <param name="engineId">The engine ID.</param>
    /// <param name="registration">The updated engine details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated engine.</returns>
    Task<ExternalEngine> UpdateAsync(string engineId, ExternalEngineRegistration registration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an external engine.
    /// Requires OAuth with engine:write scope.
    /// </summary>
    /// <param name="engineId">The engine ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(string engineId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Request analysis from an external engine.
    /// Response content is streamed as newline delimited JSON.
    /// Endpoint: https://engine.lichess.ovh/api/external-engine/{id}/analyse
    /// </summary>
    /// <param name="engineId">The engine ID.</param>
    /// <param name="request">The analysis request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of analysis lines.</returns>
    IAsyncEnumerable<EngineAnalysisLine> AnalyseAsync(string engineId, EngineAnalysisRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wait for an analysis request to any of the external engines registered with the given secret.
    /// Uses long polling. After acquiring a request, the provider should immediately start streaming the results.
    /// Endpoint: https://engine.lichess.ovh/api/external-engine/work
    /// </summary>
    /// <param name="providerSecret">The provider secret used when registering engines.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The acquired work, or null if no work is available.</returns>
    Task<EngineWork?> AcquireWorkAsync(string providerSecret, CancellationToken cancellationToken = default);

    /// <summary>
    /// Submit analysis results for a previously acquired work request.
    /// The provider should stream UCI output lines.
    /// Endpoint: https://engine.lichess.ovh/api/external-engine/work/{id}
    /// </summary>
    /// <param name="workId">The work ID from the acquired work.</param>
    /// <param name="uciLines">Stream of UCI output lines to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SubmitWorkAsync(string workId, IAsyncEnumerable<string> uciLines, CancellationToken cancellationToken = default);
}

#region Models

/// <summary>
/// A registered external engine.
/// </summary>
public class ExternalEngine
{
    /// <summary>
    /// Unique engine registration ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Display name of the engine.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// A secret token that can be used to request analysis from this external engine.
    /// </summary>
    [JsonPropertyName("clientSecret")]
    public required string ClientSecret { get; init; }

    /// <summary>
    /// The user this engine has been registered for.
    /// </summary>
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    /// <summary>
    /// Maximum number of available threads.
    /// </summary>
    [JsonPropertyName("maxThreads")]
    public int MaxThreads { get; init; }

    /// <summary>
    /// Maximum available hash table size, in MiB.
    /// </summary>
    [JsonPropertyName("maxHash")]
    public int MaxHash { get; init; }

    /// <summary>
    /// List of supported chess variants.
    /// </summary>
    [JsonPropertyName("variants")]
    public IReadOnlyList<string> Variants { get; init; } = [];

    /// <summary>
    /// Arbitrary data that the engine provider can use for identification or bookkeeping.
    /// </summary>
    [JsonPropertyName("providerData")]
    public string? ProviderData { get; init; }
}

/// <summary>
/// Registration request for a new external engine.
/// </summary>
public class ExternalEngineRegistration
{
    /// <summary>
    /// Display name of the engine (3-200 characters).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Maximum number of available threads (1-65536).
    /// </summary>
    public required int MaxThreads { get; init; }

    /// <summary>
    /// Maximum available hash table size, in MiB (1-1048576).
    /// </summary>
    public required int MaxHash { get; init; }

    /// <summary>
    /// A secret token used to identify this provider when connecting as an engine provider.
    /// Must be at least 16 characters.
    /// </summary>
    public required string ProviderSecret { get; init; }

    /// <summary>
    /// Optional list of supported chess variants.
    /// If not specified, defaults to standard chess.
    /// </summary>
    public IReadOnlyList<string>? Variants { get; init; }

    /// <summary>
    /// Arbitrary data that the engine provider can use for identification or bookkeeping.
    /// </summary>
    public string? ProviderData { get; init; }
}

/// <summary>
/// Request for engine analysis.
/// </summary>
public class EngineAnalysisRequest
{
    /// <summary>
    /// The client secret from the engine registration.
    /// </summary>
    public required string ClientSecret { get; init; }

    /// <summary>
    /// The analysis work parameters.
    /// </summary>
    public required EngineAnalysisWork Work { get; init; }
}

/// <summary>
/// Analysis work parameters.
/// </summary>
public class EngineAnalysisWork
{
    /// <summary>
    /// Arbitrary string that identifies the analysis session.
    /// Providers may wish to clear the hash table between sessions.
    /// </summary>
    public string? SessionId { get; init; }

    /// <summary>
    /// Number of threads to use for analysis.
    /// </summary>
    public int? Threads { get; init; }

    /// <summary>
    /// Hash table size to use for analysis, in MiB.
    /// </summary>
    public int? Hash { get; init; }

    /// <summary>
    /// Requested number of principal variations (1-5).
    /// </summary>
    public int? MultiPv { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    public string? Variant { get; init; }

    /// <summary>
    /// Initial position of the game in FEN format.
    /// </summary>
    public required string InitialFen { get; init; }

    /// <summary>
    /// List of UCI moves from the initial position.
    /// </summary>
    public IReadOnlyList<string>? Moves { get; init; }

    /// <summary>
    /// Request deep analysis up to this depth.
    /// </summary>
    public int? InfiniteDepth { get; init; }

    /// <summary>
    /// Request analysis for this many nodes.
    /// </summary>
    public long? Nodes { get; init; }

    /// <summary>
    /// Request analysis for this many milliseconds.
    /// </summary>
    public int? MoveTime { get; init; }
}

/// <summary>
/// A line of engine analysis output.
/// </summary>
public class EngineAnalysisLine
{
    /// <summary>
    /// Time spent on analysis in milliseconds.
    /// </summary>
    [JsonPropertyName("time")]
    public int? Time { get; init; }

    /// <summary>
    /// Search depth.
    /// </summary>
    [JsonPropertyName("depth")]
    public int? Depth { get; init; }

    /// <summary>
    /// Selective depth.
    /// </summary>
    [JsonPropertyName("seldepth")]
    public int? SelDepth { get; init; }

    /// <summary>
    /// Number of nodes searched.
    /// </summary>
    [JsonPropertyName("nodes")]
    public long? Nodes { get; init; }

    /// <summary>
    /// Principal variation (best line of play).
    /// </summary>
    [JsonPropertyName("pv")]
    public string? Pv { get; init; }

    /// <summary>
    /// The score evaluation.
    /// </summary>
    [JsonPropertyName("score")]
    public EngineScore? Score { get; init; }

    /// <summary>
    /// Current move being searched.
    /// </summary>
    [JsonPropertyName("currmove")]
    public string? CurrentMove { get; init; }

    /// <summary>
    /// Number of current move in the search.
    /// </summary>
    [JsonPropertyName("currmovenumber")]
    public int? CurrentMoveNumber { get; init; }

    /// <summary>
    /// Hash table usage in per mille.
    /// </summary>
    [JsonPropertyName("hashfull")]
    public int? HashFull { get; init; }

    /// <summary>
    /// Nodes per second.
    /// </summary>
    [JsonPropertyName("nps")]
    public long? Nps { get; init; }

    /// <summary>
    /// Tablebase hits.
    /// </summary>
    [JsonPropertyName("tbhits")]
    public long? TbHits { get; init; }

    /// <summary>
    /// Multi-PV line number (1-indexed).
    /// </summary>
    [JsonPropertyName("multipv")]
    public int? MultiPv { get; init; }
}

/// <summary>
/// Engine score evaluation.
/// </summary>
public class EngineScore
{
    /// <summary>
    /// Score in centipawns (from the engine's perspective).
    /// </summary>
    [JsonPropertyName("cp")]
    public int? Centipawns { get; init; }

    /// <summary>
    /// Mate in N moves (positive = engine wins, negative = engine loses).
    /// </summary>
    [JsonPropertyName("mate")]
    public int? Mate { get; init; }
}

/// <summary>
/// Acquired work for an engine provider.
/// </summary>
public class EngineWork
{
    /// <summary>
    /// The work ID to use when submitting results.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The analysis work parameters.
    /// </summary>
    [JsonPropertyName("work")]
    public required EngineWorkDetails Work { get; init; }
}

/// <summary>
/// Details of the analysis work to perform.
/// </summary>
public class EngineWorkDetails
{
    /// <summary>
    /// Session ID for the analysis.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; init; }

    /// <summary>
    /// Number of threads to use.
    /// </summary>
    [JsonPropertyName("threads")]
    public int Threads { get; init; }

    /// <summary>
    /// Hash table size in MiB.
    /// </summary>
    [JsonPropertyName("hash")]
    public int Hash { get; init; }

    /// <summary>
    /// Number of principal variations.
    /// </summary>
    [JsonPropertyName("multiPv")]
    public int MultiPv { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    [JsonPropertyName("variant")]
    public string? Variant { get; init; }

    /// <summary>
    /// Initial position in FEN format.
    /// </summary>
    [JsonPropertyName("initialFen")]
    public required string InitialFen { get; init; }

    /// <summary>
    /// List of UCI moves from the initial position.
    /// </summary>
    [JsonPropertyName("moves")]
    public IReadOnlyList<string>? Moves { get; init; }

    /// <summary>
    /// Maximum depth limit.
    /// </summary>
    [JsonPropertyName("infiniteDepth")]
    public int? InfiniteDepth { get; init; }

    /// <summary>
    /// Node limit.
    /// </summary>
    [JsonPropertyName("nodes")]
    public long? Nodes { get; init; }

    /// <summary>
    /// Time limit in milliseconds.
    /// </summary>
    [JsonPropertyName("movetime")]
    public int? MoveTime { get; init; }
}

/// <summary>
/// Request to acquire work as an engine provider.
/// </summary>
internal class EngineWorkRequest
{
    /// <summary>
    /// The provider secret used when registering engines.
    /// </summary>
    [JsonPropertyName("providerSecret")]
    public required string ProviderSecret { get; init; }
}

#endregion
