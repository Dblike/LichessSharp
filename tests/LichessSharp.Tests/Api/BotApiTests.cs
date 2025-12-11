using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class BotApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly BotApi _botApi;

    public BotApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _botApi = new BotApi(_httpClientMock.Object);
    }


    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new BotApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }



    [Fact]
    public async Task UpgradeAccountAsync_CallsCorrectEndpoint()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>("/api/bot/account/upgrade", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.UpgradeAccountAsync();

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>("/api/bot/account/upgrade", null, It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task StreamEventsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var events = new List<BotAccountEvent>
        {
            new() { Type = "gameStart" },
            new() { Type = "challenge" }
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BotAccountEvent>("/api/stream/event", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(events));

        // Act
        var results = new List<BotAccountEvent>();
        await foreach (var evt in _botApi.StreamEventsAsync())
        {
            results.Add(evt);
        }

        // Assert
        results.Should().HaveCount(2);
        results[0].Type.Should().Be("gameStart");
        results[1].Type.Should().Be("challenge");
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<BotAccountEvent>("/api/stream/event", It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task StreamGameAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        var events = new List<BotGameEvent>
        {
            new() { Type = "gameFull" },
            new() { Type = "gameState" }
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BotGameEvent>($"/api/bot/game/stream/{gameId}", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(events));

        // Act
        var results = new List<BotGameEvent>();
        await foreach (var evt in _botApi.StreamGameAsync(gameId))
        {
            results.Add(evt);
        }

        // Assert
        results.Should().HaveCount(2);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<BotGameEvent>($"/api/bot/game/stream/{gameId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamGameAsync_WithNullGameId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _botApi.StreamGameAsync(null!)) { }
        });
    }



    [Fact]
    public async Task MakeMoveAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        var move = "e2e4";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/move/{move}", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.MakeMoveAsync(gameId, move);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/move/{move}", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MakeMoveAsync_WithOfferingDraw_IncludesQueryParameter()
    {
        // Arrange
        var gameId = "game123";
        var move = "e2e4";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("offeringDraw=true")), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.MakeMoveAsync(gameId, move, offeringDraw: true);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("offeringDraw=true")), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MakeMoveAsync_WithNullGameId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _botApi.MakeMoveAsync(null!, "e2e4"));
    }

    [Fact]
    public async Task MakeMoveAsync_WithNullMove_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _botApi.MakeMoveAsync("game123", null!));
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
            .Setup(x => x.GetAsync<List<ChatMessage>>($"/api/bot/game/{gameId}/chat", It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        // Act
        var result = await _botApi.GetChatAsync(gameId);

        // Assert
        result.Should().HaveCount(2);
        _httpClientMock.Verify(x => x.GetAsync<List<ChatMessage>>($"/api/bot/game/{gameId}/chat", It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task WriteChatAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/chat", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.WriteChatAsync(gameId, ChatRoom.Player, "Hello!");

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/chat", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task AbortAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/abort", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.AbortAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/abort", null, It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task ResignAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/resign", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.ResignAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/resign", null, It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task HandleDrawAsync_WithAcceptTrue_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/draw/yes", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.HandleDrawAsync(gameId, accept: true);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/draw/yes", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleDrawAsync_WithAcceptFalse_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/draw/no", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.HandleDrawAsync(gameId, accept: false);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/draw/no", null, It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task HandleTakebackAsync_WithAcceptTrue_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/takeback/yes", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.HandleTakebackAsync(gameId, accept: true);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/takeback/yes", null, It.IsAny<CancellationToken>()), Times.Once);
    }



    [Fact]
    public async Task ClaimDrawAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/claim-draw", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _botApi.ClaimDrawAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/bot/game/{gameId}/claim-draw", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ClaimDrawAsync_WithNullGameId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _botApi.ClaimDrawAsync(null!));
    }



    [Fact]
    public async Task GetOnlineBotsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var bots = new List<BotUser>
        {
            new() { Id = "bot1", Username = "Bot1" },
            new() { Id = "bot2", Username = "Bot2" }
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BotUser>("/api/bot/online", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(bots));

        // Act
        var results = new List<BotUser>();
        await foreach (var bot in _botApi.GetOnlineBotsAsync())
        {
            results.Add(bot);
        }

        // Assert
        results.Should().HaveCount(2);
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<BotUser>("/api/bot/online", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOnlineBotsAsync_WithCount_IncludesQueryParameter()
    {
        // Arrange
        var bots = new List<BotUser>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<BotUser>("/api/bot/online?nb=50", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(bots));

        // Act
        await foreach (var _ in _botApi.GetOnlineBotsAsync(count: 50)) { }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<BotUser>("/api/bot/online?nb=50", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOnlineBotsAsync_WithCountBelowRange_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await foreach (var _ in _botApi.GetOnlineBotsAsync(count: 0)) { }
        });
    }

    [Fact]
    public async Task GetOnlineBotsAsync_WithCountAboveRange_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await foreach (var _ in _botApi.GetOnlineBotsAsync(count: 301)) { }
        });
    }



    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }
        await Task.CompletedTask;
    }

}
