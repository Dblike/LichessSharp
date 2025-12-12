using FluentAssertions;
using LichessSharp.Api.Contracts;
using LichessSharp.Exceptions;
using Xunit;

namespace LichessSharp.Tests.Integration.Authenticated;

/// <summary>
///     Authenticated integration tests for the Bulk Pairings API.
///     Requires a token with challenge:bulk scope.
/// </summary>
/// <remarks>
///     Note: Creating actual bulk pairings requires valid player tokens,
///     so most tests verify read operations and error handling.
/// </remarks>
[AuthenticatedTest]
[IntegrationTest]
[Trait("Category", "Integration")]
[Trait("Category", "Authenticated")]
public class BulkPairingsApiAuthenticatedTests : AuthenticatedTestBase
{
    [RequiresAuthentication]
    [RequiresScope("challenge:bulk")]
    public async Task GetAllAsync_WithValidToken_ReturnsListOrEmpty()
    {
        // Act
        var bulkPairings = await Client.BulkPairings.GetAllAsync();

        // Assert
        // The list may be empty if no bulk pairings have been created
        bulkPairings.Should().NotBeNull();
        // If there are results, verify they have valid structure
        foreach (var pairing in bulkPairings)
        {
            pairing.Id.Should().NotBeNullOrWhiteSpace();
            pairing.Games.Should().NotBeNull();
        }
    }

    [RequiresAuthentication]
    [RequiresScope("challenge:bulk")]
    public async Task GetAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        const string nonexistentId = "nonexistent_bulk_pairing_id_12345";

        // Act
        var act = async () => await Client.BulkPairings.GetAsync(nonexistentId);

        // Assert
        await act.Should().ThrowAsync<LichessException>();
    }

    [RequiresAuthentication]
    [RequiresScope("challenge:bulk")]
    public async Task CreateAsync_WithInvalidTokens_ThrowsException()
    {
        // Arrange - Use invalid player tokens
        var options = new BulkPairingCreateOptions
        {
            Players = "invalid_token_1:invalid_token_2",
            ClockLimit = 300,
            ClockIncrement = 3
        };

        // Act
        var act = async () => await Client.BulkPairings.CreateAsync(options);

        // Assert - Should fail because tokens are invalid
        await act.Should().ThrowAsync<LichessException>();
    }

    [RequiresAuthentication]
    [RequiresScope("challenge:bulk")]
    public async Task CancelAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        const string nonexistentId = "nonexistent_bulk_pairing_id_12345";

        // Act
        var act = async () => await Client.BulkPairings.CancelAsync(nonexistentId);

        // Assert
        await act.Should().ThrowAsync<LichessException>();
    }

    [RequiresAuthentication]
    [RequiresScope("challenge:bulk")]
    public async Task StartClocksAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        const string nonexistentId = "nonexistent_bulk_pairing_id_12345";

        // Act
        var act = async () => await Client.BulkPairings.StartClocksAsync(nonexistentId);

        // Assert
        await act.Should().ThrowAsync<LichessException>();
    }

    [RequiresAuthentication]
    [RequiresScope("challenge:bulk")]
    public async Task ExportGamesPgnAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        const string nonexistentId = "nonexistent_bulk_pairing_id_12345";

        // Act
        var act = async () => await Client.BulkPairings.ExportGamesAsync(nonexistentId);

        // Assert
        await act.Should().ThrowAsync<LichessException>();
    }
}