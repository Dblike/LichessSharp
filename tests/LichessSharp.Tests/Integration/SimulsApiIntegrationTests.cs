using FluentAssertions;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Integration tests for the Simuls API.
///     These tests make real HTTP calls to Lichess.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class SimulsApiIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task GetCurrentAsync_ReturnsSimulList()
    {
        // Act
        var result = await Client.Simuls.GetCurrentAsync();

        // Assert
        result.Should().NotBeNull();
        // All lists should be initialized (possibly empty)
        result.Pending.Should().NotBeNull();
        result.Created.Should().NotBeNull();
        result.Started.Should().NotBeNull();
        result.Finished.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCurrentAsync_ReturnsValidSimulStructure()
    {
        // Act
        var result = await Client.Simuls.GetCurrentAsync();

        // Assert - check any simul we can find has valid structure
        var anySimul = result.Started.FirstOrDefault()
                       ?? result.Created.FirstOrDefault()
                       ?? result.Finished.FirstOrDefault();

        if (anySimul != null)
        {
            anySimul.Id.Should().NotBeNullOrWhiteSpace();
            anySimul.Name.Should().NotBeNullOrWhiteSpace();
            anySimul.FullName.Should().NotBeNullOrWhiteSpace();
            anySimul.Host.Should().NotBeNull();
            anySimul.Host.Id.Should().NotBeNullOrWhiteSpace();
            anySimul.Host.Name.Should().NotBeNullOrWhiteSpace();
            anySimul.Variants.Should().NotBeNull();
        }
        // If no simuls are available, the test still passes
        // (simuls are not always running)
    }

    [Fact]
    public async Task GetCurrentAsync_StartedSimulsAreRunning()
    {
        // Act
        var result = await Client.Simuls.GetCurrentAsync();

        // Assert - started simuls should have IsRunning = true
        foreach (var simul in result.Started)
        {
            simul.IsRunning.Should().BeTrue("Started simuls should be running");
            simul.IsFinished.Should().BeFalse("Started simuls should not be finished");
        }
    }

    [Fact]
    public async Task GetCurrentAsync_FinishedSimulsAreMarkedFinished()
    {
        // Act
        var result = await Client.Simuls.GetCurrentAsync();

        // Assert - finished simuls should have IsFinished = true
        foreach (var simul in result.Finished)
        {
            simul.IsFinished.Should().BeTrue("Finished simuls should be marked as finished");
            simul.IsRunning.Should().BeFalse("Finished simuls should not be running");
        }
    }

    [Fact]
    public async Task GetCurrentAsync_CreatedSimulsAreNotStarted()
    {
        // Act
        var result = await Client.Simuls.GetCurrentAsync();

        // Assert - created simuls should have IsCreated = true and not be running/finished
        foreach (var simul in result.Created)
        {
            simul.IsCreated.Should().BeTrue("Created simuls should be marked as created");
            simul.IsRunning.Should().BeFalse("Created simuls should not be running yet");
            simul.IsFinished.Should().BeFalse("Created simuls should not be finished");
        }
    }

    [Fact]
    public async Task GetCurrentAsync_SimulsHaveVariants()
    {
        // Act
        var result = await Client.Simuls.GetCurrentAsync();

        // Assert - any simul should have at least one variant
        var anySimul = result.Started.FirstOrDefault()
                       ?? result.Created.FirstOrDefault()
                       ?? result.Finished.FirstOrDefault();

        if (anySimul != null)
        {
            anySimul.Variants.Should().NotBeEmpty("Simuls should have at least one variant");
            var firstVariant = anySimul.Variants[0];
            firstVariant.Key.Should().NotBeNullOrWhiteSpace("Variant should have a key");
        }
    }
}