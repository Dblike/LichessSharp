using FluentAssertions;
using LichessSharp.Api;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
/// Integration tests for the Bot API.
/// These tests make real HTTP calls to Lichess.
/// Note: Most Bot API endpoints require authentication.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class BotApiIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task GetOnlineBotsAsync_ReturnsOnlineBots()
    {
        // Arrange
        var bots = new List<BotUser>();

        // Act
        await foreach (var bot in Client.Bot.GetOnlineBotsAsync(count: 10))
        {
            bots.Add(bot);
            if (bots.Count >= 5)
            {
                break;
            }
        }

        // Assert
        // There should always be at least some bots online on Lichess
        bots.Should().NotBeEmpty("Lichess should always have some bots online");
        bots[0].Id.Should().NotBeNullOrWhiteSpace("Bot should have an ID");
        bots[0].Username.Should().NotBeNullOrWhiteSpace("Bot should have a username");
    }

    [Fact]
    public async Task GetOnlineBotsAsync_WithDefaultCount_ReturnsMultipleBots()
    {
        // Arrange
        var bots = new List<BotUser>();

        // Act
        await foreach (var bot in Client.Bot.GetOnlineBotsAsync())
        {
            bots.Add(bot);
            if (bots.Count >= 20)
            {
                break;
            }
        }

        // Assert
        bots.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetOnlineBotsAsync_BotHasValidStructure()
    {
        // Arrange
        BotUser? firstBot = null;

        // Act
        await foreach (var bot in Client.Bot.GetOnlineBotsAsync(count: 5))
        {
            firstBot = bot;
            break;
        }

        // Assert
        firstBot.Should().NotBeNull("Should receive at least one bot");
        firstBot!.Id.Should().NotBeNullOrWhiteSpace();
        firstBot.Username.Should().NotBeNullOrWhiteSpace();
        // Bots should typically have some playtime
        firstBot.PlayTime.Should().NotBeNull("Bots typically have playtime recorded");
    }

    [Fact]
    public async Task GetOnlineBotsAsync_WithSmallCount_RespectsLimit()
    {
        // Arrange
        var bots = new List<BotUser>();

        // Act
        await foreach (var bot in Client.Bot.GetOnlineBotsAsync(count: 3))
        {
            bots.Add(bot);
        }

        // Assert
        // We requested max 3, so we should get at most 3 (could be fewer if fewer bots online)
        bots.Count.Should().BeLessThanOrEqualTo(3);
    }
}
