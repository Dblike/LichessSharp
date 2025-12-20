using System.Text.Json;

namespace LichessSharp.Tests.Schema;

/// <summary>
/// Reads and parses the OpenAPI specification to extract schema definitions.
/// </summary>
public sealed class OpenApiSchemaReader : IDisposable
{
    private readonly JsonDocument _document;
    private readonly Dictionary<string, OpenApiSchema> _schemaCache = new();

    public OpenApiSchemaReader(string openApiJsonPath)
    {
        var json = File.ReadAllText(openApiJsonPath);
        _document = JsonDocument.Parse(json);
    }

    /// <summary>
    /// Gets a schema by name from components/schemas.
    /// </summary>
    public OpenApiSchema? GetSchema(string schemaName)
    {
        if (_schemaCache.TryGetValue(schemaName, out var cached))
            return cached;

        if (!_document.RootElement.TryGetProperty("components", out var components))
            return null;

        if (!components.TryGetProperty("schemas", out var schemas))
            return null;

        if (!schemas.TryGetProperty(schemaName, out var schemaElement))
            return null;

        var schema = ParseSchema(schemaElement, schemaName);
        _schemaCache[schemaName] = schema;
        return schema;
    }

    /// <summary>
    /// Gets all available schema names.
    /// </summary>
    public IEnumerable<string> GetAllSchemaNames()
    {
        if (!_document.RootElement.TryGetProperty("components", out var components))
            yield break;

        if (!components.TryGetProperty("schemas", out var schemas))
            yield break;

        foreach (var prop in schemas.EnumerateObject())
        {
            yield return prop.Name;
        }
    }

    /// <summary>
    /// Resolves $ref references recursively.
    /// </summary>
    public OpenApiSchema? ResolveRef(string refPath)
    {
        // Expected format: "#/components/schemas/LightUser"
        var parts = refPath.Split('/');
        if (parts.Length >= 4 && parts[1] == "components" && parts[2] == "schemas")
        {
            return GetSchema(parts[3]);
        }
        return null;
    }

    /// <summary>
    /// Gets all properties from a schema, resolving allOf and $ref.
    /// </summary>
    public Dictionary<string, OpenApiProperty> GetAllProperties(OpenApiSchema schema)
    {
        var properties = new Dictionary<string, OpenApiProperty>();

        // Handle $ref
        if (!string.IsNullOrEmpty(schema.Ref))
        {
            var resolved = ResolveRef(schema.Ref);
            if (resolved != null)
            {
                foreach (var prop in GetAllProperties(resolved))
                {
                    properties[prop.Key] = prop.Value;
                }
            }
            return properties;
        }

        // Handle allOf (composition)
        if (schema.AllOf != null)
        {
            foreach (var part in schema.AllOf)
            {
                foreach (var prop in GetAllProperties(part))
                {
                    properties[prop.Key] = prop.Value;
                }
            }
        }

        // Add direct properties
        if (schema.Properties != null)
        {
            foreach (var prop in schema.Properties)
            {
                properties[prop.Key] = prop.Value;
            }
        }

        return properties;
    }

    /// <summary>
    /// Gets all required fields from a schema, resolving allOf and $ref.
    /// </summary>
    public HashSet<string> GetAllRequired(OpenApiSchema schema)
    {
        var required = new HashSet<string>();

        // Handle $ref
        if (!string.IsNullOrEmpty(schema.Ref))
        {
            var resolved = ResolveRef(schema.Ref);
            if (resolved != null)
            {
                foreach (var req in GetAllRequired(resolved))
                {
                    required.Add(req);
                }
            }
            return required;
        }

        // Handle allOf
        if (schema.AllOf != null)
        {
            foreach (var part in schema.AllOf)
            {
                foreach (var req in GetAllRequired(part))
                {
                    required.Add(req);
                }
            }
        }

        // Add direct required
        if (schema.Required != null)
        {
            foreach (var req in schema.Required)
            {
                required.Add(req);
            }
        }

        return required;
    }

    private OpenApiSchema ParseSchema(JsonElement element, string name)
    {
        var schema = new OpenApiSchema { Name = name };

        // Handle $ref
        if (element.TryGetProperty("$ref", out var refElement))
        {
            schema.Ref = refElement.GetString();
            return schema;
        }

        // Handle allOf (composition)
        if (element.TryGetProperty("allOf", out var allOfElement))
        {
            schema.AllOf = allOfElement.EnumerateArray()
                .Select((e, i) => ParseSchema(e, $"{name}_allOf_{i}"))
                .ToList();
        }

        // Handle oneOf (polymorphism)
        if (element.TryGetProperty("oneOf", out var oneOfElement))
        {
            schema.OneOf = oneOfElement.EnumerateArray()
                .Select((e, i) => ParseSchema(e, $"{name}_oneOf_{i}"))
                .ToList();
        }

        // Handle anyOf
        if (element.TryGetProperty("anyOf", out var anyOfElement))
        {
            schema.AnyOf = anyOfElement.EnumerateArray()
                .Select((e, i) => ParseSchema(e, $"{name}_anyOf_{i}"))
                .ToList();
        }

        // Handle type
        if (element.TryGetProperty("type", out var typeElement))
        {
            schema.Type = typeElement.ValueKind == JsonValueKind.Array
                ? string.Join("|", typeElement.EnumerateArray().Select(t => t.GetString()))
                : typeElement.GetString();
        }

        // Handle properties
        if (element.TryGetProperty("properties", out var propertiesElement))
        {
            schema.Properties = new Dictionary<string, OpenApiProperty>();
            foreach (var prop in propertiesElement.EnumerateObject())
            {
                schema.Properties[prop.Name] = ParseProperty(prop.Value, prop.Name);
            }
        }

        // Handle required
        if (element.TryGetProperty("required", out var requiredElement))
        {
            schema.Required = requiredElement.EnumerateArray()
                .Select(r => r.GetString()!)
                .ToHashSet();
        }

        return schema;
    }

    private OpenApiProperty ParseProperty(JsonElement element, string name)
    {
        var prop = new OpenApiProperty { Name = name };

        if (element.TryGetProperty("$ref", out var refElement))
            prop.Ref = refElement.GetString();

        if (element.TryGetProperty("type", out var typeElement))
            prop.Type = typeElement.ValueKind == JsonValueKind.Array
                ? string.Join("|", typeElement.EnumerateArray().Select(t => t.GetString()))
                : typeElement.GetString();

        if (element.TryGetProperty("format", out var formatElement))
            prop.Format = formatElement.GetString();

        if (element.TryGetProperty("items", out var itemsElement))
            prop.Items = ParseProperty(itemsElement, $"{name}_items");

        if (element.TryGetProperty("additionalProperties", out var additionalPropsElement))
            prop.HasAdditionalProperties = true;

        return prop;
    }

    public void Dispose() => _document.Dispose();
}

/// <summary>
/// Represents an OpenAPI schema definition.
/// </summary>
public class OpenApiSchema
{
    public string Name { get; set; } = "";
    public string? Type { get; set; }
    public string? Ref { get; set; }
    public Dictionary<string, OpenApiProperty>? Properties { get; set; }
    public HashSet<string>? Required { get; set; }
    public List<OpenApiSchema>? AllOf { get; set; }
    public List<OpenApiSchema>? OneOf { get; set; }
    public List<OpenApiSchema>? AnyOf { get; set; }
}

/// <summary>
/// Represents an OpenAPI property definition.
/// </summary>
public class OpenApiProperty
{
    public string Name { get; set; } = "";
    public string? Type { get; set; }
    public string? Format { get; set; }
    public string? Ref { get; set; }
    public OpenApiProperty? Items { get; set; }
    public bool HasAdditionalProperties { get; set; }
}
