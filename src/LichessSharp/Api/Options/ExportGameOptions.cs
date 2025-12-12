namespace LichessSharp.Api.Options;

/// <summary>
///     Options for exporting a single game.
/// </summary>
public class ExportGameOptions
{
    /// <summary>
    ///     Include the PGN moves.
    ///     Default: true.
    /// </summary>
    public bool? Moves { get; set; }

    /// <summary>
    ///     Include the full PGN within the JSON response.
    ///     Default: false.
    /// </summary>
    public bool? PgnInJson { get; set; }

    /// <summary>
    ///     Include the PGN tags.
    ///     Default: true.
    /// </summary>
    public bool? Tags { get; set; }

    /// <summary>
    ///     Include clock status when available.
    ///     Default: true.
    /// </summary>
    public bool? Clocks { get; set; }

    /// <summary>
    ///     Include analysis evaluations and comments when available.
    ///     Default: true.
    /// </summary>
    public bool? Evals { get; set; }

    /// <summary>
    ///     Include accuracy percent of each player when available.
    ///     Default: false.
    /// </summary>
    public bool? Accuracy { get; set; }

    /// <summary>
    ///     Include the opening name.
    ///     Default: true.
    /// </summary>
    public bool? Opening { get; set; }

    /// <summary>
    ///     Include plies which mark the beginning of middlegame and endgame.
    ///     Default: false.
    /// </summary>
    public bool? Division { get; set; }

    /// <summary>
    ///     Insert textual annotations in the PGN.
    ///     Default: false.
    /// </summary>
    public bool? Literate { get; set; }

    /// <summary>
    ///     URL of a text file containing real names and ratings for PGN.
    /// </summary>
    public string? Players { get; set; }
}

/// <summary>
///     Options for exporting a user's games.
/// </summary>
public class ExportUserGamesOptions : ExportGameOptions
{
    /// <summary>
    ///     Download games played since this timestamp.
    /// </summary>
    public DateTimeOffset? Since { get; set; }

    /// <summary>
    ///     Download games played until this timestamp.
    /// </summary>
    public DateTimeOffset? Until { get; set; }

    /// <summary>
    ///     Maximum number of games to download.
    /// </summary>
    public int? Max { get; set; }

    /// <summary>
    ///     Only games played against this opponent.
    /// </summary>
    public string? Vs { get; set; }

    /// <summary>
    ///     Only rated or casual games.
    /// </summary>
    public bool? Rated { get; set; }

    /// <summary>
    ///     Only games in this performance type.
    /// </summary>
    public string? PerfType { get; set; }

    /// <summary>
    ///     Only games with this color.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    ///     Only games that were analyzed.
    /// </summary>
    public bool? Analysed { get; set; }

    /// <summary>
    ///     Include ongoing games.
    /// </summary>
    public bool? Ongoing { get; set; }

    /// <summary>
    ///     Include finished games.
    /// </summary>
    public bool? Finished { get; set; }

    /// <summary>
    ///     Sort order (dateAsc or dateDesc).
    /// </summary>
    public string? Sort { get; set; }
}

/// <summary>
///     Options for getting user information.
/// </summary>
public class GetUserOptions
{
    /// <summary>
    ///     Include user trophies.
    ///     Default: false.
    /// </summary>
    public bool? Trophies { get; set; }

    /// <summary>
    ///     Include user profile data.
    ///     Default: true.
    /// </summary>
    public bool? Profile { get; set; }

    /// <summary>
    ///     Include global lichess ranking for each perf.
    ///     Default: false.
    /// </summary>
    public bool? Rank { get; set; }
}

/// <summary>
///     Options for getting user status.
/// </summary>
public class GetUserStatusOptions
{
    /// <summary>
    ///     Include network signal strength.
    ///     Default: false.
    /// </summary>
    public bool? WithSignal { get; set; }

    /// <summary>
    ///     Include ID of game being played.
    ///     Default: false.
    /// </summary>
    public bool? WithGameIds { get; set; }

    /// <summary>
    ///     Include game metadata.
    ///     Default: false.
    /// </summary>
    public bool? WithGameMetas { get; set; }
}