using System.Net;
using FluentAssertions;
using LichessSharp.Exceptions;
using Xunit;

namespace LichessSharp.Tests.Exceptions;

public class LichessExceptionTests
{
    [Fact]
    public void LichessException_WithMessage_SetsMessage()
    {
        // Arrange & Act
        var ex = new LichessException("Test error");

        // Assert
        ex.Message.Should().Be("Test error");
        ex.StatusCode.Should().BeNull();
        ex.LichessError.Should().BeNull();
    }

    [Fact]
    public void LichessException_WithStatusCode_SetsStatusCode()
    {
        // Arrange & Act
        var ex = new LichessException("Test error", HttpStatusCode.BadRequest, "API error");

        // Assert
        ex.Message.Should().Be("Test error");
        ex.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ex.LichessError.Should().Be("API error");
    }

    [Fact]
    public void LichessRateLimitException_SetsRetryAfter()
    {
        // Arrange & Act
        var ex = new LichessRateLimitException("Rate limited", TimeSpan.FromMinutes(1));

        // Assert
        ex.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        ex.RetryAfter.Should().Be(TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void LichessAuthenticationException_SetsStatusCode()
    {
        // Arrange & Act
        var ex = new LichessAuthenticationException("Invalid token");

        // Assert
        ex.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public void LichessAuthorizationException_SetsRequiredScope()
    {
        // Arrange & Act
        var ex = new LichessAuthorizationException("Access denied", "email:read");

        // Assert
        ex.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        ex.RequiredScope.Should().Be("email:read");
    }

    [Fact]
    public void LichessNotFoundException_SetsStatusCode()
    {
        // Arrange & Act
        var ex = new LichessNotFoundException("User not found");

        // Assert
        ex.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void LichessValidationException_SetsValidationErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "username", ["Username is required"] }
        };

        // Act
        var ex = new LichessValidationException("Validation failed", errors);

        // Assert
        ex.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ex.ValidationErrors.Should().NotBeNull();
        ex.ValidationErrors!["username"].Should().Contain("Username is required");
    }
}
