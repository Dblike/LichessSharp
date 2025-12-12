using System.Text.Json.Serialization;

namespace LichessSharp.Models.Games;

/// <summary>
/// Player analysis statistics.
/// </summary>
public class PlayerAnalysis
{
    /// <summary>
    /// Number of inaccuracies.
    /// </summary>
    [JsonPropertyName("inaccuracy")]
    public int Inaccuracy { get; init; }

    /// <summary>
    /// Number of mistakes.
    /// </summary>
    [JsonPropertyName("mistake")]
    public int Mistake { get; init; }

    /// <summary>
    /// Number of blunders.
    /// </summary>
    [JsonPropertyName("blunder")]
    public int Blunder { get; init; }

    /// <summary>
    /// Average centipawn loss.
    /// </summary>
    [JsonPropertyName("acpl")]
    public int Acpl { get; init; }

    /// <summary>
    /// Accuracy percentage.
    /// </summary>
    [JsonPropertyName("accuracy")]
    public int? Accuracy { get; init; }
}

/// <summary>
/// Analysis evaluation for a position.
/// </summary>
public class Analysis
{
    /// <summary>
    /// Evaluation in centipawns.
    /// </summary>
    [JsonPropertyName("eval")]
    public int? Eval { get; init; }

    /// <summary>
    /// Mate in N moves (positive for white, negative for black).
    /// </summary>
    [JsonPropertyName("mate")]
    public int? Mate { get; init; }

    /// <summary>
    /// Best move in UCI notation.
    /// </summary>
    [JsonPropertyName("best")]
    public string? Best { get; init; }

    /// <summary>
    /// Principal variation.
    /// </summary>
    [JsonPropertyName("variation")]
    public string? Variation { get; init; }

    /// <summary>
    /// Judgment on the move quality.
    /// </summary>
    [JsonPropertyName("judgment")]
    public Judgment? Judgment { get; init; }
}

/// <summary>
/// Move quality judgment.
/// </summary>
public class Judgment
{
    /// <summary>
    /// Judgment name (Inaccuracy, Mistake, Blunder).
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Comment about the judgment.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; init; }
}
