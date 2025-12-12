using FluentAssertions;
using LichessSharp.Api.Contracts;
using LichessSharp.Models.Games;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Dedicated integration tests for the Swiss Tournaments API.
///     These tests make real HTTP calls to Lichess.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class SwissTournamentsApiIntegrationTests : IntegrationTestBase
{
    // Known team that hosts Swiss tournaments
    private const string TestTeamId = "lichess-swiss";

    [Fact]
    public async Task GetAsync_WithValidId_ReturnsTournament()
    {
        // First find a valid Swiss tournament
        SwissTournament? tournament = null;
        await foreach (var t in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 1))
        {
            tournament = t;
            break;
        }

        if (tournament == null) return; // Skip if no tournaments available

        // Act
        var result = await Client.SwissTournaments.GetAsync(tournament.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(tournament.Id);
        result.Name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetAsync_TournamentHasValidStructure()
    {
        // Find a tournament
        SwissTournament? tournament = null;
        await foreach (var t in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 1))
        {
            tournament = t;
            break;
        }

        if (tournament == null) return;

        // Act
        var result = await Client.SwissTournaments.GetAsync(tournament.Id);

        // Assert
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Name.Should().NotBeNullOrWhiteSpace();
        result.Status.Should().NotBeNullOrWhiteSpace();
        result.NbRounds.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task StreamTeamTournamentsAsync_WithKnownTeam_ReturnsTournaments()
    {
        // Act
        var tournaments = new List<SwissTournament>();
        await foreach (var t in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 5))
        {
            tournaments.Add(t);
            if (tournaments.Count >= 5) break;
        }

        // Assert
        tournaments.Should().NotBeEmpty("lichess-swiss team should have tournaments");
        tournaments.Should().AllSatisfy(t =>
        {
            t.Id.Should().NotBeNullOrWhiteSpace();
            t.Name.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task StreamTeamTournamentsAsync_TournamentsHaveValidStatus()
    {
        // Act
        await foreach (var tournament in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 10))
        {
            // Assert - Status should be one of the valid values
            tournament.Status.Should().BeOneOf("created", "started", "finished");
            return; // Just check first
        }
    }

    [Fact]
    public async Task StreamResultsAsync_WithFinishedTournament_ReturnsResults()
    {
        // Find a finished tournament
        SwissTournament? finishedTournament = null;
        await foreach (var t in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 20))
            if (t.Status == "finished")
            {
                finishedTournament = t;
                break;
            }

        if (finishedTournament == null) return;

        // Act
        var results = new List<SwissPlayerResult>();
        await foreach (var result in Client.SwissTournaments.StreamResultsAsync(finishedTournament.Id, 10))
        {
            results.Add(result);
            if (results.Count >= 10) break;
        }

        // Assert
        results.Should().NotBeEmpty("Finished tournaments should have results");
        results.Should().AllSatisfy(r =>
        {
            r.Username.Should().NotBeNullOrWhiteSpace();
            r.Rank.Should().BeGreaterThan(0);
        });
    }

    [Fact]
    public async Task StreamResultsAsync_ResultsAreOrderedByRank()
    {
        // Find a finished tournament
        SwissTournament? finishedTournament = null;
        await foreach (var t in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 20))
            if (t.Status == "finished")
            {
                finishedTournament = t;
                break;
            }

        if (finishedTournament == null) return;

        // Act
        var results = new List<SwissPlayerResult>();
        await foreach (var result in Client.SwissTournaments.StreamResultsAsync(finishedTournament.Id, 10))
        {
            results.Add(result);
            if (results.Count >= 10) break;
        }

        // Assert - Results should be ordered by rank
        if (results.Count > 1)
            for (var i = 1; i < results.Count; i++)
                results[i].Rank.Should().BeGreaterThanOrEqualTo(results[i - 1].Rank);
    }

    [Fact]
    public async Task StreamGamesAsync_WithFinishedTournament_ReturnsGames()
    {
        // Find a finished tournament
        SwissTournament? finishedTournament = null;
        await foreach (var t in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 20))
            if (t.Status == "finished" && t.NbOngoing == 0)
            {
                finishedTournament = t;
                break;
            }

        if (finishedTournament == null) return;

        // Act
        var games = new List<GameJson>();
        await foreach (var game in Client.SwissTournaments.StreamGamesAsync(finishedTournament.Id))
        {
            games.Add(game);
            if (games.Count >= 5) break;
        }

        // Assert
        games.Should().NotBeEmpty("Finished tournaments should have games");
        games.Should().AllSatisfy(g => { g.Id.Should().NotBeNullOrEmpty(); });
    }

    [Fact]
    public async Task ExportTrfAsync_WithFinishedTournament_ReturnsTrf()
    {
        // Find a finished tournament
        SwissTournament? finishedTournament = null;
        await foreach (var t in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 20))
            if (t.Status == "finished")
            {
                finishedTournament = t;
                break;
            }

        if (finishedTournament == null) return;

        // Act
        var trf = await Client.SwissTournaments.ExportTrfAsync(finishedTournament.Id);

        // Assert
        trf.Should().NotBeNullOrWhiteSpace("Finished tournaments should have TRF export");
        // TRF format starts with tournament info
        trf.Should().Contain("012", "TRF should contain tournament name line (012)");
    }

    [Fact]
    public async Task CreateAsync_WithoutAuth_ThrowsException()
    {
        // Arrange
        var options = new SwissCreateOptions
        {
            Name = "Test Swiss Tournament",
            ClockLimit = 300,
            ClockIncrement = 3,
            NbRounds = 5
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.SwissTournaments.CreateAsync(TestTeamId, options));
    }

    [Fact]
    public async Task JoinAsync_WithoutAuth_ThrowsException()
    {
        // Find a created (not started) tournament
        SwissTournament? createdTournament = null;
        await foreach (var t in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 20))
            if (t.Status == "created")
            {
                createdTournament = t;
                break;
            }

        if (createdTournament == null) return;

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.SwissTournaments.JoinAsync(createdTournament.Id));
    }

    [Fact]
    public async Task WithdrawAsync_WithoutAuth_ThrowsException()
    {
        // Find any tournament
        SwissTournament? tournament = null;
        await foreach (var t in Client.SwissTournaments.StreamTeamTournamentsAsync(TestTeamId, 1))
        {
            tournament = t;
            break;
        }

        if (tournament == null) return;

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.SwissTournaments.PauseOrWithdrawAsync(tournament.Id));
    }
}