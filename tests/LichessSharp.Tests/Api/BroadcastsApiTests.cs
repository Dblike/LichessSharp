using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class BroadcastsApiTests
{
    private readonly BroadcastsApi _broadcastsApi;
    private readonly Mock<ILichessHttpClient> _httpClientMock;

    public BroadcastsApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _broadcastsApi = new BroadcastsApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new BroadcastsApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task StreamOfficialBroadcastsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var broadcasts = new List<BroadcastWithRounds>
        {
            CreateTestBroadcast("broadcast1"),
            CreateTestBroadcast("broadcast2")
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BroadcastWithRounds>("/api/broadcast", It.IsAny<CancellationToken>()))
            .Returns(broadcasts.ToAsyncEnumerable());

        // Act
        var result = new List<BroadcastWithRounds>();
        await foreach (var broadcast in _broadcastsApi.StreamOfficialBroadcastsAsync()) result.Add(broadcast);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task StreamOfficialBroadcastsAsync_WithNbParameter_IncludesInEndpoint()
    {
        // Arrange
        var broadcasts = new List<BroadcastWithRounds>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BroadcastWithRounds>(It.Is<string>(s => s.Contains("nb=10")),
                It.IsAny<CancellationToken>()))
            .Returns(broadcasts.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _broadcastsApi.StreamOfficialBroadcastsAsync(10))
        {
        }

        // Assert
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<BroadcastWithRounds>(It.Is<string>(s => s.Contains("nb=10")),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamOfficialBroadcastsAsync_WithHtmlParameter_IncludesInEndpoint()
    {
        // Arrange
        var broadcasts = new List<BroadcastWithRounds>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BroadcastWithRounds>(It.Is<string>(s => s.Contains("html=true")),
                It.IsAny<CancellationToken>()))
            .Returns(broadcasts.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _broadcastsApi.StreamOfficialBroadcastsAsync(html: true))
        {
        }

        // Assert
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<BroadcastWithRounds>(It.Is<string>(s => s.Contains("html=true")),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTopBroadcastsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResult = new BroadcastTopPage();
        _httpClientMock
            .Setup(x => x.GetAsync<BroadcastTopPage>("/api/broadcast/top", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.GetTopBroadcastsAsync();

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAsync<BroadcastTopPage>("/api/broadcast/top", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTopBroadcastsAsync_WithPage_IncludesPageParameter()
    {
        // Arrange
        var expectedResult = new BroadcastTopPage();
        _httpClientMock
            .Setup(x => x.GetAsync<BroadcastTopPage>("/api/broadcast/top?page=2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.GetTopBroadcastsAsync(2);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(
            x => x.GetAsync<BroadcastTopPage>("/api/broadcast/top?page=2", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamUserBroadcastsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "testuser";
        var broadcasts = new List<BroadcastByUser>
        {
            new() { Tour = CreateTestTour("tour1") }
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BroadcastByUser>($"/api/broadcast/by/{username}",
                It.IsAny<CancellationToken>()))
            .Returns(broadcasts.ToAsyncEnumerable());

        // Act
        var result = new List<BroadcastByUser>();
        await foreach (var broadcast in _broadcastsApi.StreamUserBroadcastsAsync(username)) result.Add(broadcast);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task StreamUserBroadcastsAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in _broadcastsApi.StreamUserBroadcastsAsync(null!))
            {
            }
        });
    }

    [Fact]
    public async Task SearchBroadcastsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var query = "chess";
        var expectedResult = new BroadcastSearchPage();
        _httpClientMock
            .Setup(x => x.GetAsync<BroadcastSearchPage>(It.Is<string>(s => s.Contains("/api/broadcast/search?q=chess")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.SearchBroadcastsAsync(query);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchBroadcastsAsync_WithPage_IncludesPageParameter()
    {
        // Arrange
        var expectedResult = new BroadcastSearchPage();
        _httpClientMock
            .Setup(x => x.GetAsync<BroadcastSearchPage>(It.Is<string>(s => s.Contains("page=3")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.SearchBroadcastsAsync("test", 3);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchBroadcastsAsync_WithNullQuery_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.SearchBroadcastsAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetTournamentAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var tournamentId = "tour123";
        var expectedResult = CreateTestBroadcast(tournamentId);
        _httpClientMock
            .Setup(x => x.GetAsync<BroadcastWithRounds>($"/api/broadcast/{tournamentId}",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.GetTournamentAsync(tournamentId);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(
            x => x.GetAsync<BroadcastWithRounds>($"/api/broadcast/{tournamentId}", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTournamentAsync_WithHtml_IncludesHtmlParameter()
    {
        // Arrange
        var tournamentId = "tour123";
        var expectedResult = CreateTestBroadcast(tournamentId);
        _httpClientMock
            .Setup(x => x.GetAsync<BroadcastWithRounds>($"/api/broadcast/{tournamentId}?html=1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.GetTournamentAsync(tournamentId, true);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTournamentAsync_WithNullId_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.GetTournamentAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetRoundAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var tournamentSlug = "test-tournament";
        var roundSlug = "round-1";
        var roundId = "round123";
        var expectedResult = CreateTestRound(roundId);
        _httpClientMock
            .Setup(x => x.GetAsync<BroadcastRound>($"/api/broadcast/{tournamentSlug}/{roundSlug}/{roundId}",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.GetRoundAsync(tournamentSlug, roundSlug, roundId);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetRoundAsync_WithNullTournamentSlug_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.GetRoundAsync(null!, "round-slug", "round123");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetRoundAsync_WithNullRoundSlug_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.GetRoundAsync("tour-slug", null!, "round123");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetRoundAsync_WithNullRoundId_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.GetRoundAsync("tour-slug", "round-slug", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task StreamMyRoundsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var rounds = new List<BroadcastMyRound>
        {
            CreateTestMyRound("round1")
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BroadcastMyRound>("/api/broadcast/my-rounds",
                It.IsAny<CancellationToken>()))
            .Returns(rounds.ToAsyncEnumerable());

        // Act
        var result = new List<BroadcastMyRound>();
        await foreach (var round in _broadcastsApi.StreamMyRoundsAsync()) result.Add(round);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task StreamMyRoundsAsync_WithNb_IncludesNbParameter()
    {
        // Arrange
        var rounds = new List<BroadcastMyRound>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BroadcastMyRound>("/api/broadcast/my-rounds?nb=5",
                It.IsAny<CancellationToken>()))
            .Returns(rounds.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _broadcastsApi.StreamMyRoundsAsync(5))
        {
        }

        // Assert
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<BroadcastMyRound>("/api/broadcast/my-rounds?nb=5", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateTournamentAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var options = new BroadcastTournamentOptions { Name = "Test Tournament" };
        var expectedResult = CreateTestBroadcast("tour123");
        _httpClientMock
            .Setup(x => x.PostAsync<BroadcastWithRounds>("/broadcast/new", It.IsAny<FormUrlEncodedContent>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.CreateTournamentAsync(options);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(
            x => x.PostAsync<BroadcastWithRounds>("/broadcast/new", It.IsAny<FormUrlEncodedContent>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTournamentAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _broadcastsApi.CreateTournamentAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateTournamentAsync_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var options = new BroadcastTournamentOptions { Name = null! };

        // Act
        var act = () => _broadcastsApi.CreateTournamentAsync(options);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateTournamentAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var tournamentId = "tour123";
        var options = new BroadcastTournamentOptions { Name = "Updated Tournament" };
        var expectedResult = CreateTestBroadcast(tournamentId);
        _httpClientMock
            .Setup(x => x.PostAsync<BroadcastWithRounds>($"/broadcast/{tournamentId}/edit",
                It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.UpdateTournamentAsync(tournamentId, options);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTournamentAsync_WithNullTournamentId_ThrowsArgumentException()
    {
        // Arrange
        var options = new BroadcastTournamentOptions { Name = "Test" };

        // Act
        var act = () => _broadcastsApi.UpdateTournamentAsync(null!, options);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateRoundAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var tournamentId = "tour123";
        var options = new BroadcastRoundOptions { Name = "Round 1" };
        var expectedResult = CreateTestRoundNew("round1");
        _httpClientMock
            .Setup(x => x.PostAsync<BroadcastRoundNew>($"/broadcast/{tournamentId}/new",
                It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.CreateRoundAsync(tournamentId, options);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateRoundAsync_WithNullTournamentId_ThrowsArgumentException()
    {
        // Arrange
        var options = new BroadcastRoundOptions { Name = "Round 1" };

        // Act
        var act = () => _broadcastsApi.CreateRoundAsync(null!, options);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateRoundAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var roundId = "round123";
        var options = new BroadcastRoundOptions { Name = "Updated Round" };
        var expectedResult = CreateTestRoundNew(roundId);
        _httpClientMock
            .Setup(x => x.PostAsync<BroadcastRoundNew>($"/broadcast/round/{roundId}/edit",
                It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.UpdateRoundAsync(roundId, options);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateRoundAsync_WithNullRoundId_ThrowsArgumentException()
    {
        // Arrange
        var options = new BroadcastRoundOptions { Name = "Round 1" };

        // Act
        var act = () => _broadcastsApi.UpdateRoundAsync(null!, options);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ResetRoundAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var roundId = "round123";
        _httpClientMock
            .Setup(x => x.PostNoContentAsync($"/api/broadcast/round/{roundId}/reset", null,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _broadcastsApi.ResetRoundAsync(roundId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostNoContentAsync($"/api/broadcast/round/{roundId}/reset", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ResetRoundAsync_WithNullRoundId_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.ResetRoundAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task PushPgnAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var roundId = "round123";
        var pgn = "1. e4 e5 *";
        var expectedResult = new BroadcastPgnPushResult();
        _httpClientMock
            .Setup(x => x.PostPlainTextAsync<BroadcastPgnPushResult>($"/api/broadcast/round/{roundId}/push", pgn,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _broadcastsApi.PushPgnAsync(roundId, pgn);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(
            x => x.PostPlainTextAsync<BroadcastPgnPushResult>($"/api/broadcast/round/{roundId}/push", pgn,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PushPgnAsync_WithNullRoundId_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.PushPgnAsync(null!, "1. e4 e5 *");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task PushPgnAsync_WithNullPgn_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.PushPgnAsync("round123", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportRoundPgnAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var roundId = "round123";
        var expectedPgn = "[Event \"Test\"]\n1. e4 e5 *";
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync($"/api/broadcast/round/{roundId}.pgn", "application/x-chess-pgn",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = await _broadcastsApi.ExportRoundPgnAsync(roundId);

        // Assert
        result.Should().Be(expectedPgn);
    }

    [Fact]
    public async Task ExportRoundPgnAsync_WithNullRoundId_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.ExportRoundPgnAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportAllRoundsPgnAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var tournamentId = "tour123";
        var expectedPgn = "[Event \"Test\"]\n1. e4 e5 *";
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync($"/api/broadcast/{tournamentId}.pgn", "application/x-chess-pgn",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = await _broadcastsApi.ExportAllRoundsPgnAsync(tournamentId);

        // Assert
        result.Should().Be(expectedPgn);
    }

    [Fact]
    public async Task ExportAllRoundsPgnAsync_WithNullTournamentId_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.ExportAllRoundsPgnAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetPlayersAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var tournamentId = "tour123";
        var players = new List<BroadcastPlayerEntry>
        {
            new() { Name = "Player 1", Rating = 2700 },
            new() { Name = "Player 2", Rating = 2650 }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<List<BroadcastPlayerEntry>>($"/broadcast/{tournamentId}/players",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(players);

        // Act
        var result = await _broadcastsApi.GetPlayersAsync(tournamentId);

        // Assert
        result.Should().HaveCount(2);
        _httpClientMock.Verify(
            x => x.GetAsync<List<BroadcastPlayerEntry>>($"/broadcast/{tournamentId}/players",
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPlayersAsync_WithNullTournamentId_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.GetPlayersAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetPlayerAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var tournamentId = "tour123";
        var playerId = "12345";
        var player = new BroadcastPlayerWithGames
        {
            Name = "Test Player",
            Rating = 2700,
            Games = new List<BroadcastPlayerGame>()
        };
        _httpClientMock
            .Setup(x => x.GetAsync<BroadcastPlayerWithGames>($"/broadcast/{tournamentId}/players/{playerId}",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var result = await _broadcastsApi.GetPlayerAsync(tournamentId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Player");
        _httpClientMock.Verify(
            x => x.GetAsync<BroadcastPlayerWithGames>($"/broadcast/{tournamentId}/players/{playerId}",
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPlayerAsync_WithNullTournamentId_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.GetPlayerAsync(null!, "player1");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetPlayerAsync_WithNullPlayerId_ThrowsArgumentException()
    {
        // Act
        var act = () => _broadcastsApi.GetPlayerAsync("tour123", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task StreamRoundPgnAsync_CallsCorrectEndpoint()
    {
        // Arrange
        const string roundId = "round123";
        const string expectedPgn = "[Event \"Test\"]\n1. e4 e5 *";
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync($"/api/stream/broadcast/round/{roundId}.pgn",
                "application/x-chess-pgn", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = new List<string>();
        await foreach (var pgn in _broadcastsApi.StreamRoundPgnAsync(roundId)) result.Add(pgn);

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be(expectedPgn);
    }

    [Fact]
    public async Task StreamRoundPgnAsync_WithNullRoundId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in _broadcastsApi.StreamRoundPgnAsync(null!))
            {
            }
        });
    }

    [Fact]
    public async Task GetTopBroadcastsAsync_WithCancellationToken_PassesToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _httpClientMock
            .Setup(x => x.GetAsync<BroadcastTopPage>(It.IsAny<string>(), cts.Token))
            .ReturnsAsync(new BroadcastTopPage());

        // Act
        await _broadcastsApi.GetTopBroadcastsAsync(cancellationToken: cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<BroadcastTopPage>(It.IsAny<string>(), cts.Token), Times.Once);
    }

    private static BroadcastTour CreateTestTour(string id)
    {
        return new BroadcastTour
        {
            Id = id,
            Name = $"Test Tournament {id}",
            Slug = $"test-tournament-{id}",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    private static BroadcastRoundInfo CreateTestRoundInfo(string id)
    {
        return new BroadcastRoundInfo
        {
            Id = id,
            Name = $"Round {id}",
            Slug = $"round-{id}",
            Url = $"https://lichess.org/broadcast/test/round-{id}/{id}"
        };
    }

    private static BroadcastRoundStudyInfo CreateTestStudyInfo()
    {
        return new BroadcastRoundStudyInfo
        {
            Writeable = false,
            Features = new BroadcastStudyFeatures
            {
                Chat = true,
                Computer = true,
                Explorer = true
            }
        };
    }

    private static BroadcastWithRounds CreateTestBroadcast(string id)
    {
        return new BroadcastWithRounds
        {
            Tour = CreateTestTour(id),
            Rounds = new List<BroadcastRoundInfo>
            {
                CreateTestRoundInfo("round1")
            }
        };
    }

    private static BroadcastRound CreateTestRound(string id)
    {
        return new BroadcastRound
        {
            Round = CreateTestRoundInfo(id),
            Tour = CreateTestTour("tour1"),
            Study = CreateTestStudyInfo(),
            Games = new List<BroadcastRoundGame>
            {
                new() { Id = "game1", Name = "Player1 - Player2" }
            }
        };
    }

    private static BroadcastRoundNew CreateTestRoundNew(string id)
    {
        return new BroadcastRoundNew
        {
            Round = CreateTestRoundInfo(id),
            Tour = CreateTestTour("tour1"),
            Study = CreateTestStudyInfo()
        };
    }

    private static BroadcastMyRound CreateTestMyRound(string id)
    {
        return new BroadcastMyRound
        {
            Round = CreateTestRoundInfo(id),
            Tour = CreateTestTour("tour1"),
            Study = CreateTestStudyInfo()
        };
    }
}