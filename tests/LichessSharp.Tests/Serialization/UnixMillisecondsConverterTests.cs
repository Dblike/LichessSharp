using System.Text.Json;
using FluentAssertions;
using LichessSharp.Serialization.Converters;
using Xunit;

namespace LichessSharp.Tests.Serialization;

public class UnixMillisecondsConverterTests
{
    private readonly JsonSerializerOptions _options;

    public UnixMillisecondsConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new UnixMillisecondsConverter());
    }

    [Fact]
    public void Read_ValidTimestamp_ReturnsCorrectDateTimeOffset()
    {
        // Arrange
        var json = "1609459200000"; // 2021-01-01 00:00:00 UTC

        // Act
        var result = JsonSerializer.Deserialize<DateTimeOffset>(json, _options);

        // Assert
        result.Should().Be(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void Write_ValidDateTimeOffset_ReturnsCorrectTimestamp()
    {
        // Arrange
        var date = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = JsonSerializer.Serialize(date, _options);

        // Assert
        result.Should().Be("1609459200000");
    }

    [Fact]
    public void Read_ZeroTimestamp_ReturnsUnixEpoch()
    {
        // Arrange
        var json = "0";

        // Act
        var result = JsonSerializer.Deserialize<DateTimeOffset>(json, _options);

        // Assert
        result.Should().Be(DateTimeOffset.UnixEpoch);
    }
}

public class NullableUnixMillisecondsConverterTests
{
    private readonly JsonSerializerOptions _options;

    public NullableUnixMillisecondsConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new NullableUnixMillisecondsConverter());
    }

    [Fact]
    public void Read_NullValue_ReturnsNull()
    {
        // Arrange
        var json = "null";

        // Act
        var result = JsonSerializer.Deserialize<DateTimeOffset?>(json, _options);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Read_ValidTimestamp_ReturnsCorrectDateTimeOffset()
    {
        // Arrange
        var json = "1609459200000";

        // Act
        var result = JsonSerializer.Deserialize<DateTimeOffset?>(json, _options);

        // Assert
        result.Should().Be(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void Write_NullValue_ReturnsNull()
    {
        // Arrange
        DateTimeOffset? date = null;

        // Act
        var result = JsonSerializer.Serialize(date, _options);

        // Assert
        result.Should().Be("null");
    }

    [Fact]
    public void Write_ValidDateTimeOffset_ReturnsCorrectTimestamp()
    {
        // Arrange
        DateTimeOffset? date = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = JsonSerializer.Serialize(date, _options);

        // Assert
        result.Should().Be("1609459200000");
    }
}