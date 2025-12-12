using FluentAssertions;
using LichessSharp.Api.Options;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Integration tests for the Users API.
///     These tests use real Lichess usernames from the OpenAPI spec examples.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class UsersApiIntegrationTests : IntegrationTestBase
{
    // Well-known usernames from OpenAPI spec examples
    private const string ThibaultUsername = "thibault";
    private const string Maia1Username = "maia1";
    private const string Maia5Username = "maia5";

    [Fact]
    public async Task GetAsync_WithValidUsername_ReturnsUserProfile()
    {
        // Act
        var user = await Client.Users.GetAsync(ThibaultUsername);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(ThibaultUsername);
        user.Username.Should().BeEquivalentTo(ThibaultUsername);
    }

    [Fact]
    public async Task GetAsync_WithTrophiesOption_ReturnsTrophies()
    {
        // Act
        var user = await Client.Users.GetAsync(ThibaultUsername, new GetUserOptions { Trophies = true });

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(ThibaultUsername);
    }

    [Fact]
    public async Task GetManyAsync_WithMultipleUsernames_ReturnsAllUsers()
    {
        // Arrange
        var userIds = new[] { ThibaultUsername, Maia1Username, Maia5Username };

        // Act
        var users = await Client.Users.GetManyAsync(userIds);

        // Assert
        users.Should().NotBeNull();
        users.Should().HaveCountGreaterThanOrEqualTo(1);
        users.Should().Contain(u => u.Id == ThibaultUsername);
    }

    [Fact]
    public async Task GetStatusAsync_WithValidUsernames_ReturnsStatuses()
    {
        // Arrange
        var userIds = new[] { ThibaultUsername, Maia1Username };

        // Act
        var statuses = await Client.Users.GetRealTimeStatusAsync(userIds);

        // Assert
        statuses.Should().NotBeNull();
        statuses.Should().HaveCountGreaterThanOrEqualTo(1);
        statuses.Should().Contain(s => s.Id == ThibaultUsername);
    }

    [Fact]
    public async Task GetStatusAsync_WithSignalOption_ReturnsStatusesWithSignal()
    {
        // Arrange
        var userIds = new[] { ThibaultUsername };
        var options = new GetUserStatusOptions { WithSignal = true };

        // Act
        var statuses = await Client.Users.GetRealTimeStatusAsync(userIds, options);

        // Assert
        statuses.Should().NotBeNull();
        statuses.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetAllTop10Async_ReturnsLeaderboards()
    {
        // Act
        var leaderboards = await Client.Users.GetAllTop10Async();

        // Assert
        leaderboards.Should().NotBeNull();
        leaderboards.Should().NotBeEmpty();
        // Should contain common perf types
        leaderboards.Keys.Should().Contain(k => k.Contains("bullet", StringComparison.OrdinalIgnoreCase) ||
                                                k.Contains("blitz", StringComparison.OrdinalIgnoreCase) ||
                                                k.Contains("rapid", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithBulletPerf_ReturnsTopPlayers()
    {
        // Act
        var players = await Client.Users.GetLeaderboardAsync("bullet", 10);

        // Assert
        players.Should().NotBeNull();
        players.Should().HaveCount(10);
        players.Should().AllSatisfy(p =>
        {
            p.Id.Should().NotBeNullOrEmpty();
            p.Username.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithBlitzPerf_ReturnsTopPlayers()
    {
        // Act
        var players = await Client.Users.GetLeaderboardAsync("blitz", 5);

        // Assert
        players.Should().NotBeNull();
        players.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetRatingHistoryAsync_WithValidUsername_ReturnsHistory()
    {
        // Act
        var history = await Client.Users.GetRatingHistoryAsync(ThibaultUsername);

        // Assert
        history.Should().NotBeNull();
        history.Should().NotBeEmpty();
        history.Should().AllSatisfy(h => { h.Name.Should().NotBeNullOrEmpty(); });
    }

    [Fact]
    public async Task GetPerformanceAsync_WithValidUsernameAndPerfType_ReturnsPerformance()
    {
        // Act
        var performance = await Client.Users.GetPerformanceAsync(ThibaultUsername, "blitz");

        // Assert
        performance.Should().NotBeNull();
        performance.Perf.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPerformanceAsync_WithBulletPerf_ReturnsPerformance()
    {
        // Act
        var performance = await Client.Users.GetPerformanceAsync(ThibaultUsername, "bullet");

        // Assert
        performance.Should().NotBeNull();
        performance.Stat.Should().NotBeNull();
    }

    [Fact]
    public async Task GetActivityAsync_WithValidUsername_ReturnsActivity()
    {
        // Act
        var activity = await Client.Users.GetActivityAsync(ThibaultUsername);

        // Assert
        activity.Should().NotBeNull();
        // Activity might be empty if user hasn't been active recently
    }

    [Fact]
    public async Task AutocompleteAsync_WithValidTerm_ReturnsUsernames()
    {
        // Act
        var usernames = await Client.Users.AutocompleteAsync("thibault");

        // Assert
        usernames.Should().NotBeNull();
        usernames.Should().NotBeEmpty();
        usernames.Should().Contain(u => u.Contains("thibault", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task AutocompletePlayersAsync_WithValidTerm_ReturnsPlayers()
    {
        // Act
        var players = await Client.Users.AutocompletePlayersAsync("thibault");

        // Assert
        players.Should().NotBeNull();
        players.Should().NotBeEmpty();
        players.Should().AllSatisfy(p =>
        {
            p.Id.Should().NotBeNullOrEmpty();
            p.Name.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task GetCrosstableAsync_WithValidUsernames_ReturnsCrosstable()
    {
        // Act
        var crosstable = await Client.Users.GetCrosstableAsync(ThibaultUsername, "DrNykterstein");

        // Assert
        crosstable.Should().NotBeNull();
        crosstable.Users.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCrosstableAsync_WithMatchupOption_ReturnsCrosstable()
    {
        // Act
        var crosstable = await Client.Users.GetCrosstableAsync(ThibaultUsername, "DrNykterstein", true);

        // Assert
        crosstable.Should().NotBeNull();
        crosstable.Users.Should().NotBeNull();
    }

    [Fact]
    public async Task GetLiveStreamersAsync_ReturnsStreamers()
    {
        // Act
        var streamers = await Client.Users.GetLiveStreamersAsync();

        // Assert
        streamers.Should().NotBeNull();
        // Streamers list might be empty if no one is streaming
    }
}