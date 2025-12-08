using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
/// Time control speeds.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Speed>))]
public enum Speed
{
    /// <summary>UltraBullet (less than 30 seconds).</summary>
    UltraBullet,

    /// <summary>Bullet (30 seconds to 3 minutes).</summary>
    Bullet,

    /// <summary>Blitz (3 to 8 minutes).</summary>
    Blitz,

    /// <summary>Rapid (8 to 25 minutes).</summary>
    Rapid,

    /// <summary>Classical (more than 25 minutes).</summary>
    Classical,

    /// <summary>Correspondence (days per move).</summary>
    Correspondence
}
