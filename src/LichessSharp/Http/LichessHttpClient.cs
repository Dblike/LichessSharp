using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using LichessSharp.Exceptions;
using LichessSharp.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LichessSharp.Http;

/// <summary>
/// HTTP client implementation for the Lichess API.
/// </summary>
internal sealed class LichessHttpClient : ILichessHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly LichessClientOptions _options;
    private readonly ILogger<LichessHttpClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public LichessHttpClient(
        HttpClient httpClient,
        IOptions<LichessClientOptions> options,
        ILogger<LichessHttpClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = LichessJsonContext.Default.Options;

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = _options.BaseAddress;
        _httpClient.Timeout = _options.DefaultTimeout;
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrEmpty(_options.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        }
    }

    public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(HttpMethod.Get, endpoint, null, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> GetStringAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(HttpMethod.Get, endpoint, null, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> GetStringWithAcceptAsync(string endpoint, string acceptHeader, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestWithAcceptAsync(HttpMethod.Get, endpoint, null, acceptHeader, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> PostAsync<T>(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(HttpMethod.Post, endpoint, content, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> PostStringAsync(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(HttpMethod.Post, endpoint, content, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> PostPlainTextAsync<T>(string endpoint, string body, CancellationToken cancellationToken = default)
    {
        using var content = new StringContent(body, System.Text.Encoding.UTF8, "text/plain");
        var response = await SendRequestAsync(HttpMethod.Post, endpoint, content, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(HttpMethod.Delete, endpoint, null, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task PostNoContentAsync(string endpoint, HttpContent? content = null, CancellationToken cancellationToken = default)
    {
        await SendRequestAsync(HttpMethod.Post, endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteNoContentAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        await SendRequestAsync(HttpMethod.Delete, endpoint, null, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> GetAbsoluteAsync<T>(Uri absoluteUrl, CancellationToken cancellationToken = default)
    {
        var response = await SendAbsoluteRequestAsync(HttpMethod.Get, absoluteUrl, null, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<T> StreamNdjsonAsync<T>(string endpoint, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in StreamNdjsonCoreAsync<T>(HttpMethod.Get, endpoint, null, cancellationToken).ConfigureAwait(false))
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<T> StreamNdjsonPostAsync<T>(string endpoint, HttpContent? content = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in StreamNdjsonCoreAsync<T>(HttpMethod.Post, endpoint, content, cancellationToken).ConfigureAwait(false))
        {
            yield return item;
        }
    }

    private async IAsyncEnumerable<T> StreamNdjsonCoreAsync<T>(
        HttpMethod method,
        string endpoint,
        HttpContent? content,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, endpoint);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-ndjson"));

        if (content != null)
        {
            request.Content = content;
        }

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken).ConfigureAwait(false);

        await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream);

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);

            // ReadLineAsync returns null at end of stream
            if (line == null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            T? item;
            try
            {
                item = JsonSerializer.Deserialize<T>(line, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize ndjson line: {Line}", line);
                continue;
            }

            if (item != null)
            {
                yield return item;
            }
        }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        HttpMethod method,
        string endpoint,
        HttpContent? content,
        CancellationToken cancellationToken)
    {
        var retryCount = 0;
        var maxRetries = _options.AutoRetryOnRateLimit ? _options.MaxRateLimitRetries : 0;

        while (true)
        {
            using var request = new HttpRequestMessage(method, endpoint);

            if (content != null)
            {
                request.Content = content;
            }

            _logger.LogDebug("Sending {Method} request to {Endpoint}", method, endpoint);

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.TooManyRequests && retryCount < maxRetries)
            {
                retryCount++;
                var retryAfter = GetRetryAfter(response);

                _logger.LogWarning(
                    "Rate limited. Waiting {RetryAfter} before retry {RetryCount}/{MaxRetries}",
                    retryAfter,
                    retryCount,
                    maxRetries);

                await Task.Delay(retryAfter, cancellationToken).ConfigureAwait(false);
                continue;
            }

            await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);
            return response;
        }
    }

    private async Task<HttpResponseMessage> SendRequestWithAcceptAsync(
        HttpMethod method,
        string endpoint,
        HttpContent? content,
        string acceptHeader,
        CancellationToken cancellationToken)
    {
        var retryCount = 0;
        var maxRetries = _options.AutoRetryOnRateLimit ? _options.MaxRateLimitRetries : 0;

        while (true)
        {
            using var request = new HttpRequestMessage(method, endpoint);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

            if (content != null)
            {
                request.Content = content;
            }

            _logger.LogDebug("Sending {Method} request to {Endpoint} with Accept: {Accept}", method, endpoint, acceptHeader);

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.TooManyRequests && retryCount < maxRetries)
            {
                retryCount++;
                var retryAfter = GetRetryAfter(response);

                _logger.LogWarning(
                    "Rate limited. Waiting {RetryAfter} before retry {RetryCount}/{MaxRetries}",
                    retryAfter,
                    retryCount,
                    maxRetries);

                await Task.Delay(retryAfter, cancellationToken).ConfigureAwait(false);
                continue;
            }

            await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);
            return response;
        }
    }

    private async Task<HttpResponseMessage> SendAbsoluteRequestAsync(
        HttpMethod method,
        Uri absoluteUrl,
        HttpContent? content,
        CancellationToken cancellationToken)
    {
        var retryCount = 0;
        var maxRetries = _options.AutoRetryOnRateLimit ? _options.MaxRateLimitRetries : 0;

        while (true)
        {
            using var request = new HttpRequestMessage(method, absoluteUrl);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (content != null)
            {
                request.Content = content;
            }

            _logger.LogDebug("Sending {Method} request to {Url}", method, absoluteUrl);

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.TooManyRequests && retryCount < maxRetries)
            {
                retryCount++;
                var retryAfter = GetRetryAfter(response);

                _logger.LogWarning(
                    "Rate limited. Waiting {RetryAfter} before retry {RetryCount}/{MaxRetries}",
                    retryAfter,
                    retryCount,
                    maxRetries);

                await Task.Delay(retryAfter, cancellationToken).ConfigureAwait(false);
                continue;
            }

            await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);
            return response;
        }
    }

    private static TimeSpan GetRetryAfter(HttpResponseMessage response)
    {
        if (response.Headers.RetryAfter?.Delta is { } delta)
        {
            return delta;
        }

        if (response.Headers.RetryAfter?.Date is { } date)
        {
            var delay = date - DateTimeOffset.UtcNow;
            return delay > TimeSpan.Zero ? delay : TimeSpan.FromSeconds(60);
        }

        return TimeSpan.FromSeconds(60);
    }

    private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var lichessError = TryExtractError(content);

        throw response.StatusCode switch
        {
            HttpStatusCode.TooManyRequests => new LichessRateLimitException(
                "API rate limit exceeded. Please wait before making more requests.",
                GetRetryAfter(response)),

            HttpStatusCode.Unauthorized => new LichessAuthenticationException(
                "Authentication failed. Please check your access token.",
                lichessError),

            HttpStatusCode.Forbidden => new LichessAuthorizationException(
                "Access denied. Your token may not have the required scope for this operation.",
                lichessError: lichessError),

            HttpStatusCode.NotFound => new LichessNotFoundException(
                "The requested resource was not found.",
                lichessError),

            HttpStatusCode.BadRequest => new LichessValidationException(
                "The request was invalid.",
                lichessError),

            _ => new LichessException(
                $"Request failed with status code {(int)response.StatusCode}",
                response.StatusCode,
                lichessError)
        };
    }

    private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions, cancellationToken).ConfigureAwait(false);

        return result ?? throw new LichessException("Failed to deserialize response");
    }

    private static string? TryExtractError(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.TryGetProperty("error", out var errorElement))
            {
                return errorElement.GetString();
            }
        }
        catch
        {
            // Content is not JSON
        }

        return content.Length > 200 ? content[..200] : content;
    }
}
