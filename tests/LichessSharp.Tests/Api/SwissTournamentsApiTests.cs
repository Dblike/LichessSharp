using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Http;
using LichessSharp.Models;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class SwissTournamentsApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly SwissTournamentsApi _api;

    public SwissTournamentsApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _api = new SwissTournamentsApi(_httpClientMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        var act = () => new SwissTournamentsApi(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testSwiss";
        var expectedResult = CreateTestTournament(id);
        _httpClientMock
            .Setup(x => x.GetAsync<SwissTournament>($"/api/swiss/{id}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.GetAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        _httpClientMock.Verify(x => x.GetAsync<SwissTournament>($"/api/swiss/{id}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.GetAsync(null!));
    }

    [Fact]
    public async Task GetAsync_WithEmptyId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _api.GetAsync(""));
    }

    [Fact]
    public async Task GetAsync_UrlEncodesId()
    {
        // Arrange
        var id = "swiss with spaces";
        var expectedResult = CreateTestTournament(id);
        _httpClientMock
            .Setup(x => x.GetAsync<SwissTournament>(It.Is<string>(s => s.Contains("swiss%20with%20spaces")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        await _api.GetAsync(id);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<SwissTournament>(It.Is<string>(s => s.Contains("swiss%20with%20spaces")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "testteam";
        var options = new SwissCreateOptions
        {
            Name = "Test Swiss",
            ClockLimit = 300,
            ClockIncrement = 3,
            NbRounds = 5
        };
        var expectedResult = CreateTestTournament("newSwiss");
        _httpClientMock
            .Setup(x => x.PostAsync<SwissTournament>($"/api/swiss/new/{teamId}", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.CreateAsync(teamId, options);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.PostAsync<SwissTournament>($"/api/swiss/new/{teamId}", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullTeamId_ThrowsArgumentException()
    {
        var options = new SwissCreateOptions { Name = "Test", ClockLimit = 300, ClockIncrement = 0, NbRounds = 5 };
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.CreateAsync(null!, options));
    }

    [Fact]
    public async Task CreateAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.CreateAsync("testteam", null!));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testSwiss";
        var options = new SwissUpdateOptions { Name = "Updated Name" };
        var expectedResult = CreateTestTournament(id);
        _httpClientMock
            .Setup(x => x.PostAsync<SwissTournament>($"/api/swiss/{id}/edit", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.UpdateAsync(id, options);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.PostAsync<SwissTournament>($"/api/swiss/{id}/edit", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.UpdateAsync(null!, new SwissUpdateOptions()));
    }

    [Fact]
    public async Task UpdateAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.UpdateAsync("testSwiss", null!));
    }

    #endregion

    #region ScheduleNextRoundAsync Tests

    [Fact]
    public async Task ScheduleNextRoundAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testSwiss";
        var date = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeMilliseconds();
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/schedule-next-round", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _api.ScheduleNextRoundAsync(id, date);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/schedule-next-round", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ScheduleNextRoundAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.ScheduleNextRoundAsync(null!, 12345));
    }

    #endregion

    #region JoinAsync Tests

    [Fact]
    public async Task JoinAsync_WithNoPassword_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testSwiss";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/join", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _api.JoinAsync(id);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/join", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinAsync_WithPassword_IncludesPasswordInBody()
    {
        // Arrange
        var id = "testSwiss";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/join", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _api.JoinAsync(id, password: "secret");

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/join", It.Is<HttpContent>(c => c != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.JoinAsync(null!));
    }

    #endregion

    #region WithdrawAsync Tests

    [Fact]
    public async Task WithdrawAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testSwiss";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/withdraw", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _api.WithdrawAsync(id);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/withdraw", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.WithdrawAsync(null!));
    }

    #endregion

    #region TerminateAsync Tests

    [Fact]
    public async Task TerminateAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testSwiss";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/terminate", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _api.TerminateAsync(id);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/swiss/{id}/terminate", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TerminateAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.TerminateAsync(null!));
    }

    #endregion

    #region ExportTrfAsync Tests

    [Fact]
    public async Task ExportTrfAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testSwiss";
        var expectedTrf = "TRF data here";
        _httpClientMock
            .Setup(x => x.GetStringAsync($"/swiss/{id}.trf", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTrf);

        // Act
        var result = await _api.ExportTrfAsync(id);

        // Assert
        result.Should().Be(expectedTrf);
        _httpClientMock.Verify(x => x.GetStringAsync($"/swiss/{id}.trf", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExportTrfAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.ExportTrfAsync(null!));
    }

    #endregion

    #region StreamGamesAsync Tests

    [Fact]
    public async Task StreamGamesAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testSwiss";
        var games = new List<GameJson> { new() { Id = "game1" } };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>($"/api/swiss/{id}/games", It.IsAny<CancellationToken>()))
            .Returns(games.ToAsyncEnumerable());

        // Act
        var results = new List<GameJson>();
        await foreach (var game in _api.StreamGamesAsync(id))
        {
            results.Add(game);
        }

        // Assert
        results.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>($"/api/swiss/{id}/games", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamGamesAsync_WithOptions_IncludesQueryParameters()
    {
        // Arrange
        var id = "testSwiss";
        var options = new SwissGamesExportOptions { Moves = true, Clocks = true };
        var games = new List<GameJson>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s => s.Contains("moves=true") && s.Contains("clocks=true")), It.IsAny<CancellationToken>()))
            .Returns(games.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _api.StreamGamesAsync(id, options)) { }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s => s.Contains("moves=true") && s.Contains("clocks=true")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamGamesAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _api.StreamGamesAsync(null!)) { }
        });
    }

    #endregion

    #region StreamResultsAsync Tests

    [Fact]
    public async Task StreamResultsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testSwiss";
        var results = new List<SwissPlayerResult> { new() { Username = "player1", Rank = 1 } };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<SwissPlayerResult>($"/api/swiss/{id}/results", It.IsAny<CancellationToken>()))
            .Returns(results.ToAsyncEnumerable());

        // Act
        var actualResults = new List<SwissPlayerResult>();
        await foreach (var result in _api.StreamResultsAsync(id))
        {
            actualResults.Add(result);
        }

        // Assert
        actualResults.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<SwissPlayerResult>($"/api/swiss/{id}/results", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamResultsAsync_WithNb_IncludesNbParameter()
    {
        // Arrange
        var id = "testSwiss";
        var results = new List<SwissPlayerResult>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<SwissPlayerResult>($"/api/swiss/{id}/results?nb=10", It.IsAny<CancellationToken>()))
            .Returns(results.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _api.StreamResultsAsync(id, nb: 10)) { }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<SwissPlayerResult>($"/api/swiss/{id}/results?nb=10", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamResultsAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _api.StreamResultsAsync(null!)) { }
        });
    }

    #endregion

    #region StreamTeamTournamentsAsync Tests

    [Fact]
    public async Task StreamTeamTournamentsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "testteam";
        var tournaments = new List<SwissTournament> { CreateTestTournament("swiss1") };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<SwissTournament>($"/api/team/{teamId}/swiss", It.IsAny<CancellationToken>()))
            .Returns(tournaments.ToAsyncEnumerable());

        // Act
        var results = new List<SwissTournament>();
        await foreach (var tournament in _api.StreamTeamTournamentsAsync(teamId))
        {
            results.Add(tournament);
        }

        // Assert
        results.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<SwissTournament>($"/api/team/{teamId}/swiss", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamTeamTournamentsAsync_WithMax_IncludesMaxParameter()
    {
        // Arrange
        var teamId = "testteam";
        var tournaments = new List<SwissTournament>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<SwissTournament>($"/api/team/{teamId}/swiss?max=50", It.IsAny<CancellationToken>()))
            .Returns(tournaments.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _api.StreamTeamTournamentsAsync(teamId, max: 50)) { }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<SwissTournament>($"/api/team/{teamId}/swiss?max=50", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamTeamTournamentsAsync_WithNullTeamId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _api.StreamTeamTournamentsAsync(null!)) { }
        });
    }

    #endregion

    #region Helper Methods

    private static SwissTournament CreateTestTournament(string id) => new()
    {
        Id = id,
        CreatedBy = "testuser",
        Name = "Test Swiss",
        Variant = "standard",
        Round = 1,
        NbRounds = 5,
        NbPlayers = 10,
        NbOngoing = 5,
        Status = "started",
        Rated = true,
        StartsAt = DateTimeOffset.UtcNow.ToString("O")
    };

    #endregion
}
