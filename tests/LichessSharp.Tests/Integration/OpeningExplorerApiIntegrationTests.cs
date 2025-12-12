using FluentAssertions;
using LichessSharp.Api.Contracts;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Integration tests for the Opening Explorer API.
///     These tests use FEN positions and play sequences from the OpenAPI spec examples.
///     Endpoint: explorer.lichess.ovh
/// </summary>
[IntegrationTest]
[LongRunningTest]
[Trait("Category", "Integration")]
[Trait("Category", "LongRunning")]
public class OpeningExplorerApiIntegrationTests : IntegrationTestBase
{
    // Starting position FEN
    private const string StartingPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    // Sicilian Defense position after 1.e4 c5
    private const string SicilianFen = "rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2";

    // Queen's Gambit position after 1.d4 d5 2.c4
    private const string QueensGambitFen = "rnbqkbnr/ppp1pppp/8/3p4/2PP4/8/PP2PPPP/RNBQKBNR b KQkq c3 0 2";

    [Fact]
    public async Task GetMastersAsync_WithStartingPosition_ReturnsOpeningData()
    {
        // Act
        var result = await Client.OpeningExplorer.GetMastersAsync(StartingPositionFen);

        // Assert
        result.Should().NotBeNull();
        result.White.Should().BeGreaterThan(0);
        result.Draws.Should().BeGreaterThanOrEqualTo(0);
        result.Black.Should().BeGreaterThan(0);
        result.Moves.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetMastersAsync_WithSicilianPosition_ReturnsMoves()
    {
        // Act
        var result = await Client.OpeningExplorer.GetMastersAsync(SicilianFen);

        // Assert
        result.Should().NotBeNull();
        result.Moves.Should().NotBeNull();
        // Sicilian is very popular, should have games
        (result.White + result.Draws + result.Black).Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetMastersAsync_WithOptions_ReturnsFilteredData()
    {
        // Arrange
        var options = new ExplorerOptions
        {
            TopGames = 5,
            RecentGames = 5
        };

        // Act
        var result = await Client.OpeningExplorer.GetMastersAsync(StartingPositionFen, options);

        // Assert
        result.Should().NotBeNull();
        if (result.TopGames != null) result.TopGames.Should().HaveCountLessThanOrEqualTo(5);
    }

    [Fact]
    public async Task GetLichessAsync_WithStartingPosition_ReturnsOpeningData()
    {
        // Act
        var result = await Client.OpeningExplorer.GetLichessAsync(StartingPositionFen);

        // Assert
        result.Should().NotBeNull();
        result.White.Should().BeGreaterThan(0);
        result.Moves.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetLichessAsync_WithQueensGambit_ReturnsMoves()
    {
        // Act
        var result = await Client.OpeningExplorer.GetLichessAsync(QueensGambitFen);

        // Assert
        result.Should().NotBeNull();
        result.Moves.Should().NotBeNull();
        // Queen's Gambit is popular, should have games
        (result.White + result.Draws + result.Black).Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetLichessAsync_WithSpeedFilter_ReturnsFilteredData()
    {
        // Arrange
        var options = new ExplorerOptions
        {
            Speeds = ["blitz", "rapid"]
        };

        // Act
        var result = await Client.OpeningExplorer.GetLichessAsync(StartingPositionFen, options);

        // Assert
        result.Should().NotBeNull();
        result.Moves.Should().NotBeNull();
    }

    [Fact]
    public async Task GetLichessAsync_WithRatingFilter_ReturnsFilteredData()
    {
        // Arrange
        var options = new ExplorerOptions
        {
            Ratings = [2200, 2500]
        };

        // Act
        var result = await Client.OpeningExplorer.GetLichessAsync(StartingPositionFen, options);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetMasterGamePgnAsync_WithKnownGameId_ReturnsPgn()
    {
        // Arrange - Game ID from OpenAPI spec example
        var gameId = "aAbqI4ey";

        // Act
        var pgn = await Client.OpeningExplorer.GetMasterGamePgnAsync(gameId);

        // Assert
        pgn.Should().NotBeNullOrWhiteSpace();
        // PGN should contain standard headers
        pgn.Should().Contain("[Event ");
        pgn.Should().Contain("[White ");
        pgn.Should().Contain("[Black ");
        // PGN should contain moves
        pgn.Should().Contain("1.");
    }

    [Fact]
    public async Task GetPlayerAsync_WithKnownPlayer_ReturnsOpeningData()
    {
        // Arrange - thibault is a well-known Lichess user with many games
        var player = "thibault";

        // Act
        var result = await Client.OpeningExplorer.GetPlayerAsync(StartingPositionFen, player);

        // Assert
        result.Should().NotBeNull();
        // Player may or may not have games from starting position
        // Just verify the call succeeds and returns valid structure
        result.Moves.Should().NotBeNull();
    }

    //[Fact]
    //public async Task GetPlayerAsync_WithColorOption_ReturnsColorSpecificData()
    //{
    //    // Arrange
    //    var player = "thibault";
    //    var options = new ExplorerOptions
    //    {
    //        Color = "white"
    //    };

    //    // Act
    //    var result = await Client.OpeningExplorer.GetPlayerAsync(StartingPositionFen, player, options);

    //    // Assert
    //    result.Should().NotBeNull();
    //}
}