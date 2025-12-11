using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace LichessSharp.Api;

/// <summary>
/// Board API - Play on Lichess with physical boards and third-party clients.
/// For playing games with human accounts (not bots).
/// <see href="https://lichess.org/api#tag/Board"/>
/// </summary>
public interface IBoardApi
{
    /// <summary>
    /// Stream incoming events for your account (game starts, challenges, etc.).
    /// This is the main event loop for Board API clients.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of account events.</returns>
    IAsyncEnumerable<BoardAccountEvent> StreamEventsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream the full state of a game being played.
    /// Use this to track the game state in real-time.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of game events (gameFull, gameState, chatLine, opponentGone).</returns>
    IAsyncEnumerable<BoardGameEvent> StreamGameAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Make a move in a game.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="move">The move in UCI format (e.g., "e2e4", "e7e8q").</param>
    /// <param name="offeringDraw">Optionally offer or accept a draw.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> MakeMoveAsync(string gameId, string move, bool? offeringDraw = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the chat messages of a game.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of chat messages.</returns>
    Task<IReadOnlyList<ChatMessage>> GetChatAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Write a message in the game chat.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="room">The chat room ("player" or "spectator").</param>
    /// <param name="text">The message text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> WriteChatAsync(string gameId, ChatRoom room, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Abort a game.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> AbortAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resign a game.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> ResignAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handle a draw offer (offer, accept, or decline).
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="accept">True to offer/accept, false to decline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> HandleDrawAsync(string gameId, bool accept, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handle a takeback proposal (offer, accept, or decline).
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="accept">True to offer/accept, false to decline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> HandleTakebackAsync(string gameId, bool accept, CancellationToken cancellationToken = default);

    /// <summary>
    /// Claim victory when opponent has abandoned the game.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> ClaimVictoryAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Claim a draw by the 50-move rule, or by threefold repetition.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> ClaimDrawAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Go berserk on a tournament game (halve your clock for +1 point).
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> BerserkAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a public seek to find a random opponent.
    /// Stream the game start event when a game is found.
    /// Requires OAuth scope: board:play
    /// </summary>
    /// <param name="options">Seek options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream that yields the game ID when matched.</returns>
    IAsyncEnumerable<SeekResult> SeekAsync(SeekOptions options, CancellationToken cancellationToken = default);
}

#region Enums

/// <summary>
/// Chat room type.
/// </summary>
public enum ChatRoom
{
    /// <summary>
    /// Player chat (only visible to players).
    /// </summary>
    Player,

    /// <summary>
    /// Spectator chat (visible to everyone).
    /// </summary>
    Spectator
}

#endregion

#region Account Events

/// <summary>
/// Event from the board account event stream.
/// </summary>
public class BoardAccountEvent
{
    /// <summary>
    /// Event type ("gameStart", "gameFinish", "challenge", "challengeCanceled", "challengeDeclined").
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Game information (for gameStart, gameFinish events).
    /// </summary>
    [JsonPropertyName("game")]
    public BoardAccountGameInfo? Game { get; init; }

    /// <summary>
    /// Challenge information (for challenge, challengeCanceled, challengeDeclined events).
    /// </summary>
    [JsonPropertyName("challenge")]
    public ChallengeJson? Challenge { get; init; }
}

/// <summary>
/// Game information in account events.
/// </summary>
public class BoardAccountGameInfo
{
    /// <summary>
    /// The full game ID (includes player color suffix).
    /// </summary>
    [JsonPropertyName("fullId")]
    public string? FullId { get; init; }

    /// <summary>
    /// The game ID.
    /// </summary>
    [JsonPropertyName("gameId")]
    public string? GameId { get; init; }

    /// <summary>
    /// Current FEN position.
    /// </summary>
    [JsonPropertyName("fen")]
    public string? Fen { get; init; }

    /// <summary>
    /// Your color in the game.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    /// Last move in UCI format.
    /// </summary>
    [JsonPropertyName("lastMove")]
    public string? LastMove { get; init; }

    /// <summary>
    /// Game source.
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; init; }

    /// <summary>
    /// Game status.
    /// </summary>
    [JsonPropertyName("status")]
    public BoardGameStatus? Status { get; init; }

    /// <summary>
    /// Variant information.
    /// </summary>
    [JsonPropertyName("variant")]
    public BoardVariant? Variant { get; init; }

    /// <summary>
    /// Game speed.
    /// </summary>
    [JsonPropertyName("speed")]
    public string? Speed { get; init; }

    /// <summary>
    /// Performance type.
    /// </summary>
    [JsonPropertyName("perf")]
    public string? Perf { get; init; }

    /// <summary>
    /// Whether the game is rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    /// Whether a move has been made.
    /// </summary>
    [JsonPropertyName("hasMoved")]
    public bool HasMoved { get; init; }

    /// <summary>
    /// Opponent information.
    /// </summary>
    [JsonPropertyName("opponent")]
    public BoardOpponent? Opponent { get; init; }

    /// <summary>
    /// Whether it's your turn.
    /// </summary>
    [JsonPropertyName("isMyTurn")]
    public bool IsMyTurn { get; init; }

    /// <summary>
    /// Seconds left on your clock.
    /// </summary>
    [JsonPropertyName("secondsLeft")]
    public int? SecondsLeft { get; init; }

    /// <summary>
    /// Number of available rematches.
    /// </summary>
    [JsonPropertyName("rematches")]
    public int? Rematches { get; init; }

    /// <summary>
    /// Tournament ID if in a tournament.
    /// </summary>
    [JsonPropertyName("tournamentId")]
    public string? TournamentId { get; init; }

    /// <summary>
    /// Swiss ID if in a swiss tournament.
    /// </summary>
    [JsonPropertyName("swissId")]
    public string? SwissId { get; init; }

    /// <summary>
    /// Game winner color.
    /// </summary>
    [JsonPropertyName("winner")]
    public string? Winner { get; init; }

    /// <summary>
    /// Compat information.
    /// </summary>
    [JsonPropertyName("compat")]
    public BoardCompat? Compat { get; init; }
}

/// <summary>
/// Game status in board events.
/// </summary>
public class BoardGameStatus
{
    /// <summary>
    /// Status ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Status name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Variant information in board events.
/// </summary>
public class BoardVariant
{
    /// <summary>
    /// Variant key.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    /// <summary>
    /// Variant name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Opponent information.
/// </summary>
public class BoardOpponent
{
    /// <summary>
    /// Opponent user ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Opponent username.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }

    /// <summary>
    /// Opponent rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    /// AI level if playing against AI.
    /// </summary>
    [JsonPropertyName("ai")]
    public int? Ai { get; init; }
}

/// <summary>
/// Client compatibility information.
/// </summary>
public class BoardCompat
{
    /// <summary>
    /// Whether the game is compatible with the Bot API.
    /// </summary>
    [JsonPropertyName("bot")]
    public bool Bot { get; init; }

    /// <summary>
    /// Whether the game is compatible with the Board API.
    /// </summary>
    [JsonPropertyName("board")]
    public bool Board { get; init; }
}

#endregion

#region Game Events

/// <summary>
/// Event from the board game stream.
/// </summary>
public class BoardGameEvent
{
    /// <summary>
    /// Event type ("gameFull", "gameState", "chatLine", "opponentGone").
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }
}

/// <summary>
/// Full game information event (first event in game stream).
/// </summary>
public class GameFullEvent : BoardGameEvent
{
    /// <summary>
    /// The game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Variant information.
    /// </summary>
    [JsonPropertyName("variant")]
    public BoardVariant? Variant { get; init; }

    /// <summary>
    /// Clock settings.
    /// </summary>
    [JsonPropertyName("clock")]
    public BoardClock? Clock { get; init; }

    /// <summary>
    /// Game speed.
    /// </summary>
    [JsonPropertyName("speed")]
    public string? Speed { get; init; }

    /// <summary>
    /// Performance type.
    /// </summary>
    [JsonPropertyName("perf")]
    public BoardPerf? Perf { get; init; }

    /// <summary>
    /// Whether the game is rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    /// When the game was created (milliseconds since epoch).
    /// </summary>
    [JsonPropertyName("createdAt")]
    public long CreatedAt { get; init; }

    /// <summary>
    /// White player information.
    /// </summary>
    [JsonPropertyName("white")]
    public BoardPlayer? White { get; init; }

    /// <summary>
    /// Black player information.
    /// </summary>
    [JsonPropertyName("black")]
    public BoardPlayer? Black { get; init; }

    /// <summary>
    /// Initial FEN position.
    /// </summary>
    [JsonPropertyName("initialFen")]
    public string? InitialFen { get; init; }

    /// <summary>
    /// Current game state.
    /// </summary>
    [JsonPropertyName("state")]
    public GameStateEvent? State { get; init; }

    /// <summary>
    /// Tournament ID if in a tournament.
    /// </summary>
    [JsonPropertyName("tournamentId")]
    public string? TournamentId { get; init; }

    /// <summary>
    /// Swiss ID if in a swiss tournament.
    /// </summary>
    [JsonPropertyName("swissId")]
    public string? SwissId { get; init; }

    /// <summary>
    /// Days per turn for correspondence games.
    /// </summary>
    [JsonPropertyName("daysPerTurn")]
    public int? DaysPerTurn { get; init; }
}

/// <summary>
/// Game state update event.
/// </summary>
public class GameStateEvent : BoardGameEvent
{
    /// <summary>
    /// All moves played so far in UCI format, space-separated.
    /// </summary>
    [JsonPropertyName("moves")]
    public string? Moves { get; init; }

    /// <summary>
    /// White's remaining time in milliseconds.
    /// </summary>
    [JsonPropertyName("wtime")]
    public long? WhiteTime { get; init; }

    /// <summary>
    /// Black's remaining time in milliseconds.
    /// </summary>
    [JsonPropertyName("btime")]
    public long? BlackTime { get; init; }

    /// <summary>
    /// White's increment in milliseconds.
    /// </summary>
    [JsonPropertyName("winc")]
    public int? WhiteIncrement { get; init; }

    /// <summary>
    /// Black's increment in milliseconds.
    /// </summary>
    [JsonPropertyName("binc")]
    public int? BlackIncrement { get; init; }

    /// <summary>
    /// Game status.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Winner color ("white" or "black") if the game is over.
    /// </summary>
    [JsonPropertyName("winner")]
    public string? Winner { get; init; }

    /// <summary>
    /// True if white is offering a draw.
    /// </summary>
    [JsonPropertyName("wdraw")]
    public bool? WhiteOfferingDraw { get; init; }

    /// <summary>
    /// True if black is offering a draw.
    /// </summary>
    [JsonPropertyName("bdraw")]
    public bool? BlackOfferingDraw { get; init; }

    /// <summary>
    /// True if white is proposing a takeback.
    /// </summary>
    [JsonPropertyName("wtakeback")]
    public bool? WhiteProposingTakeback { get; init; }

    /// <summary>
    /// True if black is proposing a takeback.
    /// </summary>
    [JsonPropertyName("btakeback")]
    public bool? BlackProposingTakeback { get; init; }

    /// <summary>
    /// Rematch game ID if rematching.
    /// </summary>
    [JsonPropertyName("rematch")]
    public string? Rematch { get; init; }
}

/// <summary>
/// Chat line event.
/// </summary>
public class ChatLineEvent : BoardGameEvent
{
    /// <summary>
    /// Chat room ("player" or "spectator").
    /// </summary>
    [JsonPropertyName("room")]
    public string? Room { get; init; }

    /// <summary>
    /// Username of the sender.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }

    /// <summary>
    /// Message text.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

/// <summary>
/// Opponent gone event (opponent disconnected).
/// </summary>
public class OpponentGoneEvent : BoardGameEvent
{
    /// <summary>
    /// Whether the opponent is gone.
    /// </summary>
    [JsonPropertyName("gone")]
    public bool Gone { get; init; }

    /// <summary>
    /// Seconds until you can claim victory (-1 if not applicable).
    /// </summary>
    [JsonPropertyName("claimWinInSeconds")]
    public int? ClaimWinInSeconds { get; init; }
}

#endregion

#region Supporting Types

/// <summary>
/// Clock settings in board game events.
/// </summary>
public class BoardClock
{
    /// <summary>
    /// Initial time in milliseconds.
    /// </summary>
    [JsonPropertyName("initial")]
    public long Initial { get; init; }

    /// <summary>
    /// Increment in milliseconds.
    /// </summary>
    [JsonPropertyName("increment")]
    public int Increment { get; init; }
}

/// <summary>
/// Performance type in board game events.
/// </summary>
public class BoardPerf
{
    /// <summary>
    /// Performance name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Player information in board game events.
/// </summary>
public class BoardPlayer
{
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Username.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Player title (GM, IM, BOT, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Player rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    /// Whether the rating is provisional.
    /// </summary>
    [JsonPropertyName("provisional")]
    public bool? Provisional { get; init; }

    /// <summary>
    /// AI level if playing against AI.
    /// </summary>
    [JsonPropertyName("aiLevel")]
    public int? AiLevel { get; init; }
}

/// <summary>
/// Chat message.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Username of the sender.
    /// </summary>
    [JsonPropertyName("user")]
    public string? User { get; init; }

    /// <summary>
    /// Message text.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

/// <summary>
/// Result from a seek operation.
/// </summary>
public class SeekResult
{
    /// <summary>
    /// The game ID when a match is found.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}

#endregion

#region Options

/// <summary>
/// Options for creating a seek.
/// </summary>
public class SeekOptions
{
    /// <summary>
    /// Whether the game should be rated (default: false).
    /// </summary>
    public bool Rated { get; set; }

    /// <summary>
    /// Clock initial time in minutes (required).
    /// Must be between 0 and 180.
    /// </summary>
    public int Time { get; set; }

    /// <summary>
    /// Clock increment in seconds (required).
    /// Must be between 0 and 180.
    /// </summary>
    public int Increment { get; set; }

    /// <summary>
    /// Days per turn for correspondence games.
    /// Must be 1, 2, 3, 5, 7, 10, or 14.
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// Chess variant (default: "standard").
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// Color preference.
    /// </summary>
    public ChallengeColor? Color { get; set; }

    /// <summary>
    /// Minimum opponent rating.
    /// </summary>
    public int? RatingMin { get; set; }

    /// <summary>
    /// Maximum opponent rating.
    /// </summary>
    public int? RatingMax { get; set; }
}

#endregion
