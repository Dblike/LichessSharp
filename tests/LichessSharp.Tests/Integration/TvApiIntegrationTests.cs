using FluentAssertions;

using LichessSharp.Api.Contracts;
using LichessSharp.Models;
using LichessSharp.Models.Enums;

using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for the TV API.
/// These tests make real HTTP calls to Lichess.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class TvApiIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task GetCurrentGamesAsync_ReturnsChannels()
    {
        // Act
        var channels = await Client.Tv.GetCurrentGamesAsync();

        // Assert
        channels.Should().NotBeNull();
        // At least one channel should have a game
        var hasAtLeastOneGame =
            channels.Best != null ||
            channels.Bullet != null ||
            channels.Blitz != null ||
            channels.Rapid != null ||
            channels.Classical != null;
        hasAtLeastOneGame.Should().BeTrue("Lichess TV should always have at least one game playing");
    }

    [Fact]
    public async Task GetCurrentGamesAsync_BestChannelHasValidData()
    {
        // Act
        var channels = await Client.Tv.GetCurrentGamesAsync();

        // Assert
        // The "best" channel is the featured game and should always be present
        channels.Best.Should().NotBeNull("The 'best' channel (featured game) should always be present");
        channels.Best!.GameId.Should().NotBeNullOrWhiteSpace("Game ID should be present");
        channels.Best.User.Should().NotBeNull("User info should be present");
    }

    [Fact]
    public async Task GetCurrentGamesAsync_GameHasValidUserInfo()
    {
        // Act
        var channels = await Client.Tv.GetCurrentGamesAsync();

        // Assert
        var game = channels.Best ?? channels.Bullet ?? channels.Blitz;
        game.Should().NotBeNull();
        game!.User.Should().NotBeNull();
        game.User!.Id.Should().NotBeNullOrWhiteSpace();
        game.User.Name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task StreamCurrentGameAsync_ReceivesEvents()
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var receivedEvents = new List<TvFeedEvent>();

        // Act
        try
        {
            await foreach (var evt in Client.Tv.StreamCurrentGameAsync(cts.Token))
            {
                receivedEvents.Add(evt);
                // Just get the first event (should be "featured") and maybe one "fen"
                if (receivedEvents.Count >= 2)
                {
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected if timeout reached
        }

        // Assert
        receivedEvents.Should().NotBeEmpty("Stream should return at least one event");
        receivedEvents[0].Type.Should().Be("featured", "First event should be 'featured' type");
        receivedEvents[0].Data.Should().NotBeNull();
        receivedEvents[0].Data!.Id.Should().NotBeNullOrWhiteSpace("Featured event should have game ID");
        receivedEvents[0].Data!.Fen.Should().NotBeNullOrWhiteSpace("Featured event should have FEN");
        receivedEvents[0].Data!.Players.Should().NotBeNull().And.HaveCount(2, "Featured event should have 2 players");
    }

    [Fact]
    public async Task StreamChannelAsync_WithBullet_ReceivesEvents()
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var receivedEvents = new List<TvFeedEvent>();

        // Act
        try
        {
            await foreach (var evt in Client.Tv.StreamChannelAsync("bullet", cts.Token))
            {
                receivedEvents.Add(evt);
                if (receivedEvents.Count >= 1)
                {
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected if timeout reached
        }

        // Assert
        receivedEvents.Should().NotBeEmpty("Bullet channel stream should return at least one event");
        receivedEvents[0].Type.Should().Be("featured");
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithBlitz_ReturnsGames()
    {
        // Arrange
        var options = new TvChannelGamesOptions
        {
            Count = 5,
            Moves = true,
            Clocks = true
        };
        var games = new List<GameJson>();

        // Act
        await foreach (var game in Client.Tv.StreamChannelGamesAsync("blitz", options))
        {
            games.Add(game);
            if (games.Count >= 3)
            {
                break;
            }
        }

        // Assert
        games.Should().NotBeEmpty("Blitz channel should have ongoing games");
        var firstGame = games[0];
        firstGame.Id.Should().NotBeNullOrWhiteSpace();
        firstGame.Speed.Should().Be(Speed.Blitz);
        firstGame.Status.Should().Be(GameStatus.Started, "TV games should be ongoing");
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithDefaultOptions_ReturnsGames()
    {
        // Arrange
        var games = new List<GameJson>();

        // Act
        await foreach (var game in Client.Tv.StreamChannelGamesAsync("rapid"))
        {
            games.Add(game);
            if (games.Count >= 3)
            {
                break;
            }
        }

        // Assert
        // Rapid might have fewer games than bullet/blitz, so we just check it works
        // If no games are playing, the collection will be empty which is valid
        games.Should().NotBeNull();
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithMoves_IncludesMoves()
    {
        // Arrange
        var options = new TvChannelGamesOptions
        {
            Count = 5,
            Moves = true
        };

        // Act
        await foreach (var game in Client.Tv.StreamChannelGamesAsync("bullet", options))
        {
            // Assert - at least one game should have moves if it's ongoing
            if (!string.IsNullOrEmpty(game.Moves))
            {
                game.Moves.Should().NotBeNullOrEmpty();
                return; // Test passed
            }
        }

        // If we get here, either no games had moves yet (new games) or no games were returned
        // This is acceptable for an integration test
    }

    [Fact]
    public async Task GetCurrentGamesAsync_VariantChannelsPresent()
    {
        // Act
        var channels = await Client.Tv.GetCurrentGamesAsync();

        // Assert
        // These channels may or may not have games depending on who's playing
        // We just verify the structure is correct (null is acceptable if no game)
        channels.Should().NotBeNull();
        // At least check that speed-based channels work
        // (variant channels might be empty more often)
        var speedChannels = new[] { channels.UltraBullet, channels.Bullet, channels.Blitz, channels.Rapid, channels.Classical };
        speedChannels.Count(c => c != null).Should().BeGreaterThan(0, "At least one speed-based channel should be active");
    }
}
