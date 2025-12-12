using System.Text.Json;
using System.Text.Json.Serialization;
using LichessSharp.Models.Enums;
using LichessSharp.Models.Puzzles;

namespace LichessSharp.Serialization.Converters;

/// <summary>
///     Helper class for parsing perf field values into Speed or Variant.
/// </summary>
public static class PerfFieldParser
{
    private static readonly Dictionary<string, Speed> SpeedMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ultraBullet"] = Speed.UltraBullet,
        ["bullet"] = Speed.Bullet,
        ["blitz"] = Speed.Blitz,
        ["rapid"] = Speed.Rapid,
        ["classical"] = Speed.Classical,
        ["correspondence"] = Speed.Correspondence
    };

    private static readonly Dictionary<string, Variant> VariantMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["chess960"] = Variant.Chess960,
        ["crazyhouse"] = Variant.Crazyhouse,
        ["antichess"] = Variant.Antichess,
        ["atomic"] = Variant.Atomic,
        ["horde"] = Variant.Horde,
        ["kingOfTheHill"] = Variant.KingOfTheHill,
        ["racingKings"] = Variant.RacingKings,
        ["threeCheck"] = Variant.ThreeCheck
    };

    private static readonly Dictionary<Speed, string> SpeedToString = SpeedMap.ToDictionary(x => x.Value, x => x.Key);
    private static readonly Dictionary<Variant, string> VariantToString = VariantMap.ToDictionary(x => x.Value, x => x.Key);

    /// <summary>
    ///     Tries to parse a perf value as a Speed.
    /// </summary>
    public static Speed? TryParseSpeed(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        return SpeedMap.TryGetValue(value, out var speed) ? speed : null;
    }

    /// <summary>
    ///     Tries to parse a perf value as a Variant.
    /// </summary>
    public static Variant? TryParseVariant(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        return VariantMap.TryGetValue(value, out var variant) ? variant : null;
    }

    /// <summary>
    ///     Converts a Speed to its JSON string representation.
    /// </summary>
    public static string? SpeedToJson(Speed? speed)
    {
        if (!speed.HasValue)
            return null;
        return SpeedToString.TryGetValue(speed.Value, out var str) ? str : null;
    }

    /// <summary>
    ///     Converts a Variant to its JSON string representation.
    /// </summary>
    public static string? VariantToJson(Variant? variant)
    {
        if (!variant.HasValue)
            return null;
        return VariantToString.TryGetValue(variant.Value, out var str) ? str : null;
    }
}

/// <summary>
///     JSON converter for PuzzleGame that parses the perf field into Speed and Variant properties.
/// </summary>
public sealed class PuzzleGameConverter : JsonConverter<PuzzleGame>
{
    /// <inheritdoc />
    public override PuzzleGame Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object");

        string? id = null;
        string? pgn = null;
        string? perfValue = null;
        PuzzlePlayer[]? players = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name");

            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case "id":
                    id = reader.GetString();
                    break;
                case "pgn":
                    pgn = reader.GetString();
                    break;
                case "perf":
                    if (reader.TokenType == JsonTokenType.String)
                        perfValue = reader.GetString();
                    else if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        // Handle object format with "key" property
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                        {
                            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "key")
                            {
                                reader.Read();
                                perfValue = reader.GetString();
                            }
                            else if (reader.TokenType == JsonTokenType.PropertyName)
                            {
                                reader.Read();
                                reader.Skip();
                            }
                        }
                    }
                    else
                        reader.Skip();
                    break;
                case "players":
                    players = JsonSerializer.Deserialize<PuzzlePlayer[]>(ref reader, options);
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        return new PuzzleGame
        {
            Id = id ?? throw new JsonException("PuzzleGame.Id is required"),
            Pgn = pgn,
            Speed = PerfFieldParser.TryParseSpeed(perfValue),
            Variant = PerfFieldParser.TryParseVariant(perfValue),
            Players = players
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, PuzzleGame value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("id", value.Id);

        if (value.Pgn != null)
            writer.WriteString("pgn", value.Pgn);

        // Write perf from either Speed or Variant
        var perfValue = PerfFieldParser.SpeedToJson(value.Speed) ?? PerfFieldParser.VariantToJson(value.Variant);
        if (perfValue != null)
            writer.WriteString("perf", perfValue);

        if (value.Players != null)
        {
            writer.WritePropertyName("players");
            JsonSerializer.Serialize(writer, value.Players, options);
        }

        writer.WriteEndObject();
    }
}
