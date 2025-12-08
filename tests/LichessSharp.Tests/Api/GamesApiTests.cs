using System.Net.Http;
using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Options;
using LichessSharp.Http;
using LichessSharp.Models;
using LichessSharp.Models.Enums;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class GamesApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly GamesApi _gamesApi;

    public GamesApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _gamesApi = new GamesApi(_httpClientMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new GamesApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithGameId_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "q7ZvsdUF";
        var expectedGame = CreateTestGameJson(gameId);
        _httpClientMock
            .Setup(x => x.GetAsync<GameJson>($"/game/export/{gameId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGame);

        // Act
        var result = await _gamesApi.GetAsync(gameId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(gameId);
        _httpClientMock.Verify(x => x.GetAsync<GameJson>($"/game/export/{gameId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithOptions_AppendsQueryParameters()
    {
        // Arrange
        var gameId = "q7ZvsdUF";
        var options = new ExportGameOptions
        {
            Moves = true,
            Clocks = true,
            Evals = false
        };
        var expectedGame = CreateTestGameJson(gameId);
        _httpClientMock
            .Setup(x => x.GetAsync<GameJson>(It.Is<string>(s =>
                s.Contains("/game/export/q7ZvsdUF") &&
                s.Contains("moves=true") &&
                s.Contains("clocks=true") &&
                s.Contains("evals=false")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGame);

        // Act
        var result = await _gamesApi.GetAsync(gameId, options);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_WithNullGameId_ThrowsArgumentException()
    {
        // Act
        var act = () => _gamesApi.GetAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_WithEmptyGameId_ThrowsArgumentException()
    {
        // Act
        var act = () => _gamesApi.GetAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_UrlEncodesGameId()
    {
        // Arrange
        var gameId = "game id";
        var expectedGame = CreateTestGameJson(gameId);
        _httpClientMock
            .Setup(x => x.GetAsync<GameJson>(It.Is<string>(s => s.Contains("game%20id")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGame);

        // Act
        await _gamesApi.GetAsync(gameId);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<GameJson>(It.Is<string>(s => s.Contains("game%20id")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetPgnAsync Tests

    [Fact]
    public async Task GetPgnAsync_WithGameId_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "q7ZvsdUF";
        var expectedPgn = "[Event \"Rated Blitz game\"]\n1. e4 e5 2. Nf3 *";
        _httpClientMock
            .Setup(x => x.GetStringAsync($"/game/export/{gameId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = await _gamesApi.GetPgnAsync(gameId);

        // Assert
        result.Should().Be(expectedPgn);
        _httpClientMock.Verify(x => x.GetStringAsync($"/game/export/{gameId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPgnAsync_WithNullGameId_ThrowsArgumentException()
    {
        // Act
        var act = () => _gamesApi.GetPgnAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region GetCurrentGameAsync Tests

    [Fact]
    public async Task GetCurrentGameAsync_WithUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "DrNykterstein";
        var expectedGame = CreateTestGameJson("abc123");
        _httpClientMock
            .Setup(x => x.GetAsync<GameJson>($"/api/user/{username}/current-game", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGame);

        // Act
        var result = await _gamesApi.GetCurrentGameAsync(username);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAsync<GameJson>($"/api/user/{username}/current-game", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCurrentGameAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _gamesApi.GetCurrentGameAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCurrentGameAsync_UrlEncodesUsername()
    {
        // Arrange
        var username = "user name";
        var expectedGame = CreateTestGameJson("abc123");
        _httpClientMock
            .Setup(x => x.GetAsync<GameJson>(It.Is<string>(s => s.Contains("user%20name")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGame);

        // Act
        await _gamesApi.GetCurrentGameAsync(username);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<GameJson>(It.Is<string>(s => s.Contains("user%20name")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region StreamUserGamesAsync Tests

    [Fact]
    public async Task StreamUserGamesAsync_WithUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "DrNykterstein";
        var games = new List<GameJson>
        {
            CreateTestGameJson("game1"),
            CreateTestGameJson("game2")
        };

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>($"/api/games/user/{username}", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        var results = new List<GameJson>();
        await foreach (var game in _gamesApi.StreamUserGamesAsync(username))
        {
            results.Add(game);
        }

        // Assert
        results.Should().HaveCount(2);
        results[0].Id.Should().Be("game1");
        results[1].Id.Should().Be("game2");
    }

    [Fact]
    public async Task StreamUserGamesAsync_WithOptions_AppendsQueryParameters()
    {
        // Arrange
        var username = "DrNykterstein";
        var options = new ExportUserGamesOptions
        {
            Max = 10,
            Rated = true,
            PerfType = "blitz"
        };
        var games = new List<GameJson>();

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
                s.Contains($"/api/games/user/{username}") &&
                s.Contains("max=10") &&
                s.Contains("rated=true") &&
                s.Contains("perfType=blitz")), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        await foreach (var _ in _gamesApi.StreamUserGamesAsync(username, options))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
            s.Contains("max=10") &&
            s.Contains("rated=true") &&
            s.Contains("perfType=blitz")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamUserGamesAsync_WithNullUsername_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _gamesApi.StreamUserGamesAsync(null!))
            {
            }
        });
    }

    #endregion

    #region StreamByIdsAsync Tests

    [Fact]
    public async Task StreamByIdsAsync_WithGameIds_CallsCorrectEndpoint()
    {
        // Arrange
        var gameIds = new[] { "game1", "game2", "game3" };
        var games = new List<GameJson>
        {
            CreateTestGameJson("game1"),
            CreateTestGameJson("game2"),
            CreateTestGameJson("game3")
        };

        _httpClientMock
            .Setup(x => x.StreamNdjsonPostAsync<GameJson>(
                It.Is<string>(s => s.StartsWith("/api/games/export/_ids")),
                It.Is<HttpContent>(c => c != null),
                It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        var results = new List<GameJson>();
        await foreach (var game in _gamesApi.StreamByIdsAsync(gameIds))
        {
            results.Add(game);
        }

        // Assert
        results.Should().HaveCount(3);
    }

    [Fact]
    public async Task StreamByIdsAsync_WithEmptyIds_YieldsNothing()
    {
        // Arrange
        var gameIds = Array.Empty<string>();

        // Act
        var results = new List<GameJson>();
        await foreach (var game in _gamesApi.StreamByIdsAsync(gameIds))
        {
            results.Add(game);
        }

        // Assert
        results.Should().BeEmpty();
        _httpClientMock.Verify(x => x.StreamNdjsonPostAsync<GameJson>(
            It.IsAny<string>(),
            It.IsAny<HttpContent>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StreamByIdsAsync_WithMoreThan300Ids_ThrowsArgumentException()
    {
        // Arrange
        var gameIds = Enumerable.Range(1, 301).Select(i => $"game{i}").ToList();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in _gamesApi.StreamByIdsAsync(gameIds))
            {
            }
        });
    }

    [Fact]
    public async Task StreamByIdsAsync_WithNullIds_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _gamesApi.StreamByIdsAsync(null!))
            {
            }
        });
    }

    #endregion

    #region StreamGamesByUsersAsync Tests

    [Fact]
    public async Task StreamGamesByUsersAsync_WithUserIds_CallsCorrectEndpoint()
    {
        // Arrange
        var userIds = new[] { "user1", "user2" };
        var games = new List<GameJson> { CreateTestGameJson("game1") };

        _httpClientMock
            .Setup(x => x.StreamNdjsonPostAsync<GameJson>(
                "/api/stream/games-by-users",
                It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        var results = new List<GameJson>();
        await foreach (var game in _gamesApi.StreamGamesByUsersAsync(userIds))
        {
            results.Add(game);
        }

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public async Task StreamGamesByUsersAsync_WithCurrentGames_AppendsQueryParameter()
    {
        // Arrange
        var userIds = new[] { "user1" };
        var games = new List<GameJson>();

        _httpClientMock
            .Setup(x => x.StreamNdjsonPostAsync<GameJson>(
                "/api/stream/games-by-users?withCurrentGames=true",
                It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        await foreach (var _ in _gamesApi.StreamGamesByUsersAsync(userIds, withCurrentGames: true))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonPostAsync<GameJson>(
            "/api/stream/games-by-users?withCurrentGames=true",
            It.IsAny<HttpContent>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamGamesByUsersAsync_WithEmptyUserIds_YieldsNothing()
    {
        // Arrange
        var userIds = Array.Empty<string>();

        // Act
        var results = new List<GameJson>();
        await foreach (var game in _gamesApi.StreamGamesByUsersAsync(userIds))
        {
            results.Add(game);
        }

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task StreamGamesByUsersAsync_WithMoreThan300Users_ThrowsArgumentException()
    {
        // Arrange
        var userIds = Enumerable.Range(1, 301).Select(i => $"user{i}").ToList();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in _gamesApi.StreamGamesByUsersAsync(userIds))
            {
            }
        });
    }

    #endregion

    #region GetOngoingGamesAsync Tests

    [Fact]
    public async Task GetOngoingGamesAsync_WithDefaultCount_CallsCorrectEndpoint()
    {
        // Arrange
        var response = new OngoingGamesResponse
        {
            NowPlaying = new List<OngoingGame>
            {
                CreateTestOngoingGame("game1"),
                CreateTestOngoingGame("game2")
            }
        };

        _httpClientMock
            .Setup(x => x.GetAsync<OngoingGamesResponse>("/api/account/playing?nb=9", It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _gamesApi.GetOngoingGamesAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].GameId.Should().Be("game1");
    }

    [Fact]
    public async Task GetOngoingGamesAsync_WithCustomCount_AppendsQueryParameter()
    {
        // Arrange
        var response = new OngoingGamesResponse { NowPlaying = new List<OngoingGame>() };

        _httpClientMock
            .Setup(x => x.GetAsync<OngoingGamesResponse>("/api/account/playing?nb=25", It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        await _gamesApi.GetOngoingGamesAsync(25);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<OngoingGamesResponse>("/api/account/playing?nb=25", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOngoingGamesAsync_WithCountLessThan1_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => _gamesApi.GetOngoingGamesAsync(0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("count");
    }

    [Fact]
    public async Task GetOngoingGamesAsync_WithCountGreaterThan50_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => _gamesApi.GetOngoingGamesAsync(51);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("count");
    }

    [Fact]
    public async Task GetOngoingGamesAsync_WithNullNowPlaying_ReturnsEmptyList()
    {
        // Arrange
        var response = new OngoingGamesResponse { NowPlaying = null };

        _httpClientMock
            .Setup(x => x.GetAsync<OngoingGamesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _gamesApi.GetOngoingGamesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region ImportPgnAsync Tests

    [Fact]
    public async Task ImportPgnAsync_WithPgn_CallsCorrectEndpoint()
    {
        // Arrange
        var pgn = "[Event \"Test\"]\n1. e4 e5 *";
        var expectedResponse = new ImportGameResponse { Id = "newgame123", Url = "https://lichess.org/newgame123" };

        _httpClientMock
            .Setup(x => x.PostAsync<ImportGameResponse>("/api/import", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _gamesApi.ImportPgnAsync(pgn);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("newgame123");
        result.Url.Should().Be("https://lichess.org/newgame123");
    }

    [Fact]
    public async Task ImportPgnAsync_WithNullPgn_ThrowsArgumentException()
    {
        // Act
        var act = () => _gamesApi.ImportPgnAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ImportPgnAsync_WithEmptyPgn_ThrowsArgumentException()
    {
        // Act
        var act = () => _gamesApi.ImportPgnAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region CancellationToken Tests

    [Fact]
    public async Task GetAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var expectedGame = CreateTestGameJson("game1");
        _httpClientMock
            .Setup(x => x.GetAsync<GameJson>(It.IsAny<string>(), cts.Token))
            .ReturnsAsync(expectedGame);

        // Act
        await _gamesApi.GetAsync("game1", cancellationToken: cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<GameJson>(It.IsAny<string>(), cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetOngoingGamesAsync_PassesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var response = new OngoingGamesResponse { NowPlaying = new List<OngoingGame>() };
        _httpClientMock
            .Setup(x => x.GetAsync<OngoingGamesResponse>(It.IsAny<string>(), cts.Token))
            .ReturnsAsync(response);

        // Act
        await _gamesApi.GetOngoingGamesAsync(cancellationToken: cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<OngoingGamesResponse>(It.IsAny<string>(), cts.Token), Times.Once);
    }

    #endregion

    #region ExportGameOptions Tests

    [Fact]
    public async Task GetAsync_WithAllOptions_AppendsAllQueryParameters()
    {
        // Arrange
        var gameId = "test123";
        var options = new ExportGameOptions
        {
            Moves = true,
            PgnInJson = true,
            Tags = false,
            Clocks = true,
            Evals = true,
            Accuracy = true,
            Opening = true,
            Division = false,
            Literate = true,
            Players = "lichess.org"
        };
        var expectedGame = CreateTestGameJson(gameId);

        _httpClientMock
            .Setup(x => x.GetAsync<GameJson>(It.Is<string>(s =>
                s.Contains("moves=true") &&
                s.Contains("pgnInJson=true") &&
                s.Contains("tags=false") &&
                s.Contains("clocks=true") &&
                s.Contains("evals=true") &&
                s.Contains("accuracy=true") &&
                s.Contains("opening=true") &&
                s.Contains("division=false") &&
                s.Contains("literate=true") &&
                s.Contains("players=lichess.org")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGame);

        // Act
        await _gamesApi.GetAsync(gameId, options);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<GameJson>(It.Is<string>(s =>
            s.Contains("moves=true") &&
            s.Contains("players=lichess.org")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region ExportUserGamesOptions Tests

    [Fact]
    public async Task StreamUserGamesAsync_WithDateRange_AppendsTimestamps()
    {
        // Arrange
        var username = "testuser";
        var since = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var until = new DateTimeOffset(2023, 12, 31, 23, 59, 59, TimeSpan.Zero);
        var options = new ExportUserGamesOptions
        {
            Since = since,
            Until = until
        };
        var games = new List<GameJson>();

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
                s.Contains($"since={since.ToUnixTimeMilliseconds()}") &&
                s.Contains($"until={until.ToUnixTimeMilliseconds()}")), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        await foreach (var _ in _gamesApi.StreamUserGamesAsync(username, options))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
            s.Contains("since=") && s.Contains("until=")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamUserGamesAsync_WithVsAndColor_AppendsParameters()
    {
        // Arrange
        var username = "testuser";
        var options = new ExportUserGamesOptions
        {
            Vs = "opponent",
            Color = "white"
        };
        var games = new List<GameJson>();

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
                s.Contains("vs=opponent") &&
                s.Contains("color=white")), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        await foreach (var _ in _gamesApi.StreamUserGamesAsync(username, options))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
            s.Contains("vs=opponent") && s.Contains("color=white")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamUserGamesAsync_WithSortAndAnalysed_AppendsParameters()
    {
        // Arrange
        var username = "testuser";
        var options = new ExportUserGamesOptions
        {
            Sort = "dateAsc",
            Analysed = true,
            Ongoing = false,
            Finished = true
        };
        var games = new List<GameJson>();

        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
                s.Contains("sort=dateAsc") &&
                s.Contains("analysed=true") &&
                s.Contains("ongoing=false") &&
                s.Contains("finished=true")), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(games));

        // Act
        await foreach (var _ in _gamesApi.StreamUserGamesAsync(username, options))
        {
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>(It.Is<string>(s =>
            s.Contains("sort=dateAsc")), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Helper Methods

    private static GameJson CreateTestGameJson(string id) => new()
    {
        Id = id,
        Rated = true,
        Variant = Variant.Standard,
        Speed = Speed.Blitz,
        Perf = "blitz",
        Status = GameStatus.Started,
        CreatedAt = DateTimeOffset.UtcNow
    };

    private static OngoingGame CreateTestOngoingGame(string gameId) => new()
    {
        FullId = $"{gameId}abcd",
        GameId = gameId,
        Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
        Color = Color.White,
        IsMyTurn = true,
        Opponent = new OngoingGameOpponent { Id = "opponent", Username = "Opponent" }
    };

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }
        await Task.CompletedTask;
    }

    #endregion
}
