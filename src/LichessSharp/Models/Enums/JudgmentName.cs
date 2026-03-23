using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
///     Move quality judgment categories from computer analysis.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<JudgmentName>))]
public enum JudgmentName
{
    /// <summary>Inaccuracy — a suboptimal move.</summary>
    Inaccuracy,

    /// <summary>Mistake — a significantly weaker move.</summary>
    Mistake,

    /// <summary>Blunder — a move that loses substantial advantage.</summary>
    Blunder
}
