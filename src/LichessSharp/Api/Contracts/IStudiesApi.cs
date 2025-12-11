using System.Text.Json.Serialization;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Studies API - Access Lichess studies.
/// Studies are collaborative analysis boards with multiple chapters.
/// </summary>
public interface IStudiesApi
{
    /// <summary>
    /// Export one study chapter in PGN format.
    /// If authenticated, then all public, unlisted, and private study chapters are read.
    /// If not, only public (non-unlisted) study chapters are read.
    /// </summary>
    /// <param name="studyId">The study ID (8 characters).</param>
    /// <param name="chapterId">The chapter ID (8 characters).</param>
    /// <param name="options">Export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PGN content as a string.</returns>
    Task<string> ExportChapterPgnAsync(string studyId, string chapterId, StudyExportOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export all chapters of a study in PGN format.
    /// If authenticated, then all public, unlisted, and private study chapters are read.
    /// If not, only public (non-unlisted) study chapters are read.
    /// </summary>
    /// <param name="studyId">The study ID (8 characters).</param>
    /// <param name="options">Export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PGN content as a string.</returns>
    Task<string> ExportStudyPgnAsync(string studyId, StudyExportOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export all chapters of all studies of a user in PGN format.
    /// If authenticated, then all public, unlisted, and private studies are included.
    /// If not, only public (non-unlisted) studies are included.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="options">Export options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PGN content as a string.</returns>
    Task<string> ExportUserStudiesPgnAsync(string username, StudyExportOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get metadata (name and dates) of all studies of a user.
    /// If authenticated, then all public, unlisted, and private studies are included.
    /// If not, only public (non-unlisted) studies are included.
    /// Studies are streamed as NDJSON.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of study metadata.</returns>
    IAsyncEnumerable<StudyMetadata> StreamUserStudiesAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Import PGN into an existing study. Creates a new chapter in the study.
    /// If the PGN contains multiple games (separated by 2 or more newlines)
    /// then multiple chapters will be created within the study.
    /// Note that a study can contain at most 64 chapters.
    /// Requires OAuth with study:write scope.
    /// </summary>
    /// <param name="studyId">The study ID (8 characters).</param>
    /// <param name="pgn">PGN to import. Can contain multiple games separated by 2 or more newlines.</param>
    /// <param name="options">Import options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The chapters that were created.</returns>
    Task<StudyImportResult> ImportPgnAsync(string studyId, string pgn, StudyImportOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add, update and delete the PGN tags of a study chapter.
    /// By providing a list of PGN tags, you can:
    /// - Add new tags if the chapter doesn't have them yet
    /// - Update existing chapter tags
    /// - Delete existing chapter tags by providing a tag with an empty value.
    /// Requires OAuth with study:write scope.
    /// </summary>
    /// <param name="studyId">The study ID (8 characters).</param>
    /// <param name="chapterId">The chapter ID (8 characters).</param>
    /// <param name="pgnTags">PGN text containing the tags. Only the tags are used. Moves are ignored.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if tags were updated successfully.</returns>
    Task<bool> UpdateChapterTagsAsync(string studyId, string chapterId, string pgnTags, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a chapter of a study you own. This is definitive.
    /// A study must have at least one chapter; so if you delete the last chapter,
    /// an empty one will be automatically created to replace it.
    /// Requires OAuth with study:write scope.
    /// </summary>
    /// <param name="studyId">The study ID (8 characters).</param>
    /// <param name="chapterId">The chapter ID (8 characters).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the chapter was deleted successfully.</returns>
    Task<bool> DeleteChapterAsync(string studyId, string chapterId, CancellationToken cancellationToken = default);
}


/// <summary>
/// Options for exporting study PGN.
/// </summary>
public class StudyExportOptions
{
    /// <summary>
    /// Include clock comments in the PGN moves.
    /// Example: 2. Nf3 { [%clk 1:59:01] }
    /// </summary>
    public bool? Clocks { get; set; }

    /// <summary>
    /// Include analysis and eval comments in the PGN moves.
    /// Example: 2. Nf3 { [%eval 0.17] }
    /// </summary>
    public bool? Comments { get; set; }

    /// <summary>
    /// Include non-mainline moves, when available.
    /// </summary>
    public bool? Variations { get; set; }

    /// <summary>
    /// Include the full opening name.
    /// </summary>
    public bool? Opening { get; set; }

    /// <summary>
    /// Add the source URL of the study chapter as a comment.
    /// </summary>
    public bool? Source { get; set; }

    /// <summary>
    /// Add a Lichess orientation PGN tag.
    /// </summary>
    public bool? Orientation { get; set; }
}

/// <summary>
/// Options for importing PGN into a study.
/// </summary>
public class StudyImportOptions
{
    /// <summary>
    /// Name of the new chapter.
    /// If not specified, or if multiple chapters are created, the names will be inferred from the PGN tags.
    /// Maximum 100 characters.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Default board orientation. Either "white" or "black".
    /// </summary>
    public string? Orientation { get; set; }

    /// <summary>
    /// Chess variant key. Defaults to standard chess.
    /// </summary>
    public string? Variant { get; set; }
}



/// <summary>
/// Study metadata (name and dates).
/// </summary>
public class StudyMetadata
{
    /// <summary>
    /// The study ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The study name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The study creation date (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("createdAt")]
    public long CreatedAt { get; init; }

    /// <summary>
    /// The study last update date (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public long UpdatedAt { get; init; }
}

/// <summary>
/// Result of importing PGN into a study.
/// </summary>
public class StudyImportResult
{
    /// <summary>
    /// The chapters that were created.
    /// </summary>
    [JsonPropertyName("chapters")]
    public IReadOnlyList<StudyChapter> Chapters { get; init; } = [];
}

/// <summary>
/// A chapter in a study.
/// </summary>
public class StudyChapter
{
    /// <summary>
    /// The chapter ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The chapter name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The players in this chapter (typically 2: white and black).
    /// </summary>
    [JsonPropertyName("players")]
    public IReadOnlyList<StudyChapterPlayer>? Players { get; init; }

    /// <summary>
    /// The game result status (e.g., "1-0", "0-1", "1/2-1/2").
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}

/// <summary>
/// A player in a study chapter.
/// </summary>
public class StudyChapterPlayer
{
    /// <summary>
    /// The player name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// The player rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; init; }
}

