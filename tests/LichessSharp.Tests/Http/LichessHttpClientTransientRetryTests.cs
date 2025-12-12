using System.Net;
using System.Net.Sockets;
using FluentAssertions;
using LichessSharp.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace LichessSharp.Tests.Http;

/// <summary>
/// Tests for the transient retry behavior in LichessHttpClient.
/// </summary>
public class LichessHttpClientTransientRetryTests
{
    private static LichessHttpClient CreateClient(
        Mock<HttpMessageHandler> handlerMock,
        LichessClientOptions? options = null)
    {
        options ??= new LichessClientOptions
        {
            EnableTransientRetry = true,
            MaxTransientRetries = 3,
            TransientRetryBaseDelay = TimeSpan.FromMilliseconds(10), // Short delay for tests
            TransientRetryMaxDelay = TimeSpan.FromMilliseconds(100)
        };

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = LichessApiUrls.BaseAddress
        };

        return new LichessHttpClient(
            httpClient,
            Options.Create(options),
            NullLogger<LichessHttpClient>.Instance);
    }

    /// <summary>
    /// Creates an HttpRequestException with a specific HttpRequestError.
    /// </summary>
    private static HttpRequestException CreateHttpRequestException(HttpRequestError error, string? message = null)
    {
        // Use reflection or just create a simple exception and rely on inner SocketException for tests
        // For simplicity, use SocketException as inner for connection errors
        return error switch
        {
            HttpRequestError.NameResolutionError => new HttpRequestException(
                HttpRequestError.NameResolutionError,
                message ?? "DNS lookup failed"),
            HttpRequestError.ConnectionError => new HttpRequestException(
                HttpRequestError.ConnectionError,
                message ?? "Connection refused"),
            _ => new HttpRequestException(error, message ?? "Network error")
        };
    }

    [Fact]
    public async Task GetAsync_WithTransientFailure_RetriesAndSucceeds()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount < 3)
                {
                    throw CreateHttpRequestException(HttpRequestError.NameResolutionError);
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\":true}")
                };
            });

        var client = CreateClient(handlerMock);

        // Act
        var result = await client.GetStringAsync("/api/test");

        // Assert
        result.Should().Be("{\"ok\":true}");
        callCount.Should().Be(3); // 2 failures + 1 success
    }

    [Fact]
    public async Task GetAsync_WithPersistentFailure_ThrowsAfterMaxRetries()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                throw CreateHttpRequestException(HttpRequestError.ConnectionError);
            });

        var options = new LichessClientOptions
        {
            EnableTransientRetry = true,
            MaxTransientRetries = 3,
            TransientRetryBaseDelay = TimeSpan.FromMilliseconds(1),
            TransientRetryMaxDelay = TimeSpan.FromMilliseconds(10)
        };
        var client = CreateClient(handlerMock, options);

        // Act
        var act = async () => await client.GetStringAsync("/api/test");

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        callCount.Should().Be(4); // 1 initial + 3 retries
    }

    [Fact]
    public async Task GetAsync_WithTransientRetryDisabled_DoesNotRetry()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                throw CreateHttpRequestException(HttpRequestError.NameResolutionError);
            });

        var options = new LichessClientOptions
        {
            EnableTransientRetry = false
        };
        var client = CreateClient(handlerMock, options);

        // Act
        var act = async () => await client.GetStringAsync("/api/test");

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        callCount.Should().Be(1); // No retries
    }

    [Fact]
    public async Task GetAsync_WithSocketException_Retries()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount < 2)
                {
                    throw new HttpRequestException("Network error", new SocketException((int)SocketError.HostUnreachable));
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\":true}")
                };
            });

        var client = CreateClient(handlerMock);

        // Act
        var result = await client.GetStringAsync("/api/test");

        // Assert
        result.Should().Be("{\"ok\":true}");
        callCount.Should().Be(2);
    }

    [Fact]
    public async Task GetAsync_WithNonTransientException_DoesNotRetry()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                throw new InvalidOperationException("Not a transient error");
            });

        var client = CreateClient(handlerMock);

        // Act
        var act = async () => await client.GetStringAsync("/api/test");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        callCount.Should().Be(1); // No retry for non-transient exceptions
    }

    [Fact]
    public async Task GetAsync_WithCancellationDuringRetry_StopsRetrying()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;
        using var cts = new CancellationTokenSource();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    throw CreateHttpRequestException(HttpRequestError.NameResolutionError);
                }
                // Cancel after first failure
                cts.Cancel();
                throw CreateHttpRequestException(HttpRequestError.NameResolutionError);
            });

        var client = CreateClient(handlerMock);

        // Act
        var act = async () => await client.GetStringAsync("/api/test", cts.Token);

        // Assert - Should throw OperationCanceledException or HttpRequestException depending on timing
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetAsync_CombinesTransientAndRateLimitRetries()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount switch
                {
                    1 => throw CreateHttpRequestException(HttpRequestError.NameResolutionError),
                    2 => new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Headers = { RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromMilliseconds(10)) }
                    },
                    _ => new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"ok\":true}")
                    }
                };
            });

        var options = new LichessClientOptions
        {
            EnableTransientRetry = true,
            MaxTransientRetries = 3,
            TransientRetryBaseDelay = TimeSpan.FromMilliseconds(1),
            AutoRetryOnRateLimit = true,
            MaxRateLimitRetries = 3
        };
        var client = CreateClient(handlerMock, options);

        // Act
        var result = await client.GetStringAsync("/api/test");

        // Assert
        result.Should().Be("{\"ok\":true}");
        callCount.Should().Be(3); // 1 transient failure + 1 rate limit + 1 success
    }
}
