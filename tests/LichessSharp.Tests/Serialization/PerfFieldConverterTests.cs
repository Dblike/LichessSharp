using System.Text.Json;
using FluentAssertions;
using LichessSharp.Models.Enums;
using LichessSharp.Models.Puzzles;
using LichessSharp.Serialization.Converters;
using Xunit;

namespace LichessSharp.Tests.Serialization;

public class PerfFieldParserTests
{
    [Theory]
    [InlineData("ultraBullet", Speed.UltraBullet)]
    [InlineData("bullet", Speed.Bullet)]
    [InlineData("blitz", Speed.Blitz)]
    [InlineData("rapid", Speed.Rapid)]
    [InlineData("classical", Speed.Classical)]
    [InlineData("correspondence", Speed.Correspondence)]
    public void TryParseSpeed_ValidSpeed_ReturnsSpeed(string value, Speed expected)
    {
        // Act
        var result = PerfFieldParser.TryParseSpeed(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("BLITZ", Speed.Blitz)]
    [InlineData("Bullet", Speed.Bullet)]
    [InlineData("RAPID", Speed.Rapid)]
    public void TryParseSpeed_CaseInsensitive_ReturnsSpeed(string value, Speed expected)
    {
        // Act
        var result = PerfFieldParser.TryParseSpeed(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("chess960")]
    [InlineData("crazyhouse")]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParseSpeed_InvalidOrVariant_ReturnsNull(string? value)
    {
        // Act
        var result = PerfFieldParser.TryParseSpeed(value);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("chess960", Variant.Chess960)]
    [InlineData("crazyhouse", Variant.Crazyhouse)]
    [InlineData("antichess", Variant.Antichess)]
    [InlineData("atomic", Variant.Atomic)]
    [InlineData("horde", Variant.Horde)]
    [InlineData("kingOfTheHill", Variant.KingOfTheHill)]
    [InlineData("racingKings", Variant.RacingKings)]
    [InlineData("threeCheck", Variant.ThreeCheck)]
    public void TryParseVariant_ValidVariant_ReturnsVariant(string value, Variant expected)
    {
        // Act
        var result = PerfFieldParser.TryParseVariant(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("CHESS960", Variant.Chess960)]
    [InlineData("KingOfTheHill", Variant.KingOfTheHill)]
    [InlineData("THREECHECK", Variant.ThreeCheck)]
    public void TryParseVariant_CaseInsensitive_ReturnsVariant(string value, Variant expected)
    {
        // Act
        var result = PerfFieldParser.TryParseVariant(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("blitz")]
    [InlineData("bullet")]
    [InlineData("standard")]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParseVariant_InvalidOrSpeed_ReturnsNull(string? value)
    {
        // Act
        var result = PerfFieldParser.TryParseVariant(value);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(Speed.UltraBullet, "ultraBullet")]
    [InlineData(Speed.Bullet, "bullet")]
    [InlineData(Speed.Blitz, "blitz")]
    [InlineData(Speed.Rapid, "rapid")]
    [InlineData(Speed.Classical, "classical")]
    [InlineData(Speed.Correspondence, "correspondence")]
    public void SpeedToJson_ValidSpeed_ReturnsString(Speed speed, string expected)
    {
        // Act
        var result = PerfFieldParser.SpeedToJson(speed);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void SpeedToJson_NullSpeed_ReturnsNull()
    {
        // Act
        var result = PerfFieldParser.SpeedToJson(null);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(Variant.Chess960, "chess960")]
    [InlineData(Variant.Crazyhouse, "crazyhouse")]
    [InlineData(Variant.Antichess, "antichess")]
    [InlineData(Variant.Atomic, "atomic")]
    [InlineData(Variant.Horde, "horde")]
    [InlineData(Variant.KingOfTheHill, "kingOfTheHill")]
    [InlineData(Variant.RacingKings, "racingKings")]
    [InlineData(Variant.ThreeCheck, "threeCheck")]
    public void VariantToJson_ValidVariant_ReturnsString(Variant variant, string expected)
    {
        // Act
        var result = PerfFieldParser.VariantToJson(variant);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void VariantToJson_NullVariant_ReturnsNull()
    {
        // Act
        var result = PerfFieldParser.VariantToJson(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void VariantToJson_StandardVariant_ReturnsNull()
    {
        // Standard is not in the variant map (it's a speed category)
        // Act
        var result = PerfFieldParser.VariantToJson(Variant.Standard);

        // Assert
        result.Should().BeNull();
    }
}

public class PuzzleGameConverterTests
{
    private readonly JsonSerializerOptions _options;

    public PuzzleGameConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new PuzzleGameConverter());
    }

    [Fact]
    public void Read_WithStringPerfSpeed_ParsesCorrectly()
    {
        // Arrange
        var json = """
            {
                "id": "abc123",
                "pgn": "1. e4 e5",
                "perf": "blitz"
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<PuzzleGame>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("abc123");
        result.Pgn.Should().Be("1. e4 e5");
        result.Speed.Should().Be(Speed.Blitz);
        result.Variant.Should().BeNull();
    }

    [Fact]
    public void Read_WithStringPerfVariant_ParsesCorrectly()
    {
        // Arrange
        var json = """
            {
                "id": "xyz789",
                "pgn": "1. e4 e5",
                "perf": "chess960"
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<PuzzleGame>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("xyz789");
        result.Speed.Should().BeNull();
        result.Variant.Should().Be(Variant.Chess960);
    }

    [Fact]
    public void Read_WithObjectPerf_ParsesCorrectly()
    {
        // Arrange
        var json = """
            {
                "id": "def456",
                "pgn": "1. d4 d5",
                "perf": {"key": "rapid", "name": "Rapid"}
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<PuzzleGame>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("def456");
        result.Speed.Should().Be(Speed.Rapid);
        result.Variant.Should().BeNull();
    }

    [Fact]
    public void Read_WithPlayers_ParsesCorrectly()
    {
        // Arrange
        var json = """
            {
                "id": "game123",
                "perf": "bullet",
                "players": [
                    {"userId": "player1", "name": "Player One", "color": "white"},
                    {"userId": "player2", "name": "Player Two", "color": "black"}
                ]
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<PuzzleGame>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("game123");
        result.Players.Should().HaveCount(2);
        result.Players![0].UserId.Should().Be("player1");
        result.Players[1].Color.Should().Be("black");
    }

    [Fact]
    public void Read_MinimalObject_ParsesCorrectly()
    {
        // Arrange - only id is required
        var json = """{"id": "minimal"}""";

        // Act
        var result = JsonSerializer.Deserialize<PuzzleGame>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("minimal");
        result.Pgn.Should().BeNull();
        result.Speed.Should().BeNull();
        result.Variant.Should().BeNull();
        result.Players.Should().BeNull();
    }

    [Fact]
    public void Read_MissingId_ThrowsJsonException()
    {
        // Arrange
        var json = """{"pgn": "1. e4 e5", "perf": "blitz"}""";

        // Act
        var act = () => JsonSerializer.Deserialize<PuzzleGame>(json, _options);

        // Assert
        act.Should().Throw<JsonException>().WithMessage("*Id is required*");
    }

    [Fact]
    public void Write_WithSpeed_WritesCorrectly()
    {
        // Arrange
        var puzzleGame = new PuzzleGame
        {
            Id = "test123",
            Pgn = "1. e4 e5",
            Speed = Speed.Blitz
        };

        // Act
        var result = JsonSerializer.Serialize(puzzleGame, _options);

        // Assert
        result.Should().Contain("\"id\":\"test123\"");
        result.Should().Contain("\"pgn\":\"1. e4 e5\"");
        result.Should().Contain("\"perf\":\"blitz\"");
    }

    [Fact]
    public void Write_WithVariant_WritesCorrectly()
    {
        // Arrange
        var puzzleGame = new PuzzleGame
        {
            Id = "variant123",
            Variant = Variant.Crazyhouse
        };

        // Act
        var result = JsonSerializer.Serialize(puzzleGame, _options);

        // Assert
        result.Should().Contain("\"id\":\"variant123\"");
        result.Should().Contain("\"perf\":\"crazyhouse\"");
    }

    [Fact]
    public void Write_WithPlayers_WritesCorrectly()
    {
        // Arrange
        var puzzleGame = new PuzzleGame
        {
            Id = "players123",
            Players = new[]
            {
                new PuzzlePlayer { UserId = "user1", Name = "User One", Color = "white" }
            }
        };

        // Act
        var result = JsonSerializer.Serialize(puzzleGame, _options);

        // Assert
        result.Should().Contain("\"players\":");
        result.Should().Contain("\"userId\":\"user1\"");
    }

    [Fact]
    public void Write_MinimalObject_WritesCorrectly()
    {
        // Arrange
        var puzzleGame = new PuzzleGame { Id = "minimal" };

        // Act
        var result = JsonSerializer.Serialize(puzzleGame, _options);

        // Assert
        result.Should().Be("{\"id\":\"minimal\"}");
    }
}
