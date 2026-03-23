using FluentAssertions;
using Xunit;

namespace LichessSharp.Tests.Integration.Authenticated;

/// <summary>
///     Authenticated integration tests for the Teams API.
///     Requires a valid OAuth token.
/// </summary>
[AuthenticatedTest]
[IntegrationTest]
[LongRunningTest]
[Trait("Category", "Integration")]
[Trait("Category", "Authenticated")]
[Collection("Lichess API")]
public class TeamsApiAuthenticatedTests : AuthenticatedTestBase
{
    public TeamsApiAuthenticatedTests(LichessTestFixture fixture) : base(fixture) { }

    [RequiresAuthentication]
    public async Task GetUserTeamsAsync_WithValidUser_ReturnsTeams()
    {
        // Arrange
        var username = await GetAuthenticatedUsernameAsync();
        await ThrottleAsync();

        // Act
        var teams = await Client.Teams.GetUserTeamsAsync(username);

        // Assert
        teams.Should().NotBeNull();
        // The authenticated user may or may not be in any teams, but the call should succeed
    }
}
