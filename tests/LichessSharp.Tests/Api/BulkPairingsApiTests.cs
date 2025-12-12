using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models.Games;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class BulkPairingsApiTests
{
    private readonly BulkPairingsApi _bulkPairingsApi;
    private readonly Mock<ILichessHttpClient> _httpClientMock;

    public BulkPairingsApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _bulkPairingsApi = new BulkPairingsApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new BulkPairingsApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task GetAllAsync_CallsCorrectEndpoint()
    {
        // Arrange - Lichess returns {"bulks": [...]} wrapper object
        var expectedResponse = new BulkPairingListResponse
        {
            Bulks = new List<BulkPairing> { CreateTestBulkPairing("test1") }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<BulkPairingListResponse>("/api/bulk-pairing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _bulkPairingsApi.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Id.Should().Be("test1");
        _httpClientMock.Verify(
            x => x.GetAsync<BulkPairingListResponse>("/api/bulk-pairing", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenEmptyBulksArray()
    {
        // Arrange - Lichess returns {"bulks": []} when no bulk pairings exist
        var expectedResponse = new BulkPairingListResponse
        {
            Bulks = new List<BulkPairing>()
        };
        _httpClientMock
            .Setup(x => x.GetAsync<BulkPairingListResponse>("/api/bulk-pairing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _bulkPairingsApi.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenBulksIsNull()
    {
        // Arrange - Handle null Bulks property
        var expectedResponse = new BulkPairingListResponse
        {
            Bulks = null
        };
        _httpClientMock
            .Setup(x => x.GetAsync<BulkPairingListResponse>("/api/bulk-pairing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _bulkPairingsApi.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_PassesToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var expectedResponse = new BulkPairingListResponse { Bulks = new List<BulkPairing>() };
        _httpClientMock
            .Setup(x => x.GetAsync<BulkPairingListResponse>("/api/bulk-pairing", cts.Token))
            .ReturnsAsync(expectedResponse);

        // Act
        await _bulkPairingsApi.GetAllAsync(cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<BulkPairingListResponse>("/api/bulk-pairing", cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResult = CreateTestBulkPairing("test123");
        _httpClientMock
            .Setup(x => x.GetAsync<BulkPairing>("/api/bulk-pairing/test123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _bulkPairingsApi.GetAsync("test123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("test123");
        _httpClientMock.Verify(x => x.GetAsync<BulkPairing>("/api/bulk-pairing/test123", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithNullId_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _bulkPairingsApi.GetAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _bulkPairingsApi.GetAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_WithWhitespaceId_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _bulkPairingsApi.GetAsync("   ");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_EscapesIdInUrl()
    {
        // Arrange
        var expectedResult = CreateTestBulkPairing("test/special");
        _httpClientMock
            .Setup(x => x.GetAsync<BulkPairing>("/api/bulk-pairing/test%2Fspecial", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        await _bulkPairingsApi.GetAsync("test/special");

        // Assert
        _httpClientMock.Verify(
            x => x.GetAsync<BulkPairing>("/api/bulk-pairing/test%2Fspecial", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var options = new BulkPairingCreateOptions
        {
            Players = "token1:token2",
            ClockLimit = 600,
            ClockIncrement = 5
        };
        var expectedResult = CreateTestBulkPairing("new123");
        _httpClientMock
            .Setup(x => x.PostAsync<BulkPairing>("/api/bulk-pairing", It.IsAny<FormUrlEncodedContent>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _bulkPairingsApi.CreateAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("new123");
        _httpClientMock.Verify(
            x => x.PostAsync<BulkPairing>("/api/bulk-pairing", It.IsAny<FormUrlEncodedContent>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act
        var act = async () => await _bulkPairingsApi.CreateAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateAsync_WithNullPlayers_ThrowsArgumentException()
    {
        // Arrange
        var options = new BulkPairingCreateOptions { Players = null! };

        // Act
        var act = async () => await _bulkPairingsApi.CreateAsync(options);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateAsync_WithEmptyPlayers_ThrowsArgumentException()
    {
        // Arrange
        var options = new BulkPairingCreateOptions { Players = "" };

        // Act
        var act = async () => await _bulkPairingsApi.CreateAsync(options);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateAsync_WithAllOptions_IncludesAllParameters()
    {
        // Arrange
        var options = new BulkPairingCreateOptions
        {
            Players = "token1:token2,token3:token4",
            ClockLimit = 600,
            ClockIncrement = 5,
            Days = 3,
            PairAt = 1700000000000,
            StartClocksAt = 1700000600000,
            Rated = true,
            Variant = "chess960",
            Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
            Message = "Your game is ready!",
            Rules = "noAbort,noRematch"
        };
        var expectedResult = CreateTestBulkPairing("new123");

        HttpContent? capturedContent = null;
        _httpClientMock
            .Setup(x => x.PostAsync<BulkPairing>("/api/bulk-pairing", It.IsAny<FormUrlEncodedContent>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, HttpContent, CancellationToken>((_, content, _) => capturedContent = content)
            .ReturnsAsync(expectedResult);

        // Act
        await _bulkPairingsApi.CreateAsync(options);

        // Assert
        capturedContent.Should().NotBeNull();
        var formData = await capturedContent!.ReadAsStringAsync();
        formData.Should().Contain("players=");
        formData.Should().Contain("clock.limit=600");
        formData.Should().Contain("clock.increment=5");
        formData.Should().Contain("days=3");
        formData.Should().Contain("pairAt=1700000000000");
        formData.Should().Contain("startClocksAt=1700000600000");
        formData.Should().Contain("rated=true");
        formData.Should().Contain("variant=chess960");
        formData.Should().Contain("fen=");
        formData.Should().Contain("message=");
        formData.Should().Contain("rules=");
    }

    [Fact]
    public async Task StartClocksAsync_CallsCorrectEndpoint()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.PostNoContentAsync("/api/bulk-pairing/test123/start-clocks", null,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bulkPairingsApi.StartClocksAsync("test123");

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostNoContentAsync("/api/bulk-pairing/test123/start-clocks", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StartClocksAsync_WithNullId_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _bulkPairingsApi.StartClocksAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task StartClocksAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _bulkPairingsApi.StartClocksAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CancelAsync_CallsCorrectEndpoint()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.DeleteNoContentAsync("/api/bulk-pairing/test123", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bulkPairingsApi.CancelAsync("test123");

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.DeleteNoContentAsync("/api/bulk-pairing/test123", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelAsync_WithNullId_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _bulkPairingsApi.CancelAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CancelAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _bulkPairingsApi.CancelAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportGamesPgnAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedPgn = "[White \"Player1\"]\n[Black \"Player2\"]\n1. e4 e5 2. Nf3 Nc6 *";
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync("/api/bulk-pairing/test123/games", "application/x-chess-pgn",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = await _bulkPairingsApi.ExportGamesAsync("test123");

        // Assert
        result.Should().Be(expectedPgn);
        _httpClientMock.Verify(
            x => x.GetStringWithAcceptAsync("/api/bulk-pairing/test123/games", "application/x-chess-pgn",
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExportGamesPgnAsync_WithNullId_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _bulkPairingsApi.ExportGamesAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportGamesPgnAsync_WithOptions_IncludesQueryParameters()
    {
        // Arrange
        var options = new BulkPairingExportOptions
        {
            Moves = true,
            PgnInJson = true,
            Tags = true,
            Clocks = true,
            Opening = true
        };
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync(
                It.Is<string>(s => s.StartsWith("/api/bulk-pairing/test123/games?")),
                "application/x-chess-pgn",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("");

        // Act
        await _bulkPairingsApi.ExportGamesAsync("test123", options);

        // Assert
        _httpClientMock.Verify(x => x.GetStringWithAcceptAsync(
            It.Is<string>(s =>
                s.Contains("moves=true") &&
                s.Contains("pgnInJson=true") &&
                s.Contains("tags=true") &&
                s.Contains("clocks=true") &&
                s.Contains("opening=true")),
            "application/x-chess-pgn",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamGamesAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var games = new List<GameJson>
        {
            new() { Id = "game1" },
            new() { Id = "game2" }
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>("/api/bulk-pairing/test123/games", It.IsAny<CancellationToken>()))
            .Returns(games.ToAsyncEnumerable());

        // Act
        var result = new List<GameJson>();
        await foreach (var game in _bulkPairingsApi.StreamGamesAsync("test123")) result.Add(game);

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be("game1");
        result[1].Id.Should().Be("game2");
    }

    [Fact]
    public async Task StreamGamesAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in _bulkPairingsApi.StreamGamesAsync(null!))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task StreamGamesAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in _bulkPairingsApi.StreamGamesAsync(""))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task StreamGamesAsync_WithOptions_IncludesQueryParameters()
    {
        // Arrange
        var options = new BulkPairingExportOptions
        {
            Moves = false,
            Opening = true
        };
        var games = new List<GameJson>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<GameJson>(
                It.Is<string>(s => s.Contains("moves=false") && s.Contains("opening=true")),
                It.IsAny<CancellationToken>()))
            .Returns(games.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _bulkPairingsApi.StreamGamesAsync("test123", options))
        {
            // Just iterate
        }

        // Assert
        _httpClientMock.Verify(x => x.StreamNdjsonAsync<GameJson>(
            It.Is<string>(s => s.Contains("moves=false") && s.Contains("opening=true")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static BulkPairing CreateTestBulkPairing(string id)
    {
        return new BulkPairing
        {
            Id = id,
            Games = new List<BulkPairingGame>
            {
                new()
                {
                    Id = $"game_{id}_1",
                    White = "player1",
                    Black = "player2"
                }
            },
            Variant = "standard",
            Clock = new BulkPairingClock
            {
                Limit = 600,
                Increment = 5
            },
            PairAt = 1700000000000,
            Rated = false,
            ScheduledAt = 1699999000000
        };
    }
}