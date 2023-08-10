namespace Lichess.NET.Options.Games
{
    public class SearchMasterGameOptions : IQueryParams
    {
        public static SearchMasterGameOptions Default { get; } = new SearchMasterGameOptions();
        public static Dictionary<string, string?> QueryParams { get; } = new()
        {
            { "fen", Default.Fen },
            { "play", Default.Play },
            { "since", Default.Since },
            { "until", Default.Until },
            { "moves", Default.Moves.ToString() },
            { "topGames", Default.TopGames.ToString() }
        };

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
        ///     Include only games from this year or later
        /// </summary>
        public string Since { get; set; } = "1952";

        /// <summary>
        ///     Include only games from this year or earlier
        /// </summary>
        public string Until { get; set; } = "3000";

        /// <summary>
        ///     Number of most common moves to display
        /// </summary>
        public int Moves { get; set; } = 12;

        /// <summary>
        ///     Number of top games to display. Maximum number of games is 15.
        /// </summary>
        public int TopGames { get; set; } = 15;
    }
}