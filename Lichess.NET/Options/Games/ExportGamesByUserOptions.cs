namespace Lichess.NET.Options.Games
{
    public class ExportGamesByUserOptions : IQueryParams
    {
        private static ExportGamesByUserOptions Default { get; } = new ExportGamesByUserOptions();
        public static Dictionary<string, string?> QueryParams { get; } = new()
        {
            { "moves", Default.IncludeMoves.ToString() },
            { "pgnInJson", Default.IncludePgnInJson.ToString() },
            { "tags", Default.IncludePgnTags.ToString() },
            { "clocks", Default.IncludeClockStatus.ToString() },
            { "evals", Default.IncludeEvals.ToString() },
            { "accuracy", Default.IncludeAccuracy.ToString() },
            { "opening", Default.IncludeOpening.ToString() },
            { "literate", Default.IncludeAnnotations.ToString() }
        };

        /// <summary>
        ///     Include only games from this month or later
        /// </summary>
        public int? Since { get; set; }

        /// <summary>
        ///     Include only games from this month or earlier
        /// </summary>
        public int? Until { get; set; }

        /// <summary>
        ///     How many games to download. Leave empty to download all games.
        /// </summary>
        public int? MaxGames { get; set; }

        /// <summary>
        ///     [Filter] Only games played against this opponent
        /// </summary>
        public string? Vs { get; set; }

        /// <summary>
        ///     [Filter] Only rated (true) or casual (false) game
        /// </summary>
        public bool? IncludeOnlyRated { get; set; }

        /// <summary>
        ///     [Filter] Only games in these speeds or variants.
        ///     Multiple perf types can be specified, separated by a comma.
        /// </summary>
        public string? PerfType { get; set; }

        /// <summary>
        ///     [Filter] Only games played as this color.
        /// </summary>
        public Color? IncludeOnlyColor { get; set; }

        /// <summary>
        ///     [Filter] Only games with or without a computer analysis available
        /// </summary>
        public bool? IncludeOnlyAnalyzed { get; set; }

        /// <summary>
        ///     Include the PGN moves.
        /// </summary>
        public bool IncludeMoves { get; set; } = true;

        /// <summary>
        ///     Include the full PGN within the JSON response, in a pgn field.
        /// </summary>
        public bool IncludePgnInJson { get; set; } = false;

        /// <summary>
        ///     Include the PGN tags.
        /// </summary>
        public bool IncludePgnTags { get; set; } = true;

        /// <summary>
        ///     Include clock status when available.
        /// </summary>
        public bool IncludeClockStatus { get; set; } = true;

        /// <summary>
        ///     Include analysis evaluations and comments, when available.
        /// </summary>
        public bool IncludeEvals { get; set; } = true;

        /// <summary>
        ///     Include accuracy percent of each player, when available.
        /// </summary>
        public bool IncludeAccuracy { get; set; } = false;

        /// <summary>
        ///     Include the opening name.
        /// </summary>
        public bool IncludeOpening { get; set; } = true;

        /// <summary>
        ///     Insert textual annotations in the PGN about the opening, analysis variations, mistakes, and game termination.
        /// </summary>
        public bool IncludeAnnotations { get; set; } = false;

        /// <summary>
        ///     URL of a text file containing real names and ratings, to replace Lichess usernames and ratings in the PGN.
        /// </summary>
        public string? PlayerUrl { get; set; }
    }
}