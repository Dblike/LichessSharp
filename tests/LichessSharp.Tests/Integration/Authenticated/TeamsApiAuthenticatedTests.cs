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
public class TeamsApiAuthenticatedTests : AuthenticatedTestBase
{
    [RequiresAuthentication]
    public async Task GetUserTeamsAsync_WithValidUser_ReturnsTeams()
    {
        // Arrange
        var username = await GetAuthenticatedUsernameAsync();

        // Act
        var teams = await Client.Teams.GetUserTeamsAsync(username);

        // Assert
        teams.Should().NotBeNull();
        // The authenticated user may or may not be in any teams, but the call should succeed
    }
}
