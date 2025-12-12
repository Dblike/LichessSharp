using System.Text.Json;
using FluentAssertions;
using LichessSharp.Api.Contracts;
using LichessSharp.Serialization.Converters;
using Xunit;

namespace LichessSharp.Tests.Serialization;

public class RatingDataPointArrayConverterTests
{
    private readonly JsonSerializerOptions _options;

    public RatingDataPointArrayConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new RatingDataPointArrayConverter());
    }

    [Fact]
    public void Read_SingleDataPoint_ReturnsCorrectObject()
    {
        // Arrange
        var json = "[[2023, 5, 15, 2850]]";

        // Act
        var result = JsonSerializer.Deserialize<IReadOnlyList<RatingDataPoint>>(json, _options);

        // Assert
        result.Should().HaveCount(1);
        result![0].Year.Should().Be(2023);
        result[0].Month.Should().Be(5);
        result[0].Day.Should().Be(15);
        result[0].Rating.Should().Be(2850);
    }

    [Fact]
    public void Read_MultipleDataPoints_ReturnsCorrectObjects()
    {
        // Arrange
        var json = "[[2023, 0, 1, 2800], [2023, 5, 15, 2850], [2023, 11, 31, 2900]]";

        // Act
        var result = JsonSerializer.Deserialize<IReadOnlyList<RatingDataPoint>>(json, _options);

        // Assert
        result.Should().HaveCount(3);

        result![0].Year.Should().Be(2023);
        result[0].Month.Should().Be(0); // January (0-indexed)
        result[0].Day.Should().Be(1);
        result[0].Rating.Should().Be(2800);

        result[1].Year.Should().Be(2023);
        result[1].Month.Should().Be(5); // June
        result[1].Day.Should().Be(15);
        result[1].Rating.Should().Be(2850);

        result[2].Year.Should().Be(2023);
        result[2].Month.Should().Be(11); // December
        result[2].Day.Should().Be(31);
        result[2].Rating.Should().Be(2900);
    }

    [Fact]
    public void Read_EmptyArray_ReturnsEmptyList()
    {
        // Arrange
        var json = "[]";

        // Act
        var result = JsonSerializer.Deserialize<IReadOnlyList<RatingDataPoint>>(json, _options);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Write_SingleDataPoint_ReturnsCorrectJson()
    {
        // Arrange
        var points = new List<RatingDataPoint>
        {
            new() { Year = 2023, Month = 5, Day = 15, Rating = 2850 }
        };

        // Act
        var result = JsonSerializer.Serialize<IReadOnlyList<RatingDataPoint>>(points, _options);

        // Assert
        result.Should().Be("[[2023,5,15,2850]]");
    }

    [Fact]
    public void Write_MultipleDataPoints_ReturnsCorrectJson()
    {
        // Arrange
        var points = new List<RatingDataPoint>
        {
            new() { Year = 2023, Month = 0, Day = 1, Rating = 2800 },
            new() { Year = 2023, Month = 5, Day = 15, Rating = 2850 }
        };

        // Act
        var result = JsonSerializer.Serialize<IReadOnlyList<RatingDataPoint>>(points, _options);

        // Assert
        result.Should().Be("[[2023,0,1,2800],[2023,5,15,2850]]");
    }

    [Fact]
    public void Write_EmptyList_ReturnsEmptyArray()
    {
        // Arrange
        var points = new List<RatingDataPoint>();

        // Act
        var result = JsonSerializer.Serialize<IReadOnlyList<RatingDataPoint>>(points, _options);

        // Assert
        result.Should().Be("[]");
    }

    [Fact]
    public void RoundTrip_PreservesData()
    {
        // Arrange
        var original = new List<RatingDataPoint>
        {
            new() { Year = 2011, Month = 0, Day = 8, Rating = 1472 },
            new() { Year = 2011, Month = 0, Day = 9, Rating = 1332 },
            new() { Year = 2023, Month = 11, Day = 25, Rating = 2856 }
        };

        // Act
        var json = JsonSerializer.Serialize<IReadOnlyList<RatingDataPoint>>(original, _options);
        var deserialized = JsonSerializer.Deserialize<IReadOnlyList<RatingDataPoint>>(json, _options);

        // Assert
        deserialized.Should().BeEquivalentTo(original);
    }
}

public class RatingHistoryDeserializationTests
{
    [Fact]
    public void Deserialize_RatingHistoryWithPoints_ReturnsCorrectObject()
    {
        // Arrange - this mimics the actual Lichess API response format
        var json = """
                   {
                       "name": "Bullet",
                       "points": [[2023, 0, 1, 2800], [2023, 5, 15, 2850]]
                   }
                   """;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new RatingDataPointArrayConverter());

        // Act
        var result = JsonSerializer.Deserialize<RatingHistory>(json, options);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Bullet");
        result.Points.Should().HaveCount(2);
        result.Points[0].Year.Should().Be(2023);
        result.Points[0].Month.Should().Be(0);
        result.Points[0].Rating.Should().Be(2800);
    }

    [Fact]
    public void Deserialize_RatingHistoryArray_ReturnsCorrectObjects()
    {
        // Arrange - this mimics the actual Lichess API response for rating-history endpoint
        var json = """
                   [
                       {
                           "name": "Bullet",
                           "points": [[2023, 0, 1, 2800]]
                       },
                       {
                           "name": "Blitz",
                           "points": [[2023, 0, 1, 2750], [2023, 1, 15, 2760]]
                       },
                       {
                           "name": "Rapid",
                           "points": []
                       }
                   ]
                   """;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new RatingDataPointArrayConverter());

        // Act
        var result = JsonSerializer.Deserialize<List<RatingHistory>>(json, options);

        // Assert
        result.Should().HaveCount(3);

        result![0].Name.Should().Be("Bullet");
        result[0].Points.Should().HaveCount(1);

        result[1].Name.Should().Be("Blitz");
        result[1].Points.Should().HaveCount(2);

        result[2].Name.Should().Be("Rapid");
        result[2].Points.Should().BeEmpty();
    }
}