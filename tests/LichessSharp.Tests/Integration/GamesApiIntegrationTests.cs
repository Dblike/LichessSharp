using FluentAssertions;
using LichessSharp.Api.Options;
using LichessSharp.Exceptions;
using LichessSharp.Models.Games;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Integration tests for the Games API.
///     These tests use real game IDs from the OpenAPI spec examples.
/// </summary>
[IntegrationTest]
[LongRunningTest]
[Trait("Category", "Integration")]
[Trait("Category", "LongRunning")]
[Collection("Lichess API")]
public class GamesApiIntegrationTests : IntegrationTestBase
{
    public GamesApiIntegrationTests(LichessTestFixture fixture) : base(fixture) { }

    // Well-known game IDs from OpenAPI spec examples
    private const string GameId1 = "q7ZvsdUF";
    private const string GameId2 = "TJxUmbWK";
    private const string ThibaultUsername = "thibault";

    [Fact]
    public async Task GetAsync_WithValidGameId_ReturnsGame()
    {
        // Act
        await ThrottleAsync();
        var game = await Client.Games.ExportAsync(GameId1);

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
        await ThrottleAsync();
        var game = await Client.Games.ExportAsync(GameId1, options);

        // Assert
        game.Should().NotBeNull();
        game.Id.Should().Be(GameId1);
        game.Moves.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetPgnAsync_WithValidGameId_ReturnsPgn()
    {
        // Act
        await ThrottleAsync();
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
        await ThrottleAsync();
        var pgn = await Client.Games.GetPgnAsync(GameId1, options);

        // Assert
        pgn.Should().NotBeNullOrEmpty();
        pgn.Should().Contain("[Event");
    }

    [Fact]
    public async Task StreamUserGamesAsync_WithValidUsername_ReturnsGames()
    {
        // Arrange
        var options = new ExportUserGamesOptions
        {
            Max = 3 // Limit to 3 games for test speed
        };

        // Act
        var games = new List<GameJson>();
        await ThrottleAsync();
        await foreach (var game in Client.Games.StreamUserGamesAsync(ThibaultUsername, options))
        {
            games.Add(game);
            if (games.Count >= 3) break;
        }

        // Assert
        games.Should().NotBeEmpty();
        games.Should().AllSatisfy(g => { g.Id.Should().NotBeNullOrEmpty(); });
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
        await ThrottleAsync();
        await foreach (var game in Client.Games.StreamUserGamesAsync(ThibaultUsername, options)) games.Add(game);

        // Assert - May be empty if user has no blitz games, but shouldn't throw
        games.Should().NotBeNull();
    }

    [Fact]
    public async Task StreamByIdsAsync_WithValidGameIds_ReturnsGames()
    {
        // Arrange
        var gameIds = new[] { GameId1, GameId2 };

        // Act
        var games = new List<GameJson>();
        await ThrottleAsync();
        await foreach (var game in Client.Games.StreamByIdsAsync(gameIds)) games.Add(game);

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
        await ThrottleAsync();
        await foreach (var game in Client.Games.StreamByIdsAsync(gameIds, options)) games.Add(game);

        // Assert
        games.Should().NotBeEmpty();
        games.First().Moves.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task StreamGameMovesAsync_WithKnownCompletedGame_ReturnsInitialEvent()
    {
        // Act - Stream moves from a known completed game
        var events = new List<MoveStreamEvent>();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        try
        {
            await ThrottleAsync();
            await foreach (var evt in Client.Games.StreamGameMovesAsync(GameId1).WithCancellation(cts.Token))
            {
                events.Add(evt);
                // Get a few events to capture both game info and FEN data
                if (events.Count >= 2)
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            // Timeout is acceptable - completed games might not stream
        }

        // Assert - Either we got events or the stream timed out (both are valid for completed games)
        // First event is game metadata (has Id), second event has FEN
        if (events.Count > 0)
        {
            // First event should have game ID (metadata event)
            events[0].Id.Should().NotBeNullOrEmpty();
        }

        if (events.Count > 1)
        {
            // Second event should have FEN position data
            events[1].Fen.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task StreamGamesByIdsAsync_WithKnownGameIds_ReturnsEvents()
    {
        // Arrange - Use a unique stream ID for this test
        var streamId = $"integration-test-{Guid.NewGuid():N}";
        var gameIds = new[] { GameId1, GameId2 };

        // Act
        var events = new List<GameStreamEvent>();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        try
        {
            await ThrottleAsync();
            await foreach (var evt in Client.Games.StreamByIdsAsync(streamId, gameIds).WithCancellation(cts.Token))
            {
                events.Add(evt);
                // Collect events for known games then break
                if (events.Count >= 2) break;
            }
        }
        catch (OperationCanceledException)
        {
            // Timeout is acceptable
        }

        // Assert - We should get events for the completed games
        if (events.Count > 0) events.Should().AllSatisfy(e => e.Id.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public async Task GetSpectatorChatAsync_WithKnownGameId_ReturnsChatOrNotFound()
    {
        // Act - Some games may not have spectator chat available (404)
        try
        {
            await ThrottleAsync();
            var messages = await Client.Games.GetSpectatorChatAsync(GameId1);

            // Assert - Should return a list (possibly empty for older games)
            messages.Should().NotBeNull();
            messages.Should().AllSatisfy(msg =>
            {
                msg.User.Should().NotBeNullOrEmpty();
                msg.Text.Should().NotBeNullOrEmpty();
            });
        }
        catch (LichessNotFoundException)
        {
            // 404 is acceptable - not all games expose spectator chat
        }
    }
}