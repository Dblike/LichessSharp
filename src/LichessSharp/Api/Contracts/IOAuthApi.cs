using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace LichessSharp.Api.Contracts;

/// <summary>
///     OAuth API - Token management and authorization for the Lichess API.
///     See <see href="https://lichess.org/api#tag/OAuth" />.
/// </summary>
public interface IOAuthApi
{
    /// <summary>
    ///     Generates PKCE values and creates the authorization URL for OAuth2 login.
    ///     This is a convenience method that combines PKCE generation and URL building.
    /// </summary>
    /// <param name="clientId">Your application's client ID (arbitrary unique identifier).</param>
    /// <param name="redirectUri">The URL to redirect back to after authorization.</param>
    /// <param name="scopes">Optional OAuth scopes to request (e.g., "preference:read", "challenge:write").</param>
    /// <param name="state">Optional state parameter for CSRF protection.</param>
    /// <param name="username">Optional hint for which Lichess account to use.</param>
    /// <returns>A tuple containing the authorization URL to redirect the user to and the code verifier to store.</returns>
    (string AuthorizationUrl, string CodeVerifier) CreateAuthorizationRequest(
        string clientId,
        string redirectUri,
        IEnumerable<string>? scopes = null,
        string? state = null,
        string? username = null);

    /// <summary>
    ///     Exchange an authorization code for an access token using the PKCE flow.
    /// </summary>
    /// <param name="request">The token request containing the authorization code and PKCE verifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The access token response.</returns>
    Task<OAuthToken> GetTokenAsync(OAuthTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Revoke the current access token.
    ///     The token sent as Bearer authorization will be invalidated.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RevokeTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Test multiple OAuth tokens to check their validity and scopes.
    ///     For up to 1000 OAuth tokens, returns their associated user ID and scopes,
    ///     or null if the token is invalid.
    /// </summary>
    /// <param name="tokens">OAuth tokens to test (up to 1000).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary mapping each token to its info, or null if invalid.</returns>
    Task<IReadOnlyDictionary<string, OAuthTokenInfo?>> TestTokensAsync(
        IEnumerable<string> tokens,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Request to obtain an access token via the OAuth2 PKCE flow.
/// </summary>
public class OAuthTokenRequest
{
    /// <summary>
    ///     The authorization code that was sent in the code parameter to your redirect_uri.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    ///     The code verifier that was used to generate the code_challenge sent in the authorization request.
    /// </summary>
    public required string CodeVerifier { get; init; }

    /// <summary>
    ///     Must match the redirect_uri used to request the authorization code.
    /// </summary>
    public required string RedirectUri { get; init; }

    /// <summary>
    ///     Must match the client_id used to request the authorization code.
    /// </summary>
    public required string ClientId { get; init; }
}

/// <summary>
///     OAuth access token response.
/// </summary>
public class OAuthToken
{
    /// <summary>
    ///     The token type (always "Bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }

    /// <summary>
    ///     The access token to use for authenticated requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    /// <summary>
    ///     The number of seconds until the token expires.
    ///     Lichess tokens are long-lived (typically one year).
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}

/// <summary>
///     Information about a tested OAuth token.
/// </summary>
public class OAuthTokenInfo
{
    /// <summary>
    ///     The user ID associated with this token.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    /// <summary>
    ///     Comma-separated list of scopes. Empty string if the token has no scopes.
    /// </summary>
    [JsonPropertyName("scopes")]
    public string? Scopes { get; init; }

    /// <summary>
    ///     Unix timestamp in milliseconds when the token expires, or null if the token never expires.
    /// </summary>
    [JsonPropertyName("expires")]
    public long? Expires { get; init; }
}

/// <summary>
///     OAuth error response.
/// </summary>
public class OAuthError
{
    /// <summary>
    ///     The error code.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    /// <summary>
    ///     A human-readable description of the error.
    /// </summary>
    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; init; }
}

/// <summary>
///     PKCE (Proof Key for Code Exchange) values for OAuth2 authorization.
/// </summary>
/// <param name="CodeVerifier">The code verifier to store and use when exchanging the authorization code.</param>
/// <param name="CodeChallenge">The code challenge to send in the authorization request.</param>
internal readonly record struct PkceValues(string CodeVerifier, string CodeChallenge);

/// <summary>
///     Internal helper class for OAuth2 PKCE flow with Lichess.
/// </summary>
internal static class OAuthHelper
{
    private const string AuthorizeUrl = "https://lichess.org/oauth";

    /// <summary>
    ///     Generates cryptographically secure PKCE values for OAuth2 authorization.
    /// </summary>
    /// <returns>A tuple containing the code verifier (to store) and code challenge (to send).</returns>
    public static PkceValues GeneratePkceValues()
    {
        var codeVerifier = GenerateCodeVerifier();
        var codeChallenge = GenerateCodeChallenge(codeVerifier);
        return new PkceValues(codeVerifier, codeChallenge);
    }

    /// <summary>
    ///     Creates the authorization URL to redirect the user to for OAuth2 login.
    /// </summary>
    /// <param name="clientId">Your application's client ID (arbitrary unique identifier).</param>
    /// <param name="redirectUri">The URL to redirect back to after authorization.</param>
    /// <param name="codeChallenge">The PKCE code challenge (from <see cref="GeneratePkceValues"/>).</param>
    /// <param name="scopes">Optional OAuth scopes to request (e.g., "preference:read", "challenge:write").</param>
    /// <param name="state">Optional state parameter for CSRF protection.</param>
    /// <param name="username">Optional hint for which Lichess account to use.</param>
    /// <returns>The authorization URL to redirect the user to.</returns>
    public static string CreateAuthorizationUrl(
        string clientId,
        string redirectUri,
        string codeChallenge,
        IEnumerable<string>? scopes = null,
        string? state = null,
        string? username = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(redirectUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(codeChallenge);

        var queryParams = new List<string>
        {
            $"response_type=code",
            $"client_id={Uri.EscapeDataString(clientId)}",
            $"redirect_uri={Uri.EscapeDataString(redirectUri)}",
            $"code_challenge_method=S256",
            $"code_challenge={Uri.EscapeDataString(codeChallenge)}"
        };

        if (scopes != null)
        {
            var scopeString = string.Join(" ", scopes);
            if (!string.IsNullOrWhiteSpace(scopeString))
                queryParams.Add($"scope={Uri.EscapeDataString(scopeString)}");
        }

        if (!string.IsNullOrWhiteSpace(state))
            queryParams.Add($"state={Uri.EscapeDataString(state)}");

        if (!string.IsNullOrWhiteSpace(username))
            queryParams.Add($"username={Uri.EscapeDataString(username)}");

        return $"{AuthorizeUrl}?{string.Join("&", queryParams)}";
    }

    /// <summary>
    ///     Convenience method that generates PKCE values and creates the authorization URL in one call.
    /// </summary>
    /// <param name="clientId">Your application's client ID.</param>
    /// <param name="redirectUri">The URL to redirect back to after authorization.</param>
    /// <param name="scopes">Optional OAuth scopes to request.</param>
    /// <param name="state">Optional state parameter for CSRF protection.</param>
    /// <param name="username">Optional hint for which Lichess account to use.</param>
    /// <returns>A tuple containing the authorization URL and the code verifier to store.</returns>
    public static (string AuthorizationUrl, string CodeVerifier) CreateAuthorizationRequest(
        string clientId,
        string redirectUri,
        IEnumerable<string>? scopes = null,
        string? state = null,
        string? username = null)
    {
        var pkce = GeneratePkceValues();
        var url = CreateAuthorizationUrl(clientId, redirectUri, pkce.CodeChallenge, scopes, state, username);
        return (url, pkce.CodeVerifier);
    }

    /// <summary>
    ///     Generates a cryptographically secure random state parameter for CSRF protection.
    /// </summary>
    /// <returns>A random state string.</returns>
    public static string GenerateState()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        var challengeBytes = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncode(challengeBytes);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}