using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Exceptions;
using LichessSharp.Http;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class AnalysisApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly AnalysisApi _analysisApi;

    public AnalysisApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _analysisApi = new AnalysisApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new AnalysisApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }



    [Fact]
    public async Task GetCloudEvaluationAsync_WithFen_CallsCorrectEndpoint()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        var expectedEval = CreateTestCloudEvaluation(fen);
        _httpClientMock
            .Setup(x => x.GetAsync<CloudEvaluation>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEval);

        // Act
        var result = await _analysisApi.GetCloudEvaluationAsync(fen);

        // Assert
        result.Should().NotBeNull();
        result!.Fen.Should().Be(fen);
        _httpClientMock.Verify(x => x.GetAsync<CloudEvaluation>(
            It.Is<string>(s => s.StartsWith("/api/cloud-eval?fen=") && s.Contains("rnbqkbnr")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WithMultiPv_IncludesInQueryString()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1";
        _httpClientMock
            .Setup(x => x.GetAsync<CloudEvaluation>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestCloudEvaluation(fen));

        // Act
        await _analysisApi.GetCloudEvaluationAsync(fen, multiPv: 3);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<CloudEvaluation>(
            It.Is<string>(s => s.Contains("multiPv=3")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WithVariant_IncludesInQueryString()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1";
        _httpClientMock
            .Setup(x => x.GetAsync<CloudEvaluation>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestCloudEvaluation(fen));

        // Act
        await _analysisApi.GetCloudEvaluationAsync(fen, variant: "chess960");

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<CloudEvaluation>(
            It.Is<string>(s => s.Contains("variant=chess960")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WithAllParameters_IncludesAllInQueryString()
    {
        // Arrange
        var fen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1";
        _httpClientMock
            .Setup(x => x.GetAsync<CloudEvaluation>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestCloudEvaluation(fen));

        // Act
        await _analysisApi.GetCloudEvaluationAsync(fen, multiPv: 5, variant: "atomic");

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<CloudEvaluation>(
            It.Is<string>(s =>
                s.Contains("/api/cloud-eval?fen=") &&
                s.Contains("multiPv=5") &&
                s.Contains("variant=atomic")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var fen = "some/rare/position";
        _httpClientMock
            .Setup(x => x.GetAsync<CloudEvaluation>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new LichessNotFoundException("Position not found", "No cloud evaluation available"));

        // Act
        var result = await _analysisApi.GetCloudEvaluationAsync(fen);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WithNullFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _analysisApi.GetCloudEvaluationAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WithEmptyFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _analysisApi.GetCloudEvaluationAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WithWhitespaceFen_ThrowsArgumentException()
    {
        // Act
        var act = () => _analysisApi.GetCloudEvaluationAsync("   ");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_UrlEncodesFen()
    {
        // Arrange - FEN contains spaces and special characters that need encoding
        var fen = "r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3";
        _httpClientMock
            .Setup(x => x.GetAsync<CloudEvaluation>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestCloudEvaluation(fen));

        // Act
        await _analysisApi.GetCloudEvaluationAsync(fen);

        // Assert - The FEN should be URL encoded (spaces become %20, slashes become %2F)
        _httpClientMock.Verify(x => x.GetAsync<CloudEvaluation>(
            It.Is<string>(s =>
                s.Contains("%2F") && // Slashes encoded
                !s.Contains(" ")),   // No raw spaces
            It.IsAny<CancellationToken>()), Times.Once);
    }



    private static CloudEvaluation CreateTestCloudEvaluation(string fen) => new()
    {
        Fen = fen,
        Depth = 40,
        Knodes = 123456789,
        Pvs = new List<PrincipalVariation>
        {
            new() { Moves = "e2e4 e7e5 g1f3", Cp = 25 },
            new() { Moves = "d2d4 d7d5", Cp = 15 }
        }
    };

}
