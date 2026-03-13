using System.Runtime.CompilerServices;
using System.Text;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
///     Implementation of the Studies API.
/// </summary>
internal sealed class StudiesApi(ILichessHttpClient httpClient) : IStudiesApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<StudyCreateResult> CreateStudyAsync(CreateStudyOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Name);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("name", options.Name),
            new("visibility", options.Visibility),
            new("computer", options.Computer),
            new("explorer", options.Explorer),
            new("cloneable", options.Cloneable),
            new("shareable", options.Shareable),
            new("chat", options.Chat)
        };

        if (options.Sticky.HasValue)
            parameters.Add(new("sticky", options.Sticky.Value ? "true" : "false"));

        var content = new FormUrlEncodedContent(parameters);
        return await _httpClient.PostAsync<StudyCreateResult>("/api/study", content, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> ExportChapterPgnAsync(string studyId, string chapterId,
        StudyExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(chapterId);

        var endpoint =
            BuildExportEndpoint($"/api/study/{Uri.EscapeDataString(studyId)}/{Uri.EscapeDataString(chapterId)}.pgn",
                options);
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> ExportStudyPgnAsync(string studyId, StudyExportOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studyId);

        var endpoint = BuildExportEndpoint($"/api/study/{Uri.EscapeDataString(studyId)}.pgn", options);
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> ExportUserStudiesPgnAsync(string username, StudyExportOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = BuildExportEndpoint($"/api/study/by/{Uri.EscapeDataString(username)}/export.pgn", options, requiresOrder: true);
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<StudyMetadata> StreamUserStudiesAsync(string username,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = $"/api/study/by/{Uri.EscapeDataString(username)}";
        await foreach (var study in _httpClient.StreamNdjsonAsync<StudyMetadata>(endpoint, cancellationToken)
                           .ConfigureAwait(false)) yield return study;
    }

    /// <inheritdoc />
    public async Task<StudyImportResult> ImportPgnAsync(string studyId, string pgn, StudyImportOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(pgn);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("pgn", pgn)
        };

        if (options != null)
        {
            if (!string.IsNullOrWhiteSpace(options.Name)) parameters.Add(new("name", options.Name));

            if (!string.IsNullOrWhiteSpace(options.Orientation))
                parameters.Add(new("orientation", options.Orientation));

            if (!string.IsNullOrWhiteSpace(options.Variant)) parameters.Add(new("variant", options.Variant));
        }

        var content = new FormUrlEncodedContent(parameters);
        var endpoint = $"/api/study/{Uri.EscapeDataString(studyId)}/import-pgn";
        return await _httpClient.PostAsync<StudyImportResult>(endpoint, content, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateChapterTagsAsync(string studyId, string chapterId, string pgnTags,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(chapterId);
        ArgumentException.ThrowIfNullOrWhiteSpace(pgnTags);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("pgn", pgnTags)
        };

        var content = new FormUrlEncodedContent(parameters);
        var endpoint = $"/api/study/{Uri.EscapeDataString(studyId)}/{Uri.EscapeDataString(chapterId)}/tags";
        await _httpClient.PostNoContentAsync(endpoint, content, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteChapterAsync(string studyId, string chapterId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(chapterId);

        var endpoint = $"/api/study/{Uri.EscapeDataString(studyId)}/{Uri.EscapeDataString(chapterId)}";
        await _httpClient.DeleteNoContentAsync(endpoint, cancellationToken).ConfigureAwait(false);
        return true;
    }

    private static string BuildExportEndpoint(string baseEndpoint, StudyExportOptions? options, bool requiresOrder = false)
    {
        var sb = new StringBuilder(baseEndpoint);
        var hasParams = false;

        // Order is required for user studies export
        if (requiresOrder || options?.Order != null)
        {
            AppendStringParam("order", options?.Order ?? "newest");
        }

        if (options != null)
        {
            AppendBoolParam("clocks", options.Clocks);
            AppendBoolParam("comments", options.Comments);
            AppendBoolParam("variations", options.Variations);
            AppendBoolParam("opening", options.Opening);
            AppendBoolParam("source", options.Source);
            AppendBoolParam("orientation", options.Orientation);
        }

        return sb.ToString();

        void AppendBoolParam(string name, bool? value)
        {
            if (value.HasValue)
            {
                sb.Append(hasParams ? '&' : '?');
                sb.Append(name);
                sb.Append('=');
                sb.Append(value.Value.ToString().ToLowerInvariant());
                hasParams = true;
            }
        }

        void AppendStringParam(string name, string value)
        {
            sb.Append(hasParams ? '&' : '?');
            sb.Append(name);
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(value));
            hasParams = true;
        }
    }
}