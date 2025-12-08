namespace LichessSharp.Tests.Integration;

/// <summary>
/// Base class for integration tests that make real HTTP calls to Lichess.
/// These tests verify that the API client works correctly against the live API.
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected LichessClient Client { get; } = new();

    // Create client without authentication for public API tests

    public void Dispose()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Trait to mark integration tests that require network access.
/// Use: dotnet test --filter "Category=Integration"
/// Skip: dotnet test --filter "Category!=Integration"
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class IntegrationTestAttribute : Attribute, Xunit.Sdk.ITraitAttribute
{
    public const string Category = "Integration";
}

/// <summary>
/// Trait discoverer for integration tests.
/// </summary>
public class IntegrationTestDiscoverer : Xunit.Sdk.ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(Xunit.Abstractions.IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Category", IntegrationTestAttribute.Category);
    }
}
