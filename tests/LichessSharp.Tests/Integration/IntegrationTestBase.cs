using Xunit.Abstractions;
using Xunit.Sdk;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Base class for integration tests that make real HTTP calls to Lichess.
///     Uses <see cref="LichessTestFixture" /> for request throttling to avoid rate limits.
/// </summary>
/// <remarks>
///     All subclasses must be annotated with <c>[Collection("Lichess API")]</c> to share
///     the fixture and run sequentially. Call <see cref="ThrottleAsync" /> before each API call.
/// </remarks>
public abstract class IntegrationTestBase : IDisposable
{
    protected IntegrationTestBase(LichessTestFixture fixture)
    {
        Fixture = fixture;
        Client = fixture.CreateClient();
    }

    /// <summary>
    ///     The shared test fixture providing throttling and client creation.
    /// </summary>
    protected LichessTestFixture Fixture { get; }

    /// <summary>
    ///     The LichessClient configured for integration testing.
    /// </summary>
    protected LichessClient Client { get; }

    /// <summary>
    ///     Throttles the next API request to respect Lichess rate limits.
    ///     Call this before each API call.
    /// </summary>
    protected Task ThrottleAsync(CancellationToken cancellationToken = default) =>
        Fixture.ThrottleAsync(cancellationToken);

    public void Dispose()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
///     Trait to mark integration tests that require network access.
///     Use: dotnet test --filter "Category=Integration"
///     Skip: dotnet test --filter "Category!=Integration"
/// </summary>
[TraitDiscoverer("LichessSharp.Tests.Integration.IntegrationTestDiscoverer", "LichessSharp.Tests")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class IntegrationTestAttribute : Attribute, ITraitAttribute
{
    public const string Category = "Integration";
}

/// <summary>
///     Trait discoverer for integration tests.
/// </summary>
public class IntegrationTestDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Category", IntegrationTestAttribute.Category);
    }
}

/// <summary>
///     Trait to mark tests as long-running.
///     Use: dotnet test --filter "Category=LongRunning"
///     Skip: dotnet test --filter "Category!=LongRunning"
/// </summary>
[TraitDiscoverer("LichessSharp.Tests.Integration.LongRunningTestDiscoverer", "LichessSharp.Tests")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class LongRunningTestAttribute : Attribute, ITraitAttribute
{
    public const string Category = "LongRunning";
}

/// <summary>
///     Trait discoverer for long-running tests.
/// </summary>
public class LongRunningTestDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Category", LongRunningTestAttribute.Category);
    }
}
