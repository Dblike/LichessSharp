namespace Lichess.NET.Entities;

public class OpeningExplorerResult
{
    public int White { get; set; }
    public int Draws { get; set; }
    public int Black { get; set; }
    public List<OpeningExplorerMove> Moves { get; set; }
    public List<Game> TopGames { get; set; }
    public List<Game> RecentGames { get; set; }
}

public class Game
{
    public string Id { get; set; }
}

public class OpeningExplorerMove
{
    public string Uci { get; set; }
    public int White { get; set; }
    public int Draws { get; set; }
    public int Black { get; set; }
}
