using System.Text.Json.Serialization;

namespace LichessSharp.Api;

/// <summary>
/// Teams API - Access and manage Lichess teams and their members.
/// </summary>
public interface ITeamsApi
{
    /// <summary>
    /// Get information about a team.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Team information.</returns>
    Task<Team> GetAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the most popular teams (paginated).
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of popular teams.</returns>
    Task<TeamPaginator> GetPopularAsync(int page = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get teams a user is a member of.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of teams the user belongs to.</returns>
    Task<IReadOnlyList<Team>> GetUserTeamsAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for teams.
    /// </summary>
    /// <param name="text">Search text.</param>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated search results.</returns>
    Task<TeamPaginator> SearchAsync(string text, int page = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream members of a team.
    /// Members are sorted by reverse chronological order of joining (most recent first).
    /// Up to 5,000 users are streamed.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="full">If true, returns full user documents with performance ratings (limited to 1,000 users).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of team members.</returns>
    IAsyncEnumerable<TeamMember> StreamMembersAsync(string teamId, bool full = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Join a team. Requires OAuth with team:write scope.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="message">Required if team manually reviews admission requests (30-2000 chars).</param>
    /// <param name="password">Optional password, if the team requires one.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the join request was sent successfully.</returns>
    Task<bool> JoinAsync(string teamId, string? message = null, string? password = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Leave a team. Requires OAuth with team:write scope.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successfully left the team.</returns>
    Task<bool> LeaveAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending join requests for a team you lead.
    /// Requires OAuth with team:read scope.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="declined">If true, returns declined requests instead of pending ones.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of join requests with user info.</returns>
    Task<IReadOnlyList<TeamRequestWithUser>> GetJoinRequestsAsync(string teamId, bool declined = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Accept a user's request to join your team.
    /// Requires OAuth with team:lead scope.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="userId">The user ID to accept.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the request was accepted.</returns>
    Task<bool> AcceptJoinRequestAsync(string teamId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decline a user's request to join your team.
    /// Requires OAuth with team:lead scope.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="userId">The user ID to decline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the request was declined.</returns>
    Task<bool> DeclineJoinRequestAsync(string teamId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kick a member from your team.
    /// Requires OAuth with team:lead scope.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="userId">The user ID to kick.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user was kicked.</returns>
    Task<bool> KickMemberAsync(string teamId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a private message to all members of a team.
    /// You must be a team leader with the "Messages" permission.
    /// Requires OAuth with team:lead scope.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the message was sent.</returns>
    Task<bool> MessageAllMembersAsync(string teamId, string message, CancellationToken cancellationToken = default);
}

#region Models

/// <summary>
/// A Lichess team.
/// </summary>
public class Team
{
    /// <summary>
    /// The team ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The team name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The team description (may contain markdown).
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// The team flair emoji.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }

    /// <summary>
    /// Number of members.
    /// </summary>
    [JsonPropertyName("nbMembers")]
    public int NbMembers { get; init; }

    /// <summary>
    /// Single team leader (deprecated, use Leaders instead).
    /// </summary>
    [JsonPropertyName("leader")]
    public TeamLeader? Leader { get; init; }

    /// <summary>
    /// List of team leaders.
    /// </summary>
    [JsonPropertyName("leaders")]
    public List<TeamLeader>? Leaders { get; init; }

    /// <summary>
    /// Whether the team is open for anyone to join.
    /// </summary>
    [JsonPropertyName("open")]
    public bool Open { get; init; }

    /// <summary>
    /// Whether the authenticated user has joined this team.
    /// </summary>
    [JsonPropertyName("joined")]
    public bool? Joined { get; init; }

    /// <summary>
    /// Whether the authenticated user has requested to join this team.
    /// </summary>
    [JsonPropertyName("requested")]
    public bool? Requested { get; init; }
}

/// <summary>
/// A team leader (light user info).
/// </summary>
public class TeamLeader
{
    /// <summary>
    /// The leader's user ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The leader's username.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The leader's title (GM, IM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// The leader's flair emoji.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }

    /// <summary>
    /// Patron wing color if they are a patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }
}

/// <summary>
/// Paginated list of teams.
/// </summary>
public class TeamPaginator
{
    /// <summary>
    /// Current page number.
    /// </summary>
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; init; }

    /// <summary>
    /// Maximum results per page.
    /// </summary>
    [JsonPropertyName("maxPerPage")]
    public int MaxPerPage { get; init; }

    /// <summary>
    /// Teams on the current page.
    /// </summary>
    [JsonPropertyName("currentPageResults")]
    public List<Team>? CurrentPageResults { get; init; }

    /// <summary>
    /// Previous page number (null if on first page).
    /// </summary>
    [JsonPropertyName("previousPage")]
    public int? PreviousPage { get; init; }

    /// <summary>
    /// Next page number (null if on last page).
    /// </summary>
    [JsonPropertyName("nextPage")]
    public int? NextPage { get; init; }

    /// <summary>
    /// Total number of results across all pages.
    /// </summary>
    [JsonPropertyName("nbResults")]
    public int NbResults { get; init; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    [JsonPropertyName("nbPages")]
    public int NbPages { get; init; }
}

/// <summary>
/// A team member streamed from the members endpoint.
/// </summary>
public class TeamMember
{
    /// <summary>
    /// The member's user ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The member's username.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// When the member joined the team (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("joinedTeamAt")]
    public long? JoinedTeamAt { get; init; }

    /// <summary>
    /// The member's title (GM, IM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Patron wing color if they are a patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }

    /// <summary>
    /// The member's flair emoji.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }

    /// <summary>
    /// Performance ratings (only when full=true).
    /// </summary>
    [JsonPropertyName("perfs")]
    public TeamMemberPerfs? Perfs { get; init; }
}

/// <summary>
/// Performance ratings for a team member (when full=true).
/// </summary>
public class TeamMemberPerfs
{
    /// <summary>
    /// Bullet rating.
    /// </summary>
    [JsonPropertyName("bullet")]
    public TeamMemberPerfStats? Bullet { get; init; }

    /// <summary>
    /// Blitz rating.
    /// </summary>
    [JsonPropertyName("blitz")]
    public TeamMemberPerfStats? Blitz { get; init; }

    /// <summary>
    /// Rapid rating.
    /// </summary>
    [JsonPropertyName("rapid")]
    public TeamMemberPerfStats? Rapid { get; init; }

    /// <summary>
    /// Classical rating.
    /// </summary>
    [JsonPropertyName("classical")]
    public TeamMemberPerfStats? Classical { get; init; }

    /// <summary>
    /// Correspondence rating.
    /// </summary>
    [JsonPropertyName("correspondence")]
    public TeamMemberPerfStats? Correspondence { get; init; }

    /// <summary>
    /// Chess960 rating.
    /// </summary>
    [JsonPropertyName("chess960")]
    public TeamMemberPerfStats? Chess960 { get; init; }

    /// <summary>
    /// King of the Hill rating.
    /// </summary>
    [JsonPropertyName("kingOfTheHill")]
    public TeamMemberPerfStats? KingOfTheHill { get; init; }

    /// <summary>
    /// Three-check rating.
    /// </summary>
    [JsonPropertyName("threeCheck")]
    public TeamMemberPerfStats? ThreeCheck { get; init; }

    /// <summary>
    /// Antichess rating.
    /// </summary>
    [JsonPropertyName("antichess")]
    public TeamMemberPerfStats? Antichess { get; init; }

    /// <summary>
    /// Atomic rating.
    /// </summary>
    [JsonPropertyName("atomic")]
    public TeamMemberPerfStats? Atomic { get; init; }

    /// <summary>
    /// Horde rating.
    /// </summary>
    [JsonPropertyName("horde")]
    public TeamMemberPerfStats? Horde { get; init; }

    /// <summary>
    /// Racing Kings rating.
    /// </summary>
    [JsonPropertyName("racingKings")]
    public TeamMemberPerfStats? RacingKings { get; init; }

    /// <summary>
    /// Crazyhouse rating.
    /// </summary>
    [JsonPropertyName("crazyhouse")]
    public TeamMemberPerfStats? Crazyhouse { get; init; }

    /// <summary>
    /// Puzzle rating.
    /// </summary>
    [JsonPropertyName("puzzle")]
    public TeamMemberPerfStats? Puzzle { get; init; }
}

/// <summary>
/// Performance statistics for a specific game variant.
/// </summary>
public class TeamMemberPerfStats
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
    /// Rating progress (change).
    /// </summary>
    [JsonPropertyName("prog")]
    public int Prog { get; init; }

    /// <summary>
    /// Whether the rating is provisional.
    /// </summary>
    [JsonPropertyName("prov")]
    public bool? Prov { get; init; }
}

/// <summary>
/// A join request for a team.
/// </summary>
public class TeamRequest
{
    /// <summary>
    /// The team ID.
    /// </summary>
    [JsonPropertyName("teamId")]
    public required string TeamId { get; init; }

    /// <summary>
    /// The requesting user's ID.
    /// </summary>
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    /// <summary>
    /// When the request was made (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("date")]
    public long Date { get; init; }

    /// <summary>
    /// The message attached to the join request.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }
}

/// <summary>
/// A join request with full user information.
/// </summary>
public class TeamRequestWithUser
{
    /// <summary>
    /// The join request details.
    /// </summary>
    [JsonPropertyName("request")]
    public required TeamRequest Request { get; init; }

    /// <summary>
    /// Full user information for the requester.
    /// </summary>
    [JsonPropertyName("user")]
    public required TeamRequestUser User { get; init; }
}

/// <summary>
/// User information included with a team join request.
/// </summary>
public class TeamRequestUser
{
    /// <summary>
    /// The user's ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The user's username.
    /// </summary>
    [JsonPropertyName("username")]
    public required string Username { get; init; }

    /// <summary>
    /// The user's flair emoji.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }

    /// <summary>
    /// When the account was created (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("createdAt")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// When the user was last seen online (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("seenAt")]
    public long? SeenAt { get; init; }

    /// <summary>
    /// The user's performance ratings.
    /// </summary>
    [JsonPropertyName("perfs")]
    public TeamMemberPerfs? Perfs { get; init; }

    /// <summary>
    /// The user's play time statistics.
    /// </summary>
    [JsonPropertyName("playTime")]
    public TeamPlayTime? PlayTime { get; init; }
}

/// <summary>
/// Play time statistics.
/// </summary>
public class TeamPlayTime
{
    /// <summary>
    /// Total play time in seconds.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; init; }

    /// <summary>
    /// Time spent on TV in seconds.
    /// </summary>
    [JsonPropertyName("tv")]
    public int Tv { get; init; }
}

#endregion

#region Deprecated Types (for backward compatibility)

/// <summary>
/// Team search result (deprecated, use TeamPaginator instead).
/// </summary>
[Obsolete("Use TeamPaginator instead")]
public class TeamSearchResult
{
    /// <summary>
    /// Current page.
    /// </summary>
    public int CurrentPage { get; init; }

    /// <summary>
    /// Maximum pages.
    /// </summary>
    public int MaxPerPage { get; init; }

    /// <summary>
    /// Teams found.
    /// </summary>
    public IReadOnlyList<Team>? CurrentPageResults { get; init; }

    /// <summary>
    /// Total number of results.
    /// </summary>
    public int NbResults { get; init; }

    /// <summary>
    /// Number of pages.
    /// </summary>
    public int NbPages { get; init; }
}

#endregion
