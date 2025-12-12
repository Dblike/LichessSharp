using FluentAssertions;
using LichessSharp.Exceptions;
using LichessSharp.Models.Users;
using Xunit;

namespace LichessSharp.Tests.Integration.Authenticated;

/// <summary>
/// Authenticated integration tests for the Relations API.
/// Requires a token with follow:read and follow:write scopes.
/// </summary>
[AuthenticatedTest]
[IntegrationTest]
[Trait("Category", "Integration")]
[Trait("Category", "Authenticated")]
public class RelationsApiAuthenticatedTests : AuthenticatedTestBase
{
    [RequiresAuthentication]
    [RequiresScope("follow:read")]
    public async Task StreamFollowingAsync_WithValidToken_ReturnsFollowedUsers()
    {
        // Act
        var following = new List<UserExtended>();
        await foreach (var user in Client.Relations.StreamFollowingUsersAsync())
        {
            following.Add(user);
            // Limit to first 10 for test performance
            if (following.Count >= 10)
            {
                break;
            }
        }

        // Assert
        // The test account may or may not follow anyone
        // We just verify the streaming works without error
        following.Should().NotBeNull();
        foreach (var user in following)
        {
            user.Id.Should().NotBeNullOrWhiteSpace();
            user.Username.Should().NotBeNullOrWhiteSpace();
        }
    }

    [RequiresAuthentication]
    [RequiresScope("follow:write")]
    public async Task FollowAndUnfollow_WithValidToken_Succeeds()
    {
        // Arrange - Use a well-known account that won't change
        const string targetUser = "DrNykterstein"; // Magnus Carlsen's account

        try
        {
            // Act - Follow the user
            var followResult = await Client.Relations.FollowUserAsync(targetUser);

            // Assert
            followResult.Should().BeTrue();

            // Act - Unfollow to restore state
            var unfollowResult = await Client.Relations.UnfollowUserAsync(targetUser);

            // Assert
            unfollowResult.Should().BeTrue();
        }
        catch (LichessException ex) when (ex.Message.Contains("already"))
        {
            // If already following, just unfollow
            var unfollowResult = await Client.Relations.UnfollowUserAsync(targetUser);
            unfollowResult.Should().BeTrue();
        }
    }
}
