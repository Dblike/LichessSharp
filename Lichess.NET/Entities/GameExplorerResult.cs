namespace Lichess.NET.Entities;

public class User
{
    public string Name { get; set; }
    public bool Patron { get; set; }
    public string Id { get; set; }
}

public class Analysis
{
    public int Inaccuracy { get; set; }
    public int Mistake { get; set; }
    public int Blunder { get; set; }
    public int Acpl { get; set; }
}

public class Player
{
    public User User { get; set; }
    public int Rating { get; set; }
    public int RatingDiff { get; set; }
    public Analysis Analysis { get; set; }
}

public class Opening
{
    public string Eco { get; set; }
    public string Name { get; set; }
    public int Ply { get; set; }
}

public class Judgment
{
    public string Name { get; set; }
    public string Comment { get; set; }
}

public class MoveAnalysis
{
    public int Eval { get; set; }
    public string Best { get; set; }
    public string Variation { get; set; }
    public Judgment Judgment { get; set; }
}

public class Clock
{
    public int Initial { get; set; }
    public int Increment { get; set; }
    public int TotalTime { get; set; }
}

public class GameExplorerResult
{
    public string Id { get; set; }
    public bool Rated { get; set; }
    public string Variant { get; set; }
    public string Speed { get; set; }
    public string Perf { get; set; }
    public long CreatedAt { get; set; }
    public long LastMoveAt { get; set; }
    public string Status { get; set; }
    public Dictionary<string, Player> Players { get; set; }
    public string Winner { get; set; }
    public Opening Opening { get; set; }
    public string Moves { get; set; }
    public List<int> Clocks { get; set; }
    public List<MoveAnalysis> Analysis { get; set; }
    public string Swiss { get; set; }
    public Clock Clock { get; set; }
}

public class GameCollectionResult
{
    public GameCollectionResult()
    {
        Games = new List<GameExplorerResult>();
    }

    public List<GameExplorerResult> Games { get; set; }
}
