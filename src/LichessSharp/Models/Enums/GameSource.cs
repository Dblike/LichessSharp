using System.Text.Json.Serialization;

namespace LichessSharp.Models.Enums;

/// <summary>
///     Source of a game.
///     Maps to the GameSource schema in the Lichess API.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<GameSource>))]
public enum GameSource
{
    /// <summary>Game created from the lobby.</summary>
    [JsonStringEnumMemberName("lobby")] Lobby,

    /// <summary>Game created as a friendly challenge.</summary>
    [JsonStringEnumMemberName("friend")] Friend,

    /// <summary>Game against the AI.</summary>
    [JsonStringEnumMemberName("ai")] Ai,

    /// <summary>Game created via the API.</summary>
    [JsonStringEnumMemberName("api")] Api,

    /// <summary>Game from a tournament.</summary>
    [JsonStringEnumMemberName("tournament")]
    Tournament,

    /// <summary>Game from a custom position.</summary>
    [JsonStringEnumMemberName("position")]
    Position,

    /// <summary>Imported game.</summary>
    [JsonStringEnumMemberName("import")] Import,

    /// <summary>Imported live game.</summary>
    [JsonStringEnumMemberName("importlive")]
    ImportLive,

    /// <summary>Game from a simultaneous exhibition.</summary>
    [JsonStringEnumMemberName("simul")] Simul,

    /// <summary>Game from a relay broadcast.</summary>
    [JsonStringEnumMemberName("relay")] Relay,

    /// <summary>Game from the pool (matchmaking).</summary>
    [JsonStringEnumMemberName("pool")] Pool,

    /// <summary>Game from an arena tournament.</summary>
    [JsonStringEnumMemberName("arena")] Arena,

    /// <summary>Game from a Swiss tournament.</summary>
    [JsonStringEnumMemberName("swiss")] Swiss
}
