using LichessSharp.Api.Contracts;

namespace LichessSharp;

/// <summary>
///     The main client interface for interacting with the Lichess API.
///     Provides access to all Lichess API surface areas through dedicated sub-clients.
/// </summary>
public interface ILichessClient : IDisposable
{
    /// <summary>
    ///     Access to Account API endpoints.
    ///     Read and write account information and preferences.
    /// </summary>
    IAccountApi Account { get; }

    /// <summary>
    ///     Access to Users API endpoints.
    ///     Access registered users on Lichess.
    /// </summary>
    IUsersApi Users { get; }

    /// <summary>
    ///     Access to Relations API endpoints.
    ///     Follow, unfollow, block, and unblock users.
    /// </summary>
    IRelationsApi Relations { get; }

    /// <summary>
    ///     Access to Games API endpoints.
    ///     Export and stream games played on Lichess.
    /// </summary>
    IGamesApi Games { get; }

    /// <summary>
    ///     Access to TV API endpoints.
    ///     Access Lichess TV channels and games.
    /// </summary>
    ITvApi Tv { get; }

    /// <summary>
    ///     Access to Puzzles API endpoints.
    ///     Access Lichess puzzle history and dashboard.
    /// </summary>
    IPuzzlesApi Puzzles { get; }

    /// <summary>
    ///     Access to Teams API endpoints.
    ///     Access and manage Lichess teams and their members.
    /// </summary>
    ITeamsApi Teams { get; }

    /// <summary>
    ///     Access to Board API endpoints.
    ///     Play on Lichess with physical boards and third-party clients.
    /// </summary>
    IBoardApi Board { get; }

    /// <summary>
    ///     Access to Bot API endpoints.
    ///     Play on Lichess as a bot with engine assistance.
    /// </summary>
    IBotApi Bot { get; }

    /// <summary>
    ///     Access to Challenges API endpoints.
    ///     Send and receive challenges to play.
    /// </summary>
    IChallengesApi Challenges { get; }

    /// <summary>
    ///     Access to Bulk Pairings API endpoints.
    ///     Create many games for other players.
    /// </summary>
    IBulkPairingsApi BulkPairings { get; }

    /// <summary>
    ///     Access to Arena Tournaments API endpoints.
    ///     Access Arena tournaments played on Lichess.
    /// </summary>
    IArenaTournamentsApi ArenaTournaments { get; }

    /// <summary>
    ///     Access to Swiss Tournaments API endpoints.
    ///     Access Swiss tournaments played on Lichess.
    /// </summary>
    ISwissTournamentsApi SwissTournaments { get; }

    /// <summary>
    ///     Access to Simuls API endpoints.
    ///     Access simultaneous exhibitions played on Lichess.
    /// </summary>
    ISimulsApi Simuls { get; }

    /// <summary>
    ///     Access to Studies API endpoints.
    ///     Access Lichess studies.
    /// </summary>
    IStudiesApi Studies { get; }

    /// <summary>
    ///     Access to Messaging API endpoints.
    ///     Private messages with other players.
    /// </summary>
    IMessagingApi Messaging { get; }

    /// <summary>
    ///     Access to Broadcasts API endpoints.
    ///     Relay chess events on Lichess.
    /// </summary>
    IBroadcastsApi Broadcasts { get; }

    /// <summary>
    ///     Access to Analysis API endpoints.
    ///     Access Lichess cloud evaluations database.
    /// </summary>
    IAnalysisApi Analysis { get; }

    /// <summary>
    ///     Access to Opening Explorer API endpoints.
    ///     Lookup positions from the Lichess opening explorer.
    /// </summary>
    IOpeningExplorerApi OpeningExplorer { get; }

    /// <summary>
    ///     Access to Tablebase API endpoints.
    ///     Lookup positions from the Lichess tablebase server.
    /// </summary>
    ITablebaseApi Tablebase { get; }

    /// <summary>
    ///     Access to FIDE API endpoints.
    ///     Access FIDE player data.
    /// </summary>
    IFideApi Fide { get; }

    /// <summary>
    ///     Access to OAuth API endpoints.
    ///     Token management for the Lichess API.
    /// </summary>
    IOAuthApi OAuth { get; }

    /// <summary>
    ///     Access to External Engine API endpoints.
    ///     Register and manage external analysis engines.
    ///     WARNING: This API is in alpha and subject to change.
    /// </summary>
    IExternalEngineApi ExternalEngine { get; }
}