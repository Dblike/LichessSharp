using FluentAssertions;
using LichessSharp.Exceptions;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for the Puzzles API.
/// These tests verify that the API client works correctly against the live Lichess API.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class PuzzlesApiIntegrationTests : IntegrationTestBase
{

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

    [Fact]
    public async Task GetRaceAsync_WithNonExistentRace_ThrowsNotFoundException()
    {
        // Arrange - Use a random race ID that likely doesn't exist
        // Note: Puzzle races are only available for 30 minutes after creation
        var raceId = "nonexistent123";

        // Act & Assert - Should throw LichessNotFoundException for non-existent races
        await Assert.ThrowsAsync<LichessNotFoundException>(async () =>
            await Client.Puzzles.GetRaceAsync(raceId));
    }

}
