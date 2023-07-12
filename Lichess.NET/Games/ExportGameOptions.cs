namespace Lichess.NET.Games
{
    public class ExportGameOptions
    {
        private static ExportGameOptions Default { get; } = new ExportGameOptions();
        public static Dictionary<string, string> QueryParams { get; } = new()
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