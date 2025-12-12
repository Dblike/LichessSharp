using System.Text.Json;
using System.Text.Json.Serialization;
using LichessSharp.Api.Contracts;

namespace LichessSharp.Serialization.Converters;

/// <summary>
///     Converts variant fields that can be either a string (e.g., "standard") or an object
///     with key/name/short properties to an <see cref="ArenaVariant" />.
///     The Lichess API returns different formats depending on the endpoint.
/// </summary>
public sealed class FlexibleVariantConverter : JsonConverter<ArenaVariant?>
{
    /// <inheritdoc />
    public override ArenaVariant? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            // Variant is returned as a simple string like "standard"
            var variantKey = reader.GetString();
            return new ArenaVariant
            {
                Key = variantKey ?? "",
                Name = variantKey
            };
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            // Variant is returned as an object with key, name, and optional short properties
            string? key = null;
            string? name = null;
            string? shortName = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException($"Expected property name, got {reader.TokenType}");

                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName?.ToLowerInvariant())
                {
                    case "key":
                        key = reader.GetString();
                        break;
                    case "name":
                        name = reader.GetString();
                        break;
                    case "short":
                        shortName = reader.GetString();
                        break;
                    default:
                        // Skip unknown properties
                        reader.Skip();
                        break;
                }
            }

            return new ArenaVariant
            {
                Key = key ?? "",
                Name = name,
                Short = shortName
            };
        }

        throw new JsonException($"Unexpected token type for variant: {reader.TokenType}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ArenaVariant? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Write as an object for consistency
        writer.WriteStartObject();
        writer.WriteString("key", value.Key);
        if (value.Name != null) writer.WriteString("name", value.Name);
        if (value.Short != null) writer.WriteString("short", value.Short);
        writer.WriteEndObject();
    }
}