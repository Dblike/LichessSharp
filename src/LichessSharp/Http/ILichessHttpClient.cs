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
    /// Sends a POST request to the specified endpoint.
    /// </summary>
    Task<T> PostAsync<T>(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request and returns the raw response content as a string.
    /// </summary>
    Task<string> PostStringAsync(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DELETE request to the specified endpoint.
    /// </summary>
    Task<T> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams newline-delimited JSON from the specified endpoint.
    /// </summary>
    IAsyncEnumerable<T> StreamNdjsonAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams newline-delimited JSON from the specified endpoint using POST.
    /// </summary>
    IAsyncEnumerable<T> StreamNdjsonPostAsync<T>(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default);
}
