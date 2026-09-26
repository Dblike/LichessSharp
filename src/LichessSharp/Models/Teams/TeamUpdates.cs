using System.Text.Json.Serialization;
using LichessSharp.Models.Common;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models.Teams;

/// <summary>
///     Updates posted by the leaders of the teams the authenticated user has joined.
///     Returned by <c>GET /team/updates</c>.
/// </summary>
[ResponseOnly]
public class TeamUpdates
{
    /// <summary>
    ///     The requested page of updates, most recent first.
    /// </summary>
    [JsonPropertyName("updates")]
    public required TeamUpdatesPager Updates { get; init; }

    /// <summary>
    ///     Per-team summary of unread updates across all joined teams.
    /// </summary>
    [JsonPropertyName("byTeam")]
    public required IReadOnlyList<TeamUpdatesByTeamEntry> ByTeam { get; init; }
}

/// <summary>
///     Updates posted by the leaders of one team the authenticated user has joined.
///     Returned by <c>GET /team/updates/{teamId}</c>.
/// </summary>
[ResponseOnly]
public class TeamUpdatesOfTeam
{
    /// <summary>
    ///     The team whose updates are listed.
    /// </summary>
    [JsonPropertyName("team")]
    public required LightTeam Team { get; init; }

    /// <summary>
    ///     Whether the authenticated user is subscribed to this team's updates.
    /// </summary>
    [JsonPropertyName("subscribed")]
    public bool Subscribed { get; init; }

    /// <summary>
    ///     The requested page of updates, most recent first.
    /// </summary>
    [JsonPropertyName("updates")]
    public required TeamUpdatesPager Updates { get; init; }

    /// <summary>
    ///     Per-team summary of unread updates across all joined teams.
    /// </summary>
    [JsonPropertyName("byTeam")]
    public required IReadOnlyList<TeamUpdatesByTeamEntry> ByTeam { get; init; }
}

/// <summary>
///     A page of team updates.
/// </summary>
[ResponseOnly]
public class TeamUpdatesPager
{
    /// <summary>
    ///     Current page number (1-indexed).
    /// </summary>
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; init; }

    /// <summary>
    ///     Maximum results per page.
    /// </summary>
    [JsonPropertyName("maxPerPage")]
    public int MaxPerPage { get; init; }

    /// <summary>
    ///     Updates on the current page.
    /// </summary>
    [JsonPropertyName("currentPageResults")]
    public required IReadOnlyList<TeamUpdate> CurrentPageResults { get; init; }

    /// <summary>
    ///     Previous page number (null if on the first page).
    /// </summary>
    [JsonPropertyName("previousPage")]
    public int? PreviousPage { get; init; }

    /// <summary>
    ///     Next page number (null if on the last page).
    /// </summary>
    [JsonPropertyName("nextPage")]
    public int? NextPage { get; init; }

    /// <summary>
    ///     Total number of updates across all pages.
    /// </summary>
    [JsonPropertyName("nbResults")]
    public int NbResults { get; init; }

    /// <summary>
    ///     Total number of pages.
    /// </summary>
    [JsonPropertyName("nbPages")]
    public int NbPages { get; init; }
}

/// <summary>
///     A single team update together with its read state for the authenticated user.
/// </summary>
[ResponseOnly]
public class TeamUpdate
{
    /// <summary>
    ///     The update message.
    /// </summary>
    [JsonPropertyName("msg")]
    public required TeamUpdateMessage Message { get; init; }

    /// <summary>
    ///     Whether the authenticated user has already seen this update.
    /// </summary>
    [JsonPropertyName("seen")]
    public bool Seen { get; init; }
}

/// <summary>
///     The content of a team update posted by a team leader.
/// </summary>
[ResponseOnly]
public class TeamUpdateMessage
{
    /// <summary>
    ///     The update ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     When the update was posted.
    /// </summary>
    [JsonPropertyName("date")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset Date { get; init; }

    /// <summary>
    ///     The team leader who posted the update.
    /// </summary>
    [JsonPropertyName("sender")]
    public required LightUser Sender { get; init; }

    /// <summary>
    ///     The team the update was posted to.
    /// </summary>
    [JsonPropertyName("team")]
    public LightTeam? Team { get; init; }

    /// <summary>
    ///     The update text.
    /// </summary>
    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

/// <summary>
///     Unread-update summary for one joined team.
/// </summary>
[ResponseOnly]
public class TeamUpdatesByTeamEntry
{
    /// <summary>
    ///     The team.
    /// </summary>
    [JsonPropertyName("team")]
    public required LightTeam Team { get; init; }

    /// <summary>
    ///     When the most recent update was posted.
    /// </summary>
    [JsonPropertyName("last")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset Last { get; init; }

    /// <summary>
    ///     Number of updates the authenticated user has not seen yet.
    /// </summary>
    [JsonPropertyName("unread")]
    public int Unread { get; init; }
}
