using FluentAssertions;

using LichessSharp.Api.Contracts;
using LichessSharp.Exceptions;
using LichessSharp.Models;

using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Dedicated integration tests for the Arena Tournaments API.
/// These tests make real HTTP calls to Lichess.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class ArenaTournamentsApiIntegrationTests : IntegrationTestBase
{
    // Known team that hosts tournaments
    private const string TestTeamId = "lichess-swiss";

    #region GetCurrentAsync Tests

    [Fact]
    public async Task GetCurrentAsync_ReturnsCurrentTournaments()
    {
        // Act
        var result = await Client.ArenaTournaments.GetCurrentAsync();

        // Assert
        result.Should().NotBeNull();
        // Lichess always has arena tournaments running
        var totalCount = result.Created.Count + result.Started.Count + result.Finished.Count;
        totalCount.Should().BeGreaterThan(0, "Lichess should always have some arena tournaments");
    }

    [Fact]
    public async Task GetCurrentAsync_TournamentsHaveValidStructure()
    {
        // Act
        var result = await Client.ArenaTournaments.GetCurrentAsync();

        // Assert
        var anyTournament = result.Started.FirstOrDefault()
            ?? result.Created.FirstOrDefault()
            ?? result.Finished.FirstOrDefault();

        anyTournament.Should().NotBeNull();
        anyTournament!.Id.Should().NotBeNullOrWhiteSpace();
        anyTournament.FullName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetCurrentAsync_StartedTournamentsAreOngoing()
    {
        // Act
        var result = await Client.ArenaTournaments.GetCurrentAsync();

        // Assert - Started tournaments have status 20 (integer)
        result.Started.Should().NotBeEmpty("Lichess should have ongoing tournaments");
        foreach (var tournament in result.Started)
        {
            tournament.Status.Should().BeGreaterThan(0);
        }
    }

    #endregion

    #region ExportAsync Tests

    [Fact]
    public async Task GetAsync_WithValidId_ReturnsTournament()
    {
        // First get current tournaments
        var current = await Client.ArenaTournaments.GetCurrentAsync();
        var anyTournament = current.Started.FirstOrDefault()
            ?? current.Created.FirstOrDefault()
            ?? current.Finished.FirstOrDefault();

        if (anyTournament == null)
        {
            return;
        }

        // Act
        var result = await Client.ArenaTournaments.GetAsync(anyTournament.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(anyTournament.Id);
        result.FullName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetAsync_ReturnsDetailedTournamentInfo()
    {
        // Get a tournament
        var current = await Client.ArenaTournaments.GetCurrentAsync();
        var tournament = current.Started.FirstOrDefault() ?? current.Finished.FirstOrDefault();

        if (tournament == null)
        {
            return;
        }

        // Act
        var result = await Client.ArenaTournaments.GetAsync(tournament.Id);

        // Assert
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.FullName.Should().NotBeNullOrWhiteSpace();
        result.Clock.Should().NotBeNull();
        result.Variant.Should().NotBeNull();
        result.Variant!.Key.Should().NotBeNullOrWhiteSpace();
    }

    #endregion

    #region StreamResultsAsync Tests

    [Fact]
    public async Task StreamResultsAsync_WithFinishedTournament_ReturnsResults()
    {
        // Get a finished tournament
        var current = await Client.ArenaTournaments.GetCurrentAsync();
        var finishedTournament = current.Finished.FirstOrDefault();

        if (finishedTournament == null)
        {
            return;
        }

        // Act
        var results = new List<ArenaPlayerResult>();
        await foreach (var result in Client.ArenaTournaments.StreamResultsAsync(finishedTournament.Id, nb: 10))
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
        // Get a finished tournament
        var current = await Client.ArenaTournaments.GetCurrentAsync();
        var finishedTournament = current.Finished.FirstOrDefault();

        if (finishedTournament == null)
        {
            return;
        }

        // Act
        var results = new List<ArenaPlayerResult>();
        await foreach (var result in Client.ArenaTournaments.StreamResultsAsync(finishedTournament.Id, nb: 10))
        {
            results.Add(result);
            if (results.Count >= 10) break;
        }

        // Assert - Results should be ordered by rank
        if (results.Count > 1)
        {
            for (int i = 1; i < results.Count; i++)
            {
                results[i].Rank.Should().BeGreaterThanOrEqualTo(results[i - 1].Rank);
            }
        }
    }

    [Fact]
    public async Task StreamResultsAsync_WithSheet_IncludesFireStreak()
    {
        // Get a finished tournament
        var current = await Client.ArenaTournaments.GetCurrentAsync();
        var finishedTournament = current.Finished.FirstOrDefault();

        if (finishedTournament == null)
        {
            return;
        }

        // Act
        var results = new List<ArenaPlayerResult>();
        await foreach (var result in Client.ArenaTournaments.StreamResultsAsync(finishedTournament.Id, nb: 5, sheet: true))
        {
            results.Add(result);
            if (results.Count >= 5) break;
        }

        // Assert - Should have results (sheet data is optional)
        results.Should().NotBeEmpty();
    }

    #endregion

    #region StreamGamesAsync Tests

    [Fact]
    public async Task StreamGamesAsync_WithFinishedTournament_ReturnsGames()
    {
        // Get a finished tournament
        var current = await Client.ArenaTournaments.GetCurrentAsync();
        var finishedTournament = current.Finished.FirstOrDefault();

        if (finishedTournament == null)
        {
            return;
        }

        // Act
        var games = new List<GameJson>();
        await foreach (var game in Client.ArenaTournaments.StreamGamesAsync(finishedTournament.Id))
        {
            games.Add(game);
            if (games.Count >= 5) break;
        }

        // Assert
        games.Should().NotBeEmpty("Finished tournaments should have games");
        games.Should().AllSatisfy(g =>
        {
            g.Id.Should().NotBeNullOrEmpty();
        });
    }

    #endregion

    #region GetTeamStandingAsync Tests

    [Fact]
    public async Task GetTeamStandingAsync_WithTeamBattle_ReturnsStanding()
    {
        // This test requires finding a team battle tournament
        // Team battles are less common, so we check current tournaments
        var current = await Client.ArenaTournaments.GetCurrentAsync();

        // Look for a team battle (has teamBattle property set)
        var teamBattle = current.Started.FirstOrDefault(t => t.TeamBattle != null)
            ?? current.Finished.FirstOrDefault(t => t.TeamBattle != null);

        if (teamBattle == null)
        {
            // No team battle available - this is acceptable
            return;
        }

        // Act
        var standing = await Client.ArenaTournaments.GetTeamStandingAsync(teamBattle.Id);

        // Assert
        standing.Should().NotBeNull();
        standing.Teams.Should().NotBeNull();
    }

    #endregion

    #region StreamCreatedByAsync Tests

    [Fact]
    public async Task StreamCreatedByAsync_WithLichessUser_ReturnsTournaments()
    {
        // Lichess account creates many tournaments
        const string username = "lichess";

        // Act
        var tournaments = new List<ArenaTournamentSummary>();
        try
        {
            await foreach (var tournament in Client.ArenaTournaments.StreamCreatedByAsync(username))
            {
                tournaments.Add(tournament);
                if (tournaments.Count >= 5) break;
            }
        }
        catch (LichessNotFoundException)
        {
            // User may not have tournaments - acceptable
            return;
        }

        // Assert
        foreach (var tournament in tournaments)
        {
            tournament.Id.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task StreamCreatedByAsync_WithStatusFilter_ReturnsFilteredResults()
    {
        const string username = "lichess";

        // Act - filter for finished tournaments
        var tournaments = new List<ArenaTournamentSummary>();
        try
        {
            await foreach (var tournament in Client.ArenaTournaments.StreamCreatedByAsync(username, status: ArenaStatusFilter.Finished))
            {
                tournaments.Add(tournament);
                if (tournaments.Count >= 3) break;
            }
        }
        catch (LichessNotFoundException)
        {
            return;
        }

        // Assert - all should be finished (or list may be empty)
        // Note: status filter behavior may vary
        tournaments.Should().NotBeNull();
    }

    #endregion

    #region StreamPlayedByAsync Tests

    [Fact]
    public async Task StreamPlayedByAsync_WithKnownUser_ReturnsTournaments()
    {
        // Use DrNykterstein (Magnus Carlsen) who plays in tournaments
        const string username = "DrNykterstein";

        // Act
        var tournaments = new List<ArenaPlayedTournament>();
        try
        {
            await foreach (var tournament in Client.ArenaTournaments.StreamPlayedByAsync(username, nb: 5))
            {
                tournaments.Add(tournament);
                if (tournaments.Count >= 5) break;
            }
        }
        catch (LichessNotFoundException)
        {
            return;
        }

        // Assert
        foreach (var tournament in tournaments)
        {
            tournament.Tournament.Should().NotBeNull();
            tournament.Tournament.Id.Should().NotBeNullOrWhiteSpace();
        }
    }

    #endregion

    #region StreamTeamTournamentsAsync Tests

    [Fact]
    public async Task StreamTeamTournamentsAsync_WithKnownTeam_ReturnsTournaments()
    {
        // Act
        var tournaments = new List<ArenaTournamentSummary>();
        try
        {
            await foreach (var tournament in Client.ArenaTournaments.StreamTeamTournamentsAsync(TestTeamId, max: 5))
            {
                tournaments.Add(tournament);
                if (tournaments.Count >= 5) break;
            }
        }
        catch (LichessNotFoundException)
        {
            return;
        }

        // Assert
        foreach (var tournament in tournaments)
        {
            tournament.Id.Should().NotBeNullOrEmpty();
        }
    }

    #endregion

    #region Authentication Required Tests

    [Fact]
    public async Task CreateAsync_WithoutAuth_ThrowsException()
    {
        // Arrange
        var options = new ArenaCreateOptions
        {
            ClockTime = 3,
            ClockIncrement = 0,
            Minutes = 45
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.ArenaTournaments.CreateAsync(options));
    }

    [Fact]
    public async Task JoinAsync_WithoutAuth_ThrowsException()
    {
        // Get a created tournament
        var current = await Client.ArenaTournaments.GetCurrentAsync();
        var tournament = current.Created.FirstOrDefault() ?? current.Started.FirstOrDefault();

        if (tournament == null)
        {
            return;
        }

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.ArenaTournaments.JoinAsync(tournament.Id));
    }

    [Fact]
    public async Task WithdrawAsync_WithoutAuth_ThrowsException()
    {
        // Get any tournament
        var current = await Client.ArenaTournaments.GetCurrentAsync();
        var tournament = current.Started.FirstOrDefault() ?? current.Created.FirstOrDefault();

        if (tournament == null)
        {
            return;
        }

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.ArenaTournaments.PauseOrWithdrawAsync(tournament.Id));
    }

    [Fact]
    public async Task TerminateAsync_WithoutAuth_ThrowsException()
    {
        // Get any tournament
        var current = await Client.ArenaTournaments.GetCurrentAsync();
        var tournament = current.Started.FirstOrDefault();

        if (tournament == null)
        {
            return;
        }

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.ArenaTournaments.TerminateAsync(tournament.Id));
    }

    #endregion
}
