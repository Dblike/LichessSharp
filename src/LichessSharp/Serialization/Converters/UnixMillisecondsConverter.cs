using System.Text.Json;
using System.Text.Json.Serialization;

namespace LichessSharp.Serialization.Converters;

/// <summary>
///     Converts Unix timestamps in milliseconds to/from DateTimeOffset.
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

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            // Try parsing as Unix timestamp in string form
            if (long.TryParse(value, out var milliseconds))
                return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
            // Try parsing as ISO date string
            if (DateTimeOffset.TryParse(value, out var result)) return result;
            throw new JsonException($"Unable to parse timestamp string: {value}");
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
///     Converts Unix timestamps in milliseconds to/from nullable DateTimeOffset.
/// </summary>
public sealed class NullableUnixMillisecondsConverter : JsonConverter<DateTimeOffset?>
{
    /// <inheritdoc />
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        if (reader.TokenType == JsonTokenType.Number)
        {
            var milliseconds = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value)) return null;
            // Try parsing as Unix timestamp in string form
            if (long.TryParse(value, out var milliseconds))
                return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
            // Try parsing as ISO date string
            if (DateTimeOffset.TryParse(value, out var result)) return result;
            throw new JsonException($"Unable to parse timestamp string: {value}");
        }

        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value.ToUnixTimeMilliseconds());
        else
            writer.WriteNullValue();
    }
}

/// <summary>
///     Converts timestamps that can be either Unix milliseconds (number) or ISO date strings to/from DateTimeOffset.
///     The Lichess API returns different formats depending on the endpoint.
/// </summary>
public sealed class FlexibleTimestampConverter : JsonConverter<DateTimeOffset>
{
    /// <inheritdoc />
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var milliseconds = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var dateString = reader.GetString();
            if (DateTimeOffset.TryParse(dateString, out var result)) return result;
            throw new JsonException($"Unable to parse date string: {dateString}");
        }

        throw new JsonException($"Unexpected token type for timestamp: {reader.TokenType}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        // Write as Unix milliseconds for consistency
        writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
    }
}

/// <summary>
///     Converts timestamps that can be either Unix milliseconds (number) or ISO date strings to/from nullable
///     DateTimeOffset.
/// </summary>
public sealed class NullableFlexibleTimestampConverter : JsonConverter<DateTimeOffset?>
{
    /// <inheritdoc />
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        if (reader.TokenType == JsonTokenType.Number)
        {
            var milliseconds = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var dateString = reader.GetString();
            if (string.IsNullOrEmpty(dateString)) return null;
            if (DateTimeOffset.TryParse(dateString, out var result)) return result;
            throw new JsonException($"Unable to parse date string: {dateString}");
        }

        throw new JsonException($"Unexpected token type for timestamp: {reader.TokenType}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value.ToUnixTimeMilliseconds());
        else
            writer.WriteNullValue();
    }
}