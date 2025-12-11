namespace LichessSharp.Http;

/// <summary>
/// Internal interface for making HTTP requests to the Lichess API.
/// Handles authentication, rate limiting, and error handling.
/// </summary>
internal interface ILichessHttpClient
{
    /// <summary>
    /// Sends a GET request to the specified endpoint.
    /// </summary>
    Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a GET request and returns the raw response content as a string.
    /// </summary>
    Task<string> GetStringAsync(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a GET request with a custom Accept header and returns the raw response content as a string.
    /// </summary>
    Task<string> GetStringWithAcceptAsync(string endpoint, string acceptHeader, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request to the specified endpoint.
    /// </summary>
    Task<T> PostAsync<T>(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request and returns the raw response content as a string.
    /// </summary>
    Task<string> PostStringAsync(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request with plain text body content.
    /// </summary>
    Task<T> PostPlainTextAsync<T>(string endpoint, string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request with form-urlencoded body content.
    /// </summary>
    Task<T> PostFormAsync<T>(string endpoint, IDictionary<string, string> formData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DELETE request to the specified endpoint.
    /// </summary>
    Task<T> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request and expects 204 No Content response.
    /// </summary>
    Task PostNoContentAsync(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DELETE request and expects 204 No Content response.
    /// </summary>
    Task DeleteNoContentAsync(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a GET request to a fully qualified URL (for external APIs like explorer.lichess.ovh).
    /// </summary>
    Task<T> GetAbsoluteAsync<T>(Uri absoluteUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a GET request to a fully qualified URL and reads all NDJSON lines, returning the last line.
    /// This is useful for progressive APIs that send partial updates followed by a final complete result.
    /// </summary>
    Task<T> GetAbsoluteNdjsonLastAsync<T>(Uri absoluteUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a GET request to a fully qualified URL with a custom Accept header and returns the raw response content as a string.
    /// </summary>
    Task<string> GetAbsoluteStringAsync(Uri absoluteUrl, string acceptHeader, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams newline-delimited JSON from the specified endpoint.
    /// </summary>
    IAsyncEnumerable<T> StreamNdjsonAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams newline-delimited JSON from the specified endpoint using POST.
    /// </summary>
    IAsyncEnumerable<T> StreamNdjsonPostAsync<T>(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default);
}
