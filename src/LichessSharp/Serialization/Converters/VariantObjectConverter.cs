using System.Text.Json;
using System.Text.Json.Serialization;
using LichessSharp.Models.Enums;

namespace LichessSharp.Serialization.Converters;

/// <summary>
///     Converts a Lichess variant object (with key, name, short properties) to a <see cref="Variant" /> enum.
///     The Lichess API returns variant as an object like: { "key": "standard", "name": "Standard", "short": "Std" }
///     This converter extracts the "key" and parses it into the Variant enum.
/// </summary>
public sealed class VariantObjectConverter : JsonConverter<Variant>
{
    /// <inheritdoc />
    public override Variant Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // Direct string value like "standard"
            var key = reader.GetString();
            return ParseVariantKey(key);
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            // Object with key, name, short properties
            string? key = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException($"Expected property name, got {reader.TokenType}");

                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName?.Equals("key", StringComparison.OrdinalIgnoreCase) == true)
                {
                    key = reader.GetString();
                }
                else
                {
                    // Skip name, short, and any other properties
                    reader.Skip();
                }
            }

            return ParseVariantKey(key);
        }

        throw new JsonException($"Unexpected token type for variant: {reader.TokenType}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Variant value, JsonSerializerOptions options)
    {
        // Write as the key string directly
        var key = value switch
        {
            Variant.Standard => "standard",
            Variant.Chess960 => "chess960",
            Variant.Crazyhouse => "crazyhouse",
            Variant.Antichess => "antichess",
            Variant.Atomic => "atomic",
            Variant.Horde => "horde",
            Variant.KingOfTheHill => "kingOfTheHill",
            Variant.RacingKings => "racingKings",
            Variant.ThreeCheck => "threeCheck",
            Variant.FromPosition => "fromPosition",
            _ => value.ToString().ToLowerInvariant()
        };
        writer.WriteStringValue(key);
    }

    private static Variant ParseVariantKey(string? key) =>
        key?.ToLowerInvariant() switch
        {
            "standard" => Variant.Standard,
            "chess960" => Variant.Chess960,
            "crazyhouse" => Variant.Crazyhouse,
            "antichess" => Variant.Antichess,
            "atomic" => Variant.Atomic,
            "horde" => Variant.Horde,
            "kingofthehill" => Variant.KingOfTheHill,
            "racingkings" => Variant.RacingKings,
            "threecheck" => Variant.ThreeCheck,
            "fromposition" => Variant.FromPosition,
            _ => throw new JsonException($"Unknown variant key: {key}")
        };
}

/// <summary>
///     Nullable version of <see cref="VariantObjectConverter" />.
/// </summary>
public sealed class NullableVariantObjectConverter : JsonConverter<Variant?>
{
    private static readonly VariantObjectConverter InnerConverter = new();

    /// <inheritdoc />
    public override Variant? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        return InnerConverter.Read(ref reader, typeof(Variant), options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Variant? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        InnerConverter.Write(writer, value.Value, options);
    }
}
