using System.Text.Json;
using System.Text.Json.Serialization;
using LichessSharp.Api.Contracts;

namespace LichessSharp.Serialization.Converters;

/// <summary>
///     Converts polymorphic <see cref="BotGameEvent" /> types based on the "type" discriminator field.
///     Handles: gameFull, gameState, chatLine, opponentGone.
/// </summary>
public sealed class BotGameEventConverter : JsonConverter<BotGameEvent>
{
    /// <inheritdoc />
    public override BotGameEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"Expected StartObject, got {reader.TokenType}");

        // Parse the entire object into a document to allow type-based deserialization
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        // Extract the discriminator value
        string? typeValue = null;
        if (root.TryGetProperty("type", out var typeProp))
            typeValue = typeProp.GetString();

        // Deserialize to the appropriate derived type
        return typeValue switch
        {
            "gameFull" => DeserializeDerived<BotGameFullEvent>(root, options),
            "gameState" => DeserializeDerived<BotGameStateEvent>(root, options),
            "chatLine" => DeserializeDerived<BotChatLineEvent>(root, options),
            "opponentGone" => DeserializeDerived<BotOpponentGoneEvent>(root, options),
            _ => new BotGameEvent { Type = typeValue ?? "unknown" }
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, BotGameEvent value, JsonSerializerOptions options)
    {
        // Serialize as the actual runtime type
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }

    /// <summary>
    ///     Deserializes a derived type from a JsonElement.
    ///     Uses options without this converter to avoid infinite recursion.
    /// </summary>
    private static T? DeserializeDerived<T>(JsonElement element, JsonSerializerOptions options) where T : BotGameEvent
    {
        // Create options without this converter to prevent recursion
        var innerOptions = CreateOptionsWithoutThisConverter(options);
        return JsonSerializer.Deserialize<T>(element.GetRawText(), innerOptions);
    }

    private static JsonSerializerOptions CreateOptionsWithoutThisConverter(JsonSerializerOptions options)
    {
        var innerOptions = new JsonSerializerOptions(options);

        for (var i = innerOptions.Converters.Count - 1; i >= 0; i--)
            if (innerOptions.Converters[i] is BotGameEventConverter)
            {
                innerOptions.Converters.RemoveAt(i);
                break;
            }

        return innerOptions;
    }
}
