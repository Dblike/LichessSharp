using System.Reflection;
using System.Text.Json.Serialization;
using LichessSharp.Models;
using LichessSharp.Models.Common;

namespace LichessSharp.Tests.Schema;

/// <summary>
/// Reflects over C# model types to extract JSON serialization metadata.
/// </summary>
public static class ModelReflector
{
    private static readonly Assembly ModelsAssembly = typeof(LightUser).Assembly;

    /// <summary>
    /// Gets all types marked with [ResponseOnly] attribute.
    /// </summary>
    public static IEnumerable<Type> GetResponseOnlyTypes()
    {
        return ModelsAssembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetCustomAttribute<ResponseOnlyAttribute>() != null);
    }

    /// <summary>
    /// Gets all types marked with [Bidirectional] attribute.
    /// </summary>
    public static IEnumerable<Type> GetBidirectionalTypes()
    {
        return ModelsAssembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetCustomAttribute<BidirectionalAttribute>() != null);
    }

    /// <summary>
    /// Gets all types marked with [RequestOnly] attribute.
    /// </summary>
    public static IEnumerable<Type> GetRequestOnlyTypes()
    {
        return ModelsAssembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetCustomAttribute<RequestOnlyAttribute>() != null);
    }

    /// <summary>
    /// Extracts JsonPropertyName mappings for a type.
    /// </summary>
    public static Dictionary<string, ModelPropertyInfo> GetJsonPropertyMappings(Type type)
    {
        var result = new Dictionary<string, ModelPropertyInfo>();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var jsonPropAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
            var jsonName = jsonPropAttr?.Name ?? ToCamelCase(prop.Name);
            var isIgnored = prop.GetCustomAttribute<JsonIgnoreAttribute>() != null;
            var hasExtensionData = prop.GetCustomAttribute<JsonExtensionDataAttribute>() != null;
            var hasConverter = prop.GetCustomAttribute<JsonConverterAttribute>() != null;
            var converterType = prop.GetCustomAttribute<JsonConverterAttribute>()?.ConverterType;

            result[jsonName] = new ModelPropertyInfo
            {
                CSharpName = prop.Name,
                JsonName = jsonName,
                PropertyType = prop.PropertyType,
                IsNullable = IsNullable(prop),
                IsRequired = IsRequired(prop),
                IsIgnored = isIgnored,
                HasExtensionData = hasExtensionData,
                HasCustomConverter = hasConverter,
                ConverterType = converterType,
                DeclaringType = prop.DeclaringType
            };
        }

        return result;
    }

    /// <summary>
    /// Checks if a type has the JsonExtensionData attribute on any property.
    /// </summary>
    public static bool HasExtensionDataProperty(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(p => p.GetCustomAttribute<JsonExtensionDataAttribute>() != null);
    }

    /// <summary>
    /// Checks if a type has a custom JsonConverter attribute.
    /// </summary>
    public static bool HasTypeConverter(Type type)
    {
        return type.GetCustomAttribute<JsonConverterAttribute>() != null;
    }

    /// <summary>
    /// Gets the JsonConverter type if present on the type.
    /// </summary>
    public static Type? GetTypeConverter(Type type)
    {
        return type.GetCustomAttribute<JsonConverterAttribute>()?.ConverterType;
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToLowerInvariant(name[0]) + name[1..];
    }

    private static bool IsNullable(PropertyInfo prop)
    {
        // Check for Nullable<T>
        if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
            return true;

        // Check nullability context for reference types
        var nullabilityContext = new NullabilityInfoContext();
        var nullabilityInfo = nullabilityContext.Create(prop);
        return nullabilityInfo.ReadState == NullabilityState.Nullable;
    }

    private static bool IsRequired(PropertyInfo prop)
    {
        // Check for 'required' modifier (C# 11+)
        return prop.GetCustomAttributes()
            .Any(a => a.GetType().Name == "RequiredMemberAttribute");
    }
}

/// <summary>
/// Information about a model property's JSON serialization configuration.
/// </summary>
public class ModelPropertyInfo
{
    public string CSharpName { get; set; } = "";
    public string JsonName { get; set; } = "";
    public Type PropertyType { get; set; } = typeof(object);
    public Type? DeclaringType { get; set; }
    public bool IsNullable { get; set; }
    public bool IsRequired { get; set; }
    public bool IsIgnored { get; set; }
    public bool HasExtensionData { get; set; }
    public bool HasCustomConverter { get; set; }
    public Type? ConverterType { get; set; }
}
