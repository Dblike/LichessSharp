using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
/// Game termination status.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<GameStatus>))]
public enum GameStatus
{
    /// <summary>Game was created but not started.</summary>
    Created,

    /// <summary>Game is in progress.</summary>
    Started,

    /// <summary>Game was aborted.</summary>
    Aborted,

    /// <summary>Game ended by checkmate.</summary>
    Mate,

    /// <summary>Game ended by resignation.</summary>
    Resign,

    /// <summary>Game ended by stalemate.</summary>
    Stalemate,

    /// <summary>Game ended by timeout.</summary>
    Timeout,

    /// <summary>Game was drawn by agreement.</summary>
    Draw,

    /// <summary>Game ended due to time out with insufficient material.</summary>
    Outoftime,

    /// <summary>Game ended by cheat detection.</summary>
    Cheat,

    /// <summary>Player left the game without resigning.</summary>
    NoStart,

    /// <summary>Unknown by server error.</summary>
    UnknownFinish,

    /// <summary>Variant ending (e.g., racing kings).</summary>
    VariantEnd
}
