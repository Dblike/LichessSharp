using System.Text.Json;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the OAuth API.
/// </summary>
internal sealed class OAuthApi(ILichessHttpClient httpClient) : IOAuthApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<OAuthToken> GetTokenAsync(OAuthTokenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CodeVerifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RedirectUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ClientId);

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = request.Code,
            ["code_verifier"] = request.CodeVerifier,
            ["redirect_uri"] = request.RedirectUri,
            ["client_id"] = request.ClientId
        };

        return await _httpClient.PostFormAsync<OAuthToken>("/api/token", formData, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RevokeTokenAsync(CancellationToken cancellationToken = default)
    {
        await _httpClient.DeleteNoContentAsync("/api/token", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<string, OAuthTokenInfo?>> TestTokensAsync(
        IEnumerable<string> tokens,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        var tokenList = tokens.ToList();
        if (tokenList.Count == 0)
        {
            return new Dictionary<string, OAuthTokenInfo?>();
        }

        if (tokenList.Count > 1000)
        {
            throw new ArgumentException("Cannot test more than 1000 tokens at once.", nameof(tokens));
        }

        var body = string.Join(",", tokenList);
        var responseJson = await _httpClient.PostPlainTextAsync<JsonElement>("/api/token/test", body, cancellationToken).ConfigureAwait(false);

        var result = new Dictionary<string, OAuthTokenInfo?>();

        foreach (var property in responseJson.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Null)
            {
                result[property.Name] = null;
            }
            else
            {
                result[property.Name] = JsonSerializer.Deserialize<OAuthTokenInfo>(property.Value.GetRawText());
            }
        }

        return result;
    }
}
