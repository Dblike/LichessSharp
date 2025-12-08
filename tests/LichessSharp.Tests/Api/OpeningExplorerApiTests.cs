using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Http;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class OpeningExplorerApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly OpeningExplorerApi _explorerApi;
    private readonly Uri _baseAddress = new("https://explorer.lichess.ovh");

    public OpeningExplorerApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _explorerApi = new OpeningExplorerApi(_httpClientMock.Object, _baseAddress);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new OpeningExplorerApi(null!, _baseAddress);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public void Constructor_WithNullBaseAddress_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new OpeningExplorerApi(_httpClientMock.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("explorerBaseAddress");
    }

    #endregion

    #region GetMastersAsync Tests

    [Fact]
    public async Task GetMastersAsync_WithFen_CallsCorrectEndpoint()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        var expectedResult = CreateTestExplorerResult();
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<ExplorerResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _explorerApi.GetMastersAsync(fen);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<ExplorerResult>(
            It.Is<Uri>(u =>
                u.ToString().StartsWith("https://explorer.lichess.ovh/masters?fen=") &&
                u.ToString().Contains("rnbqkbnr")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMastersAsync_WithOptions_IncludesQueryParameters()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1";
        var options = new ExplorerOptions
        {
            Moves = 5,
            TopGames = 10,
            Since = 2000,
            Until = 2023
        };
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<ExplorerResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestExplorerResult());

        // Act
        await _explorerApi.GetMastersAsync(fen, options);

        // Assert
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<ExplorerResult>(
            It.Is<Uri>(u =>
                u.ToString().Contains("moves=5") &&
                u.ToString().Contains("topGames=10") &&
                u.ToString().Contains("since=2000") &&
                u.ToString().Contains("until=2023")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMastersAsync_WithNullFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _explorerApi.GetMastersAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region GetLichessAsync Tests

    [Fact]
    public async Task GetLichessAsync_WithFen_CallsCorrectEndpoint()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<ExplorerResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestExplorerResult());

        // Act
        await _explorerApi.GetLichessAsync(fen);

        // Assert
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<ExplorerResult>(
            It.Is<Uri>(u => u.ToString().StartsWith("https://explorer.lichess.ovh/lichess?fen=")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLichessAsync_WithOptions_IncludesQueryParameters()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1";
        var options = new ExplorerOptions
        {
            Variant = "standard",
            Speeds = ["blitz", "rapid"],
            Ratings = [2000, 2200, 2500],
            SinceMonth = "2020-01",
            UntilMonth = "2023-12"
        };
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<ExplorerResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestExplorerResult());

        // Act
        await _explorerApi.GetLichessAsync(fen, options);

        // Assert
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<ExplorerResult>(
            It.Is<Uri>(u =>
                u.ToString().Contains("variant=standard") &&
                u.ToString().Contains("speeds=blitz,rapid") &&
                u.ToString().Contains("ratings=2000,2200,2500") &&
                u.ToString().Contains("since=2020-01") &&
                u.ToString().Contains("until=2023-12")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLichessAsync_WithNullFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _explorerApi.GetLichessAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region GetPlayerAsync Tests

    [Fact]
    public async Task GetPlayerAsync_WithFenAndPlayer_CallsCorrectEndpoint()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        var player = "DrNykterstein";
        _httpClientMock
            .Setup(x => x.GetAbsoluteNdjsonLastAsync<ExplorerResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestExplorerResult());

        // Act
        await _explorerApi.GetPlayerAsync(fen, player);

        // Assert
        _httpClientMock.Verify(x => x.GetAbsoluteNdjsonLastAsync<ExplorerResult>(
            It.Is<Uri>(u =>
                u.ToString().StartsWith("https://explorer.lichess.ovh/player?fen=") &&
                u.ToString().Contains("player=DrNykterstein") &&
                u.ToString().Contains("color=white")), // Default color
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPlayerAsync_WithColorOption_IncludesColor()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1";
        var player = "DrNykterstein";
        var options = new ExplorerOptions { Color = "black" };
        _httpClientMock
            .Setup(x => x.GetAbsoluteNdjsonLastAsync<ExplorerResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestExplorerResult());

        // Act
        await _explorerApi.GetPlayerAsync(fen, player, options);

        // Assert
        _httpClientMock.Verify(x => x.GetAbsoluteNdjsonLastAsync<ExplorerResult>(
            It.Is<Uri>(u => u.ToString().Contains("color=black")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPlayerAsync_WithNullFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _explorerApi.GetPlayerAsync(null!, "player");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetPlayerAsync_WithNullPlayer_ThrowsArgumentException()
    {
        // Act
        var act = () => _explorerApi.GetPlayerAsync("fen", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region URL Encoding Tests

    [Fact]
    public async Task GetMastersAsync_UrlEncodesFen()
    {
        // Arrange - FEN contains slashes and spaces that need encoding
        var fen = "r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3";
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<ExplorerResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestExplorerResult());

        // Act
        await _explorerApi.GetMastersAsync(fen);

        // Assert - Uri.EscapeDataString encodes slashes as %2F
        // (Note: Uri.ToString() may decode some characters back, so we check AbsoluteUri instead)
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<ExplorerResult>(
            It.Is<Uri>(u => u.AbsoluteUri.Contains("%2F")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Helper Methods

    private static ExplorerResult CreateTestExplorerResult() => new()
    {
        White = 1000,
        Draws = 500,
        Black = 800,
        Moves = new List<ExplorerMove>
        {
            new() { Uci = "e2e4", San = "e4", White = 500, Draws = 200, Black = 300 },
            new() { Uci = "d2d4", San = "d4", White = 400, Draws = 250, Black = 350 }
        },
        TopGames = new List<ExplorerGame>
        {
            new() { Id = "game1", Winner = "white", Year = 2023 }
        },
        Opening = new ExplorerOpening { Eco = "B00", Name = "King's Pawn Opening" }
    };

    #endregion
}
