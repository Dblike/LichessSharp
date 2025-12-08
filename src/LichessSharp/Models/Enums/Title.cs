using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
/// Chess titles.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Title>))]
public enum Title
{
    /// <summary>Grandmaster.</summary>
    GM,

    /// <summary>Woman Grandmaster.</summary>
    WGM,

    /// <summary>International Master.</summary>
    IM,

    /// <summary>Woman International Master.</summary>
    WIM,

    /// <summary>FIDE Master.</summary>
    FM,

    /// <summary>Woman FIDE Master.</summary>
    WFM,

    /// <summary>National Master.</summary>
    NM,

    /// <summary>Candidate Master.</summary>
    CM,

    /// <summary>Woman Candidate Master.</summary>
    WCM,

    /// <summary>Woman National Master.</summary>
    WNM,

    /// <summary>Lichess Master (2500+ rating).</summary>
    LM,

    /// <summary>Bot account.</summary>
    BOT
}
