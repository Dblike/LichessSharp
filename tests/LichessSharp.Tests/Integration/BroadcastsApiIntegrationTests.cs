using FluentAssertions;

using LichessSharp.Api.Contracts;

using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for the Broadcasts API.
/// These tests make real HTTP calls to Lichess.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class BroadcastsApiIntegrationTests : IntegrationTestBase
{
    #region StreamOfficialBroadcastsAsync Tests

    [Fact]
    public async Task StreamOfficialBroadcastsAsync_ReturnsOfficialBroadcasts()
    {
        // Arrange
        var broadcasts = new List<BroadcastWithRounds>();

        // Act
        await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 5))
        {
            broadcasts.Add(broadcast);
            if (broadcasts.Count >= 3)
            {
                break;
            }
        }

        // Assert
        broadcasts.Should().NotBeEmpty("Lichess should have official broadcasts");
        var first = broadcasts[0];
        first.Tour.Should().NotBeNull();
        first.Tour.Id.Should().NotBeNullOrWhiteSpace();
        first.Tour.Name.Should().NotBeNullOrWhiteSpace();
        first.Rounds.Should().NotBeNull();
    }

    [Fact]
    public async Task StreamOfficialBroadcastsAsync_BroadcastsHaveValidStructure()
    {
        // Act
        await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 3))
        {
            // Assert - each broadcast should have valid structure
            broadcast.Tour.Should().NotBeNull();
            broadcast.Tour.Id.Should().NotBeNullOrWhiteSpace();
            broadcast.Tour.Name.Should().NotBeNullOrWhiteSpace();
            broadcast.Rounds.Should().NotBeNull();

            // Each round should have valid structure
            foreach (var round in broadcast.Rounds)
            {
                round.Id.Should().NotBeNullOrWhiteSpace();
                round.Name.Should().NotBeNullOrWhiteSpace();
                round.Url.Should().NotBeNullOrWhiteSpace();
            }

            return; // Test passed after verifying first broadcast
        }
    }

    #endregion

    #region GetTopBroadcastsAsync Tests

    [Fact]
    public async Task GetTopBroadcastsAsync_ReturnsTopBroadcasts()
    {
        // Act
        var result = await Client.Broadcasts.GetTopBroadcastsAsync();

        // Assert
        result.Should().NotBeNull();
        // Should have either active or past broadcasts
        var hasContent = (result.Active?.Count > 0) || (result.Past?.CurrentPageResults?.Count > 0);
        hasContent.Should().BeTrue("Top broadcasts page should have content");
    }

    [Fact]
    public async Task GetTopBroadcastsAsync_ActiveBroadcastsHaveValidStructure()
    {
        // Act
        var result = await Client.Broadcasts.GetTopBroadcastsAsync();

        // Assert
        if (result.Active?.Count > 0)
        {
            var firstActive = result.Active[0];
            firstActive.Tour.Should().NotBeNull();
            firstActive.Tour.Id.Should().NotBeNullOrWhiteSpace();
            firstActive.Tour.Name.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task GetTopBroadcastsAsync_WithPage2_ReturnsDifferentResults()
    {
        // Act
        var page1 = await Client.Broadcasts.GetTopBroadcastsAsync(page: 1);
        var page2 = await Client.Broadcasts.GetTopBroadcastsAsync(page: 2);

        // Assert
        page1.Should().NotBeNull();
        page2.Should().NotBeNull();
        // Pages should be different (if both have content)
        if (page1.Past?.CurrentPageResults?.Count > 0 && page2.Past?.CurrentPageResults?.Count > 0)
        {
            var page1Ids = page1.Past.CurrentPageResults.Select(b => b.Tour.Id).ToList();
            var page2Ids = page2.Past.CurrentPageResults.Select(b => b.Tour.Id).ToList();
            page1Ids.Should().NotBeEquivalentTo(page2Ids, "Different pages should have different content");
        }
    }

    #endregion

    #region SearchBroadcastsAsync Tests

    [Fact]
    public async Task SearchBroadcastsAsync_WithQuery_ReturnsResults()
    {
        // Act
        var result = await Client.Broadcasts.SearchBroadcastsAsync("chess");

        // Assert
        result.Should().NotBeNull();
        result.CurrentPageResults.Should().NotBeNull();
        // "chess" is a common term, should have results
        result.CurrentPageResults.Should().NotBeEmpty("'chess' should return search results");
    }

    [Fact]
    public async Task SearchBroadcastsAsync_ResultsHaveValidStructure()
    {
        // Act
        var result = await Client.Broadcasts.SearchBroadcastsAsync("tournament");

        // Assert
        result.Should().NotBeNull();
        if (result.CurrentPageResults?.Count > 0)
        {
            var first = result.CurrentPageResults[0];
            first.Tour.Should().NotBeNull();
            first.Tour.Id.Should().NotBeNullOrWhiteSpace();
            first.Tour.Name.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task SearchBroadcastsAsync_WithRareQuery_MayReturnEmptyResults()
    {
        // Act
        var result = await Client.Broadcasts.SearchBroadcastsAsync("xyznonexistent12345broadcast");

        // Assert
        result.Should().NotBeNull();
        result.CurrentPageResults.Should().NotBeNull();
        // May be empty for rare search terms - that's valid
    }

    #endregion

    #region GetTournamentAsync Tests

    [Fact]
    public async Task GetTournamentAsync_WithKnownId_ReturnsTournament()
    {
        // First get a known tournament ID from official broadcasts
        string? tournamentId = null;
        await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 1))
        {
            tournamentId = broadcast.Tour.Id;
            break;
        }

        if (tournamentId == null)
        {
            // Skip if no broadcasts available
            return;
        }

        // Act
        var result = await Client.Broadcasts.GetTournamentAsync(tournamentId);

        // Assert
        result.Should().NotBeNull();
        result.Tour.Should().NotBeNull();
        result.Tour.Id.Should().Be(tournamentId);
        result.Rounds.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTournamentAsync_ReturnsRounds()
    {
        // First get a known tournament ID from official broadcasts
        string? tournamentId = null;
        await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 1))
        {
            tournamentId = broadcast.Tour.Id;
            break;
        }

        if (tournamentId == null)
        {
            return;
        }

        // Act
        var result = await Client.Broadcasts.GetTournamentAsync(tournamentId);

        // Assert
        result.Rounds.Should().NotBeNull();
        if (result.Rounds.Count > 0)
        {
            var firstRound = result.Rounds[0];
            firstRound.Id.Should().NotBeNullOrWhiteSpace();
            firstRound.Name.Should().NotBeNullOrWhiteSpace();
        }
    }

    #endregion

    #region GetRoundAsync Tests

    [Fact]
    public async Task GetRoundAsync_WithKnownRound_ReturnsRound()
    {
        // First get a known round from official broadcasts
        BroadcastRoundInfo? roundInfo = null;
        string? tournamentSlug = null;
        await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 1))
        {
            if (broadcast.Rounds.Count > 0)
            {
                roundInfo = broadcast.Rounds[0];
                tournamentSlug = broadcast.Tour.Slug ?? broadcast.Tour.Id;
                break;
            }
        }

        if (roundInfo == null || tournamentSlug == null)
        {
            return;
        }

        // Act
        var result = await Client.Broadcasts.GetRoundAsync(tournamentSlug, roundInfo.Slug, roundInfo.Id);

        // Assert
        result.Should().NotBeNull();
        result.Round.Should().NotBeNull();
        result.Round.Id.Should().Be(roundInfo.Id);
        result.Tour.Should().NotBeNull();
        result.Games.Should().NotBeNull();
    }

    [Fact]
    public async Task GetRoundAsync_ReturnsGames()
    {
        // First get a known round from official broadcasts
        BroadcastRoundInfo? roundInfo = null;
        string? tournamentSlug = null;
        await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 5))
        {
            // Look for a round that might have games
            foreach (var round in broadcast.Rounds)
            {
                if (round.Ongoing == true || round.Finished == true)
                {
                    roundInfo = round;
                    tournamentSlug = broadcast.Tour.Slug ?? broadcast.Tour.Id;
                    break;
                }
            }
            if (roundInfo != null) break;
        }

        if (roundInfo == null || tournamentSlug == null)
        {
            return;
        }

        // Act
        var result = await Client.Broadcasts.GetRoundAsync(tournamentSlug, roundInfo.Slug, roundInfo.Id);

        // Assert
        result.Games.Should().NotBeNull();
        // Games may be empty for rounds that haven't started
        if (result.Games.Count > 0)
        {
            var firstGame = result.Games[0];
            firstGame.Id.Should().NotBeNullOrWhiteSpace();
        }
    }

    #endregion

    #region StreamUserBroadcastsAsync Tests

    [Fact]
    public async Task StreamUserBroadcastsAsync_WithLichessUser_ReturnsBroadcasts()
    {
        // Arrange - use "lichess" user who has official broadcasts
        var broadcasts = new List<BroadcastByUser>();

        // Act
        await foreach (var broadcast in Client.Broadcasts.StreamUserBroadcastsAsync("lichess", nb: 5))
        {
            broadcasts.Add(broadcast);
            if (broadcasts.Count >= 3)
            {
                break;
            }
        }

        // Assert
        // lichess user may or may not have broadcasts, but the call should succeed
        broadcasts.Should().NotBeNull();
    }

    #endregion

    #region GetPlayersAsync Tests

    [Fact]
    public async Task GetPlayersAsync_WithKnownTournament_ReturnsPlayers()
    {
        // First get a known tournament ID
        string? tournamentId = null;
        await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 5))
        {
            // Look for a tournament that might have player data
            if (broadcast.Tour.Tier >= 3) // Higher tier tournaments usually have player data
            {
                tournamentId = broadcast.Tour.Id;
                break;
            }
        }

        // Fall back to any tournament
        if (tournamentId == null)
        {
            await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 1))
            {
                tournamentId = broadcast.Tour.Id;
                break;
            }
        }

        if (tournamentId == null)
        {
            return;
        }

        // Act
        var result = await Client.Broadcasts.GetPlayersAsync(tournamentId);

        // Assert
        result.Should().NotBeNull();
        // Players list may be empty for some tournaments
        if (result.Count > 0)
        {
            var firstPlayer = result[0];
            firstPlayer.Name.Should().NotBeNullOrWhiteSpace();
        }
    }

    #endregion

    #region ExportRoundPgnAsync Tests

    [Fact]
    public async Task ExportRoundPgnAsync_WithKnownRound_ReturnsPgn()
    {
        // First get a finished round that should have PGN
        BroadcastRoundInfo? finishedRound = null;
        await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 10))
        {
            finishedRound = broadcast.Rounds.FirstOrDefault(r => r.Finished == true);
            if (finishedRound != null)
            {
                break;
            }
        }

        if (finishedRound == null)
        {
            // No finished rounds available
            return;
        }

        // Act
        var pgn = await Client.Broadcasts.ExportRoundPgnAsync(finishedRound.Id);

        // Assert
        pgn.Should().NotBeNullOrWhiteSpace("Finished rounds should have PGN");
        pgn.Should().Contain("[Event", "PGN should contain Event tag");
    }

    #endregion

    #region ExportAllRoundsPgnAsync Tests

    [Fact]
    public async Task ExportAllRoundsPgnAsync_WithKnownTournament_ReturnsPgn()
    {
        // First get a tournament with finished rounds
        string? tournamentId = null;
        await foreach (var broadcast in Client.Broadcasts.StreamOfficialBroadcastsAsync(nb: 10))
        {
            if (broadcast.Rounds.Any(r => r.Finished == true))
            {
                tournamentId = broadcast.Tour.Id;
                break;
            }
        }

        if (tournamentId == null)
        {
            return;
        }

        // Act
        var pgn = await Client.Broadcasts.ExportAllRoundsPgnAsync(tournamentId);

        // Assert
        pgn.Should().NotBeNullOrWhiteSpace("Tournament with finished rounds should have PGN");
        pgn.Should().Contain("[Event", "PGN should contain Event tag");
    }

    #endregion
}
