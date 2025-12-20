using System.Text.Json;
using System.Text.Json.Serialization;
using LichessSharp.Serialization.Converters;

namespace LichessSharp.Tests.Fixtures;

/// <summary>
/// JSON serialization options matching the library's production configuration.
/// Use these options when deserializing fixtures to ensure consistent behavior.
/// </summary>
public static class LichessJsonDefaults
{
    /// <summary>
    /// JSON options matching the library's source-generated context settings.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = CreateOptions();

    /// <summary>
    /// Creates a new instance of JSON options (for cases where mutation is needed).
    /// </summary>
    public static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        // Add the same converters used by the library
        options.Converters.Add(new UnixMillisecondsConverter());
        options.Converters.Add(new NullableUnixMillisecondsConverter());
        options.Converters.Add(new VariantObjectConverter());
        options.Converters.Add(new NullableVariantObjectConverter());
        options.Converters.Add(new FlexibleVariantConverter());

        return options;
    }

    /// <summary>
    /// JSON options with indented output for fixture files.
    /// </summary>
    public static JsonSerializerOptions IndentedOptions { get; } = CreateIndentedOptions();

    private static JsonSerializerOptions CreateIndentedOptions()
    {
        var options = CreateOptions();
        options.WriteIndented = true;
        return options;
    }
}
