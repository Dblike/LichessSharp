using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// FIDE API - Access FIDE player data from Lichess.
/// </summary>
public interface IFideApi
{
    /// <summary>
    /// Get information about a FIDE player by their FIDE ID.
    /// </summary>
    /// <param name="playerId">The FIDE player ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>FIDE player information.</returns>
    Task<FidePlayer> GetPlayerAsync(int playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for FIDE players by name.
    /// </summary>
    /// <param name="query">Search query (player name).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching FIDE players.</returns>
    Task<IReadOnlyList<FidePlayer>> SearchPlayersAsync(string query, CancellationToken cancellationToken = default);
}
