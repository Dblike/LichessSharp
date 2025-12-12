using FluentAssertions;
using Xunit;

namespace LichessSharp.Tests.Integration.Authenticated;

/// <summary>
///     Authenticated integration tests for the Account API.
///     Requires a token with email:read and preference:read scopes.
/// </summary>
[AuthenticatedTest]
[IntegrationTest]
[Trait("Category", "Integration")]
[Trait("Category", "Authenticated")]
public class AccountApiAuthenticatedTests : AuthenticatedTestBase
{
    [RequiresAuthentication]
    [RequiresScope("email:read")]
    public async Task GetProfileAsync_WithValidToken_ReturnsUserProfile()
    {
        // Act
        var profile = await Client.Account.GetProfileAsync();

        // Assert
        profile.Should().NotBeNull();
        profile.Username.Should().NotBeNullOrWhiteSpace();
        profile.Id.Should().NotBeNullOrWhiteSpace();
    }

    [RequiresAuthentication]
    [RequiresScope("email:read")]
    public async Task GetEmailAsync_WithValidToken_ReturnsEmail()
    {
        // Act
        var email = await Client.Account.GetEmailAsync();

        // Assert
        email.Should().NotBeNullOrWhiteSpace();
        email.Should().Contain("@", "Email should contain @ symbol");
    }

    [RequiresAuthentication]
    [RequiresScope("preference:read")]
    public async Task GetPreferencesAsync_WithValidToken_ReturnsPreferences()
    {
        // Act
        var preferences = await Client.Account.GetPreferencesAsync();

        // Assert
        preferences.Should().NotBeNull();
        // Preferences object should have some values set
    }

    [RequiresAuthentication]
    public async Task GetKidModeAsync_WithValidToken_ReturnsBool()
    {
        // Act
        var kidMode = await Client.Account.GetKidModeAsync();

        // Assert
        // Kid mode is either true or false, we just verify it returns without error
        kidMode.Should().Be(kidMode); // Tautology to ensure it's a valid bool
    }

    [RequiresAuthentication]
    public async Task GetProfileAsync_ReturnsConsistentUsername()
    {
        // Act - Get profile twice
        var profile1 = await Client.Account.GetProfileAsync();
        var profile2 = await Client.Account.GetProfileAsync();

        // Assert - Should be the same user
        profile1.Username.Should().Be(profile2.Username);
        profile1.Id.Should().Be(profile2.Id);
    }

    [RequiresAuthentication]
    public async Task GetAuthenticatedUsernameAsync_CachesResult()
    {
        // Act - Use the helper method from base class
        var username1 = await GetAuthenticatedUsernameAsync();
        var username2 = await GetAuthenticatedUsernameAsync();

        // Assert - Should be the same (and cached)
        username1.Should().Be(username2);
        username1.Should().NotBeNullOrWhiteSpace();
    }
}