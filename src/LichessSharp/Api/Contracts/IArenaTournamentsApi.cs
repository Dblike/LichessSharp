using System.Text.Json;
using System.Text.Json.Serialization;

using LichessSharp.Models.Games;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Arena Tournaments API - Access Arena tournaments played on Lichess.
/// See <see href="https://lichess.org/api#tag/Arena-tournaments"/>.
/// </summary>
public interface IArenaTournamentsApi
{
    /// <summary>
    /// Get upcoming, ongoing, and recently finished Arena tournaments.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Arena tournaments grouped by status (created, started, finished).</returns>
    Task<ArenaTournamentList> GetCurrentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new Arena tournament.
    /// </summary>
    /// <param name="options">Tournament creation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created tournament.</returns>
    Task<ArenaTournament> CreateAsync(ArenaCreateOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get info about an Arena tournament.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="page">Page number for standings (starting at 1).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tournament info.</returns>
    Task<ArenaTournament> GetAsync(string id, int page = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing Arena tournament.
    /// Be mindful not to make important changes to ongoing tournaments.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="options">Tournament update options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated tournament.</returns>
    Task<ArenaTournament> UpdateAsync(string id, ArenaUpdateOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Join an Arena tournament, possibly with a password and/or a team.
    /// Also unpauses if you had previously paused the tournament.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="password">Optional password for private tournaments.</param>
    /// <param name="team">Optional team ID for team battles.</param>
    /// <param name="pairMeAsap">Request immediate pairing in tournaments with less than 30 players.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successfully joined.</returns>
    Task<bool> JoinAsync(string id, string? password = null, string? team = null, bool? pairMeAsap = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Leave a future Arena tournament, or take a break on an ongoing Arena tournament.
    /// It's possible to join again later. Points and streaks are preserved.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successfully withdrawn.</returns>
    Task<bool> PauseOrWithdrawAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminate an Arena tournament.
    /// Only the tournament creator or Lichess admins can terminate tournaments.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successfully terminated.</returns>
    Task<bool> TerminateAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a team battle tournament.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="teams">Teams participating in the battle (comma-separated team IDs, max 200 teams).</param>
    /// <param name="nbLeaders">Number of leaders whose score counts (1-20, default 5).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated tournament.</returns>
    Task<ArenaTournament> UpdateTeamBattleAsync(string id, string teams, int? nbLeaders = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export games of an Arena tournament.
    /// Games are sorted by reverse chronological order (most recent first).
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="options">Export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of games.</returns>
    IAsyncEnumerable<GameJson> StreamGamesAsync(string id, ArenaGamesExportOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get results of an Arena tournament.
    /// Players are sorted by rank (best first).
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="nb">Max number of players to fetch (default: all).</param>
    /// <param name="sheet">Include score sheet with individual game results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of player results.</returns>
    IAsyncEnumerable<ArenaPlayerResult> StreamResultsAsync(string id, int? nb = null, bool sheet = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get team standings in a team battle.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Team standings.</returns>
    Task<ArenaTeamStanding> GetTeamStandingAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Arena tournaments created by a user.
    /// Tournaments are sorted by reverse chronological order.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="status">Filter by status (default: all).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of tournaments created by the user.</returns>
    IAsyncEnumerable<ArenaTournamentSummary> StreamCreatedByAsync(string username, ArenaStatusFilter? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Arena tournaments played by a user.
    /// Tournaments are sorted by reverse chronological order.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="nb">Max number of tournaments to fetch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of tournaments played by the user.</returns>
    IAsyncEnumerable<ArenaPlayedTournament> StreamPlayedByAsync(string username, int? nb = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Arena tournaments for a team.
    /// Tournaments are sorted by reverse chronological order.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="max">Max number of tournaments to fetch (default: 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of team tournaments.</returns>
    IAsyncEnumerable<ArenaTournamentSummary> StreamTeamTournamentsAsync(string teamId, int? max = null, CancellationToken cancellationToken = default);
}
/// <summary>
/// Arena tournament status values.
/// </summary>
public enum ArenaStatus
{
    /// <summary>
    /// Tournament has been created but not yet started.
    /// </summary>
    Created = 10,

    /// <summary>
    /// Tournament is currently in progress.
    /// </summary>
    Started = 20,

    /// <summary>
    /// Tournament has finished.
    /// </summary>
    Finished = 30
}

/// <summary>
/// Arena status filter for querying tournaments.
/// </summary>
public enum ArenaStatusFilter
{
    /// <summary>
    /// Created tournaments only.
    /// </summary>
    Created,

    /// <summary>
    /// Started tournaments only.
    /// </summary>
    Started,

    /// <summary>
    /// Finished tournaments only.
    /// </summary>
    Finished
}

/// <summary>
/// Options for creating an Arena tournament.
/// </summary>
public class ArenaCreateOptions
{
    /// <summary>
    /// The tournament name. Leave empty to use auto-generated name based on variant.
    /// Must be between 2 and 30 characters if specified.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Clock time in minutes (0 to 60).
    /// </summary>
    public required int ClockTime { get; init; }

    /// <summary>
    /// Clock increment in seconds (0 to 180).
    /// </summary>
    public required int ClockIncrement { get; init; }

    /// <summary>
    /// Duration of the tournament in minutes (20 to 720).
    /// </summary>
    public required int Minutes { get; init; }

    /// <summary>
    /// How long to wait before starting (0 to 60 minutes, in minutes).
    /// </summary>
    public int? WaitMinutes { get; init; }

    /// <summary>
    /// Start at a specific time (Unix timestamp in milliseconds).
    /// Overrides WaitMinutes.
    /// </summary>
    public long? StartDate { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    public string? Variant { get; init; }

    /// <summary>
    /// Whether the tournament is rated. Defaults to true.
    /// </summary>
    public bool? Rated { get; init; }

    /// <summary>
    /// Draw position from FEN for a thematic tournament.
    /// Variant must be standard, fromPosition, or chess960.
    /// </summary>
    public string? Position { get; init; }

    /// <summary>
    /// Whether players can berserk (halve time for extra point). Defaults to true.
    /// </summary>
    public bool? Berserkable { get; init; }

    /// <summary>
    /// Whether to streak. A win grants an extra point and a double win streak.
    /// </summary>
    public bool? Streakable { get; init; }

    /// <summary>
    /// Whether to add a chat room for participants.
    /// </summary>
    public bool? HasChat { get; init; }

    /// <summary>
    /// Description displayed below the tournament name (max 400 chars, Markdown supported).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Password to join the tournament.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Team ID for team-restricted tournaments.
    /// </summary>
    public string? TeamId { get; init; }

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
/// Options for updating an Arena tournament.
/// </summary>
public class ArenaUpdateOptions
{
    /// <summary>
    /// The tournament name. Must be between 2 and 30 characters if specified.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Clock time in minutes (0 to 60).
    /// </summary>
    public int? ClockTime { get; init; }

    /// <summary>
    /// Clock increment in seconds (0 to 180).
    /// </summary>
    public int? ClockIncrement { get; init; }

    /// <summary>
    /// Duration of the tournament in minutes (20 to 720).
    /// </summary>
    public int? Minutes { get; init; }

    /// <summary>
    /// Start at a specific time (Unix timestamp in milliseconds).
    /// </summary>
    public long? StartDate { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    public string? Variant { get; init; }

    /// <summary>
    /// Whether the tournament is rated.
    /// </summary>
    public bool? Rated { get; init; }

    /// <summary>
    /// Draw position from FEN for a thematic tournament.
    /// </summary>
    public string? Position { get; init; }

    /// <summary>
    /// Whether players can berserk.
    /// </summary>
    public bool? Berserkable { get; init; }

    /// <summary>
    /// Whether to streak.
    /// </summary>
    public bool? Streakable { get; init; }

    /// <summary>
    /// Whether to add a chat room.
    /// </summary>
    public bool? HasChat { get; init; }

    /// <summary>
    /// Tournament description (max 400 chars, Markdown supported).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Password to join.
    /// </summary>
    public string? Password { get; init; }

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
/// Options for exporting Arena tournament games.
/// </summary>
public class ArenaGamesExportOptions
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
/// List of Arena tournaments grouped by status.
/// </summary>
public class ArenaTournamentList
{
    /// <summary>
    /// Tournaments that have been created but not yet started.
    /// </summary>
    [JsonPropertyName("created")]
    public IReadOnlyList<ArenaTournamentSummary> Created { get; init; } = [];

    /// <summary>
    /// Tournaments that are currently in progress.
    /// </summary>
    [JsonPropertyName("started")]
    public IReadOnlyList<ArenaTournamentSummary> Started { get; init; } = [];

    /// <summary>
    /// Tournaments that have finished.
    /// </summary>
    [JsonPropertyName("finished")]
    public IReadOnlyList<ArenaTournamentSummary> Finished { get; init; } = [];
}

/// <summary>
/// Summary of an Arena tournament (used in lists).
/// </summary>
public class ArenaTournamentSummary
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
    /// Tournament system (always "arena").
    /// </summary>
    [JsonPropertyName("system")]
    public string System { get; init; } = "arena";

    /// <summary>
    /// Duration in minutes.
    /// </summary>
    [JsonPropertyName("minutes")]
    public int Minutes { get; init; }

    /// <summary>
    /// Clock settings.
    /// </summary>
    [JsonPropertyName("clock")]
    public ArenaClock? Clock { get; init; }

    /// <summary>
    /// Whether the tournament is rated.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    /// The full tournament name.
    /// </summary>
    [JsonPropertyName("fullName")]
    public string FullName { get; init; } = "";

    /// <summary>
    /// Number of players.
    /// </summary>
    [JsonPropertyName("nbPlayers")]
    public int NbPlayers { get; init; }

    /// <summary>
    /// The chess variant.
    /// </summary>
    [JsonPropertyName("variant")]
    [JsonConverter(typeof(FlexibleVariantConverter))]
    public ArenaVariant? Variant { get; init; }

    /// <summary>
    /// Tournament start time.
    /// </summary>
    [JsonPropertyName("startsAt")]
    [JsonConverter(typeof(FlexibleTimestampConverter))]
    public DateTimeOffset StartsAt { get; init; }

    /// <summary>
    /// Tournament finish time.
    /// </summary>
    [JsonPropertyName("finishesAt")]
    [JsonConverter(typeof(FlexibleTimestampConverter))]
    public DateTimeOffset FinishesAt { get; init; }

    /// <summary>
    /// Tournament status.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; init; }

    /// <summary>
    /// Performance type info.
    /// </summary>
    [JsonPropertyName("perf")]
    public ArenaPerf? Perf { get; init; }

    /// <summary>
    /// Seconds until the tournament starts (if not yet started).
    /// </summary>
    [JsonPropertyName("secondsToStart")]
    public int? SecondsToStart { get; init; }

    /// <summary>
    /// Whether the tournament has a maximum rating restriction.
    /// </summary>
    [JsonPropertyName("hasMaxRating")]
    public bool? HasMaxRating { get; init; }

    /// <summary>
    /// Maximum rating restriction.
    /// </summary>
    [JsonPropertyName("maxRating")]
    public ArenaRatingRestriction? MaxRating { get; init; }

    /// <summary>
    /// Minimum rating restriction.
    /// </summary>
    [JsonPropertyName("minRating")]
    public ArenaRatingRestriction? MinRating { get; init; }

    /// <summary>
    /// Whether this is a private tournament.
    /// </summary>
    [JsonPropertyName("private")]
    public bool? IsPrivate { get; init; }

    /// <summary>
    /// Team ID if this is a team-restricted tournament.
    /// </summary>
    [JsonPropertyName("teamMember")]
    public string? TeamMember { get; init; }

    /// <summary>
    /// Team battle info if this is a team battle.
    /// </summary>
    [JsonPropertyName("teamBattle")]
    public ArenaTeamBattle? TeamBattle { get; init; }

    /// <summary>
    /// The winner if the tournament has finished.
    /// </summary>
    [JsonPropertyName("winner")]
    public ArenaWinner? Winner { get; init; }

    /// <summary>
    /// Starting position info for thematic tournaments.
    /// </summary>
    [JsonPropertyName("position")]
    public ArenaPosition? Position { get; init; }
}

/// <summary>
/// Full Arena tournament info (returned from ExportAsync).
/// </summary>
public class ArenaTournament : ArenaTournamentSummary
{
    /// <summary>
    /// Whether players can berserk.
    /// </summary>
    [JsonPropertyName("berserkable")]
    public bool? Berserkable { get; init; }

    /// <summary>
    /// Only titled players allowed.
    /// </summary>
    [JsonPropertyName("onlyTitled")]
    public bool? OnlyTitled { get; init; }

    /// <summary>
    /// Seconds until the tournament finishes.
    /// </summary>
    [JsonPropertyName("secondsToFinish")]
    public int? SecondsToFinish { get; init; }

    /// <summary>
    /// Whether the tournament is finished.
    /// </summary>
    [JsonPropertyName("isFinished")]
    public bool? IsFinished { get; init; }

    /// <summary>
    /// Whether the tournament recently finished.
    /// </summary>
    [JsonPropertyName("isRecentlyFinished")]
    public bool? IsRecentlyFinished { get; init; }

    /// <summary>
    /// Whether pairings are closed.
    /// </summary>
    [JsonPropertyName("pairingsClosed")]
    public bool? PairingsClosed { get; init; }

    /// <summary>
    /// Verdicts for joining the tournament.
    /// </summary>
    [JsonPropertyName("verdicts")]
    public ArenaVerdicts? Verdicts { get; init; }

    /// <summary>
    /// Quote displayed on the tournament page.
    /// </summary>
    [JsonPropertyName("quote")]
    public ArenaQuote? Quote { get; init; }

    /// <summary>
    /// Tournament spotlight info.
    /// </summary>
    [JsonPropertyName("spotlight")]
    public ArenaSpotlight? Spotlight { get; init; }

    /// <summary>
    /// Whether the current user is a member.
    /// </summary>
    [JsonPropertyName("me")]
    public ArenaTournamentMe? Me { get; init; }

    /// <summary>
    /// Tournament standing.
    /// </summary>
    [JsonPropertyName("standing")]
    public ArenaStanding? Standing { get; init; }

    /// <summary>
    /// Podium (top 3 players).
    /// </summary>
    [JsonPropertyName("podium")]
    public IReadOnlyList<ArenaPodiumPlayer>? Podium { get; init; }

    /// <summary>
    /// Featured game (if any).
    /// </summary>
    [JsonPropertyName("featured")]
    public ArenaFeaturedGame? Featured { get; init; }

    /// <summary>
    /// Current duels (pairings) in the tournament.
    /// </summary>
    [JsonPropertyName("duels")]
    public IReadOnlyList<ArenaDuel>? Duels { get; init; }

    /// <summary>
    /// Duel teams info for team battles.
    /// </summary>
    [JsonPropertyName("duelTeams")]
    public ArenaDuelTeams? DuelTeams { get; init; }

    /// <summary>
    /// Tournament description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Great player displayed on the tournament page.
    /// </summary>
    [JsonPropertyName("greatPlayer")]
    public ArenaGreatPlayer? GreatPlayer { get; init; }
}

/// <summary>
/// Clock settings for an Arena tournament.
/// </summary>
public class ArenaClock
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
/// Variant info for Arena tournaments.
/// </summary>
public class ArenaVariant
{
    /// <summary>
    /// Variant key (e.g., "standard", "chess960").
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; init; } = "";

    /// <summary>
    /// Short variant name.
    /// </summary>
    [JsonPropertyName("short")]
    public string? Short { get; init; }

    /// <summary>
    /// Full variant name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Performance type info.
/// </summary>
public class ArenaPerf
{
    /// <summary>
    /// Perf key (e.g., "blitz", "rapid").
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; init; } = "";

    /// <summary>
    /// Perf name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Position.
    /// </summary>
    [JsonPropertyName("position")]
    public int? Position { get; init; }

    /// <summary>
    /// Icon character.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; init; }
}

/// <summary>
/// Rating restriction for Arena tournaments.
/// </summary>
public class ArenaRatingRestriction
{
    /// <summary>
    /// Performance type.
    /// </summary>
    [JsonPropertyName("perf")]
    public string? Perf { get; init; }

    /// <summary>
    /// Rating value.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; init; }
}

/// <summary>
/// Team battle info.
/// </summary>
public class ArenaTeamBattle
{
    /// <summary>
    /// List of team IDs.
    /// </summary>
    [JsonPropertyName("teams")]
    public IReadOnlyList<string>? Teams { get; init; }

    /// <summary>
    /// Number of leaders that count for the team score.
    /// </summary>
    [JsonPropertyName("nbLeaders")]
    public int? NbLeaders { get; init; }
}

/// <summary>
/// Arena tournament winner.
/// </summary>
public class ArenaWinner
{
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    /// <summary>
    /// Username.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    /// <summary>
    /// Title (GM, IM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }
}

/// <summary>
/// Starting position for thematic Arena tournaments.
/// </summary>
public class ArenaPosition
{
    /// <summary>
    /// ECO code.
    /// </summary>
    [JsonPropertyName("eco")]
    public string? Eco { get; init; }

    /// <summary>
    /// Opening name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// FEN position.
    /// </summary>
    [JsonPropertyName("fen")]
    public string? Fen { get; init; }

    /// <summary>
    /// Opening URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}

/// <summary>
/// Verdicts for joining an Arena tournament.
/// </summary>
public class ArenaVerdicts
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
    public IReadOnlyList<ArenaVerdict>? List { get; init; }
}

/// <summary>
/// Individual verdict for joining.
/// </summary>
public class ArenaVerdict
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
/// Quote displayed on the tournament page.
/// </summary>
public class ArenaQuote
{
    /// <summary>
    /// Quote text.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// Quote author.
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; init; }
}

/// <summary>
/// Tournament spotlight info.
/// </summary>
public class ArenaSpotlight
{
    /// <summary>
    /// Spotlight headline.
    /// </summary>
    [JsonPropertyName("headline")]
    public string? Headline { get; init; }
}

/// <summary>
/// Current user's info in the tournament.
/// </summary>
public class ArenaTournamentMe
{
    /// <summary>
    /// Current user's rank.
    /// </summary>
    [JsonPropertyName("rank")]
    public int? Rank { get; init; }

    /// <summary>
    /// Whether the user is paused.
    /// </summary>
    [JsonPropertyName("withdraw")]
    public bool? Withdraw { get; init; }

    /// <summary>
    /// Whether the current user joined the tournament.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }
}

/// <summary>
/// Arena tournament standing.
/// </summary>
public class ArenaStanding
{
    /// <summary>
    /// Current page number.
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; init; }

    /// <summary>
    /// Players in this page.
    /// </summary>
    [JsonPropertyName("players")]
    public IReadOnlyList<ArenaStandingPlayer>? Players { get; init; }
}

/// <summary>
/// Player in the standings.
/// </summary>
public class ArenaStandingPlayer
{
    /// <summary>
    /// Player name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    /// <summary>
    /// Player rank.
    /// </summary>
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    /// <summary>
    /// Player rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; init; }

    /// <summary>
    /// Player score.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; init; }

    /// <summary>
    /// Score sheet.
    /// </summary>
    [JsonPropertyName("sheet")]
    public ArenaSheet? Sheet { get; init; }

    /// <summary>
    /// Player title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Team ID.
    /// </summary>
    [JsonPropertyName("team")]
    public string? Team { get; init; }
}

/// <summary>
/// Score sheet for Arena tournament results.
/// </summary>
public class ArenaSheet
{
    /// <summary>
    /// Encoded score string.
    /// </summary>
    [JsonPropertyName("scores")]
    public string Scores { get; init; } = "";

    /// <summary>
    /// Whether the player is on fire (streak).
    /// </summary>
    [JsonPropertyName("fire")]
    public bool? Fire { get; init; }
}

/// <summary>
/// Podium player info.
/// </summary>
public class ArenaPodiumPlayer
{
    /// <summary>
    /// Player name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    /// <summary>
    /// Player rank.
    /// </summary>
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    /// <summary>
    /// Player rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; init; }

    /// <summary>
    /// Player score.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; init; }

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
    /// Number of berserk games.
    /// </summary>
    [JsonPropertyName("nb")]
    public ArenaPodiumNb? Nb { get; init; }
}

/// <summary>
/// Podium player game counts.
/// </summary>
public class ArenaPodiumNb
{
    /// <summary>
    /// Number of games played.
    /// </summary>
    [JsonPropertyName("game")]
    public int? Game { get; init; }

    /// <summary>
    /// Number of berserk games.
    /// </summary>
    [JsonPropertyName("berserk")]
    public int? Berserk { get; init; }

    /// <summary>
    /// Number of wins.
    /// </summary>
    [JsonPropertyName("win")]
    public int? Win { get; init; }
}

/// <summary>
/// Featured game in the tournament.
/// </summary>
public class ArenaFeaturedGame
{
    /// <summary>
    /// Game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Current FEN position.
    /// </summary>
    [JsonPropertyName("fen")]
    public string? Fen { get; init; }

    /// <summary>
    /// Board orientation.
    /// </summary>
    [JsonPropertyName("orientation")]
    public string? Orientation { get; init; }

    /// <summary>
    /// Color whose turn it is.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    /// Last move.
    /// </summary>
    [JsonPropertyName("lastMove")]
    public string? LastMove { get; init; }

    /// <summary>
    /// White player.
    /// </summary>
    [JsonPropertyName("white")]
    public ArenaFeaturedPlayer? White { get; init; }

    /// <summary>
    /// Black player.
    /// </summary>
    [JsonPropertyName("black")]
    public ArenaFeaturedPlayer? Black { get; init; }
}

/// <summary>
/// Player in a featured game.
/// </summary>
public class ArenaFeaturedPlayer
{
    /// <summary>
    /// Player rank.
    /// </summary>
    [JsonPropertyName("rank")]
    public int? Rank { get; init; }

    /// <summary>
    /// Player name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Player rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    /// Player title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Whether the player berserked.
    /// </summary>
    [JsonPropertyName("berserk")]
    public bool? Berserk { get; init; }
}

/// <summary>
/// A duel (pairing) in an Arena tournament.
/// </summary>
public class ArenaDuel
{
    /// <summary>
    /// Game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Players in this duel.
    /// </summary>
    [JsonPropertyName("p")]
    public IReadOnlyList<ArenaDuelPlayer>? Players { get; init; }
}

/// <summary>
/// Player in an Arena duel.
/// </summary>
public class ArenaDuelPlayer
{
    /// <summary>
    /// Player name.
    /// </summary>
    [JsonPropertyName("n")]
    public string? Name { get; init; }

    /// <summary>
    /// Player rating.
    /// </summary>
    [JsonPropertyName("r")]
    public int? Rating { get; init; }

    /// <summary>
    /// Player rank in the tournament.
    /// </summary>
    [JsonPropertyName("k")]
    public int? Rank { get; init; }

    /// <summary>
    /// Player title (e.g., "GM", "FM").
    /// </summary>
    [JsonPropertyName("t")]
    public string? Title { get; init; }
}

/// <summary>
/// Duel teams info for team battles.
/// Contains dynamic team data keyed by team ID.
/// </summary>
public class ArenaDuelTeams
{
    /// <summary>
    /// Team entries. Keys are team IDs, values are team data.
    /// Using JsonElement for AOT compatibility with extension data.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Data { get; set; }
}

/// <summary>
/// Great player displayed on the tournament page.
/// </summary>
public class ArenaGreatPlayer
{
    /// <summary>
    /// Player name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Player URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}

/// <summary>
/// Player result from streaming results endpoint.
/// </summary>
public class ArenaPlayerResult
{
    /// <summary>
    /// Player rank.
    /// </summary>
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    /// <summary>
    /// Player score.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; init; }

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
    /// Team ID.
    /// </summary>
    [JsonPropertyName("team")]
    public string? Team { get; init; }

    /// <summary>
    /// Score sheet.
    /// </summary>
    [JsonPropertyName("sheet")]
    public ArenaSheet? Sheet { get; init; }
}

/// <summary>
/// Team standing in a team battle.
/// </summary>
public class ArenaTeamStanding
{
    /// <summary>
    /// Tournament ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Team standings.
    /// </summary>
    [JsonPropertyName("teams")]
    public IReadOnlyList<ArenaTeamResult>? Teams { get; init; }
}

/// <summary>
/// Team result in a team battle.
/// </summary>
public class ArenaTeamResult
{
    /// <summary>
    /// Team ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    /// <summary>
    /// Team score.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; init; }

    /// <summary>
    /// Team rank.
    /// </summary>
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    /// <summary>
    /// Number of players.
    /// </summary>
    [JsonPropertyName("players")]
    public IReadOnlyList<ArenaTeamPlayer>? Players { get; init; }
}

/// <summary>
/// Player in a team result.
/// </summary>
public class ArenaTeamPlayer
{
    /// <summary>
    /// User info.
    /// </summary>
    [JsonPropertyName("user")]
    public ArenaTeamPlayerUser? User { get; init; }

    /// <summary>
    /// Player score.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; init; }
}

/// <summary>
/// User info for team player.
/// </summary>
public class ArenaTeamPlayerUser
{
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    /// <summary>
    /// Username.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    /// <summary>
    /// Title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }
}

/// <summary>
/// Tournament that a user played in.
/// </summary>
public class ArenaPlayedTournament
{
    /// <summary>
    /// Tournament info.
    /// </summary>
    [JsonPropertyName("tournament")]
    public ArenaTournamentSummary? Tournament { get; init; }

    /// <summary>
    /// Player's result in the tournament.
    /// </summary>
    [JsonPropertyName("player")]
    public ArenaPlayedPlayer? Player { get; init; }
}

/// <summary>
/// Player's result in a played tournament.
/// </summary>
public class ArenaPlayedPlayer
{
    /// <summary>
    /// Player rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    /// Player score.
    /// </summary>
    [JsonPropertyName("score")]
    public int? Score { get; init; }

    /// <summary>
    /// Player rank.
    /// </summary>
    [JsonPropertyName("rank")]
    public int? Rank { get; init; }
}

