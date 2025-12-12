using System.Text.Json.Serialization;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models.Users;

/// <summary>
///     User activity entry.
/// </summary>
[ResponseOnly]
public class UserActivity
{
    /// <summary>
    ///     Activity interval.
    /// </summary>
    [JsonPropertyName("interval")]
    public ActivityInterval? Interval { get; init; }

    /// <summary>
    ///     Games activity.
    /// </summary>
    [JsonPropertyName("games")]
    public ActivityGames? Games { get; init; }

    /// <summary>
    ///     Puzzles activity.
    /// </summary>
    [JsonPropertyName("puzzles")]
    public ActivityPuzzles? Puzzles { get; init; }

    /// <summary>
    ///     Tournaments activity.
    /// </summary>
    [JsonPropertyName("tournaments")]
    public ActivityTournaments? Tournaments { get; init; }

    /// <summary>
    ///     Practice activity.
    /// </summary>
    [JsonPropertyName("practice")]
    public IReadOnlyList<ActivityPractice>? Practice { get; init; }

    /// <summary>
    ///     Correspondence moves.
    /// </summary>
    [JsonPropertyName("correspondenceMoves")]
    public ActivityCorrespondence? CorrespondenceMoves { get; init; }

    /// <summary>
    ///     Correspondence ends.
    /// </summary>
    [JsonPropertyName("correspondenceEnds")]
    public ActivityCorrespondenceEnds? CorrespondenceEnds { get; init; }

    /// <summary>
    ///     Follows activity.
    /// </summary>
    [JsonPropertyName("follows")]
    public ActivityFollows? Follows { get; init; }

    /// <summary>
    ///     Studies activity.
    /// </summary>
    [JsonPropertyName("studies")]
    public IReadOnlyList<ActivityStudy>? Studies { get; init; }

    /// <summary>
    ///     Teams joined.
    /// </summary>
    [JsonPropertyName("teams")]
    public IReadOnlyList<ActivityTeam>? Teams { get; init; }

    /// <summary>
    ///     Posts activity.
    /// </summary>
    [JsonPropertyName("posts")]
    public IReadOnlyList<ActivityPost>? Posts { get; init; }

    /// <summary>
    ///     Stream activity.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool? Stream { get; init; }
}

/// <summary>
///     Activity interval.
/// </summary>
[ResponseOnly]
public class ActivityInterval
{
    /// <summary>
    ///     Start of the interval.
    /// </summary>
    [JsonPropertyName("start")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset Start { get; init; }

    /// <summary>
    ///     End of the interval.
    /// </summary>
    [JsonPropertyName("end")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset End { get; init; }
}

/// <summary>
///     Games activity by variant.
/// </summary>
[ResponseOnly]
public class ActivityGames
{
    /// <summary>
    ///     Activity per variant (dynamic keys like "bullet", "blitz", etc.).
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? Variants { get; set; }
}

/// <summary>
///     Puzzles activity.
/// </summary>
[ResponseOnly]
public class ActivityPuzzles
{
    /// <summary>
    ///     Score statistics.
    /// </summary>
    [JsonPropertyName("score")]
    public ActivityScore? Score { get; init; }
}

/// <summary>
///     Activity score.
/// </summary>
[ResponseOnly]
public class ActivityScore
{
    /// <summary>
    ///     Number won.
    /// </summary>
    [JsonPropertyName("win")]
    public int Win { get; init; }

    /// <summary>
    ///     Number lost.
    /// </summary>
    [JsonPropertyName("loss")]
    public int Loss { get; init; }

    /// <summary>
    ///     Number drawn.
    /// </summary>
    [JsonPropertyName("draw")]
    public int Draw { get; init; }

    /// <summary>
    ///     Rating change.
    /// </summary>
    [JsonPropertyName("rp")]
    public ActivityRatingProgress? RatingProgress { get; init; }
}

/// <summary>
///     Rating progress.
/// </summary>
[ResponseOnly]
public class ActivityRatingProgress
{
    /// <summary>
    ///     Rating before.
    /// </summary>
    [JsonPropertyName("before")]
    public int Before { get; init; }

    /// <summary>
    ///     Rating after.
    /// </summary>
    [JsonPropertyName("after")]
    public int After { get; init; }
}

/// <summary>
///     Tournaments activity.
/// </summary>
[ResponseOnly]
public class ActivityTournaments
{
    /// <summary>
    ///     Number of tournaments.
    /// </summary>
    [JsonPropertyName("nb")]
    public int Nb { get; init; }
}

/// <summary>
///     Practice activity.
/// </summary>
[ResponseOnly]
public class ActivityPractice
{
    /// <summary>
    ///     Practice name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     Number of positions practiced.
    /// </summary>
    [JsonPropertyName("nbPositions")]
    public int NbPositions { get; init; }
}

/// <summary>
///     Correspondence activity.
/// </summary>
[ResponseOnly]
public class ActivityCorrespondence
{
    /// <summary>
    ///     Number of moves.
    /// </summary>
    [JsonPropertyName("nb")]
    public int Nb { get; init; }

    /// <summary>
    ///     Games involved.
    /// </summary>
    [JsonPropertyName("games")]
    public IReadOnlyList<ActivityCorrespondenceGame>? Games { get; init; }
}

/// <summary>
///     Correspondence game info.
/// </summary>
[ResponseOnly]
public class ActivityCorrespondenceGame
{
    /// <summary>
    ///     Game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}

/// <summary>
///     Correspondence ends activity.
/// </summary>
[ResponseOnly]
public class ActivityCorrespondenceEnds
{
    /// <summary>
    ///     Score.
    /// </summary>
    [JsonPropertyName("score")]
    public ActivityScore? Score { get; init; }

    /// <summary>
    ///     Games ended.
    /// </summary>
    [JsonPropertyName("games")]
    public IReadOnlyList<ActivityCorrespondenceGame>? Games { get; init; }
}

/// <summary>
///     Follows activity.
/// </summary>
[ResponseOnly]
public class ActivityFollows
{
    /// <summary>
    ///     Users followed in this period.
    /// </summary>
    [JsonPropertyName("in")]
    public ActivityFollowList? In { get; init; }

    /// <summary>
    ///     Users who followed in this period.
    /// </summary>
    [JsonPropertyName("out")]
    public ActivityFollowList? Out { get; init; }
}

/// <summary>
///     Follow list.
/// </summary>
[ResponseOnly]
public class ActivityFollowList
{
    /// <summary>
    ///     User IDs.
    /// </summary>
    [JsonPropertyName("ids")]
    public IReadOnlyList<string>? Ids { get; init; }
}

/// <summary>
///     Study activity.
/// </summary>
[ResponseOnly]
public class ActivityStudy
{
    /// <summary>
    ///     Study ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    ///     Study name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
///     Team activity.
/// </summary>
[ResponseOnly]
public class ActivityTeam
{
    /// <summary>
    ///     Team URL/ID.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    ///     Team name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
///     Post activity.
/// </summary>
[ResponseOnly]
public class ActivityPost
{
    /// <summary>
    ///     Topic URL.
    /// </summary>
    [JsonPropertyName("topicUrl")]
    public string? TopicUrl { get; init; }

    /// <summary>
    ///     Topic name.
    /// </summary>
    [JsonPropertyName("topicName")]
    public string? TopicName { get; init; }

    /// <summary>
    ///     Post URLs.
    /// </summary>
    [JsonPropertyName("posts")]
    public IReadOnlyList<string>? Posts { get; init; }
}