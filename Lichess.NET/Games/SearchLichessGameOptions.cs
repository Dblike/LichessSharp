namespace Lichess.NET.Games
{
    public class SearchLichessGameOptions
    {
        public static SearchLichessGameOptions Default { get; } = new SearchLichessGameOptions();

        public static Dictionary<string, string> QueryParams { get; } = new()
        {
            { "variant", Default.Variant.ToString().ToLower() },
            { "fen", Default.Fen },
            { "play", Default.Play },
            { "speeds", string.Join(",", Default.Speeds.Select(speed => speed.ToString().ToLower())) },
            { "ratings", string.Join(",", Default.Ratings.Select(rating => rating.ToString().ToLower())) },
            { "since", Default.Since },
            { "until", Default.Until },
            { "moves", Default.Moves.ToString() },
            { "topGames", Default.TopGames.ToString() },
            { "recentGames", Default.RecentGames.ToString() },
            { "history", Default.IncludeHistory.ToString().ToLower() },
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
        ///     Comma separated list of ratings groups to filter by. Each group ranges from its value to the next higher group in the enum (0 from 0 to 999, 1000 from 1000 to 1199, ..., 2500 from 2500 to any rating above).
        /// </summary>
        public Rating[] Ratings { get; set; } = Array.Empty<Rating>();

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
        ///     Number of top games to display. Maximum number of games is 4.
        /// </summary>
        public int TopGames { get; set; } = 4;

        /// <summary>
        ///     Number of recent games to display. Maximum number of games is 4.
        /// </summary>
        public int RecentGames { get; set; } = 4;

        /// <summary>
        ///     Optionally retrieve history
        /// </summary>
        public bool IncludeHistory { get; set; } = false;
    }

    public enum Variant
    {
        Standard,
        Chess960,
        Crazyhouse,
        Antichess,
        Atomic,
        Horde,
        KingOfTheHill,
        RacingKings,
        ThreeCheck,
        FromPosition
    }

    public enum Speed
    {
        UltraBullet,
        Bullet,
        Blitz,
        Rapid,
        Classical,
        Correspondence
    }

    public enum Rating
    {
        ELO0,
        ELO1000,
        ELO1200,
        ELO1400,
        ELO1600,
        ELO1800,
        ELO2000,
        ELO2200,
        ELO2500
    }
}