using FluentAssertions;
using Xunit;

using LichessSharp;

namespace LichessSharp.Tests;

public class LichessClientTests
{
    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Arrange & Act
        var act = () => new LichessClient(null!, new LichessClientOptions());

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange & Act
        var act = () => new LichessClient(new HttpClient(), null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesClient()
    {
        // Arrange & Act
        using var client = new LichessClient(new HttpClient(), new LichessClientOptions());

        // Assert
        client.Should().NotBeNull();
        client.Account.Should().NotBeNull();
        client.Users.Should().NotBeNull();
        client.Games.Should().NotBeNull();
        client.Puzzles.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithAccessToken_CreatesClient()
    {
        // Arrange & Act
        using var client = new LichessClient("test-token");

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithoutAccessToken_CreatesClient()
    {
        // Arrange & Act
        using var client = new LichessClient();

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var client = new LichessClient();

        // Act & Assert - should not throw
        client.Dispose();
        client.Dispose();
    }
}
