using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Simuls API.
/// </summary>
internal sealed class SimulsApi(ILichessHttpClient httpClient) : ISimulsApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<SimulList> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<SimulList>("/api/simul", cancellationToken).ConfigureAwait(false);
    }
}
