using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using LichessSharp.Exceptions;
using LichessSharp.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace LichessSharp.Tests.Http;

/// <summary>
///     Tests for the rate limit retry behavior in LichessHttpClient.
/// </summary>
public class LichessHttpClientRateLimitRetryTests
{
    private static LichessHttpClient CreateClient(
        Mock<HttpMessageHandler> handlerMock,
        LichessClientOptions? options = null)
    {
        options ??= new LichessClientOptions
        {
            AutoRetryOnRateLimit = true,
            MaxRateLimitRetries = 3
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

    [Fact]
    public async Task GetAsync_WithRateLimit_RetriesUsingRetryAfterHeader()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;
        var retryAfterDelay = TimeSpan.FromMilliseconds(50);

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
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Headers = { RetryAfter = new RetryConditionHeaderValue(retryAfterDelay) }
                    };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\":true}")
                };
            });

        var client = CreateClient(handlerMock);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await client.GetStringAsync("/api/test");
        stopwatch.Stop();

        // Assert
        result.Should().Be("{\"ok\":true}");
        callCount.Should().Be(2); // 1 rate limit + 1 success
        stopwatch.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(40); // Should have waited ~50ms
    }

    [Fact]
    public async Task GetAsync_WithRateLimitNoHeader_UsesFallbackDelay()
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
                    // No Retry-After header - should use fallback of 60 seconds
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\":true}")
                };
            });

        var client = CreateClient(handlerMock);

        // Act - use a cancellation token to avoid waiting 60 seconds
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        // The first call will get rate limited, then wait 60 seconds (fallback)
        // We'll cancel before that happens
        var act = async () => await client.GetStringAsync("/api/test", cts.Token);

        // Assert - should be cancelled during the 60-second wait
        await act.Should().ThrowAsync<OperationCanceledException>();
        callCount.Should().Be(1); // Only the first call should have been made
    }

    [Fact]
    public async Task GetAsync_WithUnlimitedRetries_RetriesIndefinitely()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;
        const int rateLimitResponses = 10; // More than the default MaxRateLimitRetries of 3

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount <= rateLimitResponses)
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Headers = { RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMilliseconds(1)) }
                    };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\":true}")
                };
            });

        var options = new LichessClientOptions
        {
            AutoRetryOnRateLimit = true,
            UnlimitedRateLimitRetries = true
        };
        var client = CreateClient(handlerMock, options);

        // Act
        var result = await client.GetStringAsync("/api/test");

        // Assert
        result.Should().Be("{\"ok\":true}");
        callCount.Should().Be(rateLimitResponses + 1); // 10 rate limits + 1 success
    }

    [Fact]
    public async Task GetAsync_WithUnlimitedRetriesAndCancellation_StopsRetrying()
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
                // Always return rate limit
                return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Headers = { RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMilliseconds(50)) }
                };
            });

        var options = new LichessClientOptions
        {
            AutoRetryOnRateLimit = true,
            UnlimitedRateLimitRetries = true
        };
        var client = CreateClient(handlerMock, options);

        // Act - cancel after a short time
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));
        var act = async () => await client.GetStringAsync("/api/test", cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
        callCount.Should().BeGreaterThanOrEqualTo(1).And.BeLessThan(10); // Should have made some calls but not too many
    }

    [Fact]
    public async Task GetAsync_WithMaxRetriesExhausted_ThrowsRateLimitException()
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
                return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Headers = { RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMilliseconds(1)) }
                };
            });

        var options = new LichessClientOptions
        {
            AutoRetryOnRateLimit = true,
            MaxRateLimitRetries = 3,
            UnlimitedRateLimitRetries = false
        };
        var client = CreateClient(handlerMock, options);

        // Act
        var act = async () => await client.GetStringAsync("/api/test");

        // Assert
        await act.Should().ThrowAsync<LichessRateLimitException>();
        callCount.Should().Be(4); // 1 initial + 3 retries
    }

    [Fact]
    public async Task PostAsync_WithRateLimit_DoesNotRetry()
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
                return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Headers = { RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMilliseconds(1)) }
                };
            });

        var options = new LichessClientOptions
        {
            AutoRetryOnRateLimit = true,
            MaxRateLimitRetries = 3
        };
        var client = CreateClient(handlerMock, options);

        // Act - POST with content should not retry (content is consumed)
        using var content = new StringContent("{\"data\":\"test\"}");
        var act = async () => await client.PostStringAsync("/api/test", content);

        // Assert
        await act.Should().ThrowAsync<LichessRateLimitException>();
        callCount.Should().Be(1); // No retries for POST with content
    }

    [Fact]
    public async Task GetAsync_WithAutoRetryDisabled_DoesNotRetry()
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
                return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Headers = { RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMilliseconds(1)) }
                };
            });

        var options = new LichessClientOptions
        {
            AutoRetryOnRateLimit = false
        };
        var client = CreateClient(handlerMock, options);

        // Act
        var act = async () => await client.GetStringAsync("/api/test");

        // Assert
        await act.Should().ThrowAsync<LichessRateLimitException>();
        callCount.Should().Be(1); // No retries when auto-retry is disabled
    }

    [Fact]
    public async Task GetAsync_WithRetryAfterDate_CalculatesCorrectDelay()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        var callCount = 0;
        var retryAfterDate = DateTimeOffset.UtcNow.AddMilliseconds(100);

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
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Headers = { RetryAfter = new RetryConditionHeaderValue(retryAfterDate) }
                    };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\":true}")
                };
            });

        var client = CreateClient(handlerMock);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await client.GetStringAsync("/api/test");
        stopwatch.Stop();

        // Assert
        result.Should().Be("{\"ok\":true}");
        callCount.Should().Be(2);
        // Should have waited approximately until the retry-after date
        stopwatch.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(50);
    }

    [Fact]
    public async Task GetAsync_UnlimitedRetriesRequiresAutoRetryEnabled()
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
                return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Headers = { RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMilliseconds(1)) }
                };
            });

        // UnlimitedRateLimitRetries is true, but AutoRetryOnRateLimit is false
        var options = new LichessClientOptions
        {
            AutoRetryOnRateLimit = false,
            UnlimitedRateLimitRetries = true
        };
        var client = CreateClient(handlerMock, options);

        // Act
        var act = async () => await client.GetStringAsync("/api/test");

        // Assert - should not retry because AutoRetryOnRateLimit is false
        await act.Should().ThrowAsync<LichessRateLimitException>();
        callCount.Should().Be(1);
    }
}
