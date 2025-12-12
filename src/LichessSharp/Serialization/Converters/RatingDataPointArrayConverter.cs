using System.Text.Json;
using System.Text.Json.Serialization;
using LichessSharp.Api.Contracts;

namespace LichessSharp.Serialization.Converters;

/// <summary>
///     Converts rating data points from the Lichess array format [year, month, day, rating]
///     to/from <see cref="RatingDataPoint" /> objects.
/// </summary>
public sealed class RatingDataPointArrayConverter : JsonConverter<IReadOnlyList<RatingDataPoint>>
{
    /// <inheritdoc />
    public override IReadOnlyList<RatingDataPoint> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected array, got {reader.TokenType}");

        var points = new List<RatingDataPoint>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;

            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException($"Expected nested array for data point, got {reader.TokenType}");

            // Read [year, month, day, rating]
            reader.Read();
            var year = reader.GetInt32();

            reader.Read();
            var month = reader.GetInt32();

            reader.Read();
            var day = reader.GetInt32();

            reader.Read();
            var rating = reader.GetInt32();

            // Consume the end of the inner array
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException($"Expected end of data point array, got {reader.TokenType}");

            points.Add(new RatingDataPoint
            {
                Year = year,
                Month = month,
                Day = day,
                Rating = rating
            });
        }

        return points;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IReadOnlyList<RatingDataPoint> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var point in value)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(point.Year);
            writer.WriteNumberValue(point.Month);
            writer.WriteNumberValue(point.Day);
            writer.WriteNumberValue(point.Rating);
            writer.WriteEndArray();
        }

        writer.WriteEndArray();
    }
}