using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models.Common;
using LichessSharp.Models.Games;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class ArenaTournamentsApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly ArenaTournamentsApi _api;

    public ArenaTournamentsApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _api = new ArenaTournamentsApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        var act = () => new ArenaTournamentsApi(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task GetCurrentAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResult = new ArenaTournamentList
        {
            Created = [CreateTestTournamentSummary("tour1")],
            Started = [CreateTestTournamentSummary("tour2")],
            Finished = []
        };
        _httpClientMock
            .Setup(x => x.GetAsync<ArenaTournamentList>("/api/tournament", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.GetCurrentAsync();

        // Assert
        result.Should().NotBeNull();
        result.Created.Should().HaveCount(1);
        result.Started.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.GetAsync<ArenaTournamentList>("/api/tournament", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithDefaultPage_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testTour";
        var expectedResult = CreateTestTournament(id);
        _httpClientMock
            .Setup(x => x.GetAsync<ArenaTournament>($"/api/tournament/{id}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.GetAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        _httpClientMock.Verify(x => x.GetAsync<ArenaTournament>($"/api/tournament/{id}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithPage_IncludesPageParameter()
    {
        // Arrange
        var id = "testTour";
        var expectedResult = CreateTestTournament(id);
        _httpClientMock
            .Setup(x => x.GetAsync<ArenaTournament>($"/api/tournament/{id}?page=2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.GetAsync(id, page: 2);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAsync<ArenaTournament>($"/api/tournament/{id}?page=2", It.IsAny<CancellationToken>()), Times.Once);
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
    public async Task CreateAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var options = new ArenaCreateOptions
        {
            ClockTime = 3,
            ClockIncrement = 0,
            Minutes = 45
        };
        var expectedResult = CreateTestTournament("newTour");
        _httpClientMock
            .Setup(x => x.PostAsync<ArenaTournament>("/api/tournament", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.CreateAsync(options);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.PostAsync<ArenaTournament>("/api/tournament", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.CreateAsync(null!));
    }

    [Fact]
    public async Task UpdateAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testTour";
        var options = new ArenaUpdateOptions { Name = "Updated Name" };
        var expectedResult = CreateTestTournament(id);
        _httpClientMock
            .Setup(x => x.PostAsync<ArenaTournament>($"/api/tournament/{id}", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.UpdateAsync(id, options);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.PostAsync<ArenaTournament>($"/api/tournament/{id}", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.UpdateAsync(null!, new ArenaUpdateOptions()));
    }

    [Fact]
    public async Task UpdateAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.UpdateAsync("testTour", null!));
    }

    [Fact]
    public async Task JoinAsync_WithNoOptions_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testTour";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/tournament/{id}/join", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _api.JoinAsync(id);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/tournament/{id}/join", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinAsync_WithPassword_IncludesPasswordInBody()
    {
        // Arrange
        var id = "testTour";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/tournament/{id}/join", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _api.JoinAsync(id, password: "secret");

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/tournament/{id}/join", It.Is<HttpContent>(c => c != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.JoinAsync(null!));
    }

    [Fact]
    public async Task WithdrawAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testTour";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/tournament/{id}/withdraw", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _api.PauseOrWithdrawAsync(id);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/tournament/{id}/withdraw", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.PauseOrWithdrawAsync(null!));
    }

    [Fact]
    public async Task TerminateAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testTour";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/tournament/{id}/terminate", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _api.TerminateAsync(id);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/tournament/{id}/terminate", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TerminateAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.TerminateAsync(null!));
    }

    [Fact]
    public async Task UpdateTeamBattleAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testTour";
        var teams = "team1,team2,team3";
        var expectedResult = CreateTestTournament(id);
        _httpClientMock
            .Setup(x => x.PostAsync<ArenaTournament>($"/api/tournament/team-battle/{id}", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.UpdateTeamBattleAsync(id, teams);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.PostAsync<ArenaTournament>($"/api/tournament/team-battle/{id}", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTeamBattleAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.UpdateTeamBattleAsync(null!, "team1"));
    }

    [Fact]
    public async Task UpdateTeamBattleAsync_WithNullTeams_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.UpdateTeamBattleAsync("testTour", null!));
    }

    [Fact]
    public async Task StreamGamesAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testTour";
        var games = new List<GameJson> { new() { Id = "game1" } };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>($"/api/tournament/{id}/games", It.IsAny<CancellationToken>()))
            .Returns(games.ToAsyncEnumerable());

        // Act
        var results = new List<GameJson>();
        await foreach (var game in _api.StreamGamesAsync(id))
        {
            results.Add(game);
        }

        // Assert
        results.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>($"/api/tournament/{id}/games", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamGamesAsync_WithOptions_IncludesQueryParameters()
    {
        // Arrange
        var id = "testTour";
        var options = new ArenaGamesExportOptions { Moves = true, Clocks = true };
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

    [Fact]
    public async Task StreamResultsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testTour";
        var results = new List<ArenaPlayerResult> { new() { Username = "player1", Rank = 1 } };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<ArenaPlayerResult>($"/api/tournament/{id}/results", It.IsAny<CancellationToken>()))
            .Returns(results.ToAsyncEnumerable());

        // Act
        var actualResults = new List<ArenaPlayerResult>();
        await foreach (var result in _api.StreamResultsAsync(id))
        {
            actualResults.Add(result);
        }

        // Assert
        actualResults.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<ArenaPlayerResult>($"/api/tournament/{id}/results", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamResultsAsync_WithNb_IncludesNbParameter()
    {
        // Arrange
        var id = "testTour";
        var results = new List<ArenaPlayerResult>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<ArenaPlayerResult>($"/api/tournament/{id}/results?nb=10", It.IsAny<CancellationToken>()))
            .Returns(results.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _api.StreamResultsAsync(id, nb: 10)) { }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<ArenaPlayerResult>($"/api/tournament/{id}/results?nb=10", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamResultsAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _api.StreamResultsAsync(null!)) { }
        });
    }

    [Fact]
    public async Task GetTeamStandingAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var id = "testTour";
        var expectedResult = new ArenaTeamStanding { Id = id };
        _httpClientMock
            .Setup(x => x.GetAsync<ArenaTeamStanding>($"/api/tournament/{id}/teams", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _api.GetTeamStandingAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        _httpClientMock.Verify(x => x.GetAsync<ArenaTeamStanding>($"/api/tournament/{id}/teams", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTeamStandingAsync_WithNullId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _api.GetTeamStandingAsync(null!));
    }

    [Fact]
    public async Task StreamCreatedByAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "testuser";
        var tournaments = new List<ArenaTournamentSummary> { CreateTestTournamentSummary("tour1") };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<ArenaTournamentSummary>($"/api/user/{username}/tournament/created", It.IsAny<CancellationToken>()))
            .Returns(tournaments.ToAsyncEnumerable());

        // Act
        var results = new List<ArenaTournamentSummary>();
        await foreach (var tournament in _api.StreamCreatedByAsync(username))
        {
            results.Add(tournament);
        }

        // Assert
        results.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<ArenaTournamentSummary>($"/api/user/{username}/tournament/created", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamCreatedByAsync_WithStatus_IncludesStatusParameter()
    {
        // Arrange
        var username = "testuser";
        var tournaments = new List<ArenaTournamentSummary>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<ArenaTournamentSummary>($"/api/user/{username}/tournament/created?status=20", It.IsAny<CancellationToken>()))
            .Returns(tournaments.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _api.StreamCreatedByAsync(username, ArenaStatusFilter.Started)) { }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<ArenaTournamentSummary>($"/api/user/{username}/tournament/created?status=20", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamCreatedByAsync_WithNullUsername_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _api.StreamCreatedByAsync(null!)) { }
        });
    }

    [Fact]
    public async Task StreamPlayedByAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "testuser";
        var tournaments = new List<ArenaPlayedTournament> { new() { Tournament = CreateTestTournamentSummary("tour1") } };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<ArenaPlayedTournament>($"/api/user/{username}/tournament/played", It.IsAny<CancellationToken>()))
            .Returns(tournaments.ToAsyncEnumerable());

        // Act
        var results = new List<ArenaPlayedTournament>();
        await foreach (var tournament in _api.StreamPlayedByAsync(username))
        {
            results.Add(tournament);
        }

        // Assert
        results.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<ArenaPlayedTournament>($"/api/user/{username}/tournament/played", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamPlayedByAsync_WithNullUsername_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _api.StreamPlayedByAsync(null!)) { }
        });
    }

    [Fact]
    public async Task StreamTeamTournamentsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "testteam";
        var tournaments = new List<ArenaTournamentSummary> { CreateTestTournamentSummary("tour1") };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<ArenaTournamentSummary>($"/api/team/{teamId}/arena", It.IsAny<CancellationToken>()))
            .Returns(tournaments.ToAsyncEnumerable());

        // Act
        var results = new List<ArenaTournamentSummary>();
        await foreach (var tournament in _api.StreamTeamTournamentsAsync(teamId))
        {
            results.Add(tournament);
        }

        // Assert
        results.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<ArenaTournamentSummary>($"/api/team/{teamId}/arena", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamTeamTournamentsAsync_WithMax_IncludesMaxParameter()
    {
        // Arrange
        var teamId = "testteam";
        var tournaments = new List<ArenaTournamentSummary>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<ArenaTournamentSummary>($"/api/team/{teamId}/arena?max=50", It.IsAny<CancellationToken>()))
            .Returns(tournaments.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _api.StreamTeamTournamentsAsync(teamId, max: 50)) { }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<ArenaTournamentSummary>($"/api/team/{teamId}/arena?max=50", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamTeamTournamentsAsync_WithNullTeamId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _api.StreamTeamTournamentsAsync(null!)) { }
        });
    }

    private static ArenaTournamentSummary CreateTestTournamentSummary(string id) => new()
    {
        Id = id,
        CreatedBy = "testuser",
        FullName = "Test Tournament",
        Minutes = 60,
        NbPlayers = 10,
        Rated = true,
        Status = 20,
        StartsAt = DateTimeOffset.UtcNow,
        FinishesAt = DateTimeOffset.UtcNow.AddHours(1)
    };

    private static ArenaTournament CreateTestTournament(string id) => new()
    {
        Id = id,
        CreatedBy = "testuser",
        FullName = "Test Tournament",
        Minutes = 60,
        NbPlayers = 10,
        Rated = true,
        Status = 20,
        StartsAt = DateTimeOffset.UtcNow,
        FinishesAt = DateTimeOffset.UtcNow.AddHours(1)
    };

}
