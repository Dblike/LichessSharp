using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models.Common;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class BoardApiTests
{
    private readonly BoardApi _boardApi;
    private readonly Mock<ILichessHttpClient> _httpClientMock;

    public BoardApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _boardApi = new BoardApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new BoardApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task StreamEventsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var events = new List<BoardAccountEvent>
        {
            new() { Type = "gameStart" },
            new() { Type = "challenge" }
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BoardAccountEvent>("/api/stream/event", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(events));

        // Act
        var results = new List<BoardAccountEvent>();
        await foreach (var evt in _boardApi.StreamEventsAsync()) results.Add(evt);

        // Assert
        results.Should().HaveCount(2);
        results[0].Type.Should().Be("gameStart");
        results[1].Type.Should().Be("challenge");
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<BoardAccountEvent>("/api/stream/event", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StreamGameAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        var events = new List<BoardGameEvent>
        {
            new() { Type = "gameFull" },
            new() { Type = "gameState" }
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BoardGameEvent>($"/api/board/game/stream/{gameId}",
                It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(events));

        // Act
        var results = new List<BoardGameEvent>();
        await foreach (var evt in _boardApi.StreamGameAsync(gameId)) results.Add(evt);

        // Assert
        results.Should().HaveCount(2);
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<BoardGameEvent>($"/api/board/game/stream/{gameId}", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StreamGameAsync_WithNullGameId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _boardApi.StreamGameAsync(null!))
            {
            }
        });
    }

    [Fact]
    public async Task MakeMoveAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        var move = "e2e4";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/move/{move}", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.MakeMoveAsync(gameId, move);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/move/{move}", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MakeMoveAsync_WithOfferingDraw_IncludesQueryParameter()
    {
        // Arrange
        var gameId = "game123";
        var move = "e2e4";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("offeringDraw=true")), null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.MakeMoveAsync(gameId, move, true);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("offeringDraw=true")), null,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MakeMoveAsync_WithNullGameId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _boardApi.MakeMoveAsync(null!, "e2e4"));
    }

    [Fact]
    public async Task MakeMoveAsync_WithNullMove_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _boardApi.MakeMoveAsync("game123", null!));
    }

    [Fact]
    public async Task GetChatAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        var messages = new List<ChatMessage>
        {
            new() { User = "player1", Text = "Hello" },
            new() { User = "player2", Text = "Hi" }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<List<ChatMessage>>($"/api/board/game/{gameId}/chat", It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        // Act
        var result = await _boardApi.GetChatAsync(gameId);

        // Assert
        result.Should().HaveCount(2);
        _httpClientMock.Verify(
            x => x.GetAsync<List<ChatMessage>>($"/api/board/game/{gameId}/chat", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WriteChatAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/chat", It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.WriteChatAsync(gameId, ChatRoom.Player, "Hello!");

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/chat", It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AbortAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/abort", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.AbortAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/abort", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ResignAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/resign", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.ResignAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/resign", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleDrawAsync_WithAcceptTrue_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/draw/yes", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.HandleDrawAsync(gameId, true);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/draw/yes", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleDrawAsync_WithAcceptFalse_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/draw/no", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.HandleDrawAsync(gameId, false);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/draw/no", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleTakebackAsync_WithAcceptTrue_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/takeback/yes", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.HandleTakebackAsync(gameId, true);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/takeback/yes", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ClaimVictoryAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/claim-victory", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.ClaimVictoryAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/claim-victory", null,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ClaimDrawAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/claim-draw", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.ClaimDrawAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/claim-draw", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ClaimDrawAsync_WithNullGameId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _boardApi.ClaimDrawAsync(null!));
    }

    [Fact]
    public async Task BerserkAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/berserk", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _boardApi.BerserkAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/board/game/{gameId}/berserk", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SeekAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var options = new SeekOptions { Rated = true, Time = 5, Increment = 3 };
        var results = new List<SeekResult> { new() { Id = "game123" } };
        _httpClientMock
            .Setup(x => x.StreamNdjsonPostAsync<SeekResult>("/api/board/seek", It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(results));

        // Act
        var seekResults = new List<SeekResult>();
        await foreach (var result in _boardApi.SeekAsync(options)) seekResults.Add(result);

        // Assert
        seekResults.Should().HaveCount(1);
        _httpClientMock.Verify(
            x => x.StreamNdjsonPostAsync<SeekResult>("/api/board/seek", It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SeekAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _boardApi.SeekAsync(null!))
            {
            }
        });
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items) yield return item;
        await Task.CompletedTask;
    }
}