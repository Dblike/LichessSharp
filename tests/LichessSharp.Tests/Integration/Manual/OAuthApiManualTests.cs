using FluentAssertions;
using Xunit;

namespace LichessSharp.Tests.Integration.Manual;

/// <summary>
/// Manual integration tests for the OAuth API.
/// These tests require specific setup and are intended to be run manually.
///
/// OAuth Flow Testing Requirements:
///
/// 1. GetTokenAsync (Authorization Code Flow):
///    - Cannot be fully automated as it requires user browser interaction
///    - Steps to test manually:
///      a. Register an OAuth app or use client_id for public apps
///      b. Redirect user to: https://lichess.org/oauth?response_type=code&amp;client_id=YOUR_ID&amp;redirect_uri=YOUR_URI&amp;code_challenge=CHALLENGE&amp;code_challenge_method=S256
///      c. User authorizes the app
///      d. Exchange the code using GetTokenAsync
///
/// 2. RevokeTokenAsync:
///    - Requires a valid access token
///    - WARNING: This will invalidate your test token!
///
/// 3. TestTokensAsync:
///    - Can test with personal access tokens
///    - Does not require full OAuth flow
///
/// To run token tests:
/// 1. Create a personal access token at https://lichess.org/account/oauth/token
/// 2. Set the LICHESS_TEST_TOKEN environment variable
/// 3. Run: dotnet test --filter "Category=Manual"
/// </summary>
[Trait("Category", "Manual")]
public class OAuthApiManualTests : AuthenticatedTestBase
{
    #region TestTokensAsync Tests

    [Fact]
    public async Task TestTokensAsync_WithValidToken_ReturnsTokenInfo()
    {
        // Arrange - get the current token from TestConfiguration
        var token = TestConfiguration.LichessToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        // Act
        var results = await Client.OAuth.TestTokensAsync([token]);

        // Assert
        results.Should().NotBeNull();
        results.Should().ContainKey(token);
        results[token].Should().NotBeNull("Valid token should return info");
        results[token]!.UserId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task TestTokensAsync_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        const string invalidToken = "lip_invalid_token_12345678";

        // Act
        var results = await Client.OAuth.TestTokensAsync([invalidToken]);

        // Assert
        results.Should().NotBeNull();
        results.Should().ContainKey(invalidToken);
        results[invalidToken].Should().BeNull("Invalid token should return null");
    }

    [Fact]
    public async Task TestTokensAsync_WithMixedTokens_ReturnsCorrectResults()
    {
        // Arrange
        var validToken = TestConfiguration.LichessToken;
        if (string.IsNullOrWhiteSpace(validToken))
        {
            return;
        }

        const string invalidToken = "lip_invalid_token_12345678";

        // Act
        var results = await Client.OAuth.TestTokensAsync([validToken, invalidToken]);

        // Assert
        results.Should().HaveCount(2);
        results[validToken].Should().NotBeNull("Valid token should return info");
        results[invalidToken].Should().BeNull("Invalid token should return null");
    }

    [Fact]
    public async Task TestTokensAsync_TokenInfoHasValidStructure()
    {
        // Arrange
        var token = TestConfiguration.LichessToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        // Act
        var results = await Client.OAuth.TestTokensAsync([token]);
        var tokenInfo = results[token];

        // Assert
        tokenInfo.Should().NotBeNull();
        tokenInfo!.UserId.Should().NotBeNullOrWhiteSpace();
        // Scopes is a comma-separated string (may be empty string if no scopes)
        tokenInfo.Scopes.Should().NotBeNull();
        // Expires may be null for non-expiring tokens
    }

    #endregion

    #region RevokeTokenAsync Tests

    /// <summary>
    /// WARNING: This test will revoke your current token!
    /// Only run if you want to test token revocation.
    /// You will need to create a new token afterward.
    /// </summary>
    [Fact(Skip = "This will revoke your token - run manually only")]
    public async Task RevokeTokenAsync_WithValidToken_RevokesToken()
    {
        // Act
        await Client.OAuth.RevokeTokenAsync();

        // Assert - verify token is revoked by trying to use it
        // After revocation, API calls should fail
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.Account.GetProfileAsync());
    }

    #endregion

    #region GetTokenAsync Tests

    /// <summary>
    /// This test demonstrates the structure of GetTokenAsync but cannot be fully automated
    /// because it requires browser interaction for the OAuth authorization flow.
    ///
    /// To test manually:
    /// 1. Generate a code verifier: random 43-128 character string
    /// 2. Generate code challenge: Base64Url(SHA256(code_verifier))
    /// 3. Redirect user to authorization URL
    /// 4. Capture the authorization code from redirect
    /// 5. Call GetTokenAsync with the code
    /// </summary>
    [Fact(Skip = "Requires browser OAuth flow - run manually only")]
    public async Task GetTokenAsync_WithValidCode_ReturnsAccessToken()
    {
        // This is a template showing how to use GetTokenAsync
        // In practice, you need to complete the OAuth flow first

        // Arrange
        var request = new OAuthTokenRequest
        {
            Code = "YOUR_AUTHORIZATION_CODE_HERE",
            CodeVerifier = "YOUR_CODE_VERIFIER_HERE",
            RedirectUri = "YOUR_REDIRECT_URI_HERE",
            ClientId = "YOUR_CLIENT_ID_HERE"
        };

        // Act
        var token = await Client.OAuth.GetTokenAsync(request);

        // Assert
        token.Should().NotBeNull();
        token.AccessToken.Should().NotBeNullOrWhiteSpace();
        token.TokenType.Should().Be("Bearer");
    }

    #endregion
}

/// <summary>
/// Unauthenticated tests for OAuth API.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class OAuthApiIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task TestTokensAsync_WithEmptyList_ReturnsEmptyDictionary()
    {
        // Act
        var results = await Client.OAuth.TestTokensAsync([]);

        // Assert
        results.Should().NotBeNull();
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task TestTokensAsync_WithInvalidToken_ReturnsNullForToken()
    {
        // Arrange
        const string invalidToken = "lip_completely_fake_token_xyz";

        // Act
        var results = await Client.OAuth.TestTokensAsync([invalidToken]);

        // Assert
        results.Should().ContainKey(invalidToken);
        results[invalidToken].Should().BeNull();
    }

    [Fact]
    public async Task TestTokensAsync_WithMultipleInvalidTokens_ReturnsAllNull()
    {
        // Arrange
        var invalidTokens = new[]
        {
            "lip_fake_token_1",
            "lip_fake_token_2",
            "lip_fake_token_3"
        };

        // Act
        var results = await Client.OAuth.TestTokensAsync(invalidTokens);

        // Assert
        results.Should().HaveCount(3);
        foreach (var token in invalidTokens)
        {
            results[token].Should().BeNull();
        }
    }

    [Fact]
    public async Task RevokeTokenAsync_WithoutAuth_DoesNotThrow()
    {
        // Revoke on unauthenticated client should succeed silently
        // (there's no token to revoke)

        // Act & Assert - should not throw
        await Client.OAuth.RevokeTokenAsync();
    }
}
