using Xunit.Abstractions;
using Xunit.Sdk;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Base class for integration tests that make real HTTP calls to Lichess.
///     These tests verify that the API client works correctly against the live API.
/// </summary>
/// <remarks>
///     The client is configured with <see cref="LichessClientOptions.UnlimitedRateLimitRetries" /> enabled,
///     which means it will automatically wait and retry when rate limited by Lichess.
///     This ensures tests eventually complete rather than failing due to rate limits.
/// </remarks>
public abstract class IntegrationTestBase : IDisposable
{
    /// <summary>
    ///     Creates the LichessClient configured for integration testing.
    ///     The client has unlimited rate limit retries and an extended timeout to handle
    ///     long waits when Lichess rate limits requests (up to 60 seconds per retry).
    /// </summary>
    protected LichessClient Client { get; } = new(
        new HttpClient(),
        new LichessClientOptions
        {
            DefaultTimeout = TimeSpan.FromMinutes(10),
            UnlimitedRateLimitRetries = true
        });

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
