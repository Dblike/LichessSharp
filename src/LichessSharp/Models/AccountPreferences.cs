using System.Text.Json.Serialization;

namespace LichessSharp.Models;

/// <summary>
/// Account preferences response wrapper.
/// </summary>
public class AccountPreferences
{
    /// <summary>
    /// The user's preferences.
    /// </summary>
    [JsonPropertyName("prefs")]
    public UserPreferences? Prefs { get; init; }

    /// <summary>
    /// The user's language setting.
    /// </summary>
    [JsonPropertyName("language")]
    public string? Language { get; init; }
}

/// <summary>
/// User preferences.
/// </summary>
public class UserPreferences
{
    /// <summary>
    /// Dark mode setting.
    /// </summary>
    [JsonPropertyName("dark")]
    public bool? Dark { get; init; }

    /// <summary>
    /// Whether the background is transparent.
    /// </summary>
    [JsonPropertyName("transp")]
    public bool? Transp { get; init; }

    /// <summary>
    /// Background image URL.
    /// </summary>
    [JsonPropertyName("bgImg")]
    public string? BgImg { get; init; }

    /// <summary>
    /// Whether the board is 3D.
    /// </summary>
    [JsonPropertyName("is3d")]
    public bool? Is3d { get; init; }

    /// <summary>
    /// Theme setting.
    /// </summary>
    [JsonPropertyName("theme")]
    public string? Theme { get; init; }

    /// <summary>
    /// Piece set.
    /// </summary>
    [JsonPropertyName("pieceSet")]
    public string? PieceSet { get; init; }

    /// <summary>
    /// 3D piece set.
    /// </summary>
    [JsonPropertyName("theme3d")]
    public string? Theme3d { get; init; }

    /// <summary>
    /// 3D piece set.
    /// </summary>
    [JsonPropertyName("pieceSet3d")]
    public string? PieceSet3d { get; init; }

    /// <summary>
    /// Sound set.
    /// </summary>
    [JsonPropertyName("soundSet")]
    public string? SoundSet { get; init; }

    /// <summary>
    /// Blindfold mode.
    /// </summary>
    [JsonPropertyName("blindfold")]
    public int? Blindfold { get; init; }

    /// <summary>
    /// Auto-queen promotion setting.
    /// </summary>
    [JsonPropertyName("autoQueen")]
    public int? AutoQueen { get; init; }

    /// <summary>
    /// Auto-threefold claim setting.
    /// </summary>
    [JsonPropertyName("autoThreefold")]
    public int? AutoThreefold { get; init; }

    /// <summary>
    /// Takeback setting.
    /// </summary>
    [JsonPropertyName("takeback")]
    public int? Takeback { get; init; }

    /// <summary>
    /// Moretime setting.
    /// </summary>
    [JsonPropertyName("moretime")]
    public int? Moretime { get; init; }

    /// <summary>
    /// Clock tenths display setting.
    /// </summary>
    [JsonPropertyName("clockTenths")]
    public int? ClockTenths { get; init; }

    /// <summary>
    /// Clock bar setting.
    /// </summary>
    [JsonPropertyName("clockBar")]
    public bool? ClockBar { get; init; }

    /// <summary>
    /// Clock sound setting.
    /// </summary>
    [JsonPropertyName("clockSound")]
    public bool? ClockSound { get; init; }

    /// <summary>
    /// Premove setting.
    /// </summary>
    [JsonPropertyName("premove")]
    public bool? Premove { get; init; }

    /// <summary>
    /// Animation setting.
    /// </summary>
    [JsonPropertyName("animation")]
    public int? Animation { get; init; }

    /// <summary>
    /// Captured pieces display setting.
    /// </summary>
    [JsonPropertyName("captured")]
    public bool? Captured { get; init; }

    /// <summary>
    /// Follow game setting.
    /// </summary>
    [JsonPropertyName("follow")]
    public bool? Follow { get; init; }

    /// <summary>
    /// Highlight setting.
    /// </summary>
    [JsonPropertyName("highlight")]
    public bool? Highlight { get; init; }

    /// <summary>
    /// Destination squares setting.
    /// </summary>
    [JsonPropertyName("destination")]
    public bool? Destination { get; init; }

    /// <summary>
    /// Board coordinates setting.
    /// </summary>
    [JsonPropertyName("coords")]
    public int? Coords { get; init; }

    /// <summary>
    /// Replay setting.
    /// </summary>
    [JsonPropertyName("replay")]
    public int? Replay { get; init; }

    /// <summary>
    /// Challenge setting.
    /// </summary>
    [JsonPropertyName("challenge")]
    public int? Challenge { get; init; }

    /// <summary>
    /// Message setting.
    /// </summary>
    [JsonPropertyName("message")]
    public int? Message { get; init; }

    /// <summary>
    /// Coordinate training setting.
    /// </summary>
    [JsonPropertyName("coordColor")]
    public int? CoordColor { get; init; }

    /// <summary>
    /// Submit move setting.
    /// </summary>
    [JsonPropertyName("submitMove")]
    public int? SubmitMove { get; init; }

    /// <summary>
    /// Confirm resign setting.
    /// </summary>
    [JsonPropertyName("confirmResign")]
    public int? ConfirmResign { get; init; }

    /// <summary>
    /// Insight share setting.
    /// </summary>
    [JsonPropertyName("insightShare")]
    public int? InsightShare { get; init; }

    /// <summary>
    /// Keyboard move setting.
    /// </summary>
    [JsonPropertyName("keyboardMove")]
    public int? KeyboardMove { get; init; }

    /// <summary>
    /// Zen mode setting.
    /// </summary>
    [JsonPropertyName("zen")]
    public int? Zen { get; init; }

    /// <summary>
    /// Move event setting.
    /// </summary>
    [JsonPropertyName("moveEvent")]
    public int? MoveEvent { get; init; }

    /// <summary>
    /// Rookie piece setting.
    /// </summary>
    [JsonPropertyName("rookCastle")]
    public int? RookCastle { get; init; }
}
