using System.Text.Json;

using FluentAssertions;

using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;

using Moq;

using Xunit;

namespace LichessSharp.Tests.Api;

public class OAuthApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly OAuthApi _api;

    public OAuthApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _api = new OAuthApi(_httpClientMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new OAuthApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    #endregion

    #region GetTokenAsync Tests

    [Fact]
    public async Task GetTokenAsync_WithValidRequest_CallsCorrectEndpoint()
    {
        // Arrange
        var request = new OAuthTokenRequest
        {
            Code = "liu_test_code",
            CodeVerifier = "test_verifier_string",
            RedirectUri = "http://localhost:8080/callback",
            ClientId = "my-app"
        };

        var expectedToken = new OAuthToken
        {
            TokenType = "Bearer",
            AccessToken = "lio_test_token",
            ExpiresIn = 31536000
        };

        _httpClientMock
            .Setup(x => x.PostFormAsync<OAuthToken>(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedToken);

        // Act
        var result = await _api.GetTokenAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TokenType.Should().Be("Bearer");
        result.AccessToken.Should().Be("lio_test_token");
        result.ExpiresIn.Should().Be(31536000);

        _httpClientMock.Verify(x => x.PostFormAsync<OAuthToken>(
            "/api/token",
            It.Is<IDictionary<string, string>>(d =>
                d["grant_type"] == "authorization_code" &&
                d["code"] == "liu_test_code" &&
                d["code_verifier"] == "test_verifier_string" &&
                d["redirect_uri"] == "http://localhost:8080/callback" &&
                d["client_id"] == "my-app"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTokenAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _api.GetTokenAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Fact]
    public async Task GetTokenAsync_WithNullCode_ThrowsArgumentException()
    {
        // Arrange
        var request = new OAuthTokenRequest
        {
            Code = null!,
            CodeVerifier = "test_verifier",
            RedirectUri = "http://localhost/callback",
            ClientId = "my-app"
        };

        // Act
        var act = () => _api.GetTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetTokenAsync_WithEmptyCode_ThrowsArgumentException()
    {
        // Arrange
        var request = new OAuthTokenRequest
        {
            Code = "",
            CodeVerifier = "test_verifier",
            RedirectUri = "http://localhost/callback",
            ClientId = "my-app"
        };

        // Act
        var act = () => _api.GetTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetTokenAsync_WithNullCodeVerifier_ThrowsArgumentException()
    {
        // Arrange
        var request = new OAuthTokenRequest
        {
            Code = "test_code",
            CodeVerifier = null!,
            RedirectUri = "http://localhost/callback",
            ClientId = "my-app"
        };

        // Act
        var act = () => _api.GetTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetTokenAsync_WithNullRedirectUri_ThrowsArgumentException()
    {
        // Arrange
        var request = new OAuthTokenRequest
        {
            Code = "test_code",
            CodeVerifier = "test_verifier",
            RedirectUri = null!,
            ClientId = "my-app"
        };

        // Act
        var act = () => _api.GetTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetTokenAsync_WithNullClientId_ThrowsArgumentException()
    {
        // Arrange
        var request = new OAuthTokenRequest
        {
            Code = "test_code",
            CodeVerifier = "test_verifier",
            RedirectUri = "http://localhost/callback",
            ClientId = null!
        };

        // Act
        var act = () => _api.GetTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetTokenAsync_WithCancellationToken_PassesTokenToHttpClient()
    {
        // Arrange
        var request = new OAuthTokenRequest
        {
            Code = "test_code",
            CodeVerifier = "test_verifier",
            RedirectUri = "http://localhost/callback",
            ClientId = "my-app"
        };
        var cts = new CancellationTokenSource();

        _httpClientMock
            .Setup(x => x.PostFormAsync<OAuthToken>(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OAuthToken { TokenType = "Bearer", AccessToken = "test", ExpiresIn = 0 });

        // Act
        await _api.GetTokenAsync(request, cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.PostFormAsync<OAuthToken>(
            It.IsAny<string>(),
            It.IsAny<IDictionary<string, string>>(),
            cts.Token), Times.Once);
    }

    #endregion

    #region RevokeTokenAsync Tests

    [Fact]
    public async Task RevokeTokenAsync_CallsCorrectEndpoint()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.DeleteNoContentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _api.RevokeTokenAsync();

        // Assert
        _httpClientMock.Verify(x => x.DeleteNoContentAsync(
            "/api/token",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RevokeTokenAsync_WithCancellationToken_PassesTokenToHttpClient()
    {
        // Arrange
        var cts = new CancellationTokenSource();

        _httpClientMock
            .Setup(x => x.DeleteNoContentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _api.RevokeTokenAsync(cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.DeleteNoContentAsync(
            It.IsAny<string>(),
            cts.Token), Times.Once);
    }

    #endregion

    #region TestTokensAsync Tests

    [Fact]
    public async Task TestTokensAsync_WithValidTokens_CallsCorrectEndpoint()
    {
        // Arrange
        var tokens = new[] { "lip_token1", "lip_token2" };
        var responseJson = JsonSerializer.Deserialize<JsonElement>("""
            {
                "lip_token1": {"userId": "user1", "scopes": "read,write", "expires": 1234567890000},
                "lip_token2": null
            }
            """);

        _httpClientMock
            .Setup(x => x.PostPlainTextAsync<JsonElement>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseJson);

        // Act
        var result = await _api.TestTokensAsync(tokens);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result["lip_token1"].Should().NotBeNull();
        result["lip_token1"]!.UserId.Should().Be("user1");
        result["lip_token1"]!.Scopes.Should().Be("read,write");
        result["lip_token1"]!.Expires.Should().Be(1234567890000);
        result["lip_token2"].Should().BeNull();

        _httpClientMock.Verify(x => x.PostPlainTextAsync<JsonElement>(
            "/api/token/test",
            "lip_token1,lip_token2",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TestTokensAsync_WithEmptyTokens_ReturnsEmptyDictionary()
    {
        // Arrange
        var tokens = Array.Empty<string>();

        // Act
        var result = await _api.TestTokensAsync(tokens);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        // Verify no HTTP call was made
        _httpClientMock.Verify(x => x.PostPlainTextAsync<JsonElement>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TestTokensAsync_WithNullTokens_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _api.TestTokensAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("tokens");
    }

    [Fact]
    public async Task TestTokensAsync_WithTooManyTokens_ThrowsArgumentException()
    {
        // Arrange
        var tokens = Enumerable.Range(0, 1001).Select(i => $"token_{i}");

        // Act
        var act = () => _api.TestTokensAsync(tokens);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("tokens")
            .WithMessage("*1000*");
    }

    [Fact]
    public async Task TestTokensAsync_WithSingleToken_CallsCorrectEndpoint()
    {
        // Arrange
        var tokens = new[] { "lip_single_token" };
        var responseJson = JsonSerializer.Deserialize<JsonElement>("""
            {
                "lip_single_token": {"userId": "testuser", "scopes": "", "expires": null}
            }
            """);

        _httpClientMock
            .Setup(x => x.PostPlainTextAsync<JsonElement>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseJson);

        // Act
        var result = await _api.TestTokensAsync(tokens);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result["lip_single_token"].Should().NotBeNull();
        result["lip_single_token"]!.UserId.Should().Be("testuser");
        result["lip_single_token"]!.Scopes.Should().Be("");
        result["lip_single_token"]!.Expires.Should().BeNull();

        _httpClientMock.Verify(x => x.PostPlainTextAsync<JsonElement>(
            "/api/token/test",
            "lip_single_token",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TestTokensAsync_WithCancellationToken_PassesTokenToHttpClient()
    {
        // Arrange
        var tokens = new[] { "lip_token" };
        var cts = new CancellationTokenSource();
        var responseJson = JsonSerializer.Deserialize<JsonElement>("""{"lip_token": null}""");

        _httpClientMock
            .Setup(x => x.PostPlainTextAsync<JsonElement>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseJson);

        // Act
        await _api.TestTokensAsync(tokens, cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.PostPlainTextAsync<JsonElement>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            cts.Token), Times.Once);
    }

    [Fact]
    public async Task TestTokensAsync_WithAllValidTokens_ReturnsAllTokenInfo()
    {
        // Arrange
        var tokens = new[] { "token1", "token2", "token3" };
        var responseJson = JsonSerializer.Deserialize<JsonElement>("""
            {
                "token1": {"userId": "user1", "scopes": "read", "expires": 1000},
                "token2": {"userId": "user2", "scopes": "write", "expires": 2000},
                "token3": {"userId": "user3", "scopes": "read,write", "expires": 3000}
            }
            """);

        _httpClientMock
            .Setup(x => x.PostPlainTextAsync<JsonElement>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseJson);

        // Act
        var result = await _api.TestTokensAsync(tokens);

        // Assert
        result.Should().HaveCount(3);
        result.Values.Should().AllSatisfy(info => info.Should().NotBeNull());
    }

    [Fact]
    public async Task TestTokensAsync_WithAllInvalidTokens_ReturnsAllNulls()
    {
        // Arrange
        var tokens = new[] { "invalid1", "invalid2" };
        var responseJson = JsonSerializer.Deserialize<JsonElement>("""
            {
                "invalid1": null,
                "invalid2": null
            }
            """);

        _httpClientMock
            .Setup(x => x.PostPlainTextAsync<JsonElement>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseJson);

        // Act
        var result = await _api.TestTokensAsync(tokens);

        // Assert
        result.Should().HaveCount(2);
        result.Values.Should().AllSatisfy(info => info.Should().BeNull());
    }

    #endregion
}
