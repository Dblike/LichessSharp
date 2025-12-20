using LichessSharp.Models;
using LichessSharp.Models.Account;
using LichessSharp.Models.Common;
using LichessSharp.Models.Games;
using LichessSharp.Models.Puzzles;
using LichessSharp.Models.Users;

namespace LichessSharp.Tests.Schema;

/// <summary>
/// Maps C# model type names to their corresponding OpenAPI schema names.
/// Some types have different naming conventions between C# and the OpenAPI spec.
/// </summary>
public static class SchemaModelMapping
{
    /// <summary>
    /// Maps C# types to their OpenAPI schema names.
    /// Only add entries where the names differ; types with matching names are auto-resolved.
    /// </summary>
    private static readonly Dictionary<Type, string> TypeToSchemaName = new()
    {
        // Users - most map directly
        // User -> User (auto)
        // UserExtended -> UserExtended (auto)
        // LightUser -> LightUser (auto)

        // Games
        // Game doesn't have a direct schema - it's part of GameJson
        // GameJson -> GameJson (auto)

        // Add explicit mappings where C# and OpenAPI names differ
    };

    /// <summary>
    /// Types that should be excluded from schema validation.
    /// These use JsonExtensionData or have no direct OpenAPI schema equivalent.
    /// </summary>
    private static readonly HashSet<Type> ExcludedTypes = new()
    {
        // Types with JsonExtensionData (dynamic properties)
        typeof(ActivityGames),

        // Nested/internal types that don't have top-level schemas
    };

    /// <summary>
    /// Types that use oneOf/anyOf polymorphism in the schema.
    /// These need special handling during validation.
    /// </summary>
    private static readonly HashSet<Type> PolymorphicTypes = new()
    {
        // Add types that correspond to oneOf/anyOf schemas
    };

    /// <summary>
    /// Types that have custom JsonConverter at the type level.
    /// These may not match schema property-by-property.
    /// </summary>
    private static readonly HashSet<Type> CustomConverterTypes = new()
    {
        // Types with [JsonConverter] attribute on the class
    };

    /// <summary>
    /// Gets the OpenAPI schema name for a C# type.
    /// Returns the type's simple name if no explicit mapping exists.
    /// </summary>
    public static string GetSchemaName(Type type)
    {
        if (TypeToSchemaName.TryGetValue(type, out var name))
            return name;

        return type.Name;
    }

    /// <summary>
    /// Checks if a type should be excluded from schema validation.
    /// </summary>
    public static bool ShouldExclude(Type type)
    {
        // Check explicit exclusions
        if (ExcludedTypes.Contains(type))
            return true;

        // Check for JsonExtensionData
        if (ModelReflector.HasExtensionDataProperty(type))
            return true;

        // Check for type-level custom converter
        if (ModelReflector.HasTypeConverter(type))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if a type uses polymorphic schema (oneOf/anyOf).
    /// </summary>
    public static bool IsPolymorphic(Type type)
    {
        return PolymorphicTypes.Contains(type);
    }

    /// <summary>
    /// Checks if a type has a custom converter that transforms the JSON structure.
    /// </summary>
    public static bool HasCustomConverter(Type type)
    {
        return CustomConverterTypes.Contains(type) || ModelReflector.HasTypeConverter(type);
    }

    /// <summary>
    /// Adds an explicit type-to-schema mapping.
    /// Used for types where the C# name doesn't match the OpenAPI schema name.
    /// </summary>
    public static void AddMapping(Type type, string schemaName)
    {
        TypeToSchemaName[type] = schemaName;
    }

    /// <summary>
    /// Adds a type to the exclusion list.
    /// </summary>
    public static void AddExclusion(Type type)
    {
        ExcludedTypes.Add(type);
    }
}
