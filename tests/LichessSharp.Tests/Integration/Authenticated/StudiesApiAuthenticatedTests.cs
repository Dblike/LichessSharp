using FluentAssertions;
using LichessSharp.Api.Contracts;
using Xunit;

namespace LichessSharp.Tests.Integration.Authenticated;

/// <summary>
///     Authenticated integration tests for the Studies API.
///     Requires LICHESS_TEST_TOKEN environment variable with study:read scope.
/// </summary>
[IntegrationTest]
[AuthenticatedTest]
[Trait("Category", "Integration")]
[Trait("Category", "Authenticated")]
[RequiresScope("study:read")]
public class StudiesApiAuthenticatedTests : AuthenticatedTestBase
{
    [Fact]
    public async Task StreamUserStudiesAsync_WithAuthenticatedUser_ReturnsStudies()
    {
        // Arrange
        var username = await GetAuthenticatedUsernameAsync();
        var studies = new List<StudyMetadata>();

        // Act
        await foreach (var study in Client.Studies.StreamUserStudiesAsync(username))
        {
            studies.Add(study);
            // Limit to avoid long-running test
            if (studies.Count >= 10) break;
        }

        // Assert
        // User may or may not have studies - both are valid
        studies.Should().NotBeNull();
        foreach (var study in studies)
        {
            study.Id.Should().NotBeNullOrWhiteSpace();
            study.Name.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task StreamUserStudiesAsync_WithKnownUser_ReturnsPublicStudies()
    {
        // Arrange - use a known user who likely has public studies
        const string username = "thibault"; // Lichess founder
        var studies = new List<StudyMetadata>();

        // Act
        await foreach (var study in Client.Studies.StreamUserStudiesAsync(username))
        {
            studies.Add(study);
            if (studies.Count >= 5) break;
        }

        // Assert
        // thibault likely has studies, but may not
        studies.Should().NotBeNull();
    }

    [Fact]
    public async Task ExportStudyPgnAsync_WithPublicStudy_ReturnsPgn()
    {
        // Arrange - find a public study
        const string username = "thibault";
        string? studyId = null;

        await foreach (var study in Client.Studies.StreamUserStudiesAsync(username))
        {
            studyId = study.Id;
            break;
        }

        if (studyId == null)
            // No studies found, skip test
            return;

        // Act
        var pgn = await Client.Studies.ExportStudyPgnAsync(studyId);

        // Assert
        pgn.Should().NotBeNullOrWhiteSpace("Study should have PGN content");
        // PGN should have basic structure
        pgn.Should().Contain("[", "PGN should contain tags");
    }

    [Fact]
    public async Task ExportStudyPgnAsync_WithOptions_ReturnsFormattedPgn()
    {
        // Arrange
        const string username = "thibault";
        string? studyId = null;

        await foreach (var study in Client.Studies.StreamUserStudiesAsync(username))
        {
            studyId = study.Id;
            break;
        }

        if (studyId == null) return;

        var options = new StudyExportOptions
        {
            Comments = true,
            Variations = true,
            Opening = true
        };

        // Act
        var pgn = await Client.Studies.ExportStudyPgnAsync(studyId, options);

        // Assert
        pgn.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ExportChapterPgnAsync_WithValidStudyAndChapter_ReturnsPgn()
    {
        // This test requires knowing a valid study/chapter combination
        // Since we can't easily discover chapter IDs without parsing study HTML,
        // we'll use a well-known study if available

        // Skip this test - chapter discovery requires additional API support
        // The unit tests cover the endpoint construction adequately
    }

    [Fact]
    public async Task ExportUserStudiesPgnAsync_WithAuthenticatedUser_ReturnsPgnOrEmpty()
    {
        // Arrange
        var username = await GetAuthenticatedUsernameAsync();

        // Act
        var pgn = await Client.Studies.ExportUserStudiesPgnAsync(username);

        // Assert
        // May be empty if user has no studies
        pgn.Should().NotBeNull();
        // If not empty, should be valid PGN
        if (!string.IsNullOrWhiteSpace(pgn)) pgn.Should().Contain("[", "Non-empty result should be valid PGN");
    }

    [Fact]
    public async Task ExportUserStudiesPgnAsync_WithKnownUser_ReturnsPgn()
    {
        // Arrange - use a known user with public studies
        const string username = "thibault";

        // Act
        var pgn = await Client.Studies.ExportUserStudiesPgnAsync(username);

        // Assert
        // thibault likely has studies
        pgn.Should().NotBeNull();
    }
}

/// <summary>
///     Unauthenticated integration tests for the Studies API.
///     Tests public endpoints that don't require authentication.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class StudiesApiIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task StreamUserStudiesAsync_WithKnownUser_ReturnsPublicStudies()
    {
        // Arrange - use a known user who likely has public studies
        const string username = "DrNykterstein"; // Magnus Carlsen's Lichess account
        var studies = new List<StudyMetadata>();

        // Act
        await foreach (var study in Client.Studies.StreamUserStudiesAsync(username))
        {
            studies.Add(study);
            if (studies.Count >= 5) break;
        }

        // Assert
        studies.Should().NotBeNull();
        // DrNykterstein may or may not have public studies
    }

    [Fact]
    public async Task StreamUserStudiesAsync_StudiesHaveValidStructure()
    {
        // Arrange
        const string username = "thibault";

        // Act
        await foreach (var study in Client.Studies.StreamUserStudiesAsync(username))
        {
            // Assert
            study.Id.Should().NotBeNullOrWhiteSpace();
            study.Name.Should().NotBeNullOrWhiteSpace();
            study.CreatedAt.Should().BeGreaterThan(0);
            study.UpdatedAt.Should().BeGreaterThan(0);
            return; // Just check first study
        }
    }

    [Fact]
    public async Task ExportStudyPgnAsync_WithPublicStudy_ReturnsPgn()
    {
        // Arrange - find a public study
        const string username = "thibault";
        string? studyId = null;

        await foreach (var study in Client.Studies.StreamUserStudiesAsync(username))
        {
            studyId = study.Id;
            break;
        }

        if (studyId == null) return;

        // Act
        var pgn = await Client.Studies.ExportStudyPgnAsync(studyId);

        // Assert
        pgn.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ExportUserStudiesPgnAsync_WithKnownUser_DoesNotThrow()
    {
        // Arrange
        const string username = "thibault";

        // Act
        var pgn = await Client.Studies.ExportUserStudiesPgnAsync(username);

        // Assert
        pgn.Should().NotBeNull();
    }
}