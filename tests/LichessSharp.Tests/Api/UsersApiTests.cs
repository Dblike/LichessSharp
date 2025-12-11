using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
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
    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new UsersApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

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
        var result = await _usersApi.GetManyAsync([]);

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
        var result = await _usersApi.GetRealTimeStatusAsync(userIds);

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
            .ReturnsAsync([]);

        // Act
        await _usersApi.GetRealTimeStatusAsync(userIds, options);

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
        var result = await _usersApi.GetRealTimeStatusAsync([]);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStatusAsync_WithMoreThan100Users_ThrowsArgumentException()
    {
        // Arrange
        var userIds = Enumerable.Range(1, 101).Select(i => $"user{i}");

        // Act
        var act = () => _usersApi.GetRealTimeStatusAsync(userIds);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*100*");
    }

    [Fact]
    public async Task GetAllTop10Async_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new Dictionary<string, List<User>>
        {
            ["bullet"] = [CreateTestUser("player1")],
            ["blitz"] = [CreateTestUser("player2")]
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
            .ReturnsAsync(new LeaderboardResponse { Users = [] });

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

    [Fact]
    public async Task GetPerformanceAsync_WithValidParams_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new UserPerformance { Rank = 100 };
        _httpClientMock
            .Setup(x => x.GetAsync<UserPerformance>("/api/user/thibault/perf/bullet", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _usersApi.GetPerformanceAsync("thibault", "bullet");

        // Assert
        result.Rank.Should().Be(100);
        _httpClientMock.Verify(x => x.GetAsync<UserPerformance>("/api/user/thibault/perf/bullet", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPerformanceAsync_WithNullUsername_ThrowsArgumentException()
    {
        var act = () => _usersApi.GetPerformanceAsync(null!, "bullet");
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetPerformanceAsync_WithNullPerfType_ThrowsArgumentException()
    {
        var act = () => _usersApi.GetPerformanceAsync("thibault", null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetActivityAsync_WithValidUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new List<UserActivity> { new() };
        _httpClientMock
            .Setup(x => x.GetAsync<List<UserActivity>>("/api/user/thibault/activity", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _usersApi.GetActivityAsync("thibault");

        // Assert
        result.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.GetAsync<List<UserActivity>>("/api/user/thibault/activity", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetActivityAsync_WithNullUsername_ThrowsArgumentException()
    {
        var act = () => _usersApi.GetActivityAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AutocompleteAsync_WithValidTerm_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new List<string> { "thibault", "thib" };
        _httpClientMock
            .Setup(x => x.GetAsync<List<string>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _usersApi.AutocompleteAsync("thib");

        // Assert
        result.Should().HaveCount(2);
        _httpClientMock.Verify(x => x.GetAsync<List<string>>(
            It.Is<string>(s => s.Contains("/api/player/autocomplete?term=thib")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AutocompleteAsync_WithFriend_IncludesFriendParam()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.GetAsync<List<string>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _usersApi.AutocompleteAsync("thib", friend: "thibault");

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<List<string>>(
            It.Is<string>(s => s.Contains("friend=thibault")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AutocompleteAsync_WithNullTerm_ThrowsArgumentException()
    {
        var act = () => _usersApi.AutocompleteAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AutocompletePlayersAsync_WithValidTerm_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new AutocompleteResponse { Result = new List<AutocompletePlayer> { new() { Id = "thibault", Name = "Thibault" } } };
        _httpClientMock
            .Setup(x => x.GetAsync<AutocompleteResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _usersApi.AutocompletePlayersAsync("thib");

        // Assert
        result.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.GetAsync<AutocompleteResponse>(
            It.Is<string>(s => s.Contains("/api/player/autocomplete") && s.Contains("object=true")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCrosstableAsync_WithTwoUsers_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new Crosstable { NbGames = 10 };
        _httpClientMock
            .Setup(x => x.GetAsync<Crosstable>("/api/crosstable/user1/user2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _usersApi.GetCrosstableAsync("user1", "user2");

        // Assert
        result.NbGames.Should().Be(10);
        _httpClientMock.Verify(x => x.GetAsync<Crosstable>("/api/crosstable/user1/user2", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCrosstableAsync_WithMatchup_AddsQueryParam()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.GetAsync<Crosstable>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Crosstable());

        // Act
        await _usersApi.GetCrosstableAsync("user1", "user2", matchup: true);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<Crosstable>("/api/crosstable/user1/user2?matchup=true", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCrosstableAsync_WithNullUser1_ThrowsArgumentException()
    {
        var act = () => _usersApi.GetCrosstableAsync(null!, "user2");
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCrosstableAsync_WithNullUser2_ThrowsArgumentException()
    {
        var act = () => _usersApi.GetCrosstableAsync("user1", null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetLiveStreamersAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new List<Streamer> { new() { Id = "streamer1", Name = "Streamer1" } };
        _httpClientMock
            .Setup(x => x.GetAsync<List<Streamer>>("/api/streamer/live", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _usersApi.GetLiveStreamersAsync();

        // Assert
        result.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.GetAsync<List<Streamer>>("/api/streamer/live", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetNoteAsync_WithValidUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new NoteResponse { Text = "Test note" };
        _httpClientMock
            .Setup(x => x.GetAsync<NoteResponse>("/api/user/thibault/note", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _usersApi.GetNoteAsync("thibault");

        // Assert
        result.Should().Be("Test note");
        _httpClientMock.Verify(x => x.GetAsync<NoteResponse>("/api/user/thibault/note", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetNoteAsync_WithNullUsername_ThrowsArgumentException()
    {
        var act = () => _usersApi.GetNoteAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task WriteNoteAsync_WithValidParams_CallsCorrectEndpoint()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.PostFormAsync<OkResponse>(
                "/api/user/thibault/note",
                It.Is<IDictionary<string, string>>(d => d["text"] == "My note"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _usersApi.WriteNoteAsync("thibault", "My note");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task WriteNoteAsync_WithNullUsername_ThrowsArgumentException()
    {
        var act = () => _usersApi.WriteNoteAsync(null!, "note");
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task WriteNoteAsync_WithNullText_ThrowsArgumentNullException()
    {
        var act = () => _usersApi.WriteNoteAsync("thibault", null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetTimelineAsync_WithoutParams_CallsBaseEndpoint()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.GetAsync<Timeline>("/api/timeline", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Timeline());

        // Act
        await _usersApi.GetTimelineAsync();

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<Timeline>("/api/timeline", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTimelineAsync_WithNb_IncludesQueryParam()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.GetAsync<Timeline>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Timeline());

        // Act
        await _usersApi.GetTimelineAsync(nb: 15);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<Timeline>(
            It.Is<string>(s => s.Contains("nb=15")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTimelineAsync_WithSince_IncludesTimestamp()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.GetAsync<Timeline>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Timeline());
        var since = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        await _usersApi.GetTimelineAsync(since: since);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<Timeline>(
            It.Is<string>(s => s.Contains("since=")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

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

}
