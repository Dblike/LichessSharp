namespace Lichess.NET
{
    public interface IQueryParams
    {
        static Dictionary<string, string?>? QueryParams { get; }
    }
}
