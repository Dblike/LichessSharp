using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Marks a test class or method as requiring authentication.
///     Tests will be skipped if the <c>LICHESS_TEST_TOKEN</c> environment variable is not set.
/// </summary>
/// <remarks>
///     <para>
///         This attribute serves two purposes:
///         <list type="number">
///             <item>Marks tests with the "Authenticated" trait for filtering</item>
///             <item>Provides a clear indication that the test requires a token</item>
///         </list>
///     </para>
///     <para>
///         Usage with test filtering:
///         <code>
/// # Run only authenticated tests
/// dotnet test --filter "Category=Authenticated"
/// 
/// # Skip authenticated tests
/// dotnet test --filter "Category!=Authenticated"
/// </code>
///     </para>
/// </remarks>
/// <example>
///     <code>
/// [AuthenticatedTest]
/// public class MyTests : AuthenticatedTestBase
/// {
///     [Fact]
///     public async Task MyAuthenticatedTest()
///     {
///         // Test code that requires authentication
///     }
/// }
/// </code>
/// </example>
[TraitDiscoverer("LichessSharp.Tests.Integration.AuthenticatedTestDiscoverer", "LichessSharp.Tests")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthenticatedTestAttribute : Attribute, ITraitAttribute
{
    /// <summary>
    ///     The trait category name for authenticated tests.
    /// </summary>
    public const string Category = "Authenticated";
}

/// <summary>
///     Trait discoverer for authenticated tests.
/// </summary>
public class AuthenticatedTestDiscoverer : ITraitDiscoverer
{
    /// <inheritdoc />
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Category", AuthenticatedTestAttribute.Category);
    }
}

/// <summary>
///     A Fact attribute that skips the test if authentication is not available.
///     Use this instead of [Fact] for individual tests that require authentication.
/// </summary>
/// <remarks>
///     <para>
///         This attribute automatically skips the test with a descriptive message
///         if the <c>LICHESS_TEST_TOKEN</c> environment variable is not set.
///     </para>
///     <para>
///         For class-level skipping, use <see cref="AuthenticatedTestBase" /> which will
///         throw an exception in the constructor if no token is available.
///     </para>
/// </remarks>
/// <example>
///     <code>
/// public class MyTests
/// {
///     [RequiresAuthentication]
///     public async Task TestThatNeedsAuth()
///     {
///         // This test will be skipped if no token is available
///     }
/// }
/// </code>
/// </example>
public sealed class RequiresAuthenticationAttribute : FactAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequiresAuthenticationAttribute" /> class.
    ///     Sets the Skip property if authentication is not available.
    /// </summary>
    public RequiresAuthenticationAttribute()
    {
        if (!TestConfiguration.HasAuthentication) Skip = TestConfiguration.SkipReason;
    }
}

/// <summary>
///     A Theory attribute that skips the test if authentication is not available.
///     Use this instead of [Theory] for parameterized tests that require authentication.
/// </summary>
/// <example>
///     <code>
/// public class MyTests
/// {
///     [RequiresAuthenticationTheory]
///     [InlineData("user1")]
///     [InlineData("user2")]
///     public async Task TestWithData(string username)
///     {
///         // This test will be skipped if no token is available
///     }
/// }
/// </code>
/// </example>
public sealed class RequiresAuthenticationTheoryAttribute : TheoryAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequiresAuthenticationTheoryAttribute" /> class.
    ///     Sets the Skip property if authentication is not available.
    /// </summary>
    public RequiresAuthenticationTheoryAttribute()
    {
        if (!TestConfiguration.HasAuthentication) Skip = TestConfiguration.SkipReason;
    }
}

/// <summary>
///     Attribute to document which OAuth scopes a test requires.
///     This is for documentation purposes and does not enforce scope checking.
/// </summary>
/// <remarks>
///     <para>
///         Use this attribute to clearly document the OAuth scopes needed by a test.
///         This helps when setting up test tokens with the correct permissions.
///     </para>
///     <para>
///         Common scopes:
///         <list type="bullet">
///             <item><c>email:read</c> - Read email address</item>
///             <item><c>preference:read</c> - Read preferences</item>
///             <item><c>preference:write</c> - Change preferences</item>
///             <item><c>challenge:read</c> - Read challenges</item>
///             <item><c>challenge:write</c> - Create/accept challenges</item>
///             <item><c>challenge:bulk</c> - Bulk pairings</item>
///             <item><c>board:play</c> - Board API</item>
///             <item><c>bot:play</c> - Bot API</item>
///             <item><c>team:read</c> - Read teams</item>
///             <item><c>team:write</c> - Join/leave teams</item>
///             <item><c>team:lead</c> - Manage teams</item>
///             <item><c>follow:read</c> - Read following</item>
///             <item><c>follow:write</c> - Follow/unfollow</item>
///             <item><c>msg:write</c> - Send messages</item>
///             <item><c>study:read</c> - Read studies</item>
///             <item><c>study:write</c> - Create/modify studies</item>
///             <item><c>puzzle:read</c> - Read puzzles</item>
///         </list>
///     </para>
/// </remarks>
/// <example>
///     <code>
/// [RequiresAuthentication]
/// [RequiresScope("email:read", "preference:read")]
/// public async Task GetAccountInfo()
/// {
///     var email = await Client.Account.GetEmailAsync();
///     var prefs = await Client.Account.GetPreferencesAsync();
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class RequiresScopeAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequiresScopeAttribute" /> class.
    /// </summary>
    /// <param name="scopes">The required OAuth scopes.</param>
    public RequiresScopeAttribute(params string[] scopes)
    {
        Scopes = scopes ?? [];
    }

    /// <summary>
    ///     Gets the required OAuth scopes.
    /// </summary>
    public string[] Scopes { get; }
}