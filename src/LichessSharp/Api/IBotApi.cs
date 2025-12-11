using System.Text.Json.Serialization;

namespace LichessSharp.Api;

/// <summary>
/// Bot API - Play on Lichess as a bot with engine assistance.
/// Only works with Bot accounts. Use <see cref="UpgradeAccountAsync"/> to convert a regular account.
/// <see href="https://lichess.org/api#tag/Bot"/>
/// </summary>
public interface IBotApi
{
    /// <summary>
    /// Upgrade a regular Lichess account to a Bot account.
    /// WARNING: This action is irreversible! The account must have played no games.
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> UpgradeAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream incoming events for your bot account (game starts, challenges, etc.).
    /// This is the main event loop for Bot API clients.
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of account events.</returns>
    IAsyncEnumerable<BotAccountEvent> StreamEventsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream the full state of a game being played.
    /// Use this to track the game state in real-time.
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of game events (gameFull, gameState, chatLine, opponentGone).</returns>
    IAsyncEnumerable<BotGameEvent> StreamGameAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Make a move in a game.
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="move">The move in UCI format (e.g., "e2e4", "e7e8q").</param>
    /// <param name="offeringDraw">Optionally offer or accept a draw.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> MakeMoveAsync(string gameId, string move, bool? offeringDraw = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the chat messages of a game.
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of chat messages.</returns>
    Task<IReadOnlyList<ChatMessage>> GetChatAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Write a message in the game chat.
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="room">The chat room ("player" or "spectator").</param>
    /// <param name="text">The message text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> WriteChatAsync(string gameId, ChatRoom room, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Abort a game.
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> AbortAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resign a game.
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> ResignAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handle a draw offer (offer, accept, or decline).
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="accept">True to offer/accept, false to decline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> HandleDrawAsync(string gameId, bool accept, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handle a takeback proposal (offer, accept, or decline).
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="accept">True to offer/accept, false to decline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> HandleTakebackAsync(string gameId, bool accept, CancellationToken cancellationToken = default);

    /// <summary>
    /// Claim a draw by the 50-move rule, or by threefold repetition.
    /// Requires OAuth scope: bot:play
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> ClaimDrawAsync(string gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get online bots.
    /// </summary>
    /// <param name="count">Maximum number of bots to fetch (default: 50, max: 300).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of online bot information.</returns>
    IAsyncEnumerable<BotUser> GetOnlineBotsAsync(int? count = null, CancellationToken cancellationToken = default);
}

#region Models

/// <summary>
/// Event from the bot account event stream.
/// </summary>
public class BotAccountEvent
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
    public BotAccountGameInfo? Game { get; init; }

    /// <summary>
    /// Challenge information (for challenge, challengeCanceled, challengeDeclined events).
    /// </summary>
    [JsonPropertyName("challenge")]
    public ChallengeJson? Challenge { get; init; }
}

/// <summary>
/// Game information in bot account events.
/// </summary>
public class BotAccountGameInfo
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
    public BotGameStatus? Status { get; init; }

    /// <summary>
    /// Variant information.
    /// </summary>
    [JsonPropertyName("variant")]
    public BotVariant? Variant { get; init; }

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
    public BotOpponent? Opponent { get; init; }

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
    public BotCompat? Compat { get; init; }
}

/// <summary>
/// Game status in bot events.
/// </summary>
public class BotGameStatus
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
/// Variant information in bot events.
/// </summary>
public class BotVariant
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
/// Opponent information in bot events.
/// </summary>
public class BotOpponent
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
/// Client compatibility information for bots.
/// </summary>
public class BotCompat
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

/// <summary>
/// Event from the bot game stream.
/// </summary>
public class BotGameEvent
{
    /// <summary>
    /// Event type ("gameFull", "gameState", "chatLine", "opponentGone").
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }
}

/// <summary>
/// Full game information event for bots (first event in game stream).
/// </summary>
public class BotGameFullEvent : BotGameEvent
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
    public BotVariant? Variant { get; init; }

    /// <summary>
    /// Clock settings.
    /// </summary>
    [JsonPropertyName("clock")]
    public BotClock? Clock { get; init; }

    /// <summary>
    /// Game speed.
    /// </summary>
    [JsonPropertyName("speed")]
    public string? Speed { get; init; }

    /// <summary>
    /// Performance type.
    /// </summary>
    [JsonPropertyName("perf")]
    public BotPerf? Perf { get; init; }

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
    public BotPlayer? White { get; init; }

    /// <summary>
    /// Black player information.
    /// </summary>
    [JsonPropertyName("black")]
    public BotPlayer? Black { get; init; }

    /// <summary>
    /// Initial FEN position.
    /// </summary>
    [JsonPropertyName("initialFen")]
    public string? InitialFen { get; init; }

    /// <summary>
    /// Current game state.
    /// </summary>
    [JsonPropertyName("state")]
    public BotGameStateEvent? State { get; init; }

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
/// Game state update event for bots.
/// </summary>
public class BotGameStateEvent : BotGameEvent
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
/// Chat line event for bots.
/// </summary>
public class BotChatLineEvent : BotGameEvent
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
/// Opponent gone event for bots.
/// </summary>
public class BotOpponentGoneEvent : BotGameEvent
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

/// <summary>
/// Clock settings in bot game events.
/// </summary>
public class BotClock
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
/// Performance type in bot game events.
/// </summary>
public class BotPerf
{
    /// <summary>
    /// Performance name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Player information in bot game events.
/// </summary>
public class BotPlayer
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
/// Bot user information.
/// </summary>
public class BotUser
{
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Username.
    /// </summary>
    [JsonPropertyName("username")]
    public required string Username { get; init; }

    /// <summary>
    /// User title (should be "BOT").
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Whether the bot is online.
    /// </summary>
    [JsonPropertyName("online")]
    public bool Online { get; init; }

    /// <summary>
    /// Whether the bot is playing.
    /// </summary>
    [JsonPropertyName("playing")]
    public bool Playing { get; init; }

    /// <summary>
    /// Performance ratings for different time controls.
    /// </summary>
    [JsonPropertyName("perfs")]
    public BotPerfs? Perfs { get; init; }

    /// <summary>
    /// When the account was created (milliseconds since epoch).
    /// </summary>
    [JsonPropertyName("createdAt")]
    public long CreatedAt { get; init; }

    /// <summary>
    /// When the account was last seen (milliseconds since epoch).
    /// </summary>
    [JsonPropertyName("seenAt")]
    public long SeenAt { get; init; }

    /// <summary>
    /// Play time statistics.
    /// </summary>
    [JsonPropertyName("playTime")]
    public BotPlayTime? PlayTime { get; init; }

    /// <summary>
    /// Whether this bot account is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }

    /// <summary>
    /// Whether this bot account is banned for violating TOS.
    /// </summary>
    [JsonPropertyName("tosViolation")]
    public bool? TosViolation { get; init; }
}

/// <summary>
/// Bot performance ratings.
/// </summary>
public class BotPerfs
{
    /// <summary>
    /// Bullet performance.
    /// </summary>
    [JsonPropertyName("bullet")]
    public BotPerfStats? Bullet { get; init; }

    /// <summary>
    /// Blitz performance.
    /// </summary>
    [JsonPropertyName("blitz")]
    public BotPerfStats? Blitz { get; init; }

    /// <summary>
    /// Rapid performance.
    /// </summary>
    [JsonPropertyName("rapid")]
    public BotPerfStats? Rapid { get; init; }

    /// <summary>
    /// Classical performance.
    /// </summary>
    [JsonPropertyName("classical")]
    public BotPerfStats? Classical { get; init; }

    /// <summary>
    /// Correspondence performance.
    /// </summary>
    [JsonPropertyName("correspondence")]
    public BotPerfStats? Correspondence { get; init; }

    /// <summary>
    /// Chess960 performance.
    /// </summary>
    [JsonPropertyName("chess960")]
    public BotPerfStats? Chess960 { get; init; }
}

/// <summary>
/// Performance statistics for a time control.
/// </summary>
public class BotPerfStats
{
    /// <summary>
    /// Number of games played.
    /// </summary>
    [JsonPropertyName("games")]
    public int Games { get; init; }

    /// <summary>
    /// Current rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; init; }

    /// <summary>
    /// Rating deviation.
    /// </summary>
    [JsonPropertyName("rd")]
    public int Rd { get; init; }

    /// <summary>
    /// Progress in last x games.
    /// </summary>
    [JsonPropertyName("prog")]
    public int Prog { get; init; }

    /// <summary>
    /// Whether the rating is provisional.
    /// </summary>
    [JsonPropertyName("prov")]
    public bool? Provisional { get; init; }
}

/// <summary>
/// Bot play time statistics.
/// </summary>
public class BotPlayTime
{
    /// <summary>
    /// Total play time in seconds.
    /// </summary>
    [JsonPropertyName("total")]
    public long Total { get; init; }

    /// <summary>
    /// TV time in seconds (time featured on Lichess TV).
    /// </summary>
    [JsonPropertyName("tv")]
    public long Tv { get; init; }
}

#endregion
