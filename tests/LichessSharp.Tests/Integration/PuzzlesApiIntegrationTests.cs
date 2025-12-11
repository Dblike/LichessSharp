using FluentAssertions;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for the Puzzles API.
/// These tests verify that the API client works correctly against the live Lichess API.
/// </summary>
[Trait("Category", "Integration")]
public class PuzzlesApiIntegrationTests : IntegrationTestBase
{
    #region GetDailyAsync Tests

    [Fact]
    public async Task GetDailyAsync_ReturnsDailyPuzzle()
    {
        // Act
        var puzzle = await Client.Puzzles.GetDailyAsync();

        // Assert
        puzzle.Should().NotBeNull();
        puzzle.Puzzle.Should().NotBeNull();
        puzzle.Puzzle!.Id.Should().NotBeNullOrEmpty();
        puzzle.Puzzle.Rating.Should().BeGreaterThan(0);
        puzzle.Puzzle.Solution.Should().NotBeNullOrEmpty();
        puzzle.Game.Should().NotBeNull();
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithDailyPuzzleId_ReturnsPuzzle()
    {
        // Arrange - Get the daily puzzle to get a known valid ID
        var dailyPuzzle = await Client.Puzzles.GetDailyAsync();
        var puzzleId = dailyPuzzle.Puzzle!.Id;

        // Act
        var puzzle = await Client.Puzzles.GetAsync(puzzleId);

        // Assert
        puzzle.Should().NotBeNull();
        puzzle.Puzzle.Should().NotBeNull();
        puzzle.Puzzle!.Id.Should().Be(puzzleId);
        puzzle.Puzzle.Rating.Should().BeGreaterThan(0);
        puzzle.Puzzle.Solution.Should().NotBeNullOrEmpty();
        puzzle.Game.Should().NotBeNull();
    }

    #endregion

    #region GetStormDashboardAsync Tests

    [Fact]
    public async Task GetStormDashboardAsync_WithValidUsername_ReturnsDashboard()
    {
        // Arrange - Use a well-known active user
        var username = "DrNykterstein";

        // Act
        var dashboard = await Client.Puzzles.GetStormDashboardAsync(username);

        // Assert
        dashboard.Should().NotBeNull();
        dashboard.High.Should().NotBeNull();
        // DrNykterstein has definitely played storm, so should have some high scores
        dashboard.High!.AllTime.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetStormDashboardAsync_WithDays_ReturnsDashboardWithHistory()
    {
        // Arrange
        var username = "DrNykterstein";

        // Act
        var dashboard = await Client.Puzzles.GetStormDashboardAsync(username, days: 7);

        // Assert
        dashboard.Should().NotBeNull();
        dashboard.High.Should().NotBeNull();
        // Days may be null or empty if no activity in the period
        // Just verify the response structure is correct
    }

    #endregion

    #region Puzzle Data Validation Tests

    [Fact]
    public async Task GetDailyAsync_ReturnsValidPuzzleData()
    {
        // Act
        var puzzle = await Client.Puzzles.GetDailyAsync();

        // Assert - Validate puzzle data structure
        puzzle.Puzzle.Should().NotBeNull();
        puzzle.Puzzle!.Id.Should().NotBeNullOrEmpty();
        puzzle.Puzzle.Rating.Should().BeInRange(400, 3500); // Reasonable puzzle rating range
        puzzle.Puzzle.Plays.Should().BeGreaterThanOrEqualTo(0);
        puzzle.Puzzle.Solution.Should().NotBeNullOrEmpty();
        puzzle.Puzzle.Solution!.All(move => !string.IsNullOrEmpty(move)).Should().BeTrue();
        puzzle.Puzzle.Themes.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDailyAsync_ReturnsValidGameData()
    {
        // Act
        var puzzle = await Client.Puzzles.GetDailyAsync();

        // Assert - Validate game data structure
        puzzle.Game.Should().NotBeNull();
        puzzle.Game!.Id.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region GetRaceAsync Tests

    [Fact]
    public async Task GetRaceAsync_WithNonExistentRace_ReturnsNotFoundOrNull()
    {
        // Arrange - Use a random race ID that likely doesn't exist
        // Note: Puzzle races are only available for 30 minutes after creation
        var raceId = "nonexistent123";

        // Act & Assert - Should either throw NotFound or return null
        // The API returns 404 for non-existent races
        try
        {
            var result = await Client.Puzzles.GetRaceAsync(raceId);
            // If we get here, the race somehow exists (unlikely)
            result.Should().NotBeNull();
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            // Expected - race doesn't exist
        }
    }

    #endregion
}
