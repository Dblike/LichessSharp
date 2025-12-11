using System.Text.Json.Serialization;

namespace LichessSharp.Api;

/// <summary>
/// OAuth API - Token management for the Lichess API.
/// See <see href="https://lichess.org/api#tag/OAuth"/>.
/// </summary>
public interface IOAuthApi
{
    /// <summary>
    /// Exchange an authorization code for an access token using the PKCE flow.
    /// </summary>
    /// <param name="request">The token request containing the authorization code and PKCE verifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The access token response.</returns>
    Task<OAuthToken> GetTokenAsync(OAuthTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke the current access token.
    /// The token sent as Bearer authorization will be invalidated.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RevokeTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Test multiple OAuth tokens to check their validity and scopes.
    /// For up to 1000 OAuth tokens, returns their associated user ID and scopes,
    /// or null if the token is invalid.
    /// </summary>
    /// <param name="tokens">OAuth tokens to test (up to 1000).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary mapping each token to its info, or null if invalid.</returns>
    Task<IReadOnlyDictionary<string, OAuthTokenInfo?>> TestTokensAsync(
        IEnumerable<string> tokens,
        CancellationToken cancellationToken = default);
}

#region Models

/// <summary>
/// Request to obtain an access token via the OAuth2 PKCE flow.
/// </summary>
public class OAuthTokenRequest
{
    /// <summary>
    /// The authorization code that was sent in the code parameter to your redirect_uri.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// The code verifier that was used to generate the code_challenge sent in the authorization request.
    /// </summary>
    public required string CodeVerifier { get; init; }

    /// <summary>
    /// Must match the redirect_uri used to request the authorization code.
    /// </summary>
    public required string RedirectUri { get; init; }

    /// <summary>
    /// Must match the client_id used to request the authorization code.
    /// </summary>
    public required string ClientId { get; init; }
}

/// <summary>
/// OAuth access token response.
/// </summary>
public class OAuthToken
{
    /// <summary>
    /// The token type (always "Bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }

    /// <summary>
    /// The access token to use for authenticated requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    /// <summary>
    /// The number of seconds until the token expires.
    /// Lichess tokens are long-lived (typically one year).
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}

/// <summary>
/// Information about a tested OAuth token.
/// </summary>
public class OAuthTokenInfo
{
    /// <summary>
    /// The user ID associated with this token.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    /// <summary>
    /// Comma-separated list of scopes. Empty string if the token has no scopes.
    /// </summary>
    [JsonPropertyName("scopes")]
    public string? Scopes { get; init; }

    /// <summary>
    /// Unix timestamp in milliseconds when the token expires, or null if the token never expires.
    /// </summary>
    [JsonPropertyName("expires")]
    public long? Expires { get; init; }
}

/// <summary>
/// OAuth error response.
/// </summary>
public class OAuthError
{
    /// <summary>
    /// The error code.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    /// <summary>
    /// A human-readable description of the error.
    /// </summary>
    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; init; }
}

#endregion
