using System.Text.Json.Serialization;
using LichessSharp.Models.Enums;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Models.Users;

/// <summary>
///     Basic user information.
/// </summary>
/// <remarks>
///     Note: The Lichess API uses "username" for full User objects and "name" for LightUser objects.
///     This class uses "username" to match the full User schema.
/// </remarks>
public class User
{
    /// <summary>
    ///     The user's unique identifier (lowercase username).
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     The user's display name.
    /// </summary>
    [JsonPropertyName("username")]
    public required string Username { get; init; }

    /// <summary>
    ///     The user's FIDE/Lichess title, if any.
    /// </summary>
    [JsonPropertyName("title")]
    public Title? Title { get; init; }

    /// <summary>
    ///     Whether the user is a Lichess patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }

    /// <summary>
    ///     The user's flair emoji.
    /// </summary>
    [JsonPropertyName("flair")]
    public string? Flair { get; init; }
}

/// <summary>
///     Extended user information including profile and statistics.
/// </summary>
public class UserExtended : User
{
    /// <summary>
    ///     When the account was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    [JsonConverter(typeof(UnixMillisecondsConverter))]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    ///     When the user was last seen online.
    /// </summary>
    [JsonPropertyName("seenAt")]
    [JsonConverter(typeof(NullableUnixMillisecondsConverter))]
    public DateTimeOffset? SeenAt { get; init; }

    /// <summary>
    ///     The user's profile information.
    /// </summary>
    [JsonPropertyName("profile")]
    public UserProfile? Profile { get; init; }

    /// <summary>
    ///     The user's performance ratings per game mode.
    /// </summary>
    [JsonPropertyName("perfs")]
    public Dictionary<string, PerfStats>? Perfs { get; init; }

    /// <summary>
    ///     Total play time in seconds.
    /// </summary>
    [JsonPropertyName("playTime")]
    public PlayTime? PlayTime { get; init; }

    /// <summary>
    ///     The number of users this user is following.
    /// </summary>
    [JsonPropertyName("nbFollowing")]
    public int? NbFollowing { get; init; }

    /// <summary>
    ///     The number of users following this user.
    /// </summary>
    [JsonPropertyName("nbFollowers")]
    public int? NbFollowers { get; init; }

    /// <summary>
    ///     Number of games played.
    /// </summary>
    [JsonPropertyName("count")]
    public GameCount? Count { get; init; }

    /// <summary>
    ///     Whether the account is marked for ToS violations.
    /// </summary>
    [JsonPropertyName("tosViolation")]
    public bool? TosViolation { get; init; }

    /// <summary>
    ///     Whether the account is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }

    /// <summary>
    ///     Whether the account is verified.
    /// </summary>
    [JsonPropertyName("verified")]
    public bool? Verified { get; init; }

    /// <summary>
    ///     URL to the user's page.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}

/// <summary>
///     User profile information.
/// </summary>
public class UserProfile
{
    /// <summary>
    ///     The user's country code.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; init; }

    /// <summary>
    ///     The user's location.
    /// </summary>
    [JsonPropertyName("location")]
    public string? Location { get; init; }

    /// <summary>
    ///     The user's bio.
    /// </summary>
    [JsonPropertyName("bio")]
    public string? Bio { get; init; }

    /// <summary>
    ///     The user's first name.
    /// </summary>
    [JsonPropertyName("firstName")]
    public string? FirstName { get; init; }

    /// <summary>
    ///     The user's last name.
    /// </summary>
    [JsonPropertyName("lastName")]
    public string? LastName { get; init; }

    /// <summary>
    ///     FIDE rating.
    /// </summary>
    [JsonPropertyName("fideRating")]
    public int? FideRating { get; init; }

    /// <summary>
    ///     Links associated with the profile.
    /// </summary>
    [JsonPropertyName("links")]
    public string? Links { get; init; }
}

/// <summary>
///     Performance statistics for a game mode.
/// </summary>
public class PerfStats
{
    /// <summary>
    ///     Number of games played.
    /// </summary>
    [JsonPropertyName("games")]
    public int Games { get; init; }

    /// <summary>
    ///     Current rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; init; }

    /// <summary>
    ///     Rating deviation.
    /// </summary>
    [JsonPropertyName("rd")]
    public int Rd { get; init; }

    /// <summary>
    ///     Rating progress over recent games.
    /// </summary>
    [JsonPropertyName("prog")]
    public int Prog { get; init; }

    /// <summary>
    ///     Whether the rating is provisional.
    /// </summary>
    [JsonPropertyName("prov")]
    public bool? Prov { get; init; }
}

/// <summary>
///     Play time statistics.
/// </summary>
public class PlayTime
{
    /// <summary>
    ///     Total play time in seconds.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; init; }

    /// <summary>
    ///     Play time on TV in seconds.
    /// </summary>
    [JsonPropertyName("tv")]
    public int Tv { get; init; }
}

/// <summary>
///     Game count statistics.
/// </summary>
public class GameCount
{
    /// <summary>
    ///     Total games played.
    /// </summary>
    [JsonPropertyName("all")]
    public int All { get; init; }

    /// <summary>
    ///     Rated games played.
    /// </summary>
    [JsonPropertyName("rated")]
    public int Rated { get; init; }

    /// <summary>
    ///     Games won.
    /// </summary>
    [JsonPropertyName("win")]
    public int Win { get; init; }

    /// <summary>
    ///     Games lost.
    /// </summary>
    [JsonPropertyName("loss")]
    public int Loss { get; init; }

    /// <summary>
    ///     Games drawn.
    /// </summary>
    [JsonPropertyName("draw")]
    public int Draw { get; init; }

    /// <summary>
    ///     Games played as AI opponent.
    /// </summary>
    [JsonPropertyName("ai")]
    public int Ai { get; init; }
}

/// <summary>
///     Real-time user status.
/// </summary>
public class UserStatus
{
    /// <summary>
    ///     The user's unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     The user's display name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     The user's title, if any.
    /// </summary>
    [JsonPropertyName("title")]
    public Title? Title { get; init; }

    /// <summary>
    ///     Whether the user is online.
    /// </summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>
    ///     Whether the user is currently playing.
    /// </summary>
    [JsonPropertyName("playing")]
    public bool? Playing { get; init; }

    /// <summary>
    ///     Whether the user is currently streaming.
    /// </summary>
    [JsonPropertyName("streaming")]
    public bool? Streaming { get; init; }

    /// <summary>
    ///     Whether the user is a patron.
    /// </summary>
    [JsonPropertyName("patron")]
    public bool? Patron { get; init; }

    /// <summary>
    ///     Network signal strength (1-4), if requested.
    /// </summary>
    [JsonPropertyName("signal")]
    public int? Signal { get; init; }

    /// <summary>
    ///     ID of the game being played, if requested.
    /// </summary>
    [JsonPropertyName("playingId")]
    public string? PlayingId { get; init; }
}