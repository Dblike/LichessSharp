using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Http;
using LichessSharp.Models.Common;
using LichessSharp.Models.Users;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class RelationsApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly RelationsApi _relationsApi;

    public RelationsApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _relationsApi = new RelationsApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new RelationsApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task FollowAsync_WithUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "thibault";
        var expectedResponse = new OkResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/rel/follow/{username}", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _relationsApi.FollowUserAsync(username);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/rel/follow/{username}", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task FollowAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _relationsApi.FollowUserAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task FollowAsync_WithEmptyUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _relationsApi.FollowUserAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task FollowAsync_UrlEncodesUsername()
    {
        // Arrange - username with special characters
        var username = "user name";
        var expectedResponse = new OkResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("user%20name")), null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        await _relationsApi.FollowUserAsync(username);

        // Assert
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("user%20name")), null,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UnfollowAsync_WithUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "thibault";
        var expectedResponse = new OkResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/rel/unfollow/{username}", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _relationsApi.UnfollowUserAsync(username);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/rel/unfollow/{username}", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UnfollowAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _relationsApi.UnfollowUserAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task BlockAsync_WithUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "thibault";
        var expectedResponse = new OkResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/rel/block/{username}", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _relationsApi.BlockUserAsync(username);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/rel/block/{username}", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task BlockAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _relationsApi.BlockUserAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UnblockAsync_WithUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "thibault";
        var expectedResponse = new OkResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/rel/unblock/{username}", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _relationsApi.UnblockUserAsync(username);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/rel/unblock/{username}", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UnblockAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _relationsApi.UnblockUserAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task StreamFollowingAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var users = new List<UserExtended>
        {
            CreateTestUserExtended("user1", "User1"),
            CreateTestUserExtended("user2", "User2")
        };

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<UserExtended>("/api/rel/following", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(users));

        // Act
        var results = new List<UserExtended>();
        await foreach (var user in _relationsApi.StreamFollowingUsersAsync()) results.Add(user);

        // Assert
        results.Should().HaveCount(2);
        results[0].Id.Should().Be("user1");
        results[1].Id.Should().Be("user2");
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<UserExtended>("/api/rel/following", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamFollowingAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var users = new List<UserExtended>();

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<UserExtended>("/api/rel/following", cts.Token))
            .Returns(ToAsyncEnumerable(users));

        // Act
        await foreach (var _ in _relationsApi.StreamFollowingUsersAsync(cts.Token))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<UserExtended>("/api/rel/following", cts.Token), Times.Once);
    }

    [Fact]
    public async Task FollowAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var expectedResponse = new OkResponse { Ok = true };
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>(It.IsAny<string>(), null, cts.Token))
            .ReturnsAsync(expectedResponse);

        // Act
        await _relationsApi.FollowUserAsync("user", cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>(It.IsAny<string>(), null, cts.Token), Times.Once);
    }

    private static UserExtended CreateTestUserExtended(string id, string username)
    {
        return new UserExtended
        {
            Id = id,
            Username = username,
            CreatedAt = DateTimeOffset.UtcNow.AddYears(-1)
        };
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items) yield return item;
        await Task.CompletedTask;
    }
}