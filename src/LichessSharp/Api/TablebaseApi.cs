using System.Text;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
///     Implementation of the Tablebase API.
/// </summary>
internal sealed class TablebaseApi(ILichessHttpClient httpClient, Uri tablebaseBaseAddress) : ITablebaseApi
{
    private readonly Uri _baseAddress =
        tablebaseBaseAddress ?? throw new ArgumentNullException(nameof(tablebaseBaseAddress));

    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<TablebaseResult> LookupAsync(string fen, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fen);

        var url = BuildUrl("standard", fen);
        return await _httpClient.GetAbsoluteAsync<TablebaseResult>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TablebaseResult> LookupAtomicAsync(string fen, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fen);

        var url = BuildUrl("atomic", fen);
        return await _httpClient.GetAbsoluteAsync<TablebaseResult>(url, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TablebaseResult> LookupAntichessAsync(string fen, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fen);

        var url = BuildUrl("antichess", fen);
        return await _httpClient.GetAbsoluteAsync<TablebaseResult>(url, cancellationToken).ConfigureAwait(false);
    }

    private Uri BuildUrl(string variant, string fen)
    {
        var sb = new StringBuilder();
        sb.Append(_baseAddress.ToString().TrimEnd('/'));
        sb.Append('/');
        sb.Append(variant);
        sb.Append("?fen=");
        sb.Append(Uri.EscapeDataString(fen));

        return new Uri(sb.ToString());
    }
}