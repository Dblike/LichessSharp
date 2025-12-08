using FluentAssertions;
using LichessSharp.Exceptions;
using Xunit;

namespace LichessSharp.Tests.Integration.Authenticated;

/// <summary>
/// Authenticated integration tests for the Messaging API.
/// Requires a token with msg:write scope.
/// </summary>
/// <remarks>
/// Note: These tests are intentionally conservative to avoid spamming real users.
/// Most tests verify error handling rather than actually sending messages.
/// </remarks>
[AuthenticatedTest]
[IntegrationTest]
[Trait("Category", "Integration")]
[Trait("Category", "Authenticated")]
public class MessagingApiAuthenticatedTests : AuthenticatedTestBase
{
    [RequiresAuthentication]
    [RequiresScope("msg:write")]
    public async Task SendAsync_ToNonexistentUser_ThrowsNotFoundException()
    {
        // Arrange
        const string nonexistentUser = "this_user_definitely_does_not_exist_12345xyz";
        const string message = "Test message";

        // Act
        var act = async () => await Client.Messaging.SendAsync(nonexistentUser, message);

        // Assert - Should fail because user doesn't exist
        await act.Should().ThrowAsync<LichessException>();
    }

    [RequiresAuthentication]
    [RequiresScope("msg:write")]
    public async Task SendAsync_ToSelf_HandlesGracefully()
    {
        // Arrange - Get our own username
        var username = await GetAuthenticatedUsernameAsync();
        const string message = "Test message to self";

        // Act
        var act = async () => await Client.Messaging.SendAsync(username, message);

        // Assert - Lichess may or may not allow messaging yourself
        // We just verify it doesn't crash unexpectedly
        try
        {
            var result = await Client.Messaging.SendAsync(username, message);
            // If it succeeds, that's fine
            result.Should().BeTrue();
        }
        catch (LichessException)
        {
            // If Lichess blocks self-messaging, that's also expected
        }
    }

    [RequiresAuthentication]
    [RequiresScope("msg:write")]
    public async Task SendAsync_WithEmptyMessage_ThrowsArgumentException()
    {
        // Arrange
        const string validUser = "thibault";
        const string emptyMessage = "";

        // Act
        var act = async () => await Client.Messaging.SendAsync(validUser, emptyMessage);

        // Assert - Should throw ArgumentException before even making the API call
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [RequiresAuthentication]
    [RequiresScope("msg:write")]
    public async Task SendAsync_WithWhitespaceMessage_ThrowsArgumentException()
    {
        // Arrange
        const string validUser = "thibault";
        const string whitespaceMessage = "   ";

        // Act
        var act = async () => await Client.Messaging.SendAsync(validUser, whitespaceMessage);

        // Assert - Should throw ArgumentException before even making the API call
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
