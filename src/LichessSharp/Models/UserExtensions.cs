using System.Text.Json.Serialization;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models;

/// <summary>
/// Performance statistics for a specific variant, including detailed stats.
/// </summary>
public class UserPerformance
{
    /// <summary>
    /// The user's performance rating data.
    /// </summary>
    [JsonPropertyName("perf")]
    public PerfStats? Perf { get; init; }

    /// <summary>
    /// The user's rank in this variant (if ranked).
    /// </summary>
    [JsonPropertyName("rank")]
    public int? Rank { get; init; }

    /// <summary>
    /// The user's percentile ranking.
    /// </summary>
    [JsonPropertyName("percentile")]
    public double? Percentile { get; init; }

    /// <summary>
    /// Detailed statistics for this performance type.
    /// </summary>
    [JsonPropertyName("stat")]
    public PerformanceStatistics? Stat { get; init; }
}

/// <summary>
/// Detailed performance statistics.
/// </summary>
public class PerformanceStatistics
{
    /// <summary>
    /// The performance type key.
    /// </summary>
    [JsonPropertyName("perfType")]
    public PerfType? PerfType { get; init; }

    /// <summary>
    /// Highest rating achieved.
    /// </summary>
    [JsonPropertyName("highest")]
    public RatingAtTime? Highest { get; init; }

    /// <summary>
    /// Lowest rating achieved.
    /// </summary>
    [JsonPropertyName("lowest")]
    public RatingAtTime? Lowest { get; init; }

    /// <summary>
    /// Best wins against higher-rated opponents.
    /// </summary>
    [JsonPropertyName("bestWins")]
    public ResultsVsOpponents? BestWins { get; init; }

    /// <summary>
    /// Worst losses against lower-rated opponents.
    /// </summary>
    [JsonPropertyName("worstLosses")]
    public ResultsVsOpponents? WorstLosses { get; init; }

    /// <summary>
    /// Game count statistics.
    /// </summary>
    [JsonPropertyName("count")]
    public PerformanceCount? Count { get; init; }

    /// <summary>
    /// Results against various rating ranges.
    /// </summary>
    [JsonPropertyName("resultStreak")]
    public ResultStreak? ResultStreak { get; init; }

    /// <summary>
    /// Play streak statistics.
    /// </summary>
    [JsonPropertyName("playStreak")]
    public PlayStreak? PlayStreak { get; init; }
}

/// <summary>
/// Performance type information.
/// </summary>
public class PerfType
{
    /// <summary>
    /// The performance type key.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    /// <summary>
    /// The display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Rating at a specific time.
/// </summary>
public class RatingAtTime
{
    /// <summary>
    /// The rating value.
    /// </summary>
    [JsonPropertyName("int")]
    public int Value { get; init; }

    /// <summary>
    /// When this rating was achieved.
    /// </summary>
    [JsonPropertyName("at")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset At { get; init; }

    /// <summary>
    /// The game ID where this rating was achieved.
    /// </summary>
    [JsonPropertyName("gameId")]
    public string? GameId { get; init; }
}

/// <summary>
/// Results against opponents.
/// </summary>
public class ResultsVsOpponents
{
    /// <summary>
    /// List of results.
    /// </summary>
    [JsonPropertyName("results")]
    public IReadOnlyList<OpponentResult>? Results { get; init; }
}

/// <summary>
/// Result against a specific opponent.
/// </summary>
public class OpponentResult
{
    /// <summary>
    /// Opponent's rating.
    /// </summary>
    [JsonPropertyName("opInt")]
    public int OpponentRating { get; init; }

    /// <summary>
    /// Opponent's user ID.
    /// </summary>
    [JsonPropertyName("opId")]
    public OpponentId? OpponentId { get; init; }

    /// <summary>
    /// When the game was played.
    /// </summary>
    [JsonPropertyName("at")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset At { get; init; }

    /// <summary>
    /// The game ID.
    /// </summary>
    [JsonPropertyName("gameId")]
    public string? GameId { get; init; }
}

/// <summary>
/// Opponent identifier.
/// </summary>
public class OpponentId
{
    /// <summary>
    /// Opponent's user ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Opponent's username.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Opponent's title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }
}

/// <summary>
/// Performance game count.
/// </summary>
public class PerformanceCount
{
    /// <summary>
    /// Total games.
    /// </summary>
    [JsonPropertyName("all")]
    public int All { get; init; }

    /// <summary>
    /// Rated games.
    /// </summary>
    [JsonPropertyName("rated")]
    public int Rated { get; init; }

    /// <summary>
    /// Games won.
    /// </summary>
    [JsonPropertyName("win")]
    public int Win { get; init; }

    /// <summary>
    /// Games lost.
    /// </summary>
    [JsonPropertyName("loss")]
    public int Loss { get; init; }

    /// <summary>
    /// Games drawn.
    /// </summary>
    [JsonPropertyName("draw")]
    public int Draw { get; init; }

    /// <summary>
    /// Games won on time.
    /// </summary>
    [JsonPropertyName("tour")]
    public int Tour { get; init; }

    /// <summary>
    /// Berserk games.
    /// </summary>
    [JsonPropertyName("berserk")]
    public int Berserk { get; init; }

    /// <summary>
    /// Opponent disconnected.
    /// </summary>
    [JsonPropertyName("opDisc")]
    public int OpponentDisconnected { get; init; }

    /// <summary>
    /// Disconnected games.
    /// </summary>
    [JsonPropertyName("seconds")]
    public int Seconds { get; init; }
}

/// <summary>
/// Result streak statistics.
/// </summary>
public class ResultStreak
{
    /// <summary>
    /// Win streak.
    /// </summary>
    [JsonPropertyName("win")]
    public StreakInfo? Win { get; init; }

    /// <summary>
    /// Loss streak.
    /// </summary>
    [JsonPropertyName("loss")]
    public StreakInfo? Loss { get; init; }
}

/// <summary>
/// Streak information.
/// </summary>
public class StreakInfo
{
    /// <summary>
    /// Current streak.
    /// </summary>
    [JsonPropertyName("cur")]
    public StreakValue? Current { get; init; }

    /// <summary>
    /// Maximum streak.
    /// </summary>
    [JsonPropertyName("max")]
    public StreakValue? Max { get; init; }
}

/// <summary>
/// Streak value.
/// </summary>
public class StreakValue
{
    /// <summary>
    /// Streak count.
    /// </summary>
    [JsonPropertyName("v")]
    public int Value { get; init; }
}

/// <summary>
/// Play streak statistics.
/// </summary>
public class PlayStreak
{
    /// <summary>
    /// Number of games in current session.
    /// </summary>
    [JsonPropertyName("nb")]
    public StreakInfo? NbGames { get; init; }

    /// <summary>
    /// Time spent playing.
    /// </summary>
    [JsonPropertyName("time")]
    public StreakInfo? Time { get; init; }

    /// <summary>
    /// Last played time.
    /// </summary>
    [JsonPropertyName("lastDate")]
    [JsonConverter(typeof(NullableUnixMillisecondsConverter))]
    public DateTimeOffset? LastDate { get; init; }
}

/// <summary>
/// User activity entry.
/// </summary>
public class UserActivity
{
    /// <summary>
    /// Activity interval.
    /// </summary>
    [JsonPropertyName("interval")]
    public ActivityInterval? Interval { get; init; }

    /// <summary>
    /// Games activity.
    /// </summary>
    [JsonPropertyName("games")]
    public ActivityGames? Games { get; init; }

    /// <summary>
    /// Puzzles activity.
    /// </summary>
    [JsonPropertyName("puzzles")]
    public ActivityPuzzles? Puzzles { get; init; }

    /// <summary>
    /// Tournaments activity.
    /// </summary>
    [JsonPropertyName("tournaments")]
    public ActivityTournaments? Tournaments { get; init; }

    /// <summary>
    /// Practice activity.
    /// </summary>
    [JsonPropertyName("practice")]
    public IReadOnlyList<ActivityPractice>? Practice { get; init; }

    /// <summary>
    /// Correspondence moves.
    /// </summary>
    [JsonPropertyName("correspondenceMoves")]
    public ActivityCorrespondence? CorrespondenceMoves { get; init; }

    /// <summary>
    /// Correspondence ends.
    /// </summary>
    [JsonPropertyName("correspondenceEnds")]
    public ActivityCorrespondenceEnds? CorrespondenceEnds { get; init; }

    /// <summary>
    /// Follows activity.
    /// </summary>
    [JsonPropertyName("follows")]
    public ActivityFollows? Follows { get; init; }

    /// <summary>
    /// Studies activity.
    /// </summary>
    [JsonPropertyName("studies")]
    public IReadOnlyList<ActivityStudy>? Studies { get; init; }

    /// <summary>
    /// Teams joined.
    /// </summary>
    [JsonPropertyName("teams")]
    public IReadOnlyList<ActivityTeam>? Teams { get; init; }

    /// <summary>
    /// Posts activity.
    /// </summary>
    [JsonPropertyName("posts")]
    public IReadOnlyList<ActivityPost>? Posts { get; init; }

    /// <summary>
    /// Stream activity.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool? Stream { get; init; }
}

/// <summary>
/// Activity interval.
/// </summary>
public class ActivityInterval
{
    /// <summary>
    /// Start of the interval.
    /// </summary>
    [JsonPropertyName("start")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset Start { get; init; }

    /// <summary>
    /// End of the interval.
    /// </summary>
    [JsonPropertyName("end")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset End { get; init; }
}

/// <summary>
/// Games activity by variant.
/// </summary>
public class ActivityGames
{
    /// <summary>
    /// Activity per variant (dynamic keys like "bullet", "blitz", etc.).
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? Variants { get; set; }
}

/// <summary>
/// Puzzles activity.
/// </summary>
public class ActivityPuzzles
{
    /// <summary>
    /// Score statistics.
    /// </summary>
    [JsonPropertyName("score")]
    public ActivityScore? Score { get; init; }
}

/// <summary>
/// Activity score.
/// </summary>
public class ActivityScore
{
    /// <summary>
    /// Number won.
    /// </summary>
    [JsonPropertyName("win")]
    public int Win { get; init; }

    /// <summary>
    /// Number lost.
    /// </summary>
    [JsonPropertyName("loss")]
    public int Loss { get; init; }

    /// <summary>
    /// Number drawn.
    /// </summary>
    [JsonPropertyName("draw")]
    public int Draw { get; init; }

    /// <summary>
    /// Rating change.
    /// </summary>
    [JsonPropertyName("rp")]
    public ActivityRatingProgress? RatingProgress { get; init; }
}

/// <summary>
/// Rating progress.
/// </summary>
public class ActivityRatingProgress
{
    /// <summary>
    /// Rating before.
    /// </summary>
    [JsonPropertyName("before")]
    public int Before { get; init; }

    /// <summary>
    /// Rating after.
    /// </summary>
    [JsonPropertyName("after")]
    public int After { get; init; }
}

/// <summary>
/// Tournaments activity.
/// </summary>
public class ActivityTournaments
{
    /// <summary>
    /// Number of tournaments.
    /// </summary>
    [JsonPropertyName("nb")]
    public int Nb { get; init; }
}

/// <summary>
/// Practice activity.
/// </summary>
public class ActivityPractice
{
    /// <summary>
    /// Practice name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Number of positions practiced.
    /// </summary>
    [JsonPropertyName("nbPositions")]
    public int NbPositions { get; init; }
}

/// <summary>
/// Correspondence activity.
/// </summary>
public class ActivityCorrespondence
{
    /// <summary>
    /// Number of moves.
    /// </summary>
    [JsonPropertyName("nb")]
    public int Nb { get; init; }

    /// <summary>
    /// Games involved.
    /// </summary>
    [JsonPropertyName("games")]
    public IReadOnlyList<ActivityCorrespondenceGame>? Games { get; init; }
}

/// <summary>
/// Correspondence game info.
/// </summary>
public class ActivityCorrespondenceGame
{
    /// <summary>
    /// Game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}

/// <summary>
/// Correspondence ends activity.
/// </summary>
public class ActivityCorrespondenceEnds
{
    /// <summary>
    /// Score.
    /// </summary>
    [JsonPropertyName("score")]
    public ActivityScore? Score { get; init; }

    /// <summary>
    /// Games ended.
    /// </summary>
    [JsonPropertyName("games")]
    public IReadOnlyList<ActivityCorrespondenceGame>? Games { get; init; }
}

/// <summary>
/// Follows activity.
/// </summary>
public class ActivityFollows
{
    /// <summary>
    /// Users followed in this period.
    /// </summary>
    [JsonPropertyName("in")]
    public ActivityFollowList? In { get; init; }

    /// <summary>
    /// Users who followed in this period.
    /// </summary>
    [JsonPropertyName("out")]
    public ActivityFollowList? Out { get; init; }
}

/// <summary>
/// Follow list.
/// </summary>
public class ActivityFollowList
{
    /// <summary>
    /// User IDs.
    /// </summary>
    [JsonPropertyName("ids")]
    public IReadOnlyList<string>? Ids { get; init; }
}

/// <summary>
/// Study activity.
/// </summary>
public class ActivityStudy
{
    /// <summary>
    /// Study ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Study name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Team activity.
/// </summary>
public class ActivityTeam
{
    /// <summary>
    /// Team URL/ID.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Team name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Post activity.
/// </summary>
public class ActivityPost
{
    /// <summary>
    /// Topic URL.
    /// </summary>
    [JsonPropertyName("topicUrl")]
    public string? TopicUrl { get; init; }

    /// <summary>
    /// Topic name.
    /// </summary>
    [JsonPropertyName("topicName")]
    public string? TopicName { get; init; }

    /// <summary>
    /// Post URLs.
    /// </summary>
    [JsonPropertyName("posts")]
    public IReadOnlyList<string>? Posts { get; init; }
}

/// <summary>
/// Crosstable (head-to-head) statistics between two users.
/// </summary>
public class Crosstable
{
    /// <summary>
    /// Scores for each user (user ID to score).
    /// Score is number of points (wins = 1, draws = 0.5).
    /// </summary>
    [JsonPropertyName("users")]
    public Dictionary<string, double>? Users { get; init; }

    /// <summary>
    /// Total number of games played between the users.
    /// </summary>
    [JsonPropertyName("nbGames")]
    public int NbGames { get; init; }

    /// <summary>
    /// Current matchup information (if in an ongoing match).
    /// </summary>
    [JsonPropertyName("matchup")]
    public CrosstableMatchup? Matchup { get; init; }
}

/// <summary>
/// Current matchup information.
/// </summary>
public class CrosstableMatchup
{
    /// <summary>
    /// Scores for each user in the current matchup.
    /// </summary>
    [JsonPropertyName("users")]
    public Dictionary<string, double>? Users { get; init; }

    /// <summary>
    /// Number of games in the current matchup.
    /// </summary>
    [JsonPropertyName("nbGames")]
    public int NbGames { get; init; }
}

/// <summary>
/// A live streamer on Lichess.
/// </summary>
public class Streamer
{
    /// <summary>
    /// The user's unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The user's display name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The user's title, if any.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Whether the user is online.
    /// </summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>
    /// Whether the user is a patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }

    /// <summary>
    /// Stream information.
    /// </summary>
    [JsonPropertyName("stream")]
    public StreamInfo? Stream { get; init; }

    /// <summary>
    /// Streamer-specific information.
    /// </summary>
    [JsonPropertyName("streamer")]
    public StreamerInfo? StreamerDetails { get; init; }
}

/// <summary>
/// Stream information.
/// </summary>
public class StreamInfo
{
    /// <summary>
    /// Streaming service (twitch, youtube).
    /// </summary>
    [JsonPropertyName("service")]
    public string? Service { get; init; }

    /// <summary>
    /// Stream status.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Stream language.
    /// </summary>
    [JsonPropertyName("lang")]
    public string? Lang { get; init; }
}

/// <summary>
/// Streamer-specific information.
/// </summary>
public class StreamerInfo
{
    /// <summary>
    /// Streamer name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Streamer headline.
    /// </summary>
    [JsonPropertyName("headline")]
    public string? Headline { get; init; }

    /// <summary>
    /// Streamer description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Twitch channel URL.
    /// </summary>
    [JsonPropertyName("twitch")]
    public string? Twitch { get; init; }

    /// <summary>
    /// YouTube channel URL.
    /// </summary>
    [JsonPropertyName("youTube")]
    public string? YouTube { get; init; }

    /// <summary>
    /// Stream image URL.
    /// </summary>
    [JsonPropertyName("image")]
    public string? Image { get; init; }
}

/// <summary>
/// Stream channel information.
/// </summary>
public class StreamChannel
{
    /// <summary>
    /// Channel name/ID.
    /// </summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; init; }
}

/// <summary>
/// User note response.
/// </summary>
public class NoteResponse
{
    /// <summary>
    /// The note text.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

/// <summary>
/// User timeline.
/// </summary>
public class Timeline
{
    /// <summary>
    /// Timeline entries.
    /// </summary>
    [JsonPropertyName("entries")]
    public IReadOnlyList<TimelineEntry>? Entries { get; init; }

    /// <summary>
    /// Users referenced in the timeline.
    /// </summary>
    [JsonPropertyName("users")]
    public Dictionary<string, LightUser>? Users { get; init; }
}

/// <summary>
/// A timeline entry.
/// </summary>
public class TimelineEntry
{
    /// <summary>
    /// Entry type.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Entry data (varies by type).
    /// </summary>
    [JsonPropertyName("data")]
    public TimelineData? Data { get; init; }

    /// <summary>
    /// When this entry occurred.
    /// </summary>
    [JsonPropertyName("date")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset Date { get; init; }
}

/// <summary>
/// Timeline entry data.
/// </summary>
public class TimelineData
{
    /// <summary>
    /// User ID involved.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    /// <summary>
    /// Game ID involved.
    /// </summary>
    [JsonPropertyName("gameId")]
    public string? GameId { get; init; }

    /// <summary>
    /// Game full ID.
    /// </summary>
    [JsonPropertyName("fullId")]
    public string? FullId { get; init; }

    /// <summary>
    /// Opponent info.
    /// </summary>
    [JsonPropertyName("opponent")]
    public string? Opponent { get; init; }

    /// <summary>
    /// Study ID.
    /// </summary>
    [JsonPropertyName("studyId")]
    public string? StudyId { get; init; }

    /// <summary>
    /// Study name.
    /// </summary>
    [JsonPropertyName("studyName")]
    public string? StudyName { get; init; }

    /// <summary>
    /// Team ID.
    /// </summary>
    [JsonPropertyName("teamId")]
    public string? TeamId { get; init; }

    /// <summary>
    /// Team name.
    /// </summary>
    [JsonPropertyName("teamName")]
    public string? TeamName { get; init; }

    /// <summary>
    /// Forum topic ID.
    /// </summary>
    [JsonPropertyName("topicId")]
    public string? TopicId { get; init; }

    /// <summary>
    /// Forum topic name.
    /// </summary>
    [JsonPropertyName("topicName")]
    public string? TopicName { get; init; }

    /// <summary>
    /// Simul ID.
    /// </summary>
    [JsonPropertyName("simulId")]
    public string? SimulId { get; init; }

    /// <summary>
    /// Simul name.
    /// </summary>
    [JsonPropertyName("simulName")]
    public string? SimulName { get; init; }

    /// <summary>
    /// Tournament ID.
    /// </summary>
    [JsonPropertyName("tourId")]
    public string? TourId { get; init; }

    /// <summary>
    /// Tournament name.
    /// </summary>
    [JsonPropertyName("tourName")]
    public string? TourName { get; init; }
}

/// <summary>
/// Light user information.
/// </summary>
public class LightUser
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
    /// Title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Whether the user is a patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }
}

/// <summary>
/// Player autocomplete result (object mode).
/// </summary>
public class AutocompletePlayer
{
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Username.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Whether the user is a patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }

    /// <summary>
    /// Whether the user is online.
    /// </summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }
}

/// <summary>
/// Response wrapper for autocomplete with object mode.
/// </summary>
public class AutocompleteResponse
{
    /// <summary>
    /// The result players.
    /// </summary>
    [JsonPropertyName("result")]
    public IReadOnlyList<AutocompletePlayer>? Result { get; init; }
}
