namespace LichessSharp.Api;

/// <summary>
/// Bulk Pairings API - Create many games for other players.
/// </summary>
public interface IBulkPairingsApi
{
    // TODO: Implement bulk pairing endpoints
}

/// <summary>
/// Arena Tournaments API - Access Arena tournaments played on Lichess.
/// </summary>
public interface IArenaTournamentsApi
{
    // TODO: Implement arena tournament endpoints
}

/// <summary>
/// Swiss Tournaments API - Access Swiss tournaments played on Lichess.
/// </summary>
public interface ISwissTournamentsApi
{
    // TODO: Implement swiss tournament endpoints
}

/// <summary>
/// Simuls API - Access simultaneous exhibitions played on Lichess.
/// </summary>
public interface ISimulsApi
{
    // TODO: Implement simul endpoints
}

/// <summary>
/// Studies API - Access Lichess studies.
/// </summary>
public interface IStudiesApi
{
    // TODO: Implement study endpoints
}

/// <summary>
/// Messaging API - Private messages with other players.
/// </summary>
public interface IMessagingApi
{
    // TODO: Implement messaging endpoints
}

/// <summary>
/// Broadcasts API - Relay chess events on Lichess.
/// </summary>
public interface IBroadcastsApi
{
    // TODO: Implement broadcast endpoints
}

/// <summary>
/// Analysis API - Access Lichess cloud evaluations database.
/// </summary>
public interface IAnalysisApi
{
    /// <summary>
    /// Get cloud evaluation for a position.
    /// </summary>
    Task<CloudEvaluation?> GetCloudEvaluationAsync(string fen, int? multiPv = null, string? variant = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Opening Explorer API - Lookup positions from the Lichess opening explorer.
/// </summary>
public interface IOpeningExplorerApi
{
    /// <summary>
    /// Get opening explorer data for masters database.
    /// </summary>
    Task<ExplorerResult> GetMastersAsync(string fen, ExplorerOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get opening explorer data for Lichess database.
    /// </summary>
    Task<ExplorerResult> GetLichessAsync(string fen, ExplorerOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get opening explorer data for a player.
    /// </summary>
    Task<ExplorerResult> GetPlayerAsync(string fen, string player, ExplorerOptions? options = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Tablebase API - Lookup positions from the Lichess tablebase server.
/// </summary>
public interface ITablebaseApi
{
    /// <summary>
    /// Look up a position in the tablebase.
    /// </summary>
    Task<TablebaseResult> LookupAsync(string fen, CancellationToken cancellationToken = default);

    /// <summary>
    /// Look up a position in the atomic tablebase.
    /// </summary>
    Task<TablebaseResult> LookupAtomicAsync(string fen, CancellationToken cancellationToken = default);

    /// <summary>
    /// Look up a position in the antichess tablebase.
    /// </summary>
    Task<TablebaseResult> LookupAntichessAsync(string fen, CancellationToken cancellationToken = default);
}

/// <summary>
/// Cloud evaluation result.
/// </summary>
public class CloudEvaluation
{
    /// <summary>
    /// The FEN position.
    /// </summary>
    public required string Fen { get; init; }

    /// <summary>
    /// Number of known nodes.
    /// </summary>
    public long Knodes { get; init; }

    /// <summary>
    /// Depth of analysis.
    /// </summary>
    public int Depth { get; init; }

    /// <summary>
    /// Principal variations.
    /// </summary>
    public IReadOnlyList<PrincipalVariation>? Pvs { get; init; }
}

/// <summary>
/// Principal variation from analysis.
/// </summary>
public class PrincipalVariation
{
    /// <summary>
    /// Moves in UCI notation.
    /// </summary>
    public required string Moves { get; init; }

    /// <summary>
    /// Centipawn evaluation.
    /// </summary>
    public int? Cp { get; init; }

    /// <summary>
    /// Mate in N.
    /// </summary>
    public int? Mate { get; init; }
}

/// <summary>
/// Opening explorer result.
/// </summary>
public class ExplorerResult
{
    /// <summary>
    /// Total white wins.
    /// </summary>
    public long White { get; init; }

    /// <summary>
    /// Total draws.
    /// </summary>
    public long Draws { get; init; }

    /// <summary>
    /// Total black wins.
    /// </summary>
    public long Black { get; init; }

    /// <summary>
    /// Available moves.
    /// </summary>
    public IReadOnlyList<ExplorerMove>? Moves { get; init; }

    /// <summary>
    /// Top games.
    /// </summary>
    public IReadOnlyList<ExplorerGame>? TopGames { get; init; }

    /// <summary>
    /// Recent games.
    /// </summary>
    public IReadOnlyList<ExplorerGame>? RecentGames { get; init; }

    /// <summary>
    /// Opening information.
    /// </summary>
    public ExplorerOpening? Opening { get; init; }
}

/// <summary>
/// Explorer move.
/// </summary>
public class ExplorerMove
{
    /// <summary>
    /// UCI notation.
    /// </summary>
    public required string Uci { get; init; }

    /// <summary>
    /// SAN notation.
    /// </summary>
    public required string San { get; init; }

    /// <summary>
    /// White wins after this move.
    /// </summary>
    public long White { get; init; }

    /// <summary>
    /// Draws after this move.
    /// </summary>
    public long Draws { get; init; }

    /// <summary>
    /// Black wins after this move.
    /// </summary>
    public long Black { get; init; }

    /// <summary>
    /// Average opponent rating.
    /// </summary>
    public int? AverageRating { get; init; }
}

/// <summary>
/// Explorer game reference.
/// </summary>
public class ExplorerGame
{
    /// <summary>
    /// Game ID.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Winner.
    /// </summary>
    public string? Winner { get; init; }

    /// <summary>
    /// Year played.
    /// </summary>
    public int? Year { get; init; }

    /// <summary>
    /// Month played.
    /// </summary>
    public string? Month { get; init; }
}

/// <summary>
/// Explorer opening information.
/// </summary>
public class ExplorerOpening
{
    /// <summary>
    /// ECO code.
    /// </summary>
    public string? Eco { get; init; }

    /// <summary>
    /// Opening name.
    /// </summary>
    public string? Name { get; init; }
}

/// <summary>
/// Explorer options.
/// </summary>
public class ExplorerOptions
{
    /// <summary>
    /// Variant (for Lichess/Player databases).
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// Speeds to include (for Lichess/Player databases).
    /// </summary>
    public string[]? Speeds { get; set; }

    /// <summary>
    /// Ratings to include (for Lichess/Player databases).
    /// </summary>
    public int[]? Ratings { get; set; }

    /// <summary>
    /// Number of recent games to return.
    /// </summary>
    public int? RecentGames { get; set; }

    /// <summary>
    /// Number of top games to return.
    /// </summary>
    public int? TopGames { get; set; }

    /// <summary>
    /// Number of moves to return.
    /// </summary>
    public int? Moves { get; set; }

    /// <summary>
    /// Include only games from this year or later (for Masters database).
    /// </summary>
    public int? Since { get; set; }

    /// <summary>
    /// Include only games from this year or earlier (for Masters database).
    /// </summary>
    public int? Until { get; set; }

    /// <summary>
    /// Include only games from this month or later, format: YYYY-MM (for Lichess/Player databases).
    /// </summary>
    public string? SinceMonth { get; set; }

    /// <summary>
    /// Include only games from this month or earlier, format: YYYY-MM (for Lichess/Player databases).
    /// </summary>
    public string? UntilMonth { get; set; }

    /// <summary>
    /// Color of the player (required for Player database: "white" or "black").
    /// </summary>
    public string? Color { get; set; }
}

/// <summary>
/// Tablebase result.
/// </summary>
public class TablebaseResult
{
    /// <summary>
    /// Category of the position (win, loss, draw, cursed-win, blessed-loss,
    /// maybe-win, maybe-loss, syzygy-win, syzygy-loss, unknown).
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// DTZ50'' with rounding in plies (for Standard chess positions with not more than 7 pieces
    /// and variant positions not more than 6 pieces).
    /// </summary>
    public int? Dtz { get; init; }

    /// <summary>
    /// DTZ50'' in plies, only if guaranteed to not be rounded, or absent if unknown.
    /// </summary>
    public int? PreciseDtz { get; init; }

    /// <summary>
    /// Depth to Conversion: Moves to next capture, promotion, or checkmate.
    /// </summary>
    public int? Dtc { get; init; }

    /// <summary>
    /// Depth To Mate: Plies to mate (available only for Standard positions with not more than 5 pieces).
    /// </summary>
    public int? Dtm { get; init; }

    /// <summary>
    /// Depth To Win: Plies to win (available only for Antichess positions with not more than 4 pieces).
    /// </summary>
    public int? Dtw { get; init; }

    /// <summary>
    /// Whether the position is checkmate.
    /// </summary>
    public bool Checkmate { get; init; }

    /// <summary>
    /// Whether the position is stalemate.
    /// </summary>
    public bool Stalemate { get; init; }

    /// <summary>
    /// Whether it's a variant win (only in chess variants).
    /// </summary>
    public bool VariantWin { get; init; }

    /// <summary>
    /// Whether it's a variant loss (only in chess variants).
    /// </summary>
    public bool VariantLoss { get; init; }

    /// <summary>
    /// Whether insufficient material.
    /// </summary>
    public bool InsufficientMaterial { get; init; }

    /// <summary>
    /// Information about legal moves, best first.
    /// </summary>
    public IReadOnlyList<TablebaseMove>? Moves { get; init; }
}

/// <summary>
/// Tablebase move.
/// </summary>
public class TablebaseMove
{
    /// <summary>
    /// UCI notation.
    /// </summary>
    public required string Uci { get; init; }

    /// <summary>
    /// SAN notation.
    /// </summary>
    public required string San { get; init; }

    /// <summary>
    /// Category after this move.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// DTZ after this move.
    /// </summary>
    public int? Dtz { get; init; }

    /// <summary>
    /// Precise DTZ after this move.
    /// </summary>
    public int? PreciseDtz { get; init; }

    /// <summary>
    /// DTC (Depth to Conversion) after this move.
    /// </summary>
    public int? Dtc { get; init; }

    /// <summary>
    /// DTM (Depth to Mate) after this move.
    /// </summary>
    public int? Dtm { get; init; }

    /// <summary>
    /// DTW (Depth to Win) after this move.
    /// </summary>
    public int? Dtw { get; init; }

    /// <summary>
    /// Whether this is a zeroing move (pawn move or capture).
    /// </summary>
    public bool Zeroing { get; init; }

    /// <summary>
    /// Whether this is a conversion move (capture or promotion).
    /// </summary>
    public bool Conversion { get; init; }

    /// <summary>
    /// Whether this move leads to checkmate.
    /// </summary>
    public bool Checkmate { get; init; }

    /// <summary>
    /// Whether this move leads to stalemate.
    /// </summary>
    public bool Stalemate { get; init; }

    /// <summary>
    /// Whether this is a variant win (only in chess variants).
    /// </summary>
    public bool VariantWin { get; init; }

    /// <summary>
    /// Whether this is a variant loss (only in chess variants).
    /// </summary>
    public bool VariantLoss { get; init; }

    /// <summary>
    /// Whether insufficient material.
    /// </summary>
    public bool InsufficientMaterial { get; init; }
}
