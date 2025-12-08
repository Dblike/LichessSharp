using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Account API.
/// </summary>
internal sealed class AccountApi(ILichessHttpClient httpClient) : IAccountApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<UserExtended> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<UserExtended>("/api/account", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> GetEmailAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync<EmailResponse>("/api/account/email", cancellationToken).ConfigureAwait(false);
        return response.Email ?? string.Empty;
    }

    /// <inheritdoc />
    public async Task<AccountPreferences> GetPreferencesAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<AccountPreferences>("/api/account/preferences", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> GetKidModeAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync<KidModeResponse>("/api/account/kid", cancellationToken).ConfigureAwait(false);
        return response.Kid;
    }

    /// <inheritdoc />
    public async Task<bool> SetKidModeAsync(bool enabled, CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/account/kid?v={enabled.ToString().ToLowerInvariant()}";
        var response = await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return response.Ok;
    }
}
