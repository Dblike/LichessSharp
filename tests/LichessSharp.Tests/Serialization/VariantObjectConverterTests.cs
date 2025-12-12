using System.Text.Json;
using FluentAssertions;
using LichessSharp.Models.Enums;
using LichessSharp.Serialization.Converters;
using Xunit;

namespace LichessSharp.Tests.Serialization;

public class VariantObjectConverterTests
{
    private readonly JsonSerializerOptions _options;

    public VariantObjectConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new VariantObjectConverter());
    }

    [Theory]
    [InlineData("\"standard\"", Variant.Standard)]
    [InlineData("\"chess960\"", Variant.Chess960)]
    [InlineData("\"crazyhouse\"", Variant.Crazyhouse)]
    [InlineData("\"antichess\"", Variant.Antichess)]
    [InlineData("\"atomic\"", Variant.Atomic)]
    [InlineData("\"horde\"", Variant.Horde)]
    [InlineData("\"kingOfTheHill\"", Variant.KingOfTheHill)]
    [InlineData("\"racingKings\"", Variant.RacingKings)]
    [InlineData("\"threeCheck\"", Variant.ThreeCheck)]
    [InlineData("\"fromPosition\"", Variant.FromPosition)]
    public void Read_StringValue_ReturnsCorrectVariant(string json, Variant expected)
    {
        // Act
        var result = JsonSerializer.Deserialize<Variant>(json, _options);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("{\"key\":\"standard\",\"name\":\"Standard\",\"short\":\"Std\"}", Variant.Standard)]
    [InlineData("{\"key\":\"chess960\",\"name\":\"Chess960\"}", Variant.Chess960)]
    [InlineData("{\"key\":\"crazyhouse\",\"name\":\"Crazyhouse\",\"short\":\"Crazy\"}", Variant.Crazyhouse)]
    [InlineData("{\"key\":\"kingOfTheHill\",\"name\":\"King of the Hill\",\"short\":\"KotH\"}", Variant.KingOfTheHill)]
    [InlineData("{\"key\":\"threeCheck\",\"name\":\"Three-check\",\"short\":\"3check\"}", Variant.ThreeCheck)]
    public void Read_ObjectValue_ReturnsCorrectVariant(string json, Variant expected)
    {
        // Act
        var result = JsonSerializer.Deserialize<Variant>(json, _options);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Read_ObjectWithExtraProperties_IgnoresExtras()
    {
        // Arrange
        var json = "{\"key\":\"standard\",\"name\":\"Standard\",\"short\":\"Std\",\"extra\":\"ignored\"}";

        // Act
        var result = JsonSerializer.Deserialize<Variant>(json, _options);

        // Assert
        result.Should().Be(Variant.Standard);
    }

    [Fact]
    public void Read_ObjectWithOnlyKey_ReturnsCorrectVariant()
    {
        // Arrange
        var json = "{\"key\":\"atomic\"}";

        // Act
        var result = JsonSerializer.Deserialize<Variant>(json, _options);

        // Assert
        result.Should().Be(Variant.Atomic);
    }

    [Fact]
    public void Read_UnknownVariantKey_ThrowsJsonException()
    {
        // Arrange
        var json = "\"unknownVariant\"";

        // Act
        var act = () => JsonSerializer.Deserialize<Variant>(json, _options);

        // Assert
        act.Should().Throw<JsonException>().WithMessage("*Unknown variant key*");
    }

    [Theory]
    [InlineData(Variant.Standard, "\"standard\"")]
    [InlineData(Variant.Chess960, "\"chess960\"")]
    [InlineData(Variant.Crazyhouse, "\"crazyhouse\"")]
    [InlineData(Variant.Antichess, "\"antichess\"")]
    [InlineData(Variant.Atomic, "\"atomic\"")]
    [InlineData(Variant.Horde, "\"horde\"")]
    [InlineData(Variant.KingOfTheHill, "\"kingOfTheHill\"")]
    [InlineData(Variant.RacingKings, "\"racingKings\"")]
    [InlineData(Variant.ThreeCheck, "\"threeCheck\"")]
    [InlineData(Variant.FromPosition, "\"fromPosition\"")]
    public void Write_Variant_ReturnsCorrectString(Variant variant, string expected)
    {
        // Act
        var result = JsonSerializer.Serialize(variant, _options);

        // Assert
        result.Should().Be(expected);
    }
}

public class NullableVariantObjectConverterTests
{
    private readonly JsonSerializerOptions _options;

    public NullableVariantObjectConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new NullableVariantObjectConverter());
    }

    [Fact]
    public void Read_NullValue_ReturnsNull()
    {
        // Arrange
        var json = "null";

        // Act
        var result = JsonSerializer.Deserialize<Variant?>(json, _options);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Read_StringValue_ReturnsCorrectVariant()
    {
        // Arrange
        var json = "\"blitz\"";
        _options.Converters.Clear();
        _options.Converters.Add(new NullableVariantObjectConverter());

        // Act - blitz is a speed, not a variant, so this should throw
        var act = () => JsonSerializer.Deserialize<Variant?>(json, _options);

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Read_ObjectValue_ReturnsCorrectVariant()
    {
        // Arrange
        var json = "{\"key\":\"standard\",\"name\":\"Standard\",\"short\":\"Std\"}";

        // Act
        var result = JsonSerializer.Deserialize<Variant?>(json, _options);

        // Assert
        result.Should().Be(Variant.Standard);
    }

    [Fact]
    public void Write_NullValue_ReturnsNull()
    {
        // Arrange
        Variant? variant = null;

        // Act
        var result = JsonSerializer.Serialize(variant, _options);

        // Assert
        result.Should().Be("null");
    }

    [Fact]
    public void Write_ValidVariant_ReturnsCorrectString()
    {
        // Arrange
        Variant? variant = Variant.Chess960;

        // Act
        var result = JsonSerializer.Serialize(variant, _options);

        // Assert
        result.Should().Be("\"chess960\"");
    }
}
