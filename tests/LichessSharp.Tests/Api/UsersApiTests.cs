using System.Net;
using System.Text.Json;
using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Options;
using LichessSharp.Http;
using LichessSharp.Models;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class UsersApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly UsersApi _usersApi;

    public UsersApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _usersApi = new UsersApi(_httpClientMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new UsersApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedUser = CreateTestUserExtended("DrNykterstein");
        _httpClientMock
            .Setup(x => x.GetAsync<UserExtended>("/api/user/DrNykterstein", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _usersApi.GetAsync("DrNykterstein");

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
        _httpClientMock.Verify(x => x.GetAsync<UserExtended>("/api/user/DrNykterstein", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithOptions_BuildsCorrectQueryString()
    {
        // Arrange
        var options = new GetUserOptions
        {
            Trophies = true,
            Profile = false,
            Rank = true
        };
        var expectedUser = CreateTestUserExtended("thibault");
        _httpClientMock
            .Setup(x => x.GetAsync<UserExtended>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        await _usersApi.GetAsync("thibault", options);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<UserExtended>(
            It.Is<string>(s =>
                s.Contains("/api/user/thibault") &&
                s.Contains("trophies=true") &&
                s.Contains("profile=false") &&
                s.Contains("rank=true")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _usersApi.GetAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_WithEmptyUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _usersApi.GetAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region GetManyAsync Tests

    [Fact]
    public async Task GetManyAsync_WithUserIds_CallsCorrectEndpoint()
    {
        // Arrange
        var userIds = new[] { "user1", "user2", "user3" };
        var expectedUsers = new List<User>
        {
            CreateTestUser("user1"),
            CreateTestUser("user2"),
            CreateTestUser("user3")
        };
        _httpClientMock
            .Setup(x => x.PostPlainTextAsync<List<User>>("/api/users", "user1,user2,user3", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _usersApi.GetManyAsync(userIds);

        // Assert
        result.Should().HaveCount(3);
        _httpClientMock.Verify(x => x.PostPlainTextAsync<List<User>>("/api/users", "user1,user2,user3", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetManyAsync_WithEmptyList_ReturnsEmptyList()
    {
        // Act
        var result = await _usersApi.GetManyAsync(Array.Empty<string>());

        // Assert
        result.Should().BeEmpty();
        _httpClientMock.Verify(x => x.PostPlainTextAsync<List<User>>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetManyAsync_WithMoreThan300Users_ThrowsArgumentException()
    {
        // Arrange
        var userIds = Enumerable.Range(1, 301).Select(i => $"user{i}");

        // Act
        var act = () => _usersApi.GetManyAsync(userIds);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*300*");
    }

    [Fact]
    public async Task GetManyAsync_WithNullUserIds_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _usersApi.GetManyAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region GetStatusAsync Tests

    [Fact]
    public async Task GetStatusAsync_WithUserIds_CallsCorrectEndpoint()
    {
        // Arrange
        var userIds = new[] { "user1", "user2" };
        var expectedStatuses = new List<UserStatus>
        {
            new() { Id = "user1", Name = "User1" },
            new() { Id = "user2", Name = "User2" }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<List<UserStatus>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedStatuses);

        // Act
        var result = await _usersApi.GetStatusAsync(userIds);

        // Assert
        result.Should().HaveCount(2);
        _httpClientMock.Verify(x => x.GetAsync<List<UserStatus>>(
            It.Is<string>(s => s.StartsWith("/api/users/status?ids=user1,user2")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetStatusAsync_WithOptions_BuildsCorrectQueryString()
    {
        // Arrange
        var userIds = new[] { "user1" };
        var options = new GetUserStatusOptions
        {
            WithSignal = true,
            WithGameIds = true,
            WithGameMetas = true
        };
        _httpClientMock
            .Setup(x => x.GetAsync<List<UserStatus>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserStatus>());

        // Act
        await _usersApi.GetStatusAsync(userIds, options);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<List<UserStatus>>(
            It.Is<string>(s =>
                s.Contains("withSignal=true") &&
                s.Contains("withGameIds=true") &&
                s.Contains("withGameMetas=true")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetStatusAsync_WithEmptyList_ReturnsEmptyList()
    {
        // Act
        var result = await _usersApi.GetStatusAsync(Array.Empty<string>());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStatusAsync_WithMoreThan100Users_ThrowsArgumentException()
    {
        // Arrange
        var userIds = Enumerable.Range(1, 101).Select(i => $"user{i}");

        // Act
        var act = () => _usersApi.GetStatusAsync(userIds);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*100*");
    }

    #endregion

    #region GetAllTop10Async Tests

    [Fact]
    public async Task GetAllTop10Async_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new Dictionary<string, List<User>>
        {
            ["bullet"] = new() { CreateTestUser("player1") },
            ["blitz"] = new() { CreateTestUser("player2") }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<Dictionary<string, List<User>>>("/api/player", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _usersApi.GetAllTop10Async();

        // Assert
        result.Should().ContainKey("bullet");
        result.Should().ContainKey("blitz");
        _httpClientMock.Verify(x => x.GetAsync<Dictionary<string, List<User>>>("/api/player", It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetLeaderboardAsync Tests

    [Fact]
    public async Task GetLeaderboardAsync_WithPerfType_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedUsers = new List<User> { CreateTestUser("player1") };
        _httpClientMock
            .Setup(x => x.GetAsync<LeaderboardResponse>("/api/player/top/100/bullet", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaderboardResponse { Users = expectedUsers });

        // Act
        var result = await _usersApi.GetLeaderboardAsync("bullet");

        // Assert
        result.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.GetAsync<LeaderboardResponse>("/api/player/top/100/bullet", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithCustomCount_CallsCorrectEndpoint()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.GetAsync<LeaderboardResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaderboardResponse { Users = new List<User>() });

        // Act
        await _usersApi.GetLeaderboardAsync("blitz", 50);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<LeaderboardResponse>("/api/player/top/50/blitz", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithCountBelow1_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => _usersApi.GetLeaderboardAsync("bullet", 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithCountAbove200_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => _usersApi.GetLeaderboardAsync("bullet", 201);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithNullPerfType_ThrowsArgumentException()
    {
        // Act
        var act = () => _usersApi.GetLeaderboardAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region GetRatingHistoryAsync Tests

    [Fact]
    public async Task GetRatingHistoryAsync_WithUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedHistory = new List<RatingHistory>
        {
            new() { Name = "Bullet", Points = new List<RatingDataPoint>() }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<List<RatingHistory>>("/api/user/DrNykterstein/rating-history", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedHistory);

        // Act
        var result = await _usersApi.GetRatingHistoryAsync("DrNykterstein");

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Bullet");
        _httpClientMock.Verify(x => x.GetAsync<List<RatingHistory>>("/api/user/DrNykterstein/rating-history", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRatingHistoryAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _usersApi.GetRatingHistoryAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region Helper Methods

    private static User CreateTestUser(string username) => new()
    {
        Id = username.ToLowerInvariant(),
        Username = username
    };

    private static UserExtended CreateTestUserExtended(string username) => new()
    {
        Id = username.ToLowerInvariant(),
        Username = username,
        CreatedAt = DateTimeOffset.UtcNow
    };

    #endregion
}
