using LichessSharp.Api.Options;
using LichessSharp.Models;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Games API - Export and stream games played on Lichess.
/// </summary>
public interface IGamesApi
{
    /// <summary>
    /// Export a single game by ID.
    /// </summary>
    /// <param name="gameId">The game ID (8 characters).</param>
    /// <param name="options">Optional export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The game in JSON format.</returns>
    Task<GameJson> ExportAsync(string gameId, ExportGameOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export a single game by ID in PGN format.
    /// </summary>
    /// <param name="gameId">The game ID (8 characters).</param>
    /// <param name="options">Optional export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The game in PGN format.</returns>
    Task<string> GetPgnAsync(string gameId, ExportGameOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export the ongoing game, or last game, of a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="options">Optional export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The game in JSON format.</returns>
    Task<GameJson> GetCurrentGameByUserAsync(string username, ExportGameOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream the ongoing games of specified users.
    /// </summary>
    /// <param name="userIds">The user IDs (up to 300).</param>
    /// <param name="withCurrentGames">Include games that started before the stream was opened.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of game events.</returns>
    IAsyncEnumerable<GameJson> StreamByUsersAsync(IEnumerable<string> userIds, bool withCurrentGames = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export games by IDs.
    /// </summary>
    /// <param name="gameIds">The game IDs (up to 300).</param>
    /// <param name="options">Optional export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of games.</returns>
    IAsyncEnumerable<GameJson> StreamByIdsAsync(IEnumerable<string> gameIds, ExportGameOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream games of a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="options">Optional export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of games.</returns>
    IAsyncEnumerable<GameJson> StreamUserGamesAsync(string username, ExportUserGamesOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a stream to watch multiple games by their IDs.
    /// The stream outputs existing games immediately, then emits events when games start or finish.
    /// No authentication required.
    /// </summary>
    /// <param name="streamId">Your unique stream identifier (can be any string).</param>
    /// <param name="gameIds">Initial game IDs to watch (up to 500).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of game status events.</returns>
    IAsyncEnumerable<GameStreamEvent> StreamByIdsAsync(string streamId, IEnumerable<string> gameIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add game IDs to an existing stream created with StreamByIdsAsync.
    /// No authentication required.
    /// </summary>
    /// <param name="streamId">The stream identifier used when creating the stream.</param>
    /// <param name="gameIds">Game IDs to add to the stream (up to 500).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddGameIdsToStreamAsync(string streamId, IEnumerable<string> gameIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get ongoing games of the authenticated user.
    /// </summary>
    /// <param name="count">Maximum number of games to return (1-50, default 9).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of ongoing games.</returns>
    Task<IReadOnlyList<OngoingGame>> GetOngoingGamesAsync(int count = 9, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream a single game's moves in real-time.
    /// The first event contains full game info, subsequent events contain move updates.
    /// No authentication required.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of game move events.</returns>
    IAsyncEnumerable<MoveStreamEvent> StreamGameMovesAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Import a game from PGN.
    /// </summary>
    /// <param name="pgn">The PGN content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The imported game response containing the game ID and URL.</returns>
    Task<ImportGameResponse> ImportAsync(string pgn, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export all games you have imported, in PGN format.
    /// Requires OAuth authentication.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All imported games in PGN format.</returns>
    Task<string> ExportImportedGamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream all games you have bookmarked, in NDJSON format.
    /// Requires OAuth authentication.
    /// Games are sorted by reverse chronological order (most recent first).
    /// </summary>
    /// <param name="options">Optional export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of bookmarked games.</returns>
    IAsyncEnumerable<GameJson> StreamBookmarkedGamesAsync(ExportBookmarksOptions? options = null, CancellationToken cancellationToken = default);
}
