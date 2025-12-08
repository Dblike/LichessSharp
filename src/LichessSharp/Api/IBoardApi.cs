namespace LichessSharp.Api;

/// <summary>
/// Board API - Play on Lichess with physical boards and third-party clients.
/// For playing games with human accounts (not bots).
/// </summary>
public interface IBoardApi
{
    /// <summary>
    /// Stream incoming events (challenges, game starts).
    /// </summary>
    IAsyncEnumerable<BoardEvent> StreamEventsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream a game state.
    /// </summary>
    IAsyncEnumerable<BoardGameEvent> StreamGameAsync(string gameId, CancellationToken cancellationToken = default);

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
    /// Handle draw offers.
    /// </summary>
    Task<bool> HandleDrawOfferAsync(string gameId, bool accept, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handle takeback proposals.
    /// </summary>
    Task<bool> HandleTakebackAsync(string gameId, bool accept, CancellationToken cancellationToken = default);

    /// <summary>
    /// Claim victory when opponent has abandoned the game.
    /// </summary>
    Task<bool> ClaimVictoryAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Seek a game with random opponent.
    /// </summary>
    IAsyncEnumerable<BoardSeekEvent> SeekAsync(BoardSeekOptions options, CancellationToken cancellationToken = default);
}

/// <summary>
/// Board event (challenge or game start).
/// </summary>
public class BoardEvent
{
    /// <summary>
    /// Event type.
    /// </summary>
    public required string Type { get; init; }
}

/// <summary>
/// Board game event.
/// </summary>
public class BoardGameEvent
{
    /// <summary>
    /// Event type (gameFull, gameState, chatLine).
    /// </summary>
    public required string Type { get; init; }
}

/// <summary>
/// Board seek event.
/// </summary>
public class BoardSeekEvent
{
    /// <summary>
    /// Event type.
    /// </summary>
    public required string Type { get; init; }
}

/// <summary>
/// Options for seeking a game.
/// </summary>
public class BoardSeekOptions
{
    /// <summary>
    /// Whether the game should be rated.
    /// </summary>
    public bool Rated { get; set; }

    /// <summary>
    /// Clock initial time in minutes.
    /// </summary>
    public int Time { get; set; }

    /// <summary>
    /// Clock increment in seconds.
    /// </summary>
    public int Increment { get; set; }

    /// <summary>
    /// The variant.
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// Minimum opponent rating.
    /// </summary>
    public int? RatingMin { get; set; }

    /// <summary>
    /// Maximum opponent rating.
    /// </summary>
    public int? RatingMax { get; set; }
}
