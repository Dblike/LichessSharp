using FluentAssertions;
using LichessSharp.Models.Enums;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Integration tests for the FIDE API.
///     These tests use real FIDE player IDs from well-known players.
/// </summary>
[IntegrationTest]
[LongRunningTest]
[Trait("Category", "Integration")]
[Collection("Lichess API")]
public class FideApiIntegrationTests : IntegrationTestBase
{
    public FideApiIntegrationTests(LichessTestFixture fixture) : base(fixture) { }

    // Well-known FIDE player IDs
    private const int MagnusCarlsenFideId = 1503014;
    private const int FabianoCaruanaFideId = 2020009;

    [Fact]
    public async Task GetPlayerAsync_WithMagnusCarlsen_ReturnsPlayer()
    {
        // Act
        await ThrottleAsync();
        var player = await Client.Fide.GetPlayerAsync(MagnusCarlsenFideId);

        // Assert
        player.Should().NotBeNull();
        player.Id.Should().Be(MagnusCarlsenFideId);
        player.Name.Should().Contain("Carlsen");
        player.Title.Should().Be(Title.GM);
        player.Federation.Should().Be("NOR");
    }

    [Fact]
    public async Task GetPlayerAsync_WithFabianoCaruana_ReturnsPlayer()
    {
        // Act
        await ThrottleAsync();
        var player = await Client.Fide.GetPlayerAsync(FabianoCaruanaFideId);

        // Assert
        player.Should().NotBeNull();
        player.Id.Should().Be(FabianoCaruanaFideId);
        player.Name.Should().Contain("Caruana");
        player.Title.Should().Be(Title.GM);
    }

    [Fact]
    public async Task GetPlayerAsync_ReturnsRatings()
    {
        // Act
        await ThrottleAsync();
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
        await ThrottleAsync();
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
        await ThrottleAsync();
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
        await ThrottleAsync();
        var players = await Client.Fide.SearchPlayersAsync("Magnus Carlsen");

        // Assert
        players.Should().NotBeNull();
        players.Should().Contain(p => p.Id == MagnusCarlsenFideId);
    }

    [Fact]
    public async Task GetPlayerRatingsAsync_WithMagnusCarlsen_ReturnsRatings()
    {
        // Act
        await ThrottleAsync();
        var ratings = await Client.Fide.GetPlayerRatingsAsync(MagnusCarlsenFideId);

        // Assert
        ratings.Should().NotBeNull();
        ratings.Standard.Should().NotBeEmpty("Magnus Carlsen should have standard rating history");
        ratings.Rapid.Should().NotBeEmpty("Magnus Carlsen should have rapid rating history");
        ratings.Blitz.Should().NotBeEmpty("Magnus Carlsen should have blitz rating history");
    }

    [Fact]
    public async Task GetPlayerRatingsAsync_WithFabianoCaruana_ReturnsRatings()
    {
        // Act
        await ThrottleAsync();
        var ratings = await Client.Fide.GetPlayerRatingsAsync(FabianoCaruanaFideId);

        // Assert
        ratings.Should().NotBeNull();
        ratings.Standard.Should().NotBeEmpty("Caruana should have standard rating history");
    }

    [Fact]
    public async Task GetPlayerRatingsAsync_RatingDataPointsArePositive()
    {
        // Act
        await ThrottleAsync();
        var ratings = await Client.Fide.GetPlayerRatingsAsync(MagnusCarlsenFideId);

        // Assert - encoded data points should be positive numbers
        ratings.Standard.Should().AllSatisfy(dp => dp.Should().BeGreaterThan(0));
        ratings.Rapid.Should().AllSatisfy(dp => dp.Should().BeGreaterThan(0));
        ratings.Blitz.Should().AllSatisfy(dp => dp.Should().BeGreaterThan(0));
    }
}