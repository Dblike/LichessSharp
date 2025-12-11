using System.Text.Json.Serialization;

using LichessSharp.Models;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Bulk Pairings API - Create many games for other players.
/// These endpoints are intended for tournament organizers.
/// </summary>
public interface IBulkPairingsApi
{
    /// <summary>
    /// Get a list of bulk pairings you created.
    /// Requires OAuth with challenge:bulk scope.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of bulk pairings.</returns>
    Task<IReadOnlyList<BulkPairing>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedule many games at once, up to 24h in advance.
    /// OAuth tokens are required for all paired players, with the challenge:write scope.
    /// You can schedule up to 500 games every 10 minutes.
    /// Requires OAuth with challenge:bulk scope.
    /// </summary>
    /// <param name="options">Options for creating the bulk pairing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created bulk pairing.</returns>
    Task<BulkPairing> CreateAsync(BulkPairingCreateOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Immediately start all clocks of the games of a bulk pairing.
    /// This overrides the startClocksAt value of an existing bulk pairing.
    /// Requires OAuth with challenge:bulk scope.
    /// </summary>
    /// <param name="id">The ID of the bulk pairing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the clocks were started successfully.</returns>
    Task<bool> StartClocksAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a single bulk pairing by its ID.
    /// Requires OAuth with challenge:bulk scope.
    /// </summary>
    /// <param name="id">The ID of the bulk pairing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The bulk pairing.</returns>
    Task<BulkPairing> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel and delete a bulk pairing that is scheduled in the future.
    /// If the games have already been created, then this does nothing.
    /// Requires OAuth with challenge:bulk scope.
    /// </summary>
    /// <param name="id">The ID of the bulk pairing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the bulk pairing was cancelled successfully.</returns>
    Task<bool> CancelAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export games of a bulk pairing in PGN format.
    /// Requires OAuth with challenge:bulk scope.
    /// </summary>
    /// <param name="id">The ID of the bulk pairing.</param>
    /// <param name="options">Export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PGN content as a string.</returns>
    Task<string> ExportGamesAsync(string id, BulkPairingExportOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream games of a bulk pairing in NDJSON format.
    /// Requires OAuth with challenge:bulk scope.
    /// </summary>
    /// <param name="id">The ID of the bulk pairing.</param>
    /// <param name="options">Export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of games.</returns>
    IAsyncEnumerable<GameJson> StreamGamesAsync(string id, BulkPairingExportOptions? options = null, CancellationToken cancellationToken = default);
}


/// <summary>
/// Options for creating a bulk pairing.
/// </summary>
public class BulkPairingCreateOptions
{
    /// <summary>
    /// OAuth tokens of all the players to pair.
    /// Format: "tokenOfWhitePlayerInGame1:tokenOfBlackPlayerInGame1,tokenOfWhitePlayerInGame2:tokenOfBlackPlayerInGame2,..."
    /// The 2 tokens of the players of a game are separated with ':'. The first token gets the white pieces.
    /// Games are separated with ','. Up to 1000 tokens can be sent, for a max of 500 games.
    /// </summary>
    public required string Players { get; set; }

    /// <summary>
    /// Clock initial time in seconds (0-10800).
    /// Required for real-time games.
    /// </summary>
    public int? ClockLimit { get; set; }

    /// <summary>
    /// Clock increment in seconds (0-60).
    /// Required for real-time games.
    /// </summary>
    public int? ClockIncrement { get; set; }

    /// <summary>
    /// Days per turn. For correspondence games only.
    /// Valid values: 1, 2, 3, 5, 7, 10, 14.
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// Date at which the games will be created as a Unix timestamp in milliseconds.
    /// Up to 7 days in the future.
    /// Omit to start the games immediately.
    /// </summary>
    public long? PairAt { get; set; }

    /// <summary>
    /// Date at which the clocks will be automatically started as a Unix timestamp in milliseconds.
    /// Up to 7 days in the future.
    /// If omitted, the clocks will not start automatically.
    /// </summary>
    public long? StartClocksAt { get; set; }

    /// <summary>
    /// Game is rated and impacts players ratings.
    /// Default: false.
    /// </summary>
    public bool? Rated { get; set; }

    /// <summary>
    /// Chess variant key.
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// Custom initial position (in FEN) for the games.
    /// Only valid with variant=standard or fromPosition.
    /// </summary>
    public string? Fen { get; set; }

    /// <summary>
    /// Message that will be sent to each player when the game is created.
    /// {opponent} and {game} are placeholders that will be replaced.
    /// Default: "Your game with {opponent} is ready: {game}."
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Extra game rules separated by commas.
    /// Valid values: noAbort, noRematch, noGiveTime, noClaimWin, noEarlyDraw.
    /// </summary>
    public string? Rules { get; set; }
}

/// <summary>
/// Options for exporting games from a bulk pairing.
/// </summary>
public class BulkPairingExportOptions
{
    /// <summary>
    /// Include the PGN moves.
    /// Default: true.
    /// </summary>
    public bool? Moves { get; set; }

    /// <summary>
    /// Include the full PGN within the JSON response, in a pgn field.
    /// Default: false.
    /// </summary>
    public bool? PgnInJson { get; set; }

    /// <summary>
    /// Include the PGN tags.
    /// Default: true.
    /// </summary>
    public bool? Tags { get; set; }

    /// <summary>
    /// Include clock comments in the PGN moves.
    /// Default: false.
    /// </summary>
    public bool? Clocks { get; set; }

    /// <summary>
    /// Include the opening name.
    /// Default: false.
    /// </summary>
    public bool? Opening { get; set; }
}



/// <summary>
/// Response wrapper for the bulk pairings list endpoint.
/// </summary>
public class BulkPairingListResponse
{
    /// <summary>
    /// List of bulk pairings.
    /// </summary>
    [JsonPropertyName("bulks")]
    public IReadOnlyList<BulkPairing>? Bulks { get; init; }
}

/// <summary>
/// A bulk pairing.
/// </summary>
public class BulkPairing
{
    /// <summary>
    /// The bulk pairing ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// List of games in this bulk pairing.
    /// </summary>
    [JsonPropertyName("games")]
    public required IReadOnlyList<BulkPairingGame> Games { get; init; }

    /// <summary>
    /// Chess variant key.
    /// </summary>
    [JsonPropertyName("variant")]
    public string? Variant { get; init; }

    /// <summary>
    /// Clock settings for the games.
    /// </summary>
    [JsonPropertyName("clock")]
    public BulkPairingClock? Clock { get; init; }

    /// <summary>
    /// Unix timestamp (milliseconds) when games will be paired.
    /// </summary>
    [JsonPropertyName("pairAt")]
    public long PairAt { get; init; }

    /// <summary>
    /// Unix timestamp (milliseconds) when games were actually paired.
    /// Null if not yet paired.
    /// </summary>
    [JsonPropertyName("pairedAt")]
    public long? PairedAt { get; init; }

    /// <summary>
    /// Whether the games are rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    /// Unix timestamp (milliseconds) when clocks will start.
    /// </summary>
    [JsonPropertyName("startClocksAt")]
    public long? StartClocksAt { get; init; }

    /// <summary>
    /// Unix timestamp (milliseconds) when the bulk was scheduled.
    /// </summary>
    [JsonPropertyName("scheduledAt")]
    public long ScheduledAt { get; init; }
}

/// <summary>
/// A game within a bulk pairing.
/// </summary>
public class BulkPairingGame
{
    /// <summary>
    /// The game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The white player's user ID.
    /// </summary>
    [JsonPropertyName("white")]
    public string? White { get; init; }

    /// <summary>
    /// The black player's user ID.
    /// </summary>
    [JsonPropertyName("black")]
    public string? Black { get; init; }
}

/// <summary>
/// Clock settings for a bulk pairing.
/// </summary>
public class BulkPairingClock
{
    /// <summary>
    /// Clock initial time in seconds.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; init; }

    /// <summary>
    /// Clock increment in seconds.
    /// </summary>
    [JsonPropertyName("increment")]
    public int Increment { get; init; }
}

