using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models.Enums;
using LichessSharp.Models.Games;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class TvApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly TvApi _tvApi;

    public TvApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _tvApi = new TvApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new TvApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task GetCurrentGamesAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedChannels = CreateTestTvChannels();
        _httpClientMock
            .Setup(x => x.GetAsync<TvChannels>("/api/tv/channels", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedChannels);

        // Act
        var result = await _tvApi.GetCurrentGamesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Best.Should().NotBeNull();
        result.Best!.GameId.Should().Be("bestgame");
        _httpClientMock.Verify(x => x.GetAsync<TvChannels>("/api/tv/channels", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCurrentGamesAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var expectedChannels = CreateTestTvChannels();
        _httpClientMock
            .Setup(x => x.GetAsync<TvChannels>("/api/tv/channels", cts.Token))
            .ReturnsAsync(expectedChannels);

        // Act
        await _tvApi.GetCurrentGamesAsync(cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<TvChannels>("/api/tv/channels", cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetCurrentGamesAsync_ReturnsAllChannels()
    {
        // Arrange
        var expectedChannels = CreateTestTvChannels();
        _httpClientMock
            .Setup(x => x.GetAsync<TvChannels>("/api/tv/channels", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedChannels);

        // Act
        var result = await _tvApi.GetCurrentGamesAsync();

        // Assert
        result.Best.Should().NotBeNull();
        result.Bullet.Should().NotBeNull();
        result.Blitz.Should().NotBeNull();
        result.Rapid.Should().NotBeNull();
    }

    [Fact]
    public async Task StreamCurrentGameAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var events = new List<TvFeedEvent>
        {
            CreateTestFeaturedEvent("game1"),
            CreateTestFenEvent()
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<TvFeedEvent>("/api/tv/feed", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(events));

        // Act
        var results = new List<TvFeedEvent>();
        await foreach (var evt in _tvApi.StreamCurrentGameAsync()) results.Add(evt);

        // Assert
        results.Should().HaveCount(2);
        results[0].Type.Should().Be("featured");
        results[1].Type.Should().Be("fen");
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<TvFeedEvent>("/api/tv/feed", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StreamCurrentGameAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var events = new List<TvFeedEvent>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<TvFeedEvent>("/api/tv/feed", cts.Token))
            .Returns(ToAsyncEnumerable(events));

        // Act
        await foreach (var _ in _tvApi.StreamCurrentGameAsync(cts.Token))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<TvFeedEvent>("/api/tv/feed", cts.Token), Times.Once);
    }

    [Fact]
    public async Task StreamChannelAsync_WithChannel_CallsCorrectEndpoint()
    {
        // Arrange
        var channel = "bullet";
        var events = new List<TvFeedEvent> { CreateTestFeaturedEvent("game1") };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<TvFeedEvent>($"/api/tv/{channel}/feed", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(events));

        // Act
        var results = new List<TvFeedEvent>();
        await foreach (var evt in _tvApi.StreamChannelAsync(channel)) results.Add(evt);

        // Assert
        results.Should().HaveCount(1);
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<TvFeedEvent>($"/api/tv/{channel}/feed", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StreamChannelAsync_WithNullChannel_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _tvApi.StreamChannelAsync(null!))
            {
            }
        });
    }

    [Fact]
    public async Task StreamChannelAsync_WithEmptyChannel_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in _tvApi.StreamChannelAsync(""))
            {
            }
        });
    }

    [Fact]
    public async Task StreamChannelAsync_UrlEncodesChannel()
    {
        // Arrange
        var channel = "king of the hill";
        var events = new List<TvFeedEvent>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<TvFeedEvent>(It.Is<string>(s => s.Contains("king%20of%20the%20hill")),
                It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(events));

        // Act
        await foreach (var _ in _tvApi.StreamChannelAsync(channel))
        {
        }

        // Assert
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<TvFeedEvent>(It.Is<string>(s => s.Contains("king%20of%20the%20hill")),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamChannelAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var channel = "blitz";
        var events = new List<TvFeedEvent>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<TvFeedEvent>($"/api/tv/{channel}/feed", cts.Token))
            .Returns(ToAsyncEnumerable(events));

        // Act
        await foreach (var _ in _tvApi.StreamChannelAsync(channel, cts.Token))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<TvFeedEvent>($"/api/tv/{channel}/feed", cts.Token), Times.Once);
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithChannel_CallsCorrectEndpoint()
    {
        // Arrange
        var channel = "bullet";
        var games = new List<GameJson> { CreateTestGameJson("game1") };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>($"/api/tv/{channel}", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        var results = new List<GameJson>();
        await foreach (var game in _tvApi.StreamChannelGamesAsync(channel)) results.Add(game);

        // Assert
        results.Should().HaveCount(1);
        results[0].Id.Should().Be("game1");
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>($"/api/tv/{channel}", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithOptions_AppendsQueryParameters()
    {
        // Arrange
        var channel = "blitz";
        var options = new TvChannelGamesOptions
        {
            Count = 20,
            Moves = true,
            Clocks = true,
            Opening = false
        };
        var games = new List<GameJson>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
                s.Contains($"/api/tv/{channel}") &&
                s.Contains("nb=20") &&
                s.Contains("moves=true") &&
                s.Contains("clocks=true") &&
                s.Contains("opening=false")), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        await foreach (var _ in _tvApi.StreamChannelGamesAsync(channel, options))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
            s.Contains("nb=20") &&
            s.Contains("moves=true") &&
            s.Contains("clocks=true") &&
            s.Contains("opening=false")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithAllOptions_AppendsAllQueryParameters()
    {
        // Arrange
        var channel = "rapid";
        var options = new TvChannelGamesOptions
        {
            Count = 15,
            Moves = true,
            PgnInJson = true,
            Tags = false,
            Clocks = true,
            Evals = true,
            Opening = true
        };
        var games = new List<GameJson>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
                s.Contains("nb=15") &&
                s.Contains("moves=true") &&
                s.Contains("pgnInJson=true") &&
                s.Contains("tags=false") &&
                s.Contains("clocks=true") &&
                s.Contains("evals=true") &&
                s.Contains("opening=true")), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        await foreach (var _ in _tvApi.StreamChannelGamesAsync(channel, options))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
            s.Contains("nb=15") &&
            s.Contains("pgnInJson=true")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithNullChannel_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _tvApi.StreamChannelGamesAsync(null!))
            {
            }
        });
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithEmptyChannel_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in _tvApi.StreamChannelGamesAsync(""))
            {
            }
        });
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithCountLessThan1_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new TvChannelGamesOptions { Count = 0 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await foreach (var _ in _tvApi.StreamChannelGamesAsync("bullet", options))
            {
            }
        });
    }

    [Fact]
    public async Task StreamChannelGamesAsync_WithCountGreaterThan30_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new TvChannelGamesOptions { Count = 31 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await foreach (var _ in _tvApi.StreamChannelGamesAsync("bullet", options))
            {
            }
        });
    }

    [Fact]
    public async Task StreamChannelGamesAsync_UrlEncodesChannel()
    {
        // Arrange
        var channel = "racing kings";
        var games = new List<GameJson>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s => s.Contains("racing%20kings")),
                It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        await foreach (var _ in _tvApi.StreamChannelGamesAsync(channel))
        {
        }

        // Assert
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s => s.Contains("racing%20kings")),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamChannelGamesAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var channel = "classical";
        var games = new List<GameJson>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>($"/api/tv/{channel}", cts.Token))
            .Returns(ToAsyncEnumerable(games));

        // Act
        await foreach (var _ in _tvApi.StreamChannelGamesAsync(channel, cancellationToken: cts.Token))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>($"/api/tv/{channel}", cts.Token), Times.Once);
    }

    private static TvChannels CreateTestTvChannels()
    {
        return new TvChannels
        {
            Best = new TvGame
            {
                GameId = "bestgame",
                User = new TvUser { Id = "player1", Name = "Player1" },
                Rating = 2500,
                Color = "white"
            },
            Bullet = new TvGame
            {
                GameId = "bulletgame",
                User = new TvUser { Id = "player2", Name = "Player2" },
                Rating = 2400,
                Color = "black"
            },
            Blitz = new TvGame
            {
                GameId = "blitzgame",
                User = new TvUser { Id = "player3", Name = "Player3" },
                Rating = 2300,
                Color = "white"
            },
            Rapid = new TvGame
            {
                GameId = "rapidgame",
                User = new TvUser { Id = "player4", Name = "Player4" },
                Rating = 2200,
                Color = "black"
            }
        };
    }

    private static TvFeedEvent CreateTestFeaturedEvent(string gameId)
    {
        return new TvFeedEvent
        {
            Type = "featured",
            Data = new TvFeedData
            {
                Id = gameId,
                Orientation = "white",
                Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                Players = new List<TvFeedPlayer>
                {
                    new()
                    {
                        Color = "white",
                        User = new TvUser { Id = "player1", Name = "Player1" },
                        Rating = 2500,
                        Seconds = 180
                    },
                    new()
                    {
                        Color = "black",
                        User = new TvUser { Id = "player2", Name = "Player2" },
                        Rating = 2400,
                        Seconds = 180
                    }
                }
            }
        };
    }

    private static TvFeedEvent CreateTestFenEvent()
    {
        return new TvFeedEvent
        {
            Type = "fen",
            Data = new TvFeedData
            {
                Fen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1",
                LastMove = "e2e4",
                WhiteClock = 177,
                BlackClock = 180
            }
        };
    }

    private static GameJson CreateTestGameJson(string id)
    {
        return new GameJson
        {
            Id = id,
            Rated = true,
            Variant = Variant.Standard,
            Speed = Speed.Blitz,
            Status = GameStatus.Started,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items) yield return item;
        await Task.CompletedTask;
    }
}