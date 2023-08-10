namespace Lichess.NET
{
    public interface IQueryParams
    {
        static IDictionary<string, string?>? QueryParams { get; }
    }
}
