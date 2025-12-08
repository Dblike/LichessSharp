using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
/// Chess variants supported by Lichess.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Variant>))]
public enum Variant
{
    /// <summary>Standard chess.</summary>
    Standard,

    /// <summary>Chess960 (Fischer Random).</summary>
    Chess960,

    /// <summary>Crazyhouse variant.</summary>
    Crazyhouse,

    /// <summary>Antichess variant.</summary>
    Antichess,

    /// <summary>Atomic chess variant.</summary>
    Atomic,

    /// <summary>Horde variant.</summary>
    Horde,

    /// <summary>King of the Hill variant.</summary>
    KingOfTheHill,

    /// <summary>Racing Kings variant.</summary>
    RacingKings,

    /// <summary>Three-check variant.</summary>
    ThreeCheck,

    /// <summary>From a custom starting position.</summary>
    FromPosition
}
