using FluentAssertions;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for the Tablebase API.
/// These tests use FEN positions from the OpenAPI spec examples.
/// Endpoint: tablebase.lichess.ovh
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class TablebaseApiIntegrationTests : IntegrationTestBase
{
    // KPK endgame position from OpenAPI spec example
    // FEN: 4k3/6KP/8/8/8/8/7p/8 w - - 0 1
    // White king on g7, white pawn on h7, black king on e8, black pawn on h2
    private const string KpkEndgameFen = "4k3/6KP/8/8/8/8/7p/8 w - - 0 1";

    // Simple KQK endgame - White: Ke1, Qd1; Black: Ke8
    private const string KqkEndgameFen = "4k3/8/8/8/8/8/8/3QK3 w - - 0 1";

    // KRK endgame - White: Ke1, Ra1; Black: Ke8
    private const string KrkEndgameFen = "4k3/8/8/8/8/8/8/R3K3 w - - 0 1";

    // KPK simple - White: Ke1, Pe2; Black: Ke8
    private const string SimpleKpkFen = "4k3/8/8/8/8/8/4P3/4K3 w - - 0 1";
    [Fact]
    public async Task LookupAsync_WithKqkEndgame_ReturnsWinningPosition()
    {
        // Act
        var result = await Client.Tablebase.LookupAsync(KqkEndgameFen);

        // Assert
        result.Should().NotBeNull();
        result.Category.Should().NotBeNullOrEmpty();
        result.Moves.Should().NotBeNull();
        // KQK is always winning for the side with the queen
        result.Category.Should().Be("win");
    }

    [Fact]
    public async Task LookupAsync_WithKrkEndgame_ReturnsWinningPosition()
    {
        // Act
        var result = await Client.Tablebase.LookupAsync(KrkEndgameFen);

        // Assert
        result.Should().NotBeNull();
        result.Category.Should().NotBeNullOrEmpty();
        // KRK is always winning for the side with the rook
        result.Category.Should().Be("win");
        result.Moves.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LookupAsync_WithKpkEndgame_ReturnsMoves()
    {
        // Act
        var result = await Client.Tablebase.LookupAsync(SimpleKpkFen);

        // Assert
        result.Should().NotBeNull();
        result.Moves.Should().NotBeNull();
        result.Moves.Should().NotBeEmpty();
        result.Moves.Should().AllSatisfy(m =>
        {
            m.Uci.Should().NotBeNullOrEmpty();
            m.San.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task LookupAsync_WithOpenApiExamplePosition_ReturnsResult()
    {
        // This is the example from the OpenAPI spec
        // Act
        var result = await Client.Tablebase.LookupAsync(KpkEndgameFen);

        // Assert
        result.Should().NotBeNull();
        result.Category.Should().NotBeNullOrEmpty();
        result.Moves.Should().NotBeNull();
    }

    [Fact]
    public async Task LookupAsync_ReturnsMovesWithCorrectStructure()
    {
        // Act
        var result = await Client.Tablebase.LookupAsync(KqkEndgameFen);

        // Assert
        result.Should().NotBeNull();
        result.Moves.Should().NotBeNull();
        result.Moves.Should().AllSatisfy(move =>
        {
            move.Uci.Should().NotBeNullOrEmpty();
            move.San.Should().NotBeNullOrEmpty();
            // Category should indicate the result after this move
            move.Category.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task LookupAtomicAsync_WithSimplePosition_ReturnsResult()
    {
        // Atomic chess tablebase - use a simple position
        // Note: Atomic tablebases may have different coverage
        var atomicFen = "4k3/8/8/8/8/8/8/4K2R w - - 0 1";

        // Act
        var result = await Client.Tablebase.LookupAtomicAsync(atomicFen);

        // Assert
        result.Should().NotBeNull();
        result.Category.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LookupAntichessAsync_WithSimplePosition_ReturnsResult()
    {
        // Antichess tablebase - use a simple position
        // In antichess, you must capture if possible and the goal is to lose all pieces
        var antichessFen = "4k3/8/8/8/8/8/8/4K3 w - - 0 1";

        // Act
        var result = await Client.Tablebase.LookupAntichessAsync(antichessFen);

        // Assert
        result.Should().NotBeNull();
        result.Category.Should().NotBeNullOrEmpty();
    }

}
