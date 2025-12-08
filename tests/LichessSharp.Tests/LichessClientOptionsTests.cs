using FluentAssertions;
using Xunit;

using LichessSharp;

namespace LichessSharp.Tests;

public class LichessClientOptionsTests
{
    [Fact]
    public void DefaultOptions_HasExpectedDefaults()
    {
        // Arrange & Act
        var options = new LichessClientOptions();

        // Assert
        options.AccessToken.Should().BeNull();
        options.BaseAddress.Should().Be(new Uri("https://lichess.org"));
        options.ExplorerBaseAddress.Should().Be(new Uri("https://explorer.lichess.ovh"));
        options.TablebaseBaseAddress.Should().Be(new Uri("https://tablebase.lichess.ovh"));
        options.DefaultTimeout.Should().Be(TimeSpan.FromSeconds(30));
        options.StreamingTimeout.Should().Be(Timeout.InfiniteTimeSpan);
        options.AutoRetryOnRateLimit.Should().BeTrue();
        options.MaxRateLimitRetries.Should().Be(3);
        options.EnableTransientRetry.Should().BeTrue();
        options.MaxTransientRetries.Should().Be(3);
        options.TransientRetryBaseDelay.Should().Be(TimeSpan.FromSeconds(1));
        options.TransientRetryMaxDelay.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void Options_CanBeModified()
    {
        // Arrange
        var options = new LichessClientOptions
        {
            AccessToken = "test-token",
            BaseAddress = new Uri("https://lichess.dev"),
            DefaultTimeout = TimeSpan.FromMinutes(1),
            AutoRetryOnRateLimit = false,
            MaxRateLimitRetries = 5,
            EnableTransientRetry = false,
            MaxTransientRetries = 5,
            TransientRetryBaseDelay = TimeSpan.FromMilliseconds(500),
            TransientRetryMaxDelay = TimeSpan.FromSeconds(10)
        };

        // Assert
        options.AccessToken.Should().Be("test-token");
        options.BaseAddress.Should().Be(new Uri("https://lichess.dev"));
        options.DefaultTimeout.Should().Be(TimeSpan.FromMinutes(1));
        options.AutoRetryOnRateLimit.Should().BeFalse();
        options.MaxRateLimitRetries.Should().Be(5);
        options.EnableTransientRetry.Should().BeFalse();
        options.MaxTransientRetries.Should().Be(5);
        options.TransientRetryBaseDelay.Should().Be(TimeSpan.FromMilliseconds(500));
        options.TransientRetryMaxDelay.Should().Be(TimeSpan.FromSeconds(10));
    }
}
