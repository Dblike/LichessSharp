using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Http;
using LichessSharp.Models;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class PuzzlesApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly PuzzlesApi _puzzlesApi;

    public PuzzlesApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _puzzlesApi = new PuzzlesApi(_httpClientMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new PuzzlesApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    #endregion

    #region GetDailyAsync Tests

    [Fact]
    public async Task GetDailyAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedPuzzle = CreateTestPuzzleWithGame("abc123");
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleWithGame>("/api/puzzle/daily", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPuzzle);

        // Act
        var result = await _puzzlesApi.GetDailyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Puzzle.Should().NotBeNull();
        result.Puzzle!.Id.Should().Be("abc123");
        _httpClientMock.Verify(x => x.GetAsync<PuzzleWithGame>("/api/puzzle/daily", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDailyAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var expectedPuzzle = CreateTestPuzzleWithGame("abc123");
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleWithGame>("/api/puzzle/daily", cts.Token))
            .ReturnsAsync(expectedPuzzle);

        // Act
        await _puzzlesApi.GetDailyAsync(cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<PuzzleWithGame>("/api/puzzle/daily", cts.Token), Times.Once);
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithId_CallsCorrectEndpoint()
    {
        // Arrange
        var puzzleId = "abc123";
        var expectedPuzzle = CreateTestPuzzleWithGame(puzzleId);
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleWithGame>($"/api/puzzle/{puzzleId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPuzzle);

        // Act
        var result = await _puzzlesApi.GetAsync(puzzleId);

        // Assert
        result.Should().NotBeNull();
        result.Puzzle!.Id.Should().Be(puzzleId);
        _httpClientMock.Verify(x => x.GetAsync<PuzzleWithGame>($"/api/puzzle/{puzzleId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithNullId_ThrowsArgumentException()
    {
        // Act
        var act = () => _puzzlesApi.GetAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act
        var act = () => _puzzlesApi.GetAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_UrlEncodesId()
    {
        // Arrange
        var puzzleId = "puzzle id";
        var expectedPuzzle = CreateTestPuzzleWithGame(puzzleId);
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s => s.Contains("puzzle%20id")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPuzzle);

        // Act
        await _puzzlesApi.GetAsync(puzzleId);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s => s.Contains("puzzle%20id")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetNextAsync Tests

    [Fact]
    public async Task GetNextAsync_WithNoParameters_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedPuzzle = CreateTestPuzzleWithGame("abc123");
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleWithGame>("/api/puzzle/next", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPuzzle);

        // Act
        var result = await _puzzlesApi.GetNextAsync();

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAsync<PuzzleWithGame>("/api/puzzle/next", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetNextAsync_WithAngle_AppendsQueryParameter()
    {
        // Arrange
        var expectedPuzzle = CreateTestPuzzleWithGame("abc123");
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s => s.Contains("angle=mate")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPuzzle);

        // Act
        await _puzzlesApi.GetNextAsync(angle: "mate");

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s => s.Contains("angle=mate")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetNextAsync_WithDifficulty_AppendsQueryParameter()
    {
        // Arrange
        var expectedPuzzle = CreateTestPuzzleWithGame("abc123");
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s => s.Contains("difficulty=harder")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPuzzle);

        // Act
        await _puzzlesApi.GetNextAsync(difficulty: "harder");

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s => s.Contains("difficulty=harder")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetNextAsync_WithAngleAndDifficulty_AppendsBothQueryParameters()
    {
        // Arrange
        var expectedPuzzle = CreateTestPuzzleWithGame("abc123");
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s =>
                s.Contains("angle=fork") &&
                s.Contains("difficulty=easier")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPuzzle);

        // Act
        await _puzzlesApi.GetNextAsync(angle: "fork", difficulty: "easier");

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s =>
            s.Contains("angle=fork") &&
            s.Contains("difficulty=easier")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetNextAsync_UrlEncodesAngle()
    {
        // Arrange
        var expectedPuzzle = CreateTestPuzzleWithGame("abc123");
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s => s.Contains("angle=mate%20in%202")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPuzzle);

        // Act
        await _puzzlesApi.GetNextAsync(angle: "mate in 2");

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<PuzzleWithGame>(It.Is<string>(s => s.Contains("angle=mate%20in%202")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region StreamActivityAsync Tests

    [Fact]
    public async Task StreamActivityAsync_WithNoParameters_CallsCorrectEndpoint()
    {
        // Arrange
        var activities = new List<PuzzleActivity>
        {
            CreateTestPuzzleActivity(true),
            CreateTestPuzzleActivity(false)
        };

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<PuzzleActivity>("/api/puzzle/activity", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(activities));

        // Act
        var results = new List<PuzzleActivity>();
        await foreach (var activity in _puzzlesApi.StreamActivityAsync())
        {
            results.Add(activity);
        }

        // Assert
        results.Should().HaveCount(2);
        results[0].Win.Should().BeTrue();
        results[1].Win.Should().BeFalse();
    }

    [Fact]
    public async Task StreamActivityAsync_WithMax_AppendsQueryParameter()
    {
        // Arrange
        var activities = new List<PuzzleActivity>();

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<PuzzleActivity>(It.Is<string>(s => s.Contains("max=10")), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(activities));

        // Act
        await foreach (var _ in _puzzlesApi.StreamActivityAsync(max: 10))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<PuzzleActivity>(It.Is<string>(s => s.Contains("max=10")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamActivityAsync_WithBefore_AppendsQueryParameter()
    {
        // Arrange
        var before = new DateTimeOffset(2023, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var activities = new List<PuzzleActivity>();

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<PuzzleActivity>(It.Is<string>(s => s.Contains($"before={before.ToUnixTimeMilliseconds()}")), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(activities));

        // Act
        await foreach (var _ in _puzzlesApi.StreamActivityAsync(before: before))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<PuzzleActivity>(It.Is<string>(s => s.Contains("before=")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamActivityAsync_WithMaxAndBefore_AppendsBothQueryParameters()
    {
        // Arrange
        var before = new DateTimeOffset(2023, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var activities = new List<PuzzleActivity>();

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<PuzzleActivity>(It.Is<string>(s =>
                s.Contains("max=5") &&
                s.Contains("before=")), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(activities));

        // Act
        await foreach (var _ in _puzzlesApi.StreamActivityAsync(max: 5, before: before))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<PuzzleActivity>(It.Is<string>(s =>
            s.Contains("max=5") &&
            s.Contains("before=")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetDashboardAsync Tests

    [Fact]
    public async Task GetDashboardAsync_WithDefaultDays_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedDashboard = CreateTestPuzzleDashboard(30);
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleDashboard>("/api/puzzle/dashboard/30", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDashboard);

        // Act
        var result = await _puzzlesApi.GetDashboardAsync();

        // Assert
        result.Should().NotBeNull();
        result.Days.Should().Be(30);
        _httpClientMock.Verify(x => x.GetAsync<PuzzleDashboard>("/api/puzzle/dashboard/30", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDashboardAsync_WithCustomDays_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedDashboard = CreateTestPuzzleDashboard(7);
        _httpClientMock
            .Setup(x => x.GetAsync<PuzzleDashboard>("/api/puzzle/dashboard/7", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDashboard);

        // Act
        var result = await _puzzlesApi.GetDashboardAsync(days: 7);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAsync<PuzzleDashboard>("/api/puzzle/dashboard/7", It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetStormDashboardAsync Tests

    [Fact]
    public async Task GetStormDashboardAsync_WithDefaultDays_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "testuser";
        var expectedDashboard = CreateTestStormDashboard();
        _httpClientMock
            .Setup(x => x.GetAsync<StormDashboard>($"/api/storm/dashboard/{username}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDashboard);

        // Act
        var result = await _puzzlesApi.GetStormDashboardAsync(username);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAsync<StormDashboard>($"/api/storm/dashboard/{username}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetStormDashboardAsync_WithCustomDays_AppendsQueryParameter()
    {
        // Arrange
        var username = "testuser";
        var expectedDashboard = CreateTestStormDashboard();
        _httpClientMock
            .Setup(x => x.GetAsync<StormDashboard>(It.Is<string>(s => s.Contains("days=7")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDashboard);

        // Act
        await _puzzlesApi.GetStormDashboardAsync(username, days: 7);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<StormDashboard>(It.Is<string>(s => s.Contains("days=7")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetStormDashboardAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _puzzlesApi.GetStormDashboardAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetStormDashboardAsync_WithEmptyUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _puzzlesApi.GetStormDashboardAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetStormDashboardAsync_UrlEncodesUsername()
    {
        // Arrange
        var username = "user name";
        var expectedDashboard = CreateTestStormDashboard();
        _httpClientMock
            .Setup(x => x.GetAsync<StormDashboard>(It.Is<string>(s => s.Contains("user%20name")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDashboard);

        // Act
        await _puzzlesApi.GetStormDashboardAsync(username);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<StormDashboard>(It.Is<string>(s => s.Contains("user%20name")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region CreateRaceAsync Tests

    [Fact]
    public async Task CreateRaceAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedRace = new PuzzleRace { Id = "race123", Url = "https://lichess.org/racer/race123" };
        _httpClientMock
            .Setup(x => x.PostAsync<PuzzleRace>("/api/racer", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRace);

        // Act
        var result = await _puzzlesApi.CreateRaceAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("race123");
        result.Url.Should().Be("https://lichess.org/racer/race123");
        _httpClientMock.Verify(x => x.PostAsync<PuzzleRace>("/api/racer", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRaceAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var expectedRace = new PuzzleRace { Id = "race123", Url = "https://lichess.org/racer/race123" };
        _httpClientMock
            .Setup(x => x.PostAsync<PuzzleRace>("/api/racer", null, cts.Token))
            .ReturnsAsync(expectedRace);

        // Act
        await _puzzlesApi.CreateRaceAsync(cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.PostAsync<PuzzleRace>("/api/racer", null, cts.Token), Times.Once);
    }

    #endregion

    #region Helper Methods

    private static PuzzleWithGame CreateTestPuzzleWithGame(string id) => new()
    {
        Game = new PuzzleGame
        {
            Id = "game123"
        },
        Puzzle = new Puzzle
        {
            Id = id,
            Rating = 1500,
            Plays = 1000,
            Solution = new[] { "e2e4", "e7e5" },
            Themes = new[] { "middlegame", "fork" }
        }
    };

    private static PuzzleActivity CreateTestPuzzleActivity(bool win) => new()
    {
        Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        Win = win,
        Puzzle = new PuzzleActivityPuzzle
        {
            Id = "puzzle123",
            Rating = 1500,
            Plays = 100
        }
    };

    private static PuzzleDashboard CreateTestPuzzleDashboard(int days) => new()
    {
        Days = days,
        Global = new PuzzlePerformance
        {
            Count = 100,
            FirstWins = 80,
            ReplayWins = 15,
            PuzzleRatingAvg = 1600,
            Performance = 1700
        }
    };

    private static StormDashboard CreateTestStormDashboard() => new()
    {
        High = new StormHigh
        {
            AllTime = 50,
            Month = 45,
            Week = 40,
            Day = 35
        },
        Days = new List<StormDay>
        {
            new() { Id = "2023/6/15", Score = 35, Runs = 5, Time = 300 }
        }
    };

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }
        await Task.CompletedTask;
    }

    #endregion
}
