using Lichess.NET.Options.Games;

namespace Lichess.NET.Options.Analysis
{
    public class CloudEvaluationOptions
    {
        private static CloudEvaluationOptions Default { get; } = new CloudEvaluationOptions();
        public static Dictionary<string, string> QueryParams { get; } = new()
        {
            { "multiPv", Default.MultiPv.ToString() },
            { "variant", Default.Variant.ToString() }
        };

        public int MultiPv { get; set; } = 1;
        public Variant Variant { get; set; } = Variant.Standard;
    }
}