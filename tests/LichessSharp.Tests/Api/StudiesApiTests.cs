using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class StudiesApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly StudiesApi _studiesApi;

    public StudiesApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _studiesApi = new StudiesApi(_httpClientMock.Object);
    }
    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new StudiesApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task ExportChapterPgnAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var studyId = "abc12345";
        var chapterId = "xyz67890";
        var expectedPgn = "[Event \"Test\"]\n1. e4 e5 *";
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync($"/api/study/{studyId}/{chapterId}.pgn", "application/x-chess-pgn", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = await _studiesApi.ExportChapterPgnAsync(studyId, chapterId);

        // Assert
        result.Should().Be(expectedPgn);
        _httpClientMock.Verify(x => x.GetStringWithAcceptAsync($"/api/study/{studyId}/{chapterId}.pgn", "application/x-chess-pgn", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExportChapterPgnAsync_WithOptions_IncludesQueryParameters()
    {
        // Arrange
        var studyId = "abc12345";
        var chapterId = "xyz67890";
        var options = new StudyExportOptions
        {
            Clocks = true,
            Comments = true,
            Variations = false
        };
        var expectedPgn = "[Event \"Test\"]\n1. e4 e5 *";
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync(It.Is<string>(s => s.Contains("clocks=true") && s.Contains("comments=true") && s.Contains("variations=false")), "application/x-chess-pgn", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = await _studiesApi.ExportChapterPgnAsync(studyId, chapterId, options);

        // Assert
        result.Should().Be(expectedPgn);
    }

    [Fact]
    public async Task ExportChapterPgnAsync_WithNullStudyId_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.ExportChapterPgnAsync(null!, "chapter123");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportChapterPgnAsync_WithNullChapterId_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.ExportChapterPgnAsync("study123", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportStudyPgnAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var studyId = "abc12345";
        var expectedPgn = "[Event \"Test\"]\n1. e4 e5 *";
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync($"/api/study/{studyId}.pgn", "application/x-chess-pgn", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = await _studiesApi.ExportStudyPgnAsync(studyId);

        // Assert
        result.Should().Be(expectedPgn);
        _httpClientMock.Verify(x => x.GetStringWithAcceptAsync($"/api/study/{studyId}.pgn", "application/x-chess-pgn", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExportStudyPgnAsync_WithAllOptions_IncludesAllQueryParameters()
    {
        // Arrange
        var studyId = "abc12345";
        var options = new StudyExportOptions
        {
            Clocks = true,
            Comments = true,
            Variations = true,
            Opening = true,
            Source = true,
            Orientation = true
        };
        var expectedPgn = "[Event \"Test\"]\n1. e4 e5 *";
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync(It.Is<string>(s =>
                s.Contains("clocks=true") &&
                s.Contains("comments=true") &&
                s.Contains("variations=true") &&
                s.Contains("opening=true") &&
                s.Contains("source=true") &&
                s.Contains("orientation=true")), "application/x-chess-pgn", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = await _studiesApi.ExportStudyPgnAsync(studyId, options);

        // Assert
        result.Should().Be(expectedPgn);
    }

    [Fact]
    public async Task ExportStudyPgnAsync_WithNullStudyId_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.ExportStudyPgnAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportUserStudiesPgnAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "testuser";
        var expectedPgn = "[Event \"Test\"]\n1. e4 e5 *";
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync($"/study/by/{username}/export.pgn", "application/x-chess-pgn", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPgn);

        // Act
        var result = await _studiesApi.ExportUserStudiesPgnAsync(username);

        // Assert
        result.Should().Be(expectedPgn);
        _httpClientMock.Verify(x => x.GetStringWithAcceptAsync($"/study/by/{username}/export.pgn", "application/x-chess-pgn", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExportUserStudiesPgnAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.ExportUserStudiesPgnAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task StreamUserStudiesAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "testuser";
        var studies = new List<StudyMetadata>
        {
            new() { Id = "study1", Name = "Study 1" },
            new() { Id = "study2", Name = "Study 2" }
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<StudyMetadata>($"/api/study/by/{username}", It.IsAny<CancellationToken>()))
            .Returns(studies.ToAsyncEnumerable());

        // Act
        var result = new List<StudyMetadata>();
        await foreach (var study in _studiesApi.StreamUserStudiesAsync(username))
        {
            result.Add(study);
        }

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be("study1");
        result[1].Id.Should().Be("study2");
    }

    [Fact]
    public async Task StreamUserStudiesAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in _studiesApi.StreamUserStudiesAsync(null!))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task ImportPgnAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var studyId = "abc12345";
        var pgn = "1. e4 e5 *";
        var expectedResult = new StudyImportResult
        {
            Chapters = new List<StudyChapter>
            {
                new() { Id = "chapter1", Name = "Chapter 1" }
            }
        };
        _httpClientMock
            .Setup(x => x.PostAsync<StudyImportResult>($"/api/study/{studyId}/import-pgn", It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _studiesApi.ImportPgnAsync(studyId, pgn);

        // Assert
        result.Should().NotBeNull();
        result.Chapters.Should().HaveCount(1);
        result.Chapters[0].Id.Should().Be("chapter1");
    }

    [Fact]
    public async Task ImportPgnAsync_WithOptions_IncludesOptionsInRequest()
    {
        // Arrange
        var studyId = "abc12345";
        var pgn = "1. e4 e5 *";
        var options = new StudyImportOptions
        {
            Name = "Test Chapter",
            Orientation = "white",
            Variant = "standard"
        };
        var expectedResult = new StudyImportResult
        {
            Chapters = new List<StudyChapter>
            {
                new() { Id = "chapter1", Name = "Test Chapter" }
            }
        };
        _httpClientMock
            .Setup(x => x.PostAsync<StudyImportResult>($"/api/study/{studyId}/import-pgn", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _studiesApi.ImportPgnAsync(studyId, pgn, options);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.PostAsync<StudyImportResult>($"/api/study/{studyId}/import-pgn", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ImportPgnAsync_WithNullStudyId_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.ImportPgnAsync(null!, "1. e4 e5 *");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ImportPgnAsync_WithNullPgn_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.ImportPgnAsync("study123", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateChapterTagsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var studyId = "abc12345";
        var chapterId = "xyz67890";
        var pgnTags = "[Event \"Test\"]\n[Site \"Test Site\"]";
        _httpClientMock
            .Setup(x => x.PostNoContentAsync($"/api/study/{studyId}/{chapterId}/tags", It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _studiesApi.UpdateChapterTagsAsync(studyId, chapterId, pgnTags);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostNoContentAsync($"/api/study/{studyId}/{chapterId}/tags", It.IsAny<FormUrlEncodedContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateChapterTagsAsync_WithNullStudyId_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.UpdateChapterTagsAsync(null!, "chapter123", "[Event \"Test\"]");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateChapterTagsAsync_WithNullChapterId_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.UpdateChapterTagsAsync("study123", null!, "[Event \"Test\"]");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateChapterTagsAsync_WithNullPgnTags_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.UpdateChapterTagsAsync("study123", "chapter123", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeleteChapterAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var studyId = "abc12345";
        var chapterId = "xyz67890";
        _httpClientMock
            .Setup(x => x.DeleteNoContentAsync($"/api/study/{studyId}/{chapterId}", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _studiesApi.DeleteChapterAsync(studyId, chapterId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.DeleteNoContentAsync($"/api/study/{studyId}/{chapterId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteChapterAsync_WithNullStudyId_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.DeleteChapterAsync(null!, "chapter123");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeleteChapterAsync_WithNullChapterId_ThrowsArgumentException()
    {
        // Act
        var act = () => _studiesApi.DeleteChapterAsync("study123", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ExportChapterPgnAsync_WithCancellationToken_PassesToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _httpClientMock
            .Setup(x => x.GetStringWithAcceptAsync(It.IsAny<string>(), "application/x-chess-pgn", cts.Token))
            .ReturnsAsync("pgn content");

        // Act
        await _studiesApi.ExportChapterPgnAsync("study", "chapter", null, cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetStringWithAcceptAsync(It.IsAny<string>(), "application/x-chess-pgn", cts.Token), Times.Once);
    }

    [Fact]
    public async Task ImportPgnAsync_WithCancellationToken_PassesToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _httpClientMock
            .Setup(x => x.PostAsync<StudyImportResult>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), cts.Token))
            .ReturnsAsync(new StudyImportResult());

        // Act
        await _studiesApi.ImportPgnAsync("study", "pgn content", null, cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.PostAsync<StudyImportResult>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>(), cts.Token), Times.Once);
    }

}
