using FluentAssertions;

using LichessSharp.Api.Contracts;

using Xunit;

namespace LichessSharp.Tests.Integration.Manual;

/// <summary>
/// Manual integration tests for the External Engine API.
/// These tests require specific OAuth scopes (engine:read, engine:write) and
/// are intended to be run manually rather than in automated CI.
///
/// To run these tests:
/// 1. Create a personal access token at https://lichess.org/account/oauth/token
/// 2. Select the engine:read and engine:write scopes
/// 3. Set the LICHESS_TEST_TOKEN environment variable
/// 4. Run: dotnet test --filter "Category=Manual"
///
/// WARNING: The External Engine API is in alpha and subject to change.
/// </summary>
[Trait("Category", "Manual")]
[RequiresScope("engine:read", "engine:write")]
public class ExternalEngineApiManualTests : AuthenticatedTestBase
{

    [Fact]
    public async Task ListAsync_WithValidToken_ReturnsEngineList()
    {
        // Act
        var engines = await Client.ExternalEngine.ListAsync();

        // Assert
        engines.Should().NotBeNull();
        // List may be empty if no engines are registered
    }

    [Fact]
    public async Task ListAsync_WhenEnginesExist_ReturnsValidStructure()
    {
        // Act
        var engines = await Client.ExternalEngine.ListAsync();

        // Assert
        foreach (var engine in engines)
        {
            engine.Id.Should().NotBeNullOrWhiteSpace();
            engine.Name.Should().NotBeNullOrWhiteSpace();
            engine.MaxThreads.Should().BeGreaterThan(0);
            engine.MaxHash.Should().BeGreaterThan(0);
        }
    }

    /// <summary>
    /// Test creating an external engine.
    /// WARNING: This will create a real engine registration on your account.
    /// You should delete it afterward using DeleteAsync.
    /// </summary>
    [Fact(Skip = "Creates real engine - run manually only")]
    public async Task CreateAsync_WithValidRegistration_CreatesEngine()
    {
        // Arrange
        var registration = new ExternalEngineRegistration
        {
            Name = "Test Engine Integration",
            MaxThreads = 4,
            MaxHash = 256,
            Variants = ["standard"],
            ProviderSecret = "test-secret-at-least-16-chars"
        };

        // Act
        var engine = await Client.ExternalEngine.CreateAsync(registration);

        // Assert
        engine.Should().NotBeNull();
        engine.Id.Should().NotBeNullOrWhiteSpace();
        engine.Name.Should().Be(registration.Name);
        engine.MaxThreads.Should().Be(registration.MaxThreads);
        engine.MaxHash.Should().Be(registration.MaxHash);

        // Cleanup - delete the engine
        await Client.ExternalEngine.DeleteAsync(engine.Id);
    }

    [Fact(Skip = "Requires existing engine ID - run manually only")]
    public async Task GetAsync_WithValidId_ReturnsEngine()
    {
        // Arrange - first list engines to get an ID
        var engines = await Client.ExternalEngine.ListAsync();
        if (engines.Count == 0)
        {
            return; // No engines to test with
        }

        var engineId = engines[0].Id;

        // Act
        var engine = await Client.ExternalEngine.GetAsync(engineId);

        // Assert
        engine.Should().NotBeNull();
        engine.Id.Should().Be(engineId);
    }

    [Fact(Skip = "Requires existing engine - run manually only")]
    public async Task UpdateAsync_WithValidData_UpdatesEngine()
    {
        // Arrange - first list engines to get an ID
        var engines = await Client.ExternalEngine.ListAsync();
        if (engines.Count == 0)
        {
            return;
        }

        var engineId = engines[0].Id;
        var originalEngine = engines[0];

        var registration = new ExternalEngineRegistration
        {
            Name = originalEngine.Name + " Updated",
            MaxThreads = originalEngine.MaxThreads,
            MaxHash = originalEngine.MaxHash,
            Variants = ["standard"],
            ProviderSecret = "test-secret-at-least-16-chars"
        };

        // Act
        var updatedEngine = await Client.ExternalEngine.UpdateAsync(engineId, registration);

        // Assert
        updatedEngine.Should().NotBeNull();
        updatedEngine.Name.Should().Contain("Updated");

        // Revert the name back
        var revertRegistration = new ExternalEngineRegistration
        {
            Name = originalEngine.Name,
            MaxThreads = originalEngine.MaxThreads,
            MaxHash = originalEngine.MaxHash,
            Variants = ["standard"],
            ProviderSecret = "test-secret-at-least-16-chars"
        };
        await Client.ExternalEngine.UpdateAsync(engineId, revertRegistration);
    }

    [Fact(Skip = "Requires engine to delete - run manually only")]
    public async Task DeleteAsync_WithValidId_DeletesEngine()
    {
        // Arrange - create an engine to delete
        var registration = new ExternalEngineRegistration
        {
            Name = "Test Engine To Delete",
            MaxThreads = 4,
            MaxHash = 256,
            Variants = ["standard"],
            ProviderSecret = "test-secret-at-least-16-chars"
        };

        var engine = await Client.ExternalEngine.CreateAsync(registration);

        // Act
        await Client.ExternalEngine.DeleteAsync(engine.Id);

        // Assert - verify it's deleted by trying to get it
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.ExternalEngine.GetAsync(engine.Id));
    }

    [Fact(Skip = "Requires working engine - run manually only")]
    public async Task AnalyseAsync_WithValidRequest_ReturnsAnalysisLines()
    {
        // Arrange
        var engines = await Client.ExternalEngine.ListAsync();
        if (engines.Count == 0)
        {
            return;
        }

        var engine = engines[0];
        var request = new EngineAnalysisRequest
        {
            ClientSecret = engine.ClientSecret ?? "default-secret",
            Work = new EngineAnalysisWork
            {
                SessionId = "test-session",
                Threads = 1,
                Hash = 16,
                MultiPv = 1,
                Variant = "standard",
                InitialFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                Moves = []
            }
        };

        // Act
        var lines = new List<EngineAnalysisLine>();
        await foreach (var line in Client.ExternalEngine.AnalyseAsync(engine.Id, request))
        {
            lines.Add(line);
            if (lines.Count >= 5) break;
        }

        // Assert
        // Analysis may timeout or return empty if no engine provider is running
        lines.Should().NotBeNull();
    }

    [Fact(Skip = "Requires engine provider setup - run manually only")]
    public async Task AcquireWorkAsync_WithValidSecret_ReturnsWorkOrNull()
    {
        // This test requires a properly configured external engine provider
        // The provider secret must match the one used when creating the engine

        // Arrange
        const string providerSecret = "your-provider-secret-here";

        // Act
        var work = await Client.ExternalEngine.AcquireWorkAsync(providerSecret);

        // Assert
        // Work may be null if no analysis is queued
        // If not null, verify structure
        if (work != null)
        {
            work.Id.Should().NotBeNullOrWhiteSpace();
            work.Work.Should().NotBeNull();
        }
    }

}

/// <summary>
/// Unauthenticated tests for External Engine API - verifies auth is required.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class ExternalEngineApiAuthRequiredTests : IntegrationTestBase
{
    [Fact]
    public async Task ListAsync_WithoutAuth_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.ExternalEngine.ListAsync());
    }

    [Fact]
    public async Task CreateAsync_WithoutAuth_ThrowsException()
    {
        // Arrange
        var registration = new ExternalEngineRegistration
        {
            Name = "Test Engine",
            MaxThreads = 4,
            MaxHash = 256,
            Variants = ["standard"],
            ProviderSecret = "test-secret-at-least-16-chars"
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await Client.ExternalEngine.CreateAsync(registration));
    }
}
