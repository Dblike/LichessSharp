using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Exceptions;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for the Board API.
/// These tests make real HTTP calls to Lichess.
/// Note: All Board API endpoints require authentication.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class BoardApiIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task StreamEventsAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Act & Assert
        var act = async () =>
        {
            await foreach (var _ in Client.Board.StreamEventsAsync())
            {
                break;
            }
        };

        // Should throw authentication exception for unauthenticated requests
        await act.Should().ThrowAsync<LichessAuthenticationException>();
    }

    [Fact]
    public async Task StreamGameAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";

        // Act & Assert
        var act = async () =>
        {
            await foreach (var _ in Client.Board.StreamGameAsync(gameId))
            {
                break;
            }
        };

        // Should throw 401 for unauthenticated requests
        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task MakeMoveAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";
        var move = "e2e4";

        // Act & Assert
        var act = async () => await Client.Board.MakeMoveAsync(gameId, move);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task GetChatAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";

        // Act & Assert
        var act = async () => await Client.Board.GetChatAsync(gameId);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task WriteChatAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";

        // Act & Assert
        var act = async () => await Client.Board.WriteChatAsync(gameId, ChatRoom.Player, "Hello");

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task AbortAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";

        // Act & Assert
        var act = async () => await Client.Board.AbortAsync(gameId);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task ResignAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";

        // Act & Assert
        var act = async () => await Client.Board.ResignAsync(gameId);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task HandleDrawAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";

        // Act & Assert
        var act = async () => await Client.Board.HandleDrawAsync(gameId, accept: true);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task HandleTakebackAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";

        // Act & Assert
        var act = async () => await Client.Board.HandleTakebackAsync(gameId, accept: true);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task ClaimVictoryAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";

        // Act & Assert
        var act = async () => await Client.Board.ClaimVictoryAsync(gameId);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task BerserkAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var gameId = "somegameid";

        // Act & Assert
        var act = async () => await Client.Board.BerserkAsync(gameId);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task SeekAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var options = new SeekOptions
        {
            Rated = false,
            Time = 10,
            Increment = 0
        };

        // Act & Assert
        var act = async () =>
        {
            await foreach (var _ in Client.Board.SeekAsync(options))
            {
                break;
            }
        };

        // Lichess may return auth error or validation error depending on endpoint behavior
        await act.Should().ThrowAsync<LichessException>();
    }
}
