using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Http;
using LichessSharp.Models;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class AccountApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly AccountApi _accountApi;

    public AccountApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _accountApi = new AccountApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new AccountApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }



    [Fact]
    public async Task GetProfileAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedUser = CreateTestUserExtended();
        _httpClientMock
            .Setup(x => x.GetAsync<UserExtended>("/api/account", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _accountApi.GetProfileAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedUser.Id);
        result.Username.Should().Be(expectedUser.Username);
        _httpClientMock.Verify(x => x.GetAsync<UserExtended>("/api/account", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProfileAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var expectedUser = CreateTestUserExtended();
        _httpClientMock
            .Setup(x => x.GetAsync<UserExtended>("/api/account", cts.Token))
            .ReturnsAsync(expectedUser);

        // Act
        await _accountApi.GetProfileAsync(cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<UserExtended>("/api/account", cts.Token), Times.Once);
    }



    [Fact]
    public async Task GetEmailAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResponse = new EmailResponse { Email = "test@example.com" };
        _httpClientMock
            .Setup(x => x.GetAsync<EmailResponse>("/api/account/email", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _accountApi.GetEmailAsync();

        // Assert
        result.Should().Be("test@example.com");
        _httpClientMock.Verify(x => x.GetAsync<EmailResponse>("/api/account/email", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetEmailAsync_WhenEmailIsNull_ReturnsEmptyString()
    {
        // Arrange
        var expectedResponse = new EmailResponse { Email = null };
        _httpClientMock
            .Setup(x => x.GetAsync<EmailResponse>("/api/account/email", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _accountApi.GetEmailAsync();

        // Assert
        result.Should().Be(string.Empty);
    }



    [Fact]
    public async Task GetPreferencesAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedPrefs = new AccountPreferences
        {
            Language = "en-GB",
            Prefs = new UserPreferences { Dark = true }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<AccountPreferences>("/api/account/preferences", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPrefs);

        // Act
        var result = await _accountApi.GetPreferencesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Language.Should().Be("en-GB");
        result.Prefs?.Dark.Should().BeTrue();
        _httpClientMock.Verify(x => x.GetAsync<AccountPreferences>("/api/account/preferences", It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task GetKidModeAsync_WhenEnabled_ReturnsTrue()
    {
        // Arrange
        var expectedResponse = new KidModeResponse { Kid = true };
        _httpClientMock
            .Setup(x => x.GetAsync<KidModeResponse>("/api/account/kid", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _accountApi.GetKidModeAsync();

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.GetAsync<KidModeResponse>("/api/account/kid", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetKidModeAsync_WhenDisabled_ReturnsFalse()
    {
        // Arrange
        var expectedResponse = new KidModeResponse { Kid = false };
        _httpClientMock
            .Setup(x => x.GetAsync<KidModeResponse>("/api/account/kid", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _accountApi.GetKidModeAsync();

        // Assert
        result.Should().BeFalse();
    }



    [Fact]
    public async Task SetKidModeAsync_WhenEnabling_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResponse = new OkResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>("/api/account/kid?v=true", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _accountApi.SetKidModeAsync(true);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>("/api/account/kid?v=true", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetKidModeAsync_WhenDisabling_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResponse = new OkResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>("/api/account/kid?v=false", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _accountApi.SetKidModeAsync(false);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>("/api/account/kid?v=false", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetKidModeAsync_WhenFails_ReturnsFalse()
    {
        // Arrange
        var expectedResponse = new OkResponse { Ok = false };
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _accountApi.SetKidModeAsync(true);

        // Assert
        result.Should().BeFalse();
    }



    private static UserExtended CreateTestUserExtended() => new()
    {
        Id = "testuser",
        Username = "TestUser",
        CreatedAt = DateTimeOffset.UtcNow.AddYears(-1),
        Patron = true
    };

}
