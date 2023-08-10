namespace Lichess.NET.Options.Games
{
    public class SearchPlayerGameOptions : IQueryParams
    {
        private static SearchPlayerGameOptions Default { get; } = new();
        public static Dictionary<string, string?> QueryParams { get; } = new()
        {
            { "variant", Default.Variant.ToString().ToLower() },
            { "fen", Default.Fen },
            { "play", Default.Play },
            { "speeds", string.Join(",", Default.Speeds.Select(speed => speed.ToString().ToLower())) },
            { "since", Default.Since },
            { "until", Default.Until },
            { "moves", Default.Moves.ToString() },
            { "recentGames", Default.RecentGames.ToString() }
        };

        /// <summary>
        ///     Variant
        /// </summary>
        public Variant Variant { get; set; } = Variant.Standard;

        /// <summary>
        ///     FEN or EPD of the root position
        /// </summary>
        /// <example>
        ///     rnbqkbnr/ppp1pppp/8/3pP3/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2
        /// </example>
        public string Fen { get; set; } = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /// <summary>
        ///     Comma separated sequence of legal moves in UCI notation. Play additional moves starting from fen. Required to find an opening name, if fen is not an exact match for a named position.
        /// </summary>
        /// <example>
        ///     e2e4,e7e5,c2c4,c7c6,c4e5
        /// </example>
        public string Play { get; set; } = string.Empty;

        /// <summary>
        ///     Comma separated list of game speeds to filter by
        /// </summary>
        public Speed[] Speeds { get; set; } = Array.Empty<Speed>();

        /// <summary>
        ///     Comma separated list of modes
        /// </summary>
        public Mode[] Modes { get; set; } = Array.Empty<Mode>();

        /// <summary>
        ///     Include only games from this month or later
        /// </summary>
        public string Since { get; set; } = "1952-01";

        /// <summary>
        ///     Include only games from this month or earlier
        /// </summary>
        public string Until { get; set; } = "3000-12";

        /// <summary>
        ///     Number of most common moves to display
        /// </summary>
        public int Moves { get; set; } = 12;

        /// <summary>
        ///     Number of recent games to display. Maximum number of games is 8.
        /// </summary>
        public int RecentGames { get; set; } = 8;
    }

    public enum Color
    {
        White,
        Black
    }

    public enum Mode
    {
        Casual,
        Rated
    }
}