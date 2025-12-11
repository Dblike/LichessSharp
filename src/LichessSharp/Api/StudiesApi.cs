using System.Runtime.CompilerServices;
using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Studies API.
/// </summary>
internal sealed class StudiesApi(ILichessHttpClient httpClient) : IStudiesApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<string> ExportChapterPgnAsync(string studyId, string chapterId, StudyExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(chapterId);

        var endpoint = BuildExportEndpoint($"/api/study/{Uri.EscapeDataString(studyId)}/{Uri.EscapeDataString(chapterId)}.pgn", options);
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> ExportStudyPgnAsync(string studyId, StudyExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studyId);

        var endpoint = BuildExportEndpoint($"/api/study/{Uri.EscapeDataString(studyId)}.pgn", options);
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string> ExportUserStudiesPgnAsync(string username, StudyExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = BuildExportEndpoint($"/study/by/{Uri.EscapeDataString(username)}/export.pgn", options);
        return await _httpClient.GetStringWithAcceptAsync(endpoint, "application/x-chess-pgn", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<StudyMetadata> StreamUserStudiesAsync(string username, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = $"/api/study/by/{Uri.EscapeDataString(username)}";
        await foreach (var study in _httpClient.StreamNdjsonAsync<StudyMetadata>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return study;
        }
    }

    /// <inheritdoc />
    public async Task<StudyImportResult> ImportPgnAsync(string studyId, string pgn, StudyImportOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(pgn);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("pgn", pgn)
        };

        if (options != null)
        {
            if (!string.IsNullOrWhiteSpace(options.Name))
            {
                parameters.Add(new("name", options.Name));
            }

            if (!string.IsNullOrWhiteSpace(options.Orientation))
            {
                parameters.Add(new("orientation", options.Orientation));
            }

            if (!string.IsNullOrWhiteSpace(options.Variant))
            {
                parameters.Add(new("variant", options.Variant));
            }
        }

        var content = new FormUrlEncodedContent(parameters);
        var endpoint = $"/api/study/{Uri.EscapeDataString(studyId)}/import-pgn";
        return await _httpClient.PostAsync<StudyImportResult>(endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateChapterTagsAsync(string studyId, string chapterId, string pgnTags, CancellationToken cancellationToken = default)
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
    public async Task<bool> DeleteChapterAsync(string studyId, string chapterId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(chapterId);

        var endpoint = $"/api/study/{Uri.EscapeDataString(studyId)}/{Uri.EscapeDataString(chapterId)}";
        await _httpClient.DeleteNoContentAsync(endpoint, cancellationToken).ConfigureAwait(false);
        return true;
    }

    private static string BuildExportEndpoint(string baseEndpoint, StudyExportOptions? options)
    {
        if (options == null)
        {
            return baseEndpoint;
        }

        var sb = new StringBuilder(baseEndpoint);
        var hasParams = false;

        AppendParam("clocks", options.Clocks);
        AppendParam("comments", options.Comments);
        AppendParam("variations", options.Variations);
        AppendParam("opening", options.Opening);
        AppendParam("source", options.Source);
        AppendParam("orientation", options.Orientation);

        return sb.ToString();

        void AppendParam(string name, bool? value)
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
    }
}
