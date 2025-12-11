using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class TablebaseApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly TablebaseApi _tablebaseApi;
    private readonly Uri _baseAddress = new("https://tablebase.lichess.ovh");

    public TablebaseApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _tablebaseApi = new TablebaseApi(_httpClientMock.Object, _baseAddress);
    }
    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new TablebaseApi(null!, _baseAddress);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public void Constructor_WithNullBaseAddress_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new TablebaseApi(_httpClientMock.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("tablebaseBaseAddress");
    }

    [Fact]
    public async Task LookupAsync_WithFen_CallsCorrectEndpoint()
    {
        // Arrange
        var fen = "4k3/6KP/8/8/8/8/7p/8 w - - 0 1";
        var expectedResult = CreateTestTablebaseResult();
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<TablebaseResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _tablebaseApi.LookupAsync(fen);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<TablebaseResult>(
            It.Is<Uri>(u =>
                u.ToString().StartsWith("https://tablebase.lichess.ovh/standard?fen=")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LookupAsync_WithNullFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _tablebaseApi.LookupAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task LookupAsync_WithEmptyFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _tablebaseApi.LookupAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task LookupAsync_WithWhitespaceFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _tablebaseApi.LookupAsync("   ");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task LookupAtomicAsync_WithFen_CallsCorrectEndpoint()
    {
        // Arrange
        var fen = "4k3/6KP/8/8/8/8/7p/8 w - - 0 1";
        var expectedResult = CreateTestTablebaseResult();
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<TablebaseResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _tablebaseApi.LookupAtomicAsync(fen);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<TablebaseResult>(
            It.Is<Uri>(u =>
                u.ToString().StartsWith("https://tablebase.lichess.ovh/atomic?fen=")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LookupAtomicAsync_WithNullFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _tablebaseApi.LookupAtomicAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task LookupAntichessAsync_WithFen_CallsCorrectEndpoint()
    {
        // Arrange
        var fen = "4k3/6KP/8/8/8/8/7p/8 w - - 0 1";
        var expectedResult = CreateTestTablebaseResult();
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<TablebaseResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _tablebaseApi.LookupAntichessAsync(fen);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<TablebaseResult>(
            It.Is<Uri>(u =>
                u.ToString().StartsWith("https://tablebase.lichess.ovh/antichess?fen=")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LookupAntichessAsync_WithNullFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _tablebaseApi.LookupAntichessAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task LookupAsync_UrlEncodesFen()
    {
        // Arrange - FEN contains slashes and spaces that need encoding
        var fen = "4k3/6KP/8/8/8/8/7p/8 w - - 0 1";
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<TablebaseResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestTablebaseResult());

        // Act
        await _tablebaseApi.LookupAsync(fen);

        // Assert - Uri.EscapeDataString encodes slashes as %2F
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<TablebaseResult>(
            It.Is<Uri>(u => u.AbsoluteUri.Contains("%2F")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LookupAsync_ReturnsResultWithAllProperties()
    {
        // Arrange
        var fen = "4k3/6KP/8/8/8/8/7p/8 w - - 0 1";
        var expectedResult = CreateTestTablebaseResult();
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<TablebaseResult>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _tablebaseApi.LookupAsync(fen);

        // Assert
        result.Category.Should().Be("win");
        result.Dtz.Should().Be(1);
        result.PreciseDtz.Should().Be(1);
        result.Dtm.Should().Be(17);
        result.Checkmate.Should().BeFalse();
        result.Stalemate.Should().BeFalse();
        result.InsufficientMaterial.Should().BeFalse();
        result.Moves.Should().HaveCount(2);
        result.Moves![0].Uci.Should().Be("h7h8q");
        result.Moves![0].San.Should().Be("h8=Q+");
        result.Moves![0].Category.Should().Be("loss");
        result.Moves![0].Zeroing.Should().BeTrue();
    }

    [Fact]
    public async Task LookupAsync_PassesCancellationToken()
    {
        // Arrange
        var fen = "4k3/6KP/8/8/8/8/7p/8 w - - 0 1";
        var cts = new CancellationTokenSource();
        _httpClientMock
            .Setup(x => x.GetAbsoluteAsync<TablebaseResult>(It.IsAny<Uri>(), cts.Token))
            .ReturnsAsync(CreateTestTablebaseResult());

        // Act
        await _tablebaseApi.LookupAsync(fen, cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAbsoluteAsync<TablebaseResult>(
            It.IsAny<Uri>(),
            cts.Token), Times.Once);
    }

    private static TablebaseResult CreateTestTablebaseResult() => new()
    {
        Category = "win",
        Dtz = 1,
        PreciseDtz = 1,
        Dtm = 17,
        Checkmate = false,
        Stalemate = false,
        InsufficientMaterial = false,
        Moves = new List<TablebaseMove>
        {
            new()
            {
                Uci = "h7h8q",
                San = "h8=Q+",
                Category = "loss",
                Dtz = -2,
                PreciseDtz = -2,
                Dtm = -16,
                Zeroing = true,
                Checkmate = false,
                Stalemate = false
            },
            new()
            {
                Uci = "g7g8",
                San = "Kg8",
                Category = "draw",
                Dtz = 0,
                Zeroing = false,
                Checkmate = false,
                Stalemate = false
            }
        }
    };

}
