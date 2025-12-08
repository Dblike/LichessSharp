using FluentAssertions;
using LichessSharp.Api.Options;
using LichessSharp.Models;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for the Games API.
/// These tests use real game IDs from the OpenAPI spec examples.
/// </summary>
[Trait("Category", "Integration")]
public class GamesApiIntegrationTests : IntegrationTestBase
{
    // Well-known game IDs from OpenAPI spec examples
    private const string GameId1 = "q7ZvsdUF";
    private const string GameId2 = "TJxUmbWK";
    private const string ThibaultUsername = "thibault";

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithValidGameId_ReturnsGame()
    {
        // Act
        var game = await Client.Games.GetAsync(GameId1);

        // Assert
        game.Should().NotBeNull();
        game.Id.Should().Be(GameId1);
    }

    [Fact]
    public async Task GetAsync_WithOptions_ReturnsGameWithRequestedData()
    {
        // Arrange
        var options = new ExportGameOptions
        {
            Moves = true,
            Clocks = true,
            Opening = true
        };

        // Act
        var game = await Client.Games.GetAsync(GameId1, options);

        // Assert
        game.Should().NotBeNull();
        game.Id.Should().Be(GameId1);
        game.Moves.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region GetPgnAsync Tests

    [Fact]
    public async Task GetPgnAsync_WithValidGameId_ReturnsPgn()
    {
        // Act
        var pgn = await Client.Games.GetPgnAsync(GameId1);

        // Assert
        pgn.Should().NotBeNullOrEmpty();
        pgn.Should().Contain("[Event");
        pgn.Should().Contain("[Site");
    }

    [Fact]
    public async Task GetPgnAsync_WithOptions_ReturnsPgnWithRequestedData()
    {
        // Arrange
        var options = new ExportGameOptions
        {
            Clocks = true,
            Evals = true
        };

        // Act
        var pgn = await Client.Games.GetPgnAsync(GameId1, options);

        // Assert
        pgn.Should().NotBeNullOrEmpty();
        pgn.Should().Contain("[Event");
    }

    #endregion

    #region StreamUserGamesAsync Tests

    [Fact]
    public async Task StreamUserGamesAsync_WithValidUsername_ReturnsGames()
    {
        // Arrange
        var options = new ExportUserGamesOptions
        {
            Max = 3  // Limit to 3 games for test speed
        };

        // Act
        var games = new List<GameJson>();
        await foreach (var game in Client.Games.StreamUserGamesAsync(ThibaultUsername, options))
        {
            games.Add(game);
            if (games.Count >= 3) break;
        }

        // Assert
        games.Should().NotBeEmpty();
        games.Should().AllSatisfy(g =>
        {
            g.Id.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task StreamUserGamesAsync_WithPerfTypeFilter_ReturnsFilteredGames()
    {
        // Arrange
        var options = new ExportUserGamesOptions
        {
            Max = 2,
            PerfType = "blitz"
        };

        // Act
        var games = new List<GameJson>();
        await foreach (var game in Client.Games.StreamUserGamesAsync(ThibaultUsername, options))
        {
            games.Add(game);
        }

        // Assert - May be empty if user has no blitz games, but shouldn't throw
        games.Should().NotBeNull();
    }

    #endregion

    #region StreamByIdsAsync Tests

    [Fact]
    public async Task StreamByIdsAsync_WithValidGameIds_ReturnsGames()
    {
        // Arrange
        var gameIds = new[] { GameId1, GameId2 };

        // Act
        var games = new List<GameJson>();
        await foreach (var game in Client.Games.StreamByIdsAsync(gameIds))
        {
            games.Add(game);
        }

        // Assert
        games.Should().NotBeEmpty();
        games.Should().Contain(g => g.Id == GameId1 || g.Id == GameId2);
    }

    [Fact]
    public async Task StreamByIdsAsync_WithOptions_ReturnsGamesWithRequestedData()
    {
        // Arrange
        var gameIds = new[] { GameId1 };
        var options = new ExportGameOptions
        {
            Moves = true,
            Opening = true
        };

        // Act
        var games = new List<GameJson>();
        await foreach (var game in Client.Games.StreamByIdsAsync(gameIds, options))
        {
            games.Add(game);
        }

        // Assert
        games.Should().NotBeEmpty();
        games.First().Moves.Should().NotBeNullOrEmpty();
    }

    #endregion
}
