namespace LichessSharp.Api;

/// <summary>
/// Bot API - Play on Lichess as a bot with engine assistance.
/// Only works with Bot accounts.
/// </summary>
public interface IBotApi
{
    /// <summary>
    /// Upgrade account to bot account. Cannot be undone!
    /// </summary>
    Task<bool> UpgradeAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream incoming events (challenges, game starts).
    /// </summary>
    IAsyncEnumerable<BotEvent> StreamEventsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream a game state.
    /// </summary>
    IAsyncEnumerable<BotGameEvent> StreamGameAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Make a move.
    /// </summary>
    Task<bool> MakeMoveAsync(string gameId, string move, bool? offeringDraw = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Write in the chat.
    /// </summary>
    Task<bool> WriteChatAsync(string gameId, string room, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Abort a game.
    /// </summary>
    Task<bool> AbortAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resign a game.
    /// </summary>
    Task<bool> ResignAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get online bots.
    /// </summary>
    IAsyncEnumerable<BotInfo> GetOnlineBotsAsync(int? count = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Bot event.
/// </summary>
public class BotEvent
{
    /// <summary>
    /// Event type.
    /// </summary>
    public required string Type { get; init; }
}

/// <summary>
/// Bot game event.
/// </summary>
public class BotGameEvent
{
    /// <summary>
    /// Event type.
    /// </summary>
    public required string Type { get; init; }
}

/// <summary>
/// Bot information.
/// </summary>
public class BotInfo
{
    /// <summary>
    /// Bot username.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Whether online.
    /// </summary>
    public bool Online { get; init; }
}
