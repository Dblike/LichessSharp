using FluentAssertions;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for the FIDE API.
/// These tests use real FIDE player IDs from well-known players.
/// </summary>
[Trait("Category", "Integration")]
public class FideApiIntegrationTests : IntegrationTestBase
{
    // Well-known FIDE player IDs
    private const int MagnusCarlsenFideId = 1503014;
    private const int FabianoCaruanaFideId = 2020009;


    [Fact]
    public async Task GetPlayerAsync_WithMagnusCarlsen_ReturnsPlayer()
    {
        // Act
        var player = await Client.Fide.GetPlayerAsync(MagnusCarlsenFideId);

        // Assert
        player.Should().NotBeNull();
        player.Id.Should().Be(MagnusCarlsenFideId);
        player.Name.Should().Contain("Carlsen");
        player.Title.Should().Be("GM");
        player.Federation.Should().Be("NOR");
    }

    [Fact]
    public async Task GetPlayerAsync_WithFabianoCaruana_ReturnsPlayer()
    {
        // Act
        var player = await Client.Fide.GetPlayerAsync(FabianoCaruanaFideId);

        // Assert
        player.Should().NotBeNull();
        player.Id.Should().Be(FabianoCaruanaFideId);
        player.Name.Should().Contain("Caruana");
        player.Title.Should().Be("GM");
    }

    [Fact]
    public async Task GetPlayerAsync_ReturnsRatings()
    {
        // Act
        var player = await Client.Fide.GetPlayerAsync(MagnusCarlsenFideId);

        // Assert
        player.Should().NotBeNull();
        // Magnus Carlsen should have ratings in all time controls
        player.Standard.Should().NotBeNull();
        player.Standard.Should().BeGreaterThan(2700);
        player.Rapid.Should().NotBeNull();
        player.Blitz.Should().NotBeNull();
    }



    [Fact]
    public async Task SearchPlayersAsync_WithCarlsen_ReturnsPlayers()
    {
        // Act
        var players = await Client.Fide.SearchPlayersAsync("Carlsen");

        // Assert
        players.Should().NotBeNull();
        players.Should().NotBeEmpty();
        players.Should().Contain(p => p.Name.Contains("Carlsen", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchPlayersAsync_WithCaruana_ReturnsPlayers()
    {
        // Act
        var players = await Client.Fide.SearchPlayersAsync("Caruana");

        // Assert
        players.Should().NotBeNull();
        players.Should().NotBeEmpty();
        players.Should().Contain(p => p.Name.Contains("Caruana", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchPlayersAsync_WithExactName_ReturnsMagnusCarlsen()
    {
        // Act
        var players = await Client.Fide.SearchPlayersAsync("Magnus Carlsen");

        // Assert
        players.Should().NotBeNull();
        players.Should().Contain(p => p.Id == MagnusCarlsenFideId);
    }

}
