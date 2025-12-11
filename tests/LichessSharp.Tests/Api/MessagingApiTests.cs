using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class MessagingApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly MessagingApi _messagingApi;

    public MessagingApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _messagingApi = new MessagingApi(_httpClientMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new MessagingApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    #endregion

    #region SendAsync Tests

    [Fact]
    public async Task SendAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResponse = new MessageSentResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<MessageSentResponse>("/inbox/testuser", It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _messagingApi.SendAsync("testuser", "Hello!");

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<MessageSentResponse>("/inbox/testuser", It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_ReturnsTrue_WhenMessageSentSuccessfully()
    {
        // Arrange
        var expectedResponse = new MessageSentResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<MessageSentResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _messagingApi.SendAsync("user", "message");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_ReturnsFalse_WhenMessageFails()
    {
        // Arrange
        var expectedResponse = new MessageSentResponse { Ok = false };
        _httpClientMock
            .Setup(x => x.PostAsync<MessageSentResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _messagingApi.SendAsync("user", "message");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SendAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _messagingApi.SendAsync(null!, "Hello!");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendAsync_WithEmptyUsername_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _messagingApi.SendAsync("", "Hello!");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendAsync_WithWhitespaceUsername_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _messagingApi.SendAsync("   ", "Hello!");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendAsync_WithNullText_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _messagingApi.SendAsync("testuser", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendAsync_WithEmptyText_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _messagingApi.SendAsync("testuser", "");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendAsync_WithWhitespaceText_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _messagingApi.SendAsync("testuser", "   ");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendAsync_EscapesUsernameInUrl()
    {
        // Arrange
        var expectedResponse = new MessageSentResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<MessageSentResponse>("/inbox/user%2Fspecial", It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        await _messagingApi.SendAsync("user/special", "Hello!");

        // Assert
        _httpClientMock.Verify(x => x.PostAsync<MessageSentResponse>("/inbox/user%2Fspecial", It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_IncludesTextInFormData()
    {
        // Arrange
        var expectedResponse = new MessageSentResponse { Ok = true };
        HttpContent? capturedContent = null;
        _httpClientMock
            .Setup(x => x.PostAsync<MessageSentResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .Callback<string, HttpContent, CancellationToken>((_, content, _) => capturedContent = content)
            .ReturnsAsync(expectedResponse);

        // Act
        await _messagingApi.SendAsync("testuser", "Test message content");

        // Assert
        capturedContent.Should().NotBeNull();
        var formData = await capturedContent!.ReadAsStringAsync();
        formData.Should().Contain("text=Test+message+content");
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_PassesToken()
    {
        // Arrange
        var expectedResponse = new MessageSentResponse { Ok = true };
        var cts = new CancellationTokenSource();
        _httpClientMock
            .Setup(x => x.PostAsync<MessageSentResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), cts.Token))
            .ReturnsAsync(expectedResponse);

        // Act
        await _messagingApi.SendAsync("testuser", "Hello!", cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.PostAsync<MessageSentResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), cts.Token), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithSpecialCharactersInMessage_EncodesCorrectly()
    {
        // Arrange
        var expectedResponse = new MessageSentResponse { Ok = true };
        HttpContent? capturedContent = null;
        _httpClientMock
            .Setup(x => x.PostAsync<MessageSentResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .Callback<string, HttpContent, CancellationToken>((_, content, _) => capturedContent = content)
            .ReturnsAsync(expectedResponse);

        // Act
        await _messagingApi.SendAsync("testuser", "Hello & goodbye! <test>");

        // Assert
        capturedContent.Should().NotBeNull();
        var formData = await capturedContent!.ReadAsStringAsync();
        // FormUrlEncodedContent should encode special characters
        formData.Should().Contain("text=");
        formData.Should().NotContain("&goodbye"); // & should be encoded
    }

    [Fact]
    public async Task SendAsync_WithLongMessage_SendsCorrectly()
    {
        // Arrange
        var longMessage = new string('a', 10000);
        var expectedResponse = new MessageSentResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<MessageSentResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _messagingApi.SendAsync("testuser", longMessage);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<MessageSentResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
