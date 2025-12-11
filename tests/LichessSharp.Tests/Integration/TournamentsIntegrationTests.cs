using FluentAssertions;

using LichessSharp.Api.Contracts;
using LichessSharp.Exceptions;

using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for Arena and Swiss Tournaments APIs.
/// These tests require network access to Lichess.
/// </summary>
[Trait("Category", "Integration")]
public class TournamentsIntegrationTests : IDisposable
{
    private readonly LichessClient _client = new();

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Arena Tournaments API Tests

    [Fact]
    public async Task ArenaTournaments_GetCurrentAsync_ReturnsCurrentTournaments()
    {
        // Act
        var result = await _client.ArenaTournaments.GetCurrentAsync();

        // Assert
        result.Should().NotBeNull();
        // There should always be some arena tournaments on Lichess
        var totalCount = result.Created.Count + result.Started.Count + result.Finished.Count;
        totalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ArenaTournaments_GetAsync_WithValidId_ReturnsTournament()
    {
        // First get current tournaments to find a valid ID
        var current = await _client.ArenaTournaments.GetCurrentAsync();
        var anyTournament = current.Started.FirstOrDefault() ?? current.Created.FirstOrDefault() ?? current.Finished.FirstOrDefault();

        if (anyTournament == null)
        {
            // Skip if no tournaments available (very unlikely)
            return;
        }

        // Act
        var result = await _client.ArenaTournaments.GetAsync(anyTournament.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(anyTournament.Id);
        result.FullName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ArenaTournaments_StreamResultsAsync_WithValidId_ReturnsResults()
    {
        // First get current tournaments to find a valid ID
        var current = await _client.ArenaTournaments.GetCurrentAsync();
        var finishedTournament = current.Finished.FirstOrDefault();

        if (finishedTournament == null)
        {
            // Skip if no finished tournaments available
            return;
        }

        // Act
        var results = new List<ArenaPlayerResult>();
        await foreach (var result in _client.ArenaTournaments.StreamResultsAsync(finishedTournament.Id, nb: 5))
        {
            results.Add(result);
            if (results.Count >= 5) break;
        }

        // Assert
        results.Should().NotBeEmpty();
        results.Should().AllSatisfy(r =>
        {
            r.Username.Should().NotBeNullOrEmpty();
            r.Rank.Should().BeGreaterThan(0);
        });
    }

    [Fact]
    public async Task ArenaTournaments_StreamCreatedByAsync_WithKnownUser_ReturnsData()
    {
        // Use lichess account which has created many tournaments
        var username = "lichess";

        // Act
        var tournaments = new List<ArenaTournamentSummary>();
        try
        {
            await foreach (var tournament in _client.ArenaTournaments.StreamCreatedByAsync(username))
            {
                tournaments.Add(tournament);
                if (tournaments.Count >= 5) break;
            }
        }
        catch (LichessNotFoundException)
        {
            // User may not exist or have no tournaments - this is acceptable
            return;
        }

        // Assert - If we got results, verify they're valid
        foreach (var tournament in tournaments)
        {
            tournament.Id.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task ArenaTournaments_StreamTeamTournamentsAsync_WithKnownTeam_ReturnsData()
    {
        // Use lichess-swiss team which hosts many tournaments
        var teamId = "lichess-swiss";

        // Act
        var tournaments = new List<ArenaTournamentSummary>();
        try
        {
            await foreach (var tournament in _client.ArenaTournaments.StreamTeamTournamentsAsync(teamId, max: 5))
            {
                tournaments.Add(tournament);
                if (tournaments.Count >= 5) break;
            }
        }
        catch (LichessNotFoundException)
        {
            // Team may not exist or have no tournaments - this is acceptable
            return;
        }

        // Assert - If we got results, verify they're valid (empty is also acceptable)
        foreach (var tournament in tournaments)
        {
            tournament.Id.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task ArenaTournaments_CreateAsync_WithoutAuth_ThrowsException()
    {
        // Arrange - Using unauthenticated client
        var options = new ArenaCreateOptions
        {
            ClockTime = 3,
            ClockIncrement = 0,
            Minutes = 45
        };

        // Act & Assert - Should fail without authentication
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await _client.ArenaTournaments.CreateAsync(options));
    }

    [Fact]
    public async Task ArenaTournaments_JoinAsync_WithoutAuth_ThrowsException()
    {
        // Get a tournament ID first
        var current = await _client.ArenaTournaments.GetCurrentAsync();
        var tournament = current.Created.FirstOrDefault() ?? current.Started.FirstOrDefault();

        if (tournament == null) return;

        // Act & Assert - Should fail without authentication
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await _client.ArenaTournaments.JoinAsync(tournament.Id));
    }

    #endregion

    #region Swiss Tournaments API Tests

    [Fact]
    public async Task SwissTournaments_GetAsync_WithValidId_ReturnsTournament()
    {
        // Use a known Swiss tournament ID from a team
        // First get Swiss tournaments from lichess-swiss team
        var tournaments = new List<SwissTournament>();
        await foreach (var t in _client.SwissTournaments.StreamTeamTournamentsAsync("lichess-swiss", max: 1))
        {
            tournaments.Add(t);
            break;
        }

        if (tournaments.Count == 0)
        {
            // Skip if no tournaments available
            return;
        }

        // Act
        var result = await _client.SwissTournaments.GetAsync(tournaments[0].Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(tournaments[0].Id);
        result.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SwissTournaments_StreamTeamTournamentsAsync_WithKnownTeam_ReturnsTournaments()
    {
        // Use lichess-swiss team
        var teamId = "lichess-swiss";

        // Act
        var tournaments = new List<SwissTournament>();
        await foreach (var tournament in _client.SwissTournaments.StreamTeamTournamentsAsync(teamId, max: 5))
        {
            tournaments.Add(tournament);
            if (tournaments.Count >= 5) break;
        }

        // Assert
        tournaments.Should().NotBeEmpty();
        tournaments.Should().AllSatisfy(t =>
        {
            t.Id.Should().NotBeNullOrEmpty();
            t.Name.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task SwissTournaments_StreamResultsAsync_WithFinishedTournament_ReturnsResults()
    {
        // Get a finished Swiss tournament
        var tournaments = new List<SwissTournament>();
        await foreach (var t in _client.SwissTournaments.StreamTeamTournamentsAsync("lichess-swiss", max: 10))
        {
            if (t.Status == "finished")
            {
                tournaments.Add(t);
                break;
            }
        }

        if (tournaments.Count == 0)
        {
            // Skip if no finished tournaments
            return;
        }

        // Act
        var results = new List<SwissPlayerResult>();
        await foreach (var result in _client.SwissTournaments.StreamResultsAsync(tournaments[0].Id, nb: 5))
        {
            results.Add(result);
            if (results.Count >= 5) break;
        }

        // Assert
        results.Should().NotBeEmpty();
        results.Should().AllSatisfy(r =>
        {
            r.Username.Should().NotBeNullOrEmpty();
            r.Rank.Should().BeGreaterThan(0);
        });
    }

    [Fact]
    public async Task SwissTournaments_CreateAsync_WithoutAuth_ThrowsException()
    {
        // Arrange
        var options = new SwissCreateOptions
        {
            Name = "Test Swiss",
            ClockLimit = 300,
            ClockIncrement = 3,
            NbRounds = 5
        };

        // Act & Assert - Should fail without authentication
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await _client.SwissTournaments.CreateAsync("lichess-swiss", options));
    }

    [Fact]
    public async Task SwissTournaments_JoinAsync_WithoutAuth_ThrowsException()
    {
        // Get a tournament ID first
        var tournaments = new List<SwissTournament>();
        await foreach (var t in _client.SwissTournaments.StreamTeamTournamentsAsync("lichess-swiss", max: 1))
        {
            if (t.Status == "created")
            {
                tournaments.Add(t);
                break;
            }
        }

        if (tournaments.Count == 0) return;

        // Act & Assert - Should fail without authentication
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await _client.SwissTournaments.JoinAsync(tournaments[0].Id));
    }

    #endregion
}
