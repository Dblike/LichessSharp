using System.Text.Json.Serialization;

using LichessSharp.Models.Games;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Swiss Tournaments API - Access Swiss tournaments played on Lichess.
/// See <see href="https://lichess.org/api#tag/Swiss-tournaments"/>.
/// </summary>
public interface ISwissTournamentsApi
{
    /// <summary>
    /// Create a new Swiss tournament for your team.
    /// You can create up to 12 tournaments per day.
    /// </summary>
    /// <param name="teamId">The team ID to create the tournament for.</param>
    /// <param name="options">Tournament creation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created tournament.</returns>
    Task<SwissTournament> CreateAsync(string teamId, SwissCreateOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get info about a Swiss tournament.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tournament info.</returns>
    Task<SwissTournament> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing Swiss tournament.
    /// Be mindful not to make important changes to ongoing tournaments.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="options">Tournament update options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated tournament.</returns>
    Task<SwissTournament> UpdateAsync(string id, SwissUpdateOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Manually schedule the next round date and time.
    /// This sets the round interval to manual scheduling.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="date">The date and time for the next round (Unix timestamp in milliseconds).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successfully scheduled.</returns>
    Task<bool> ScheduleNextRoundAsync(string id, long date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Join a Swiss tournament, possibly with a password.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="password">Optional password for private tournaments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successfully joined.</returns>
    Task<bool> JoinAsync(string id, string? password = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Leave a future Swiss tournament, or take a break on an ongoing Swiss tournament.
    /// It's possible to join again later. Points are preserved.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successfully withdrawn.</returns>
    Task<bool> PauseOrWithdrawAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminate a Swiss tournament.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successfully terminated.</returns>
    Task<bool> TerminateAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export TRF (FIDE) data of a Swiss tournament.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The TRF data as a string.</returns>
    Task<string> ExportTrfAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export games of a Swiss tournament.
    /// Games are sorted by round and board.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="options">Export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of games.</returns>
    IAsyncEnumerable<GameJson> StreamGamesAsync(string id, SwissGamesExportOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get results of a Swiss tournament.
    /// Players are sorted by rank (best first).
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="nb">Max number of players to fetch (default: all).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of player results.</returns>
    IAsyncEnumerable<SwissPlayerResult> StreamResultsAsync(string id, int? nb = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Swiss tournaments for a team.
    /// Tournaments are sorted by reverse chronological order.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="max">Max number of tournaments to fetch (default: 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of team tournaments.</returns>
    IAsyncEnumerable<SwissTournament> StreamTeamTournamentsAsync(string teamId, int? max = null, CancellationToken cancellationToken = default);
}
/// <summary>
/// Swiss tournament status values.
/// </summary>
public enum SwissStatus
{
    /// <summary>
    /// Tournament has been created but not yet started.
    /// </summary>
    Created,

    /// <summary>
    /// Tournament is currently in progress.
    /// </summary>
    Started,

    /// <summary>
    /// Tournament has finished.
    /// </summary>
    Finished
}

/// <summary>
/// Options for creating a Swiss tournament.
/// </summary>
public class SwissCreateOptions
{
    /// <summary>
    /// The tournament name (2 to 30 characters).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Clock time in seconds (0 to 86400, i.e., up to 24 hours).
    /// </summary>
    public required int ClockLimit { get; init; }

    /// <summary>
    /// Clock increment in seconds (0 to 600).
    /// </summary>
    public required int ClockIncrement { get; init; }

    /// <summary>
    /// Maximum number of rounds (3 to 100).
    /// </summary>
    public required int NbRounds { get; init; }

    /// <summary>
    /// Timestamp in milliseconds when the tournament starts.
    /// If not provided, the tournament starts immediately.
    /// </summary>
    public long? StartsAt { get; init; }

    /// <summary>
    /// How long to wait between each round, in seconds.
    /// Set to 99999999 to manually schedule each round.
    /// If empty or -1, a sensible value is picked automatically.
    /// </summary>
    public int? RoundInterval { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    public string? Variant { get; init; }

    /// <summary>
    /// Tournament description (max 400 chars, Markdown supported).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Whether the tournament is rated. Defaults to true.
    /// </summary>
    public bool? Rated { get; init; }

    /// <summary>
    /// Password to join the tournament.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Forbid late pairings. When checked, late-joining players won't be matched with players
    /// who have already played all their games. Instead they'll be paired with each other.
    /// </summary>
    public bool? ForbiddenPairings { get; init; }

    /// <summary>
    /// Manual pairing: tournament players can be manually paired.
    /// </summary>
    public bool? ManualPairings { get; init; }

    /// <summary>
    /// Whether to add a chat room for participants.
    /// </summary>
    public bool? ChatFor { get; init; }

    /// <summary>
    /// Draw position from FEN for a thematic tournament.
    /// Variant must be standard and the game cannot be rated.
    /// </summary>
    public string? Position { get; init; }

    /// <summary>
    /// Minimum rating to join.
    /// </summary>
    public int? MinRating { get; init; }

    /// <summary>
    /// Maximum rating to join.
    /// </summary>
    public int? MaxRating { get; init; }

    /// <summary>
    /// Minimum number of rated games to join.
    /// </summary>
    public int? MinRatedGames { get; init; }

    /// <summary>
    /// Whether only team leaders can join.
    /// </summary>
    public bool? OnlyLeaders { get; init; }

    /// <summary>
    /// Minimum account age in days.
    /// </summary>
    public int? MinAccountAge { get; init; }

    /// <summary>
    /// Restrict to titled players only.
    /// </summary>
    public bool? OnlyTitled { get; init; }
}

/// <summary>
/// Options for updating a Swiss tournament.
/// </summary>
public class SwissUpdateOptions
{
    /// <summary>
    /// The tournament name (2 to 30 characters).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Clock time in seconds.
    /// </summary>
    public int? ClockLimit { get; init; }

    /// <summary>
    /// Clock increment in seconds.
    /// </summary>
    public int? ClockIncrement { get; init; }

    /// <summary>
    /// Maximum number of rounds.
    /// </summary>
    public int? NbRounds { get; init; }

    /// <summary>
    /// Timestamp in milliseconds when the tournament starts.
    /// </summary>
    public long? StartsAt { get; init; }

    /// <summary>
    /// How long to wait between each round, in seconds.
    /// Set to 99999999 to manually schedule each round.
    /// </summary>
    public int? RoundInterval { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    public string? Variant { get; init; }

    /// <summary>
    /// Tournament description (max 400 chars, Markdown supported).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Whether the tournament is rated.
    /// </summary>
    public bool? Rated { get; init; }

    /// <summary>
    /// Password to join.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Forbid late pairings.
    /// </summary>
    public bool? ForbiddenPairings { get; init; }

    /// <summary>
    /// Manual pairing.
    /// </summary>
    public bool? ManualPairings { get; init; }

    /// <summary>
    /// Chat room settings.
    /// </summary>
    public bool? ChatFor { get; init; }

    /// <summary>
    /// Draw position from FEN.
    /// </summary>
    public string? Position { get; init; }

    /// <summary>
    /// Minimum rating to join.
    /// </summary>
    public int? MinRating { get; init; }

    /// <summary>
    /// Maximum rating to join.
    /// </summary>
    public int? MaxRating { get; init; }

    /// <summary>
    /// Minimum number of rated games to join.
    /// </summary>
    public int? MinRatedGames { get; init; }
}

/// <summary>
/// Options for exporting Swiss tournament games.
/// </summary>
public class SwissGamesExportOptions
{
    /// <summary>
    /// Only get games of a specific player.
    /// </summary>
    public string? Player { get; init; }

    /// <summary>
    /// Include the PGN moves.
    /// </summary>
    public bool? Moves { get; init; }

    /// <summary>
    /// Include the full PGN within the JSON response.
    /// </summary>
    public bool? PgnInJson { get; init; }

    /// <summary>
    /// Include the PGN tags.
    /// </summary>
    public bool? Tags { get; init; }

    /// <summary>
    /// Include clock status when available.
    /// </summary>
    public bool? Clocks { get; init; }

    /// <summary>
    /// Include analysis evaluations when available.
    /// </summary>
    public bool? Evals { get; init; }

    /// <summary>
    /// Include opening information.
    /// </summary>
    public bool? Opening { get; init; }
}

/// <summary>
/// Swiss tournament info.
/// </summary>
public class SwissTournament
{
    /// <summary>
    /// The tournament ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    /// <summary>
    /// Who created the tournament.
    /// </summary>
    [JsonPropertyName("createdBy")]
    public string CreatedBy { get; init; } = "";

    /// <summary>
    /// Tournament start time (ISO 8601 format).
    /// </summary>
    [JsonPropertyName("startsAt")]
    public string StartsAt { get; init; } = "";

    /// <summary>
    /// Tournament name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    /// <summary>
    /// Clock settings.
    /// </summary>
    [JsonPropertyName("clock")]
    public SwissClock? Clock { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    [JsonPropertyName("variant")]
    public string Variant { get; init; } = "";

    /// <summary>
    /// Current round number.
    /// </summary>
    [JsonPropertyName("round")]
    public int Round { get; init; }

    /// <summary>
    /// Maximum number of rounds.
    /// </summary>
    [JsonPropertyName("nbRounds")]
    public int NbRounds { get; init; }

    /// <summary>
    /// Number of players.
    /// </summary>
    [JsonPropertyName("nbPlayers")]
    public int NbPlayers { get; init; }

    /// <summary>
    /// Number of ongoing games.
    /// </summary>
    [JsonPropertyName("nbOngoing")]
    public int NbOngoing { get; init; }

    /// <summary>
    /// Tournament status.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = "";

    /// <summary>
    /// Whether the tournament is rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    /// Verdicts for joining the tournament.
    /// </summary>
    [JsonPropertyName("verdicts")]
    public SwissVerdicts? Verdicts { get; init; }

    /// <summary>
    /// Tournament statistics.
    /// </summary>
    [JsonPropertyName("stats")]
    public SwissStats? Stats { get; init; }

    /// <summary>
    /// Next round info.
    /// </summary>
    [JsonPropertyName("nextRound")]
    public SwissNextRound? NextRound { get; init; }

    /// <summary>
    /// Tournament description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Whether the tournament is a team-only tournament.
    /// </summary>
    [JsonPropertyName("teamId")]
    public string? TeamId { get; init; }
}

/// <summary>
/// Clock settings for Swiss tournaments.
/// </summary>
public class SwissClock
{
    /// <summary>
    /// Initial time in seconds.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; init; }

    /// <summary>
    /// Increment per move in seconds.
    /// </summary>
    [JsonPropertyName("increment")]
    public int Increment { get; init; }
}

/// <summary>
/// Verdicts for joining a Swiss tournament.
/// </summary>
public class SwissVerdicts
{
    /// <summary>
    /// Whether the current user is accepted.
    /// </summary>
    [JsonPropertyName("accepted")]
    public bool Accepted { get; init; }

    /// <summary>
    /// List of individual verdicts.
    /// </summary>
    [JsonPropertyName("list")]
    public IReadOnlyList<SwissVerdict>? List { get; init; }
}

/// <summary>
/// Individual verdict for joining.
/// </summary>
public class SwissVerdict
{
    /// <summary>
    /// The condition being checked.
    /// </summary>
    [JsonPropertyName("condition")]
    public string? Condition { get; init; }

    /// <summary>
    /// The verdict result.
    /// </summary>
    [JsonPropertyName("verdict")]
    public string? Verdict { get; init; }
}

/// <summary>
/// Swiss tournament statistics.
/// </summary>
public class SwissStats
{
    /// <summary>
    /// Total number of games played.
    /// </summary>
    [JsonPropertyName("games")]
    public int Games { get; init; }

    /// <summary>
    /// Number of white wins.
    /// </summary>
    [JsonPropertyName("whiteWins")]
    public int WhiteWins { get; init; }

    /// <summary>
    /// Number of black wins.
    /// </summary>
    [JsonPropertyName("blackWins")]
    public int BlackWins { get; init; }

    /// <summary>
    /// Number of draws.
    /// </summary>
    [JsonPropertyName("draws")]
    public int Draws { get; init; }

    /// <summary>
    /// Number of byes.
    /// </summary>
    [JsonPropertyName("byes")]
    public int Byes { get; init; }

    /// <summary>
    /// Number of absences.
    /// </summary>
    [JsonPropertyName("absences")]
    public int Absences { get; init; }

    /// <summary>
    /// Average rating of players.
    /// </summary>
    [JsonPropertyName("averageRating")]
    public int AverageRating { get; init; }
}

/// <summary>
/// Next round info.
/// </summary>
public class SwissNextRound
{
    /// <summary>
    /// When the next round starts (ISO 8601 format).
    /// </summary>
    [JsonPropertyName("at")]
    public string? At { get; init; }

    /// <summary>
    /// Seconds until the next round starts.
    /// </summary>
    [JsonPropertyName("in")]
    public int? In { get; init; }
}

/// <summary>
/// Player result from streaming results endpoint.
/// </summary>
public class SwissPlayerResult
{
    /// <summary>
    /// Player rank.
    /// </summary>
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    /// <summary>
    /// Player points (wins count as 1, draws as 0.5).
    /// </summary>
    [JsonPropertyName("points")]
    public double Points { get; init; }

    /// <summary>
    /// Tie-break score.
    /// </summary>
    [JsonPropertyName("tieBreak")]
    public double TieBreak { get; init; }

    /// <summary>
    /// Player rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; init; }

    /// <summary>
    /// Player username.
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; init; } = "";

    /// <summary>
    /// Player title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Performance rating.
    /// </summary>
    [JsonPropertyName("performance")]
    public int? Performance { get; init; }

    /// <summary>
    /// Whether the player is absent (withdrew).
    /// </summary>
    [JsonPropertyName("absent")]
    public bool? Absent { get; init; }

    /// <summary>
    /// Whether the player is excluded.
    /// </summary>
    [JsonPropertyName("excluded")]
    public bool? Excluded { get; init; }
}

