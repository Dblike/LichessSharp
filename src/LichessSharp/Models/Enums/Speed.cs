using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
///     Time control speeds.
///     Maps to the Speed schema in the Lichess API.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Speed>))]
public enum Speed
{
    /// <summary>UltraBullet (less than 30 seconds).</summary>
    [JsonStringEnumMemberName("ultraBullet")]
    UltraBullet,

    /// <summary>Bullet (30 seconds to 3 minutes).</summary>
    [JsonStringEnumMemberName("bullet")] Bullet,

    /// <summary>Blitz (3 to 8 minutes).</summary>
    [JsonStringEnumMemberName("blitz")] Blitz,

    /// <summary>Rapid (8 to 25 minutes).</summary>
    [JsonStringEnumMemberName("rapid")] Rapid,

    /// <summary>Classical (more than 25 minutes).</summary>
    [JsonStringEnumMemberName("classical")]
    Classical,

    /// <summary>Correspondence (days per move).</summary>
    [JsonStringEnumMemberName("correspondence")]
    Correspondence
}