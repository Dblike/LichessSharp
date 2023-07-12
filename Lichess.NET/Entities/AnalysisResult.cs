namespace Lichess.NET.Entities;

public class AnalysisResult
{
    public AnalysisResult()
    {
        Opponents = new List<int>();
    }

    public List<int> Opponents { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
}
