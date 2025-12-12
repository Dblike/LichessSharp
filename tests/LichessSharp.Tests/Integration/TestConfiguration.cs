namespace LichessSharp.Tests.Integration;

/// <summary>
///     Configuration for integration tests, particularly those requiring authentication.
/// </summary>
/// <remarks>
///     <para>
///         To run authenticated tests, set the following environment variables:
///     </para>
///     <list type="bullet">
///         <item><c>LICHESS_TEST_TOKEN</c> - A personal access token with required scopes</item>
///     </list>
///     <para>
///         Generate tokens at: https://lichess.org/account/oauth/token
///     </para>
///     <para>
///         For CI/CD, store the token as a secret and inject it as an environment variable.
///     </para>
/// </remarks>
public static class TestConfiguration
{
    /// <summary>
    ///     Environment variable name for the test token.
    /// </summary>
    public const string TokenEnvironmentVariable = "LICHESS_TEST_TOKEN";

    /// <summary>
    ///     Gets the Lichess test token from environment variables.
    /// </summary>
    /// <returns>The token if set, otherwise null.</returns>
    public static string? LichessToken =>
        Environment.GetEnvironmentVariable(TokenEnvironmentVariable);

    /// <summary>
    ///     Gets whether authentication is available for tests.
    /// </summary>
    public static bool HasAuthentication =>
        !string.IsNullOrWhiteSpace(LichessToken);

    /// <summary>
    ///     Gets the skip reason message when authentication is not available.
    /// </summary>
    public static string SkipReason =>
        $"Requires {TokenEnvironmentVariable} environment variable to be set. " +
        $"Generate a token at https://lichess.org/account/oauth/token";
}