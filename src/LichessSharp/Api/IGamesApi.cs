using LichessSharp.Api.Options;
using LichessSharp.Models;

namespace LichessSharp.Api;

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
    Task<GameJson> GetAsync(string gameId, ExportGameOptions? options = null, CancellationToken cancellationToken = default);

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
    Task<GameJson> GetCurrentGameAsync(string username, ExportGameOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream games of a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="options">Optional export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of games.</returns>
    IAsyncEnumerable<GameJson> StreamUserGamesAsync(string username, ExportUserGamesOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export games by IDs.
    /// </summary>
    /// <param name="gameIds">The game IDs (up to 300).</param>
    /// <param name="options">Optional export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of games.</returns>
    IAsyncEnumerable<GameJson> StreamByIdsAsync(IEnumerable<string> gameIds, ExportGameOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream the ongoing games of specified users.
    /// </summary>
    /// <param name="userIds">The user IDs (up to 300).</param>
    /// <param name="withCurrentGames">Include games that started before the stream was opened.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of game events.</returns>
    IAsyncEnumerable<GameJson> StreamGamesByUsersAsync(IEnumerable<string> userIds, bool withCurrentGames = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get ongoing games of the authenticated user.
    /// </summary>
    /// <param name="count">Maximum number of games to return (1-50, default 9).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of ongoing games.</returns>
    Task<IReadOnlyList<Game>> GetOngoingGamesAsync(int count = 9, CancellationToken cancellationToken = default);

    /// <summary>
    /// Import a game from PGN.
    /// </summary>
    /// <param name="pgn">The PGN content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The imported game.</returns>
    Task<Game> ImportPgnAsync(string pgn, CancellationToken cancellationToken = default);
}
