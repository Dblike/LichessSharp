namespace LichessSharp.Api;

/// <summary>
/// Teams API - Access and manage Lichess teams and their members.
/// </summary>
public interface ITeamsApi
{
    /// <summary>
    /// Get information about a team.
    /// </summary>
    Task<Team> GetAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get teams a user is a member of.
    /// </summary>
    Task<IReadOnlyList<Team>> GetUserTeamsAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream members of a team.
    /// </summary>
    IAsyncEnumerable<TeamMember> StreamMembersAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Join a team.
    /// </summary>
    Task<bool> JoinAsync(string teamId, string? message = null, string? password = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Leave a team.
    /// </summary>
    Task<bool> LeaveAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for teams.
    /// </summary>
    Task<TeamSearchResult> SearchAsync(string text, int page = 1, CancellationToken cancellationToken = default);
}

/// <summary>
/// A Lichess team.
/// </summary>
public class Team
{
    /// <summary>
    /// The team ID.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The team name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The team description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Number of members.
    /// </summary>
    public int NbMembers { get; init; }

    /// <summary>
    /// Team leader.
    /// </summary>
    public TeamLeader? Leader { get; init; }
}

/// <summary>
/// A team leader.
/// </summary>
public class TeamLeader
{
    /// <summary>
    /// The leader's ID.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The leader's name.
    /// </summary>
    public required string Name { get; init; }
}

/// <summary>
/// A team member.
/// </summary>
public class TeamMember
{
    /// <summary>
    /// The member's user ID.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// When the member joined.
    /// </summary>
    public DateTimeOffset JoinedAt { get; init; }
}

/// <summary>
/// Team search result.
/// </summary>
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
