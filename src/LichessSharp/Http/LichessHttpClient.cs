using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
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

    public async Task<T> PostFormAsync<T>(string endpoint, IDictionary<string, string> formData, CancellationToken cancellationToken = default)
    {
        using var content = new FormUrlEncodedContent(formData);
        var response = await SendRequestAsync(HttpMethod.Post, endpoint, content, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(HttpMethod.Delete, endpoint, null, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> PutJsonAsync<T>(string endpoint, object body, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendRequestAsync(HttpMethod.Put, endpoint, content, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> PostJsonAsync<T>(string endpoint, object body, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendRequestAsync(HttpMethod.Post, endpoint, content, cancellationToken).ConfigureAwait(false);
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

    public async Task<T> GetAbsoluteNdjsonLastAsync<T>(Uri absoluteUrl, CancellationToken cancellationToken = default)
    {
        var response = await SendAbsoluteRequestAsync(HttpMethod.Get, absoluteUrl, null, cancellationToken).ConfigureAwait(false);

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream);

        T? lastItem = default;
        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            try
            {
                var item = JsonSerializer.Deserialize<T>(line, _jsonOptions);
                if (item != null)
                {
                    lastItem = item;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize ndjson line: {Line}", line);
            }
        }

        if (lastItem == null)
        {
            throw new LichessException("No valid JSON lines found in response");
        }

        return lastItem;
    }

    public async Task<string> GetAbsoluteStringAsync(Uri absoluteUrl, string acceptHeader, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, absoluteUrl);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
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

    public async Task<T> PostAbsoluteJsonAsync<T>(Uri absoluteUrl, object body, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendAbsoluteRequestAsync(HttpMethod.Post, absoluteUrl, content, cancellationToken).ConfigureAwait(false);
        return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<T> StreamAbsoluteNdjsonPostAsync<T>(Uri absoluteUrl, object body, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, absoluteUrl);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-ndjson"));
        request.Content = content;

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

    public async Task PostAbsoluteStreamAsync(Uri absoluteUrl, IAsyncEnumerable<string> lines, CancellationToken cancellationToken = default)
    {
        // Create a pipe for streaming content
        using var pipeContent = new StreamContent(new AsyncEnumerableStream(lines, cancellationToken));
        pipeContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

        using var request = new HttpRequestMessage(HttpMethod.Post, absoluteUrl);
        request.Content = pipeContent;

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessStatusCodeAsync(response, cancellationToken).ConfigureAwait(false);
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
        var rateLimitRetryCount = 0;
        var transientRetryCount = 0;
        var maxRateLimitRetries = _options.AutoRetryOnRateLimit ? _options.MaxRateLimitRetries : 0;
        // Transient retry is only safe for requests without content (GET, etc.)
        // Content cannot be resent after the first attempt as the stream is consumed
        var maxTransientRetries = content == null && _options.EnableTransientRetry ? _options.MaxTransientRetries : 0;

        while (true)
        {
            HttpResponseMessage response;

            try
            {
                using var request = new HttpRequestMessage(method, endpoint);

                if (content != null)
                {
                    request.Content = content;
                }

                _logger.LogDebug("Sending {Method} request to {Endpoint}", method, endpoint);

                response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested &&
                                       transientRetryCount < maxTransientRetries &&
                                       IsTransientException(ex))
            {
                transientRetryCount++;
                var delay = CalculateTransientRetryDelay(transientRetryCount);

                _logger.LogWarning(
                    ex,
                    "Transient failure on {Method} {Endpoint}. Waiting {Delay} before retry {RetryCount}/{MaxRetries}",
                    method,
                    endpoint,
                    delay,
                    transientRetryCount,
                    maxTransientRetries);

                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                continue;
            }

            // Note: Rate limit retry with content is problematic because the content has been consumed.
            // However, we skip rate limit retry for requests with content to avoid this issue.
            if (response.StatusCode == HttpStatusCode.TooManyRequests &&
                content == null &&
                rateLimitRetryCount < maxRateLimitRetries)
            {
                rateLimitRetryCount++;
                var retryAfter = GetRetryAfter(response);

                _logger.LogWarning(
                    "Rate limited. Waiting {RetryAfter} before retry {RetryCount}/{MaxRetries}",
                    retryAfter,
                    rateLimitRetryCount,
                    maxRateLimitRetries);

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
        var rateLimitRetryCount = 0;
        var transientRetryCount = 0;
        var maxRateLimitRetries = _options.AutoRetryOnRateLimit ? _options.MaxRateLimitRetries : 0;
        // Transient retry is only safe for requests without content (GET, etc.)
        // Content cannot be resent after the first attempt as the stream is consumed
        var maxTransientRetries = content == null && _options.EnableTransientRetry ? _options.MaxTransientRetries : 0;

        while (true)
        {
            HttpResponseMessage response;

            try
            {
                using var request = new HttpRequestMessage(method, endpoint);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

                if (content != null)
                {
                    request.Content = content;
                }

                _logger.LogDebug("Sending {Method} request to {Endpoint} with Accept: {Accept}", method, endpoint, acceptHeader);

                response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested &&
                                       transientRetryCount < maxTransientRetries &&
                                       IsTransientException(ex))
            {
                transientRetryCount++;
                var delay = CalculateTransientRetryDelay(transientRetryCount);

                _logger.LogWarning(
                    ex,
                    "Transient failure on {Method} {Endpoint}. Waiting {Delay} before retry {RetryCount}/{MaxRetries}",
                    method,
                    endpoint,
                    delay,
                    transientRetryCount,
                    maxTransientRetries);

                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                continue;
            }

            // Skip rate limit retry for requests with content as the content stream is consumed
            if (response.StatusCode == HttpStatusCode.TooManyRequests &&
                content == null &&
                rateLimitRetryCount < maxRateLimitRetries)
            {
                rateLimitRetryCount++;
                var retryAfter = GetRetryAfter(response);

                _logger.LogWarning(
                    "Rate limited. Waiting {RetryAfter} before retry {RetryCount}/{MaxRetries}",
                    retryAfter,
                    rateLimitRetryCount,
                    maxRateLimitRetries);

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
        var rateLimitRetryCount = 0;
        var transientRetryCount = 0;
        var maxRateLimitRetries = _options.AutoRetryOnRateLimit ? _options.MaxRateLimitRetries : 0;
        // Transient retry is only safe for requests without content (GET, etc.)
        // Content cannot be resent after the first attempt as the stream is consumed
        var maxTransientRetries = content == null && _options.EnableTransientRetry ? _options.MaxTransientRetries : 0;

        while (true)
        {
            HttpResponseMessage response;

            try
            {
                using var request = new HttpRequestMessage(method, absoluteUrl);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (content != null)
                {
                    request.Content = content;
                }

                _logger.LogDebug("Sending {Method} request to {Url}", method, absoluteUrl);

                response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested &&
                                       transientRetryCount < maxTransientRetries &&
                                       IsTransientException(ex))
            {
                transientRetryCount++;
                var delay = CalculateTransientRetryDelay(transientRetryCount);

                _logger.LogWarning(
                    ex,
                    "Transient failure on {Method} {Url}. Waiting {Delay} before retry {RetryCount}/{MaxRetries}",
                    method,
                    absoluteUrl,
                    delay,
                    transientRetryCount,
                    maxTransientRetries);

                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                continue;
            }

            // Skip rate limit retry for requests with content as the content stream is consumed
            if (response.StatusCode == HttpStatusCode.TooManyRequests &&
                content == null &&
                rateLimitRetryCount < maxRateLimitRetries)
            {
                rateLimitRetryCount++;
                var retryAfter = GetRetryAfter(response);

                _logger.LogWarning(
                    "Rate limited. Waiting {RetryAfter} before retry {RetryCount}/{MaxRetries}",
                    retryAfter,
                    rateLimitRetryCount,
                    maxRateLimitRetries);

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
        var str = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
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

    /// <summary>
    /// Determines if an exception represents a transient failure that should be retried.
    /// </summary>
    private static bool IsTransientException(Exception ex)
    {
        return ex switch
        {
            // HttpRequestException covers DNS failures, connection refused, etc.
            HttpRequestException httpEx => IsTransientHttpRequestException(httpEx),

            // TaskCanceledException with TimeoutException inner exception indicates a timeout
            TaskCanceledException { InnerException: TimeoutException } => true,

            // Socket exceptions are transient network issues
            SocketException => true,

            _ => false
        };
    }

    /// <summary>
    /// Determines if an HttpRequestException represents a transient failure.
    /// </summary>
    private static bool IsTransientHttpRequestException(HttpRequestException ex)
    {
        // Check inner exception for socket/network errors
        if (ex.InnerException is SocketException)
        {
            return true;
        }

        // ObjectDisposedException means the content was already consumed - not transient
        if (ex.InnerException is ObjectDisposedException)
        {
            return false;
        }

        // DNS resolution failures, connection refused, etc.
        // HttpRequestError was added in .NET 7 and is non-nullable
        return ex.HttpRequestError switch
        {
            HttpRequestError.NameResolutionError => true,
            HttpRequestError.ConnectionError => true,
            HttpRequestError.SecureConnectionError => true,
            HttpRequestError.HttpProtocolError => true,
            HttpRequestError.ResponseEnded => true,
            HttpRequestError.Unknown => true, // Treat unknown as potentially transient
            _ => false
        };
    }

    /// <summary>
    /// Calculates the delay for a transient retry with exponential backoff and jitter.
    /// </summary>
    private TimeSpan CalculateTransientRetryDelay(int retryAttempt)
    {
        // Exponential backoff: baseDelay * 2^(retryAttempt-1)
        var exponentialDelay = _options.TransientRetryBaseDelay.TotalMilliseconds * Math.Pow(2, retryAttempt - 1);

        // Add jitter (0-25% of the delay) to prevent thundering herd
        var jitter = exponentialDelay * Random.Shared.NextDouble() * 0.25;

        var totalDelayMs = exponentialDelay + jitter;

        // Cap at max delay
        var maxDelayMs = _options.TransientRetryMaxDelay.TotalMilliseconds;
        if (totalDelayMs > maxDelayMs)
        {
            totalDelayMs = maxDelayMs;
        }

        return TimeSpan.FromMilliseconds(totalDelayMs);
    }

    /// <summary>
    /// A stream that reads from an async enumerable of strings, converting each to a line.
    /// </summary>
    private sealed class AsyncEnumerableStream : Stream
    {
        private readonly IAsyncEnumerator<string> _enumerator;
        private readonly CancellationToken _cancellationToken;
        private byte[] _currentBuffer = [];
        private int _currentPosition;
        private bool _completed;

        public AsyncEnumerableStream(IAsyncEnumerable<string> lines, CancellationToken cancellationToken)
        {
            _enumerator = lines.GetAsyncEnumerator(cancellationToken);
            _cancellationToken = cancellationToken;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_completed)
            {
                return 0;
            }

            // If we have data in the current buffer, return it
            if (_currentPosition < _currentBuffer.Length)
            {
                var bytesToCopy = Math.Min(count, _currentBuffer.Length - _currentPosition);
                Array.Copy(_currentBuffer, _currentPosition, buffer, offset, bytesToCopy);
                _currentPosition += bytesToCopy;
                return bytesToCopy;
            }

            // Try to get the next line
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationToken);
            if (!await _enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                _completed = true;
                return 0;
            }

            // Convert line to bytes with newline
            _currentBuffer = System.Text.Encoding.UTF8.GetBytes(_enumerator.Current + "\n");
            _currentPosition = 0;

            var bytesToReturn = Math.Min(count, _currentBuffer.Length);
            Array.Copy(_currentBuffer, 0, buffer, offset, bytesToReturn);
            _currentPosition = bytesToReturn;
            return bytesToReturn;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _enumerator.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            await _enumerator.DisposeAsync().ConfigureAwait(false);
            await base.DisposeAsync().ConfigureAwait(false);
        }
    }
}
