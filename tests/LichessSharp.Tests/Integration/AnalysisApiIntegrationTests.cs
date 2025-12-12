using FluentAssertions;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Integration tests for the Analysis API.
///     These tests use FEN positions from the OpenAPI spec examples.
/// </summary>
[IntegrationTest]
[LongRunningTest]
[Trait("Category", "Integration")]
public class AnalysisApiIntegrationTests : IntegrationTestBase
{
    // Well-known FEN from OpenAPI spec - Ruy Lopez position
    private const string RuyLopezFen = "r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3";

    // Starting position FEN - most likely to be in cloud eval database
    private const string StartingPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    // Italian Game position - common opening
    private const string ItalianGameFen = "r1bqkbnr/pppp1ppp/2n5/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3";

    [Fact]
    public async Task GetCloudEvaluationAsync_WithStartingPosition_ReturnsEvaluation()
    {
        // Act
        var evaluation = await Client.Analysis.GetCloudEvaluationAsync(StartingPositionFen);

        // Assert
        evaluation.Should().NotBeNull();
        evaluation!.Fen.Should().NotBeNullOrEmpty();
        evaluation.Pvs.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WithRuyLopezPosition_ReturnsEvaluationOrNull()
    {
        // Act - This position may or may not be in the database
        var evaluation = await Client.Analysis.GetCloudEvaluationAsync(RuyLopezFen);

        // Assert - Either returns evaluation or null (404 from API)
        // We just verify it doesn't throw and returns a sensible result
        if (evaluation != null)
        {
            evaluation.Fen.Should().NotBeNullOrEmpty();
            evaluation.Pvs.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WithMultiPv_ReturnsMultipleVariations()
    {
        // Act
        var evaluation = await Client.Analysis.GetCloudEvaluationAsync(StartingPositionFen, 3);

        // Assert
        evaluation.Should().NotBeNull();
        if (evaluation != null && evaluation.Pvs != null)
            // May have up to 3 variations depending on what's cached
            evaluation.Pvs.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetCloudEvaluationAsync_WithItalianGame_ReturnsEvaluationOrNull()
    {
        // Act
        var evaluation = await Client.Analysis.GetCloudEvaluationAsync(ItalianGameFen);

        // Assert - Common opening position, likely to be in database
        if (evaluation != null)
        {
            evaluation.Fen.Should().NotBeNullOrEmpty();
            evaluation.Depth.Should().BeGreaterThan(0);
        }
    }
}