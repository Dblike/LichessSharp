using System.Text.Json.Serialization;

namespace LichessSharp.Models.Fide;

/// <summary>
///     Historical FIDE ratings for a player across time controls.
///     Data points are encoded: each number contains a year, month, and ELO rating.
///     Example: 2015081568 means August 2015, rating 1568.
/// </summary>
[ResponseOnly]
public class FidePlayerRatings
{
    /// <summary>
    ///     Historical standard (classical) ratings.
    /// </summary>
    [JsonPropertyName("standard")]
    public required IReadOnlyList<long> Standard { get; init; }

    /// <summary>
    ///     Historical rapid ratings.
    /// </summary>
    [JsonPropertyName("rapid")]
    public required IReadOnlyList<long> Rapid { get; init; }

    /// <summary>
    ///     Historical blitz ratings.
    /// </summary>
    [JsonPropertyName("blitz")]
    public required IReadOnlyList<long> Blitz { get; init; }
}
