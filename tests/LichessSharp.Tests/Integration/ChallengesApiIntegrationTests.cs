using FluentAssertions;
using LichessSharp.Api.Contracts;
using LichessSharp.Exceptions;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Integration tests for the Challenges API.
///     These tests make real HTTP calls to Lichess.
///     Note: Most Challenges API endpoints require authentication.
/// </summary>
[IntegrationTest]
[LongRunningTest]
[Trait("Category", "Integration")]
[Collection("Lichess API")]
public class ChallengesApiIntegrationTests : IntegrationTestBase
{
    public ChallengesApiIntegrationTests(LichessTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetPendingAsync_WithoutAuthentication_ThrowsLichessException()
    {
        await ThrottleAsync();

        // Act & Assert
        // The pending challenges endpoint requires authentication
        var act = async () => await Client.Challenges.GetPendingAsync();

        await act.Should().ThrowAsync<LichessAuthenticationException>();
    }

    [Fact]
    public async Task ShowAsync_WithInvalidChallengeId_ThrowsLichessException()
    {
        // Arrange
        var invalidChallengeId = "nonexistent123";

        await ThrottleAsync();

        // Act & Assert
        var act = async () => await Client.Challenges.ShowAsync(invalidChallengeId);

        // Should throw 404 for non-existent challenge
        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task CreateAsync_WithoutAuthentication_ThrowsLichessException()
    {
        await ThrottleAsync();

        // Act & Assert
        // Creating a challenge requires authentication
        var act = async () => await Client.Challenges.CreateAsync("DrNykterstein");

        await act.Should().ThrowAsync<LichessAuthenticationException>();
    }

    [Fact]
    public async Task AcceptAsync_WithoutAuthentication_ThrowsLichessException()
    {
        await ThrottleAsync();

        // Act & Assert
        var act = async () => await Client.Challenges.AcceptAsync("somechallenge");

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task DeclineAsync_WithoutAuthentication_ThrowsLichessException()
    {
        await ThrottleAsync();

        // Act & Assert
        var act = async () => await Client.Challenges.DeclineAsync("somechallenge");

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task CancelAsync_WithoutAuthentication_ThrowsLichessException()
    {
        await ThrottleAsync();

        // Act & Assert
        var act = async () => await Client.Challenges.CancelAsync("somechallenge");

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task ChallengeAiAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var options = new ChallengeAiOptions { Level = 5 };

        await ThrottleAsync();

        // Act & Assert
        var act = async () => await Client.Challenges.ChallengeAiAsync(options);

        await act.Should().ThrowAsync<LichessAuthenticationException>();
    }

    [Fact]
    public async Task CreateOpenAsync_WithoutAuthentication_MaySucceedOrThrow()
    {
        await ThrottleAsync();

        // Act & Assert
        // Note: Open challenges may or may not require authentication depending on Lichess behavior
        // The endpoint might succeed anonymously or throw an auth exception
        try
        {
            var result = await Client.Challenges.CreateOpenAsync();
            // If it succeeds, verify we got a valid response
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrWhiteSpace();
        }
        catch (LichessAuthenticationException)
        {
            // This is also acceptable - some configurations require auth
        }
    }
}