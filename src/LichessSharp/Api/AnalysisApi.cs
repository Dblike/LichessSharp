using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Exceptions;
using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Analysis API.
/// </summary>
internal sealed class AnalysisApi(ILichessHttpClient httpClient) : IAnalysisApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<CloudEvaluation?> GetCloudEvaluationAsync(
        string fen,
        int? multiPv = null,
        string? variant = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fen);

        var endpoint = BuildCloudEvalEndpoint(fen, multiPv, variant);

        try
        {
            return await _httpClient.GetAsync<CloudEvaluation>(endpoint, cancellationToken).ConfigureAwait(false);
        }
        catch (LichessNotFoundException)
        {
            // Position not found in cloud evaluation database - return null as per interface contract
            return null;
        }
    }

    private static string BuildCloudEvalEndpoint(string fen, int? multiPv, string? variant)
    {
        var sb = new StringBuilder("/api/cloud-eval?fen=");
        sb.Append(Uri.EscapeDataString(fen));

        if (multiPv.HasValue)
        {
            sb.Append("&multiPv=");
            sb.Append(multiPv.Value);
        }

        if (!string.IsNullOrWhiteSpace(variant))
        {
            sb.Append("&variant=");
            sb.Append(Uri.EscapeDataString(variant));
        }

        return sb.ToString();
    }
}
