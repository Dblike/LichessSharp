using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
///     Chess variants supported by Lichess.
///     Maps to the VariantKey schema in the Lichess API.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Variant>))]
public enum Variant
{
    /// <summary>Standard chess.</summary>
    [JsonStringEnumMemberName("standard")] Standard,

    /// <summary>Chess960 (Fischer Random).</summary>
    [JsonStringEnumMemberName("chess960")] Chess960,

    /// <summary>Crazyhouse variant.</summary>
    [JsonStringEnumMemberName("crazyhouse")]
    Crazyhouse,

    /// <summary>Antichess variant.</summary>
    [JsonStringEnumMemberName("antichess")]
    Antichess,

    /// <summary>Atomic chess variant.</summary>
    [JsonStringEnumMemberName("atomic")] Atomic,

    /// <summary>Horde variant.</summary>
    [JsonStringEnumMemberName("horde")] Horde,

    /// <summary>King of the Hill variant.</summary>
    [JsonStringEnumMemberName("kingOfTheHill")]
    KingOfTheHill,

    /// <summary>Racing Kings variant.</summary>
    [JsonStringEnumMemberName("racingKings")]
    RacingKings,

    /// <summary>Three-check variant.</summary>
    [JsonStringEnumMemberName("threeCheck")]
    ThreeCheck,

    /// <summary>From a custom starting position.</summary>
    [JsonStringEnumMemberName("fromPosition")]
    FromPosition
}