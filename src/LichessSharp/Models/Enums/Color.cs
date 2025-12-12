using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
///     Chess piece colors.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Color>))]
public enum Color
{
    /// <summary>White pieces.</summary>
    White,

    /// <summary>Black pieces.</summary>
    Black
}