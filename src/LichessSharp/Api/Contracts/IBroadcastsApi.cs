using System.Text.Json.Serialization;

namespace LichessSharp.Api.Contracts;

/// <summary>
///     Broadcasts API - Relay chess events on Lichess.
///     Broadcasts are organized in tournaments, which have several rounds, which have several games.
/// </summary>
public interface IBroadcastsApi
{
    /// <summary>
    ///     Get ongoing official broadcasts sorted by tier.
    ///     After that, returns finished broadcasts sorted by most recent sync time.
    ///     Broadcasts are streamed as NDJSON.
    /// </summary>
    /// <param name="nb">Max number of broadcasts to fetch (1-100, default 20).</param>
    /// <param name="html">Convert the description from markdown to HTML.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of broadcasts with their rounds.</returns>
    IAsyncEnumerable<BroadcastWithRounds> StreamOfficialBroadcastsAsync(int? nb = null, bool? html = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get paginated top broadcast previews, same data as shown on https://lichess.org/broadcast.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated broadcast results.</returns>
    Task<BroadcastTopPage> GetTopBroadcastsAsync(int? page = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all incoming, ongoing, and finished broadcasts created by a user.
    ///     If you are authenticated as the user, you will also see private and unlisted broadcasts.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="nb">Max number of broadcasts to fetch (1-100, default 20).</param>
    /// <param name="html">Convert the description from markdown to HTML.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of broadcasts.</returns>
    IAsyncEnumerable<BroadcastByUser> StreamUserBroadcastsAsync(string username, int? nb = null, bool? html = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Search across recent official broadcasts.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated search results.</returns>
    Task<BroadcastSearchPage> SearchBroadcastsAsync(string query, int? page = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get information about a broadcast tournament.
    /// </summary>
    /// <param name="broadcastTournamentId">The broadcast tournament ID.</param>
    /// <param name="html">Convert the description from markdown to HTML.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The broadcast tournament with rounds.</returns>
    Task<BroadcastWithRounds> GetTournamentAsync(string broadcastTournamentId, bool? html = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get information about a broadcast round.
    /// </summary>
    /// <param name="broadcastTournamentSlug">The tournament slug.</param>
    /// <param name="broadcastRoundSlug">The round slug.</param>
    /// <param name="broadcastRoundId">The round ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The broadcast round with games.</returns>
    Task<BroadcastRound> GetRoundAsync(string broadcastTournamentSlug, string broadcastRoundSlug,
        string broadcastRoundId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Stream all broadcast rounds you are a member of.
    ///     Includes rounds you were invited to and non-writing member rounds.
    ///     Requires OAuth with study:read scope.
    /// </summary>
    /// <param name="nb">Max number of rounds to fetch (1-100, default 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of your broadcast rounds.</returns>
    IAsyncEnumerable<BroadcastMyRound> StreamMyRoundsAsync(int? nb = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a new broadcast tournament.
    ///     Requires OAuth with study:write scope.
    /// </summary>
    /// <param name="options">Tournament creation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created broadcast tournament.</returns>
    Task<BroadcastWithRounds> CreateTournamentAsync(BroadcastTournamentOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update an existing broadcast tournament.
    ///     Requires OAuth with study:write scope.
    /// </summary>
    /// <param name="broadcastTournamentId">The broadcast tournament ID.</param>
    /// <param name="options">Tournament update options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated broadcast tournament.</returns>
    Task<BroadcastWithRounds> UpdateTournamentAsync(string broadcastTournamentId, BroadcastTournamentOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a new broadcast round.
    ///     Requires OAuth with study:write scope.
    /// </summary>
    /// <param name="broadcastTournamentId">The broadcast tournament ID.</param>
    /// <param name="options">Round creation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created broadcast round.</returns>
    Task<BroadcastRoundNew> CreateRoundAsync(string broadcastTournamentId, BroadcastRoundOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update an existing broadcast round.
    ///     Requires OAuth with study:write scope.
    /// </summary>
    /// <param name="broadcastRoundId">The broadcast round ID.</param>
    /// <param name="options">Round update options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated broadcast round.</returns>
    Task<BroadcastRoundNew> UpdateRoundAsync(string broadcastRoundId, BroadcastRoundOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Reset a broadcast round to its initial state.
    ///     Remove any games from the broadcast round.
    ///     Requires OAuth with study:write scope.
    /// </summary>
    /// <param name="broadcastRoundId">The broadcast round ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the round was reset successfully.</returns>
    Task<bool> ResetRoundAsync(string broadcastRoundId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Push PGN to a broadcast round.
    ///     Only for broadcasts without a source URL.
    ///     Requires OAuth with study:write scope.
    /// </summary>
    /// <param name="broadcastRoundId">The broadcast round ID.</param>
    /// <param name="pgn">The PGN content to push.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Information about the pushed games.</returns>
    Task<BroadcastPgnPushResult> PushPgnAsync(string broadcastRoundId, string pgn,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Download all games of a single round of a broadcast tournament in PGN format.
    /// </summary>
    /// <param name="broadcastRoundId">The broadcast round ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PGN content as a string.</returns>
    Task<string> ExportRoundPgnAsync(string broadcastRoundId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Download all games of all rounds of a broadcast tournament in PGN format.
    /// </summary>
    /// <param name="broadcastTournamentId">The broadcast tournament ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PGN content as a string.</returns>
    Task<string> ExportAllRoundsPgnAsync(string broadcastTournamentId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Stream an ongoing broadcast round as PGN.
    ///     Returns a new PGN every time a game is updated in real-time.
    /// </summary>
    /// <param name="broadcastRoundId">The broadcast round ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of PGN strings.</returns>
    IAsyncEnumerable<string>
        StreamRoundPgnAsync(string broadcastRoundId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get the list of players of a broadcast tournament, if available.
    /// </summary>
    /// <param name="tournamentId">The broadcast tournament ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of players in the broadcast tournament.</returns>
    Task<IReadOnlyList<BroadcastPlayerEntry>> GetPlayersAsync(string tournamentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get the details of a specific player and their games from a broadcast tournament.
    /// </summary>
    /// <param name="tournamentId">The broadcast tournament ID.</param>
    /// <param name="playerId">The unique player ID within the broadcast. Usually their fideId or name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The player details with their games.</returns>
    Task<BroadcastPlayerWithGames> GetPlayerAsync(string tournamentId, string playerId,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Options for creating or updating a broadcast tournament.
/// </summary>
public class BroadcastTournamentOptions
{
    /// <summary>
    ///     Name of the tournament (2-80 characters). Required.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Short description of the tournament (max 400 characters).
    /// </summary>
    public string? ShortDescription { get; set; }

    /// <summary>
    ///     Full description in markdown format (max 20,000 characters).
    /// </summary>
    public string? FullDescription { get; set; }

    /// <summary>
    ///     Whether to enable auto leaderboard.
    /// </summary>
    public bool? AutoLeaderboard { get; set; }

    /// <summary>
    ///     Whether to enable team standings table.
    /// </summary>
    public bool? TeamTable { get; set; }

    /// <summary>
    ///     List of team names (comma-separated, max 5) for team standings.
    /// </summary>
    public string? Players { get; set; }

    /// <summary>
    ///     Custom URL to the event website.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    ///     Custom URL to the standings page.
    /// </summary>
    public string? Standings { get; set; }

    /// <summary>
    ///     Tournament location.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    ///     Tournament format description.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    ///     Time control description.
    /// </summary>
    public string? TimeControl { get; set; }

    /// <summary>
    ///     FIDE rating category: "standard", "rapid", or "blitz".
    /// </summary>
    public string? FideTimeControl { get; set; }

    /// <summary>
    ///     Timezone identifier (e.g., "America/New_York").
    /// </summary>
    public string? TimeZone { get; set; }
}

/// <summary>
///     Options for creating or updating a broadcast round.
/// </summary>
public class BroadcastRoundOptions
{
    /// <summary>
    ///     Name of the round (3-80 characters). Required.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     URL from which to sync PGN updates.
    /// </summary>
    public string? SyncUrl { get; set; }

    /// <summary>
    ///     Unix timestamp in milliseconds for when the round starts.
    /// </summary>
    public long? StartsAt { get; set; }

    /// <summary>
    ///     Whether the round starts immediately after the previous round finishes.
    /// </summary>
    public bool? StartsAfterPrevious { get; set; }

    /// <summary>
    ///     Whether the games are rated for FIDE rating calculation.
    /// </summary>
    public bool? Rated { get; set; }

    /// <summary>
    ///     Delay in seconds between the source and the broadcast (0-1800).
    /// </summary>
    public int? Delay { get; set; }

    /// <summary>
    ///     Period in seconds for how often to check for updates (2-60, default 6).
    /// </summary>
    public int? SyncPeriod { get; set; }
}

/// <summary>
///     A broadcast tournament.
/// </summary>
public class BroadcastTour
{
    /// <summary>
    ///     Broadcast tournament ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Name of the tournament.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     URL-friendly slug.
    /// </summary>
    [JsonPropertyName("slug")]
    public string? Slug { get; init; }

    /// <summary>
    ///     Creation timestamp (Unix milliseconds).
    /// </summary>
    [JsonPropertyName("createdAt")]
    public long? CreatedAt { get; init; }

    /// <summary>
    ///     Start and end dates (Unix milliseconds). Array of 1-2 elements.
    /// </summary>
    [JsonPropertyName("dates")]
    public IReadOnlyList<long>? Dates { get; init; }

    /// <summary>
    ///     Additional display information about the tournament.
    /// </summary>
    [JsonPropertyName("info")]
    public BroadcastTourInfo? Info { get; init; }

    /// <summary>
    ///     Tier for featured tournaments (used by Lichess).
    /// </summary>
    [JsonPropertyName("tier")]
    public int? Tier { get; init; }

    /// <summary>
    ///     Tournament image URL.
    /// </summary>
    [JsonPropertyName("image")]
    public string? Image { get; init; }

    /// <summary>
    ///     Full description in markdown or HTML.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    ///     Whether auto-leaderboard is enabled.
    /// </summary>
    [JsonPropertyName("leaderboard")]
    public bool? Leaderboard { get; init; }

    /// <summary>
    ///     Whether team standings table is enabled.
    /// </summary>
    [JsonPropertyName("teamTable")]
    public bool? TeamTable { get; init; }

    /// <summary>
    ///     URL to the broadcast on Lichess.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}

/// <summary>
///     Additional display information about a broadcast tournament.
/// </summary>
public class BroadcastTourInfo
{
    /// <summary>
    ///     Official external website URL.
    /// </summary>
    [JsonPropertyName("website")]
    public string? Website { get; init; }

    /// <summary>
    ///     Featured players.
    /// </summary>
    [JsonPropertyName("players")]
    public string? Players { get; init; }

    /// <summary>
    ///     Tournament location.
    /// </summary>
    [JsonPropertyName("location")]
    public string? Location { get; init; }

    /// <summary>
    ///     Time control description.
    /// </summary>
    [JsonPropertyName("tc")]
    public string? TimeControl { get; init; }

    /// <summary>
    ///     FIDE rating category (standard, rapid, blitz).
    /// </summary>
    [JsonPropertyName("fideTc")]
    public string? FideTimeControl { get; init; }

    /// <summary>
    ///     Timezone identifier.
    /// </summary>
    [JsonPropertyName("timeZone")]
    public string? TimeZone { get; init; }

    /// <summary>
    ///     Official standings website URL.
    /// </summary>
    [JsonPropertyName("standings")]
    public string? Standings { get; init; }

    /// <summary>
    ///     Tournament format description.
    /// </summary>
    [JsonPropertyName("format")]
    public string? Format { get; init; }
}

/// <summary>
///     Information about a broadcast round.
/// </summary>
public class BroadcastRoundInfo
{
    /// <summary>
    ///     Round ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Round name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     URL-friendly slug.
    /// </summary>
    [JsonPropertyName("slug")]
    public required string Slug { get; init; }

    /// <summary>
    ///     Creation timestamp (Unix milliseconds).
    /// </summary>
    [JsonPropertyName("createdAt")]
    public long CreatedAt { get; init; }

    /// <summary>
    ///     Whether the round is used for rating calculations.
    /// </summary>
    [JsonPropertyName("rated")]
    public bool Rated { get; init; }

    /// <summary>
    ///     Whether the round is currently ongoing.
    /// </summary>
    [JsonPropertyName("ongoing")]
    public bool? Ongoing { get; init; }

    /// <summary>
    ///     Scheduled start time (Unix milliseconds).
    /// </summary>
    [JsonPropertyName("startsAt")]
    public long? StartsAt { get; init; }

    /// <summary>
    ///     Whether the round starts after the previous round completes.
    /// </summary>
    [JsonPropertyName("startsAfterPrevious")]
    public bool? StartsAfterPrevious { get; init; }

    /// <summary>
    ///     Finish time (Unix milliseconds).
    /// </summary>
    [JsonPropertyName("finishedAt")]
    public long? FinishedAt { get; init; }

    /// <summary>
    ///     Whether the round has finished.
    /// </summary>
    [JsonPropertyName("finished")]
    public bool? Finished { get; init; }

    /// <summary>
    ///     URL to the round on Lichess.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    /// <summary>
    ///     Broadcast delay in seconds.
    /// </summary>
    [JsonPropertyName("delay")]
    public long? Delay { get; init; }
}

/// <summary>
///     A broadcast tournament with its rounds.
/// </summary>
public class BroadcastWithRounds
{
    /// <summary>
    ///     The broadcast tournament.
    /// </summary>
    [JsonPropertyName("tour")]
    public required BroadcastTour Tour { get; init; }

    /// <summary>
    ///     Optional group information.
    /// </summary>
    [JsonPropertyName("group")]
    public BroadcastGroup? Group { get; init; }

    /// <summary>
    ///     List of rounds in this tournament.
    /// </summary>
    [JsonPropertyName("rounds")]
    public required IReadOnlyList<BroadcastRoundInfo> Rounds { get; init; }

    /// <summary>
    ///     The default round ID to show.
    /// </summary>
    [JsonPropertyName("defaultRoundId")]
    public string? DefaultRoundId { get; init; }
}

/// <summary>
///     Broadcast group information.
/// </summary>
public class BroadcastGroup
{
    /// <summary>
    ///     Group ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Group slug.
    /// </summary>
    [JsonPropertyName("slug")]
    public required string Slug { get; init; }

    /// <summary>
    ///     Group name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     List of tournaments in this group.
    /// </summary>
    [JsonPropertyName("tours")]
    public IReadOnlyList<BroadcastGroupTour>? Tours { get; init; }
}

/// <summary>
///     A tour entry within a broadcast group.
/// </summary>
public class BroadcastGroupTour
{
    /// <summary>
    ///     Tour ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Tour name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

/// <summary>
///     A broadcast round with full details including games.
/// </summary>
public class BroadcastRound
{
    /// <summary>
    ///     Round information.
    /// </summary>
    [JsonPropertyName("round")]
    public required BroadcastRoundInfo Round { get; init; }

    /// <summary>
    ///     Tournament information.
    /// </summary>
    [JsonPropertyName("tour")]
    public required BroadcastTour Tour { get; init; }

    /// <summary>
    ///     Study-related information.
    /// </summary>
    [JsonPropertyName("study")]
    public required BroadcastRoundStudyInfo Study { get; init; }

    /// <summary>
    ///     Games in this round.
    /// </summary>
    [JsonPropertyName("games")]
    public required IReadOnlyList<BroadcastRoundGame> Games { get; init; }

    /// <summary>
    ///     Optional group information.
    /// </summary>
    [JsonPropertyName("group")]
    public BroadcastGroup? Group { get; init; }

    /// <summary>
    ///     Whether the current user is subscribed to this broadcast.
    /// </summary>
    [JsonPropertyName("isSubscribed")]
    public bool? IsSubscribed { get; init; }
}

/// <summary>
///     Study-related information for a broadcast round.
/// </summary>
public class BroadcastRoundStudyInfo
{
    /// <summary>
    ///     Whether the authenticated user has write permission.
    /// </summary>
    [JsonPropertyName("writeable")]
    public bool? Writeable { get; init; }

    /// <summary>
    ///     Available features for the user.
    /// </summary>
    [JsonPropertyName("features")]
    public BroadcastStudyFeatures? Features { get; init; }
}

/// <summary>
///     Features available for a broadcast study.
/// </summary>
public class BroadcastStudyFeatures
{
    /// <summary>
    ///     Whether chat is enabled.
    /// </summary>
    [JsonPropertyName("chat")]
    public bool? Chat { get; init; }

    /// <summary>
    ///     Whether engine analysis is enabled.
    /// </summary>
    [JsonPropertyName("computer")]
    public bool? Computer { get; init; }

    /// <summary>
    ///     Whether opening explorer and tablebase are enabled.
    /// </summary>
    [JsonPropertyName("explorer")]
    public bool? Explorer { get; init; }
}

/// <summary>
///     A game in a broadcast round.
/// </summary>
public class BroadcastRoundGame
{
    /// <summary>
    ///     Game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Game name (e.g., "Carlsen - Caruana").
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     Current FEN position.
    /// </summary>
    [JsonPropertyName("fen")]
    public string? Fen { get; init; }

    /// <summary>
    ///     Players in this game.
    /// </summary>
    [JsonPropertyName("players")]
    public IReadOnlyList<BroadcastGamePlayer>? Players { get; init; }

    /// <summary>
    ///     Last move in UCI notation.
    /// </summary>
    [JsonPropertyName("lastMove")]
    public string? LastMove { get; init; }

    /// <summary>
    ///     Game status/result.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    ///     Whose turn it is to play.
    /// </summary>
    [JsonPropertyName("thinkTime")]
    public int? ThinkTime { get; init; }
}

/// <summary>
///     A player in a broadcast game.
/// </summary>
public class BroadcastGamePlayer
{
    /// <summary>
    ///     Player name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     Chess title (GM, IM, FM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    ///     Player rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    ///     FIDE ID.
    /// </summary>
    [JsonPropertyName("fideId")]
    public int? FideId { get; init; }

    /// <summary>
    ///     Federation code (ISO 3166-1 alpha-3).
    /// </summary>
    [JsonPropertyName("fed")]
    public string? Federation { get; init; }

    /// <summary>
    ///     Remaining clock time in centiseconds.
    /// </summary>
    [JsonPropertyName("clock")]
    public int? Clock { get; init; }
}

/// <summary>
///     A broadcast with last round info (used in top broadcasts list).
/// </summary>
public class BroadcastWithLastRound
{
    /// <summary>
    ///     Optional group name.
    /// </summary>
    [JsonPropertyName("group")]
    public string? Group { get; init; }

    /// <summary>
    ///     The broadcast tournament.
    /// </summary>
    [JsonPropertyName("tour")]
    public required BroadcastTour Tour { get; init; }

    /// <summary>
    ///     The most recent round.
    /// </summary>
    [JsonPropertyName("round")]
    public BroadcastRoundInfo? Round { get; init; }
}

/// <summary>
///     Paginated top broadcasts response.
/// </summary>
public class BroadcastTopPage
{
    /// <summary>
    ///     Active ongoing broadcasts.
    /// </summary>
    [JsonPropertyName("active")]
    public IReadOnlyList<BroadcastWithLastRound>? Active { get; init; }

    /// <summary>
    ///     Paginated past broadcasts.
    /// </summary>
    [JsonPropertyName("past")]
    public BroadcastPagination? Past { get; init; }
}

/// <summary>
///     Broadcast pagination information.
/// </summary>
public class BroadcastPagination
{
    /// <summary>
    ///     Current page number.
    /// </summary>
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; init; }

    /// <summary>
    ///     Maximum items per page.
    /// </summary>
    [JsonPropertyName("maxPerPage")]
    public int MaxPerPage { get; init; }

    /// <summary>
    ///     Results on the current page.
    /// </summary>
    [JsonPropertyName("currentPageResults")]
    public IReadOnlyList<BroadcastWithLastRound>? CurrentPageResults { get; init; }

    /// <summary>
    ///     Previous page number (null if no previous page).
    /// </summary>
    [JsonPropertyName("previousPage")]
    public int? PreviousPage { get; init; }

    /// <summary>
    ///     Next page number (null if no next page).
    /// </summary>
    [JsonPropertyName("nextPage")]
    public int? NextPage { get; init; }
}

/// <summary>
///     Search results page for broadcasts.
/// </summary>
public class BroadcastSearchPage
{
    /// <summary>
    ///     Current page number.
    /// </summary>
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; init; }

    /// <summary>
    ///     Maximum items per page.
    /// </summary>
    [JsonPropertyName("maxPerPage")]
    public int MaxPerPage { get; init; }

    /// <summary>
    ///     Search results on the current page.
    /// </summary>
    [JsonPropertyName("currentPageResults")]
    public IReadOnlyList<BroadcastWithLastRound>? CurrentPageResults { get; init; }

    /// <summary>
    ///     Previous page number (null if no previous page).
    /// </summary>
    [JsonPropertyName("previousPage")]
    public int? PreviousPage { get; init; }

    /// <summary>
    ///     Next page number (null if no next page).
    /// </summary>
    [JsonPropertyName("nextPage")]
    public int? NextPage { get; init; }
}

/// <summary>
///     A broadcast created by a user.
/// </summary>
public class BroadcastByUser
{
    /// <summary>
    ///     The broadcast tournament.
    /// </summary>
    [JsonPropertyName("tour")]
    public required BroadcastTour Tour { get; init; }
}

/// <summary>
///     A newly created broadcast round.
/// </summary>
public class BroadcastRoundNew
{
    /// <summary>
    ///     Round information.
    /// </summary>
    [JsonPropertyName("round")]
    public required BroadcastRoundInfo Round { get; init; }

    /// <summary>
    ///     Tournament information.
    /// </summary>
    [JsonPropertyName("tour")]
    public required BroadcastTour Tour { get; init; }

    /// <summary>
    ///     Study-related information.
    /// </summary>
    [JsonPropertyName("study")]
    public required BroadcastRoundStudyInfo Study { get; init; }
}

/// <summary>
///     A broadcast round from the "my rounds" endpoint.
/// </summary>
public class BroadcastMyRound
{
    /// <summary>
    ///     Round information.
    /// </summary>
    [JsonPropertyName("round")]
    public required BroadcastRoundInfo Round { get; init; }

    /// <summary>
    ///     Tournament information.
    /// </summary>
    [JsonPropertyName("tour")]
    public required BroadcastTour Tour { get; init; }

    /// <summary>
    ///     Study-related information.
    /// </summary>
    [JsonPropertyName("study")]
    public required BroadcastRoundStudyInfo Study { get; init; }
}

/// <summary>
///     Result of pushing PGN to a broadcast round.
/// </summary>
public class BroadcastPgnPushResult
{
    /// <summary>
    ///     Information about the games that were pushed.
    /// </summary>
    [JsonPropertyName("games")]
    public IReadOnlyList<BroadcastPgnPushGame>? Games { get; init; }
}

/// <summary>
///     Information about a game that was pushed via PGN.
/// </summary>
public class BroadcastPgnPushGame
{
    /// <summary>
    ///     PGN tags of the game.
    /// </summary>
    [JsonPropertyName("tags")]
    public Dictionary<string, string>? Tags { get; init; }

    /// <summary>
    ///     Number of moves in the game.
    /// </summary>
    [JsonPropertyName("moves")]
    public int? Moves { get; init; }

    /// <summary>
    ///     Error message if the game could not be processed.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }
}

/// <summary>
///     A player entry in a broadcast tournament leaderboard.
/// </summary>
public class BroadcastPlayerEntry
{
    /// <summary>
    ///     Player name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Chess title (GM, IM, FM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    ///     Player rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    ///     FIDE ID.
    /// </summary>
    [JsonPropertyName("fideId")]
    public int? FideId { get; init; }

    /// <summary>
    ///     Federation code (ISO 3166-1 alpha-3).
    /// </summary>
    [JsonPropertyName("fed")]
    public string? Federation { get; init; }

    /// <summary>
    ///     Team name.
    /// </summary>
    [JsonPropertyName("team")]
    public string? Team { get; init; }

    /// <summary>
    ///     Player's score in the tournament.
    /// </summary>
    [JsonPropertyName("score")]
    public double? Score { get; init; }

    /// <summary>
    ///     Number of games played.
    /// </summary>
    [JsonPropertyName("played")]
    public int? Played { get; init; }

    /// <summary>
    ///     Rating difference from the tournament.
    /// </summary>
    [JsonPropertyName("ratingDiff")]
    public int? RatingDiff { get; init; }

    /// <summary>
    ///     Performance rating.
    /// </summary>
    [JsonPropertyName("performance")]
    public int? Performance { get; init; }

    /// <summary>
    ///     Tiebreak values.
    /// </summary>
    [JsonPropertyName("tiebreaks")]
    public IReadOnlyList<BroadcastPlayerTiebreak>? Tiebreaks { get; init; }

    /// <summary>
    ///     Player's rank in the tournament.
    /// </summary>
    [JsonPropertyName("rank")]
    public int? Rank { get; init; }
}

/// <summary>
///     Tiebreak value for a player.
/// </summary>
public class BroadcastPlayerTiebreak
{
    /// <summary>
    ///     Tiebreak code.
    /// </summary>
    [JsonPropertyName("extendedCode")]
    public string? ExtendedCode { get; init; }

    /// <summary>
    ///     Description of the tiebreak.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    ///     Tiebreak points.
    /// </summary>
    [JsonPropertyName("points")]
    public double? Points { get; init; }
}

/// <summary>
///     A player entry with FIDE details and games.
/// </summary>
public class BroadcastPlayerWithGames : BroadcastPlayerEntry
{
    /// <summary>
    ///     FIDE details for the player.
    /// </summary>
    [JsonPropertyName("fide")]
    public BroadcastPlayerFide? Fide { get; init; }

    /// <summary>
    ///     Games played by this player in the broadcast.
    /// </summary>
    [JsonPropertyName("games")]
    public IReadOnlyList<BroadcastPlayerGame>? Games { get; init; }
}

/// <summary>
///     FIDE details for a broadcast player.
/// </summary>
public class BroadcastPlayerFide
{
    /// <summary>
    ///     Year of birth.
    /// </summary>
    [JsonPropertyName("year")]
    public int? Year { get; init; }

    /// <summary>
    ///     FIDE ratings by time control.
    /// </summary>
    [JsonPropertyName("ratings")]
    public BroadcastPlayerFideRatings? Ratings { get; init; }
}

/// <summary>
///     FIDE ratings by time control.
/// </summary>
public class BroadcastPlayerFideRatings
{
    /// <summary>
    ///     Standard (classical) rating.
    /// </summary>
    [JsonPropertyName("standard")]
    public int? Standard { get; init; }

    /// <summary>
    ///     Rapid rating.
    /// </summary>
    [JsonPropertyName("rapid")]
    public int? Rapid { get; init; }

    /// <summary>
    ///     Blitz rating.
    /// </summary>
    [JsonPropertyName("blitz")]
    public int? Blitz { get; init; }
}

/// <summary>
///     A game played by a player in a broadcast.
/// </summary>
public class BroadcastPlayerGame
{
    /// <summary>
    ///     Round ID.
    /// </summary>
    [JsonPropertyName("round")]
    public string? Round { get; init; }

    /// <summary>
    ///     Game/chapter ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Opponent information.
    /// </summary>
    [JsonPropertyName("opponent")]
    public BroadcastPlayerOpponent? Opponent { get; init; }

    /// <summary>
    ///     Color the player had in this game.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    ///     Points earned ("1", "1/2", "0").
    /// </summary>
    [JsonPropertyName("points")]
    public string? Points { get; init; }

    /// <summary>
    ///     Custom points (for special scoring systems).
    /// </summary>
    [JsonPropertyName("customPoints")]
    public string? CustomPoints { get; init; }

    /// <summary>
    ///     Rating change from this game.
    /// </summary>
    [JsonPropertyName("ratingDiff")]
    public int? RatingDiff { get; init; }
}

/// <summary>
///     Opponent information in a broadcast player game.
/// </summary>
public class BroadcastPlayerOpponent
{
    /// <summary>
    ///     Opponent name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Chess title (GM, IM, FM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    ///     Opponent rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }

    /// <summary>
    ///     FIDE ID.
    /// </summary>
    [JsonPropertyName("fideId")]
    public int? FideId { get; init; }

    /// <summary>
    ///     Federation code (ISO 3166-1 alpha-3).
    /// </summary>
    [JsonPropertyName("fed")]
    public string? Federation { get; init; }
}