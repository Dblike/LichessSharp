using System.Text.Json;
using System.Text.Json.Serialization;

namespace LichessSharp.Serialization.Converters;

/// <summary>
/// Converts Unix timestamps in milliseconds to/from DateTimeOffset.
/// </summary>
public sealed class UnixMillisecondsConverter : JsonConverter<DateTimeOffset>
{
    /// <inheritdoc />
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var milliseconds = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }

        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
    }
}

/// <summary>
/// Converts Unix timestamps in milliseconds to/from nullable DateTimeOffset.
/// </summary>
public sealed class NullableUnixMillisecondsConverter : JsonConverter<DateTimeOffset?>
{
    /// <inheritdoc />
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            var milliseconds = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }

        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value.ToUnixTimeMilliseconds());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
