using System.Text.Json;
using FluentAssertions;
using LichessSharp.Api.Contracts;
using LichessSharp.Serialization.Converters;
using Xunit;

namespace LichessSharp.Tests.Serialization;

public class FlexibleVariantConverterTests
{
    private readonly JsonSerializerOptions _options;

    public FlexibleVariantConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new FlexibleVariantConverter());
    }

    [Fact]
    public void Read_NullValue_ReturnsNull()
    {
        // Arrange
        var json = "null";

        // Act
        var result = JsonSerializer.Deserialize<ArenaVariant?>(json, _options);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("\"standard\"", "standard")]
    [InlineData("\"chess960\"", "chess960")]
    [InlineData("\"crazyhouse\"", "crazyhouse")]
    [InlineData("\"blitz\"", "blitz")]
    public void Read_StringValue_ReturnsArenaVariantWithKeyAndName(string json, string expected)
    {
        // Act
        var result = JsonSerializer.Deserialize<ArenaVariant?>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Key.Should().Be(expected);
        result.Name.Should().Be(expected);
        result.Short.Should().BeNull();
    }

    [Fact]
    public void Read_ObjectWithAllProperties_ReturnsCompleteArenaVariant()
    {
        // Arrange
        var json = """{"key":"standard","name":"Standard","short":"Std"}""";

        // Act
        var result = JsonSerializer.Deserialize<ArenaVariant?>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Key.Should().Be("standard");
        result.Name.Should().Be("Standard");
        result.Short.Should().Be("Std");
    }

    [Fact]
    public void Read_ObjectWithOnlyKey_ReturnsArenaVariantWithKey()
    {
        // Arrange
        var json = """{"key":"chess960"}""";

        // Act
        var result = JsonSerializer.Deserialize<ArenaVariant?>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Key.Should().Be("chess960");
        result.Name.Should().BeNull();
        result.Short.Should().BeNull();
    }

    [Fact]
    public void Read_ObjectWithExtraProperties_IgnoresExtras()
    {
        // Arrange
        var json = """{"key":"atomic","name":"Atomic","short":"Atom","extra":"ignored","another":123}""";

        // Act
        var result = JsonSerializer.Deserialize<ArenaVariant?>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Key.Should().Be("atomic");
        result.Name.Should().Be("Atomic");
        result.Short.Should().Be("Atom");
    }

    [Fact]
    public void Read_ObjectWithMixedCase_ParsesCorrectly()
    {
        // Arrange - property names are case-insensitive
        var json = """{"KEY":"horde","NAME":"Horde","SHORT":"Horde"}""";

        // Act
        var result = JsonSerializer.Deserialize<ArenaVariant?>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Key.Should().Be("horde");
        result.Name.Should().Be("Horde");
        result.Short.Should().Be("Horde");
    }

    [Fact]
    public void Read_EmptyObject_ReturnsArenaVariantWithEmptyKey()
    {
        // Arrange
        var json = "{}";

        // Act
        var result = JsonSerializer.Deserialize<ArenaVariant?>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Key.Should().Be("");
        result.Name.Should().BeNull();
        result.Short.Should().BeNull();
    }

    [Fact]
    public void Read_InvalidTokenType_ThrowsJsonException()
    {
        // Arrange
        var json = "123";

        // Act
        var act = () => JsonSerializer.Deserialize<ArenaVariant?>(json, _options);

        // Assert
        act.Should().Throw<JsonException>().WithMessage("*Unexpected token type*");
    }

    [Fact]
    public void Write_NullValue_WritesNull()
    {
        // Arrange
        ArenaVariant? variant = null;

        // Act
        var result = JsonSerializer.Serialize(variant, _options);

        // Assert
        result.Should().Be("null");
    }

    [Fact]
    public void Write_CompleteArenaVariant_WritesObject()
    {
        // Arrange
        var variant = new ArenaVariant
        {
            Key = "standard",
            Name = "Standard",
            Short = "Std"
        };

        // Act
        var result = JsonSerializer.Serialize(variant, _options);

        // Assert
        result.Should().Contain("\"key\":\"standard\"");
        result.Should().Contain("\"name\":\"Standard\"");
        result.Should().Contain("\"short\":\"Std\"");
    }

    [Fact]
    public void Write_ArenaVariantWithOnlyKey_WritesOnlyKey()
    {
        // Arrange
        var variant = new ArenaVariant { Key = "chess960" };

        // Act
        var result = JsonSerializer.Serialize(variant, _options);

        // Assert
        result.Should().Contain("\"key\":\"chess960\"");
        result.Should().NotContain("\"name\"");
        result.Should().NotContain("\"short\"");
    }

    [Fact]
    public void Write_ArenaVariantWithKeyAndName_WritesKeyAndName()
    {
        // Arrange
        var variant = new ArenaVariant
        {
            Key = "atomic",
            Name = "Atomic"
        };

        // Act
        var result = JsonSerializer.Serialize(variant, _options);

        // Assert
        result.Should().Contain("\"key\":\"atomic\"");
        result.Should().Contain("\"name\":\"Atomic\"");
        result.Should().NotContain("\"short\"");
    }

    [Fact]
    public void RoundTrip_StringFormat_PreservesData()
    {
        // Arrange
        var json = "\"kingOfTheHill\"";

        // Act
        var deserialized = JsonSerializer.Deserialize<ArenaVariant?>(json, _options);
        var reserialized = JsonSerializer.Serialize(deserialized, _options);
        var final = JsonSerializer.Deserialize<ArenaVariant?>(reserialized, _options);

        // Assert
        final.Should().NotBeNull();
        final!.Key.Should().Be("kingOfTheHill");
    }

    [Fact]
    public void RoundTrip_ObjectFormat_PreservesData()
    {
        // Arrange
        var json = """{"key":"threeCheck","name":"Three-check","short":"3check"}""";

        // Act
        var deserialized = JsonSerializer.Deserialize<ArenaVariant?>(json, _options);
        var reserialized = JsonSerializer.Serialize(deserialized, _options);
        var final = JsonSerializer.Deserialize<ArenaVariant?>(reserialized, _options);

        // Assert
        final.Should().NotBeNull();
        final!.Key.Should().Be("threeCheck");
        final.Name.Should().Be("Three-check");
        final.Short.Should().Be("3check");
    }
}
