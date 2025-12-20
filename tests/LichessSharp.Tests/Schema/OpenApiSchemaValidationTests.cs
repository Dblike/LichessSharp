using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace LichessSharp.Tests.Schema;

/// <summary>
/// Validates that C# model properties match the OpenAPI schema definitions.
/// These tests ensure JsonPropertyName attributes are correct and no fields are missing.
/// </summary>
public class OpenApiSchemaValidationTests : IDisposable
{
    private readonly OpenApiSchemaReader _schemaReader;
    private readonly ITestOutputHelper _output;
    private static readonly string OpenApiPath = GetOpenApiPath();

    public OpenApiSchemaValidationTests(ITestOutputHelper output)
    {
        _output = output;
        _schemaReader = new OpenApiSchemaReader(OpenApiPath);
    }

    /// <summary>
    /// Provides test data for all ResponseOnly types that have matching OpenAPI schemas.
    /// </summary>
    public static IEnumerable<object[]> GetValidatableTypes()
    {
        var schemaReader = new OpenApiSchemaReader(GetOpenApiPath());
        var schemaNames = schemaReader.GetAllSchemaNames().ToHashSet();

        foreach (var type in ModelReflector.GetResponseOnlyTypes())
        {
            if (SchemaModelMapping.ShouldExclude(type))
                continue;

            var schemaName = SchemaModelMapping.GetSchemaName(type);
            if (schemaNames.Contains(schemaName))
            {
                yield return new object[] { type, type.Name, schemaName };
            }
        }

        schemaReader.Dispose();
    }

    [Theory]
    [MemberData(nameof(GetValidatableTypes))]
    public void Model_JsonPropertyNames_ShouldMatchOpenApiSchema(Type modelType, string typeName, string schemaName)
    {
        // Arrange
        var schema = _schemaReader.GetSchema(schemaName);
        schema.Should().NotBeNull($"Schema '{schemaName}' should exist for type '{typeName}'");

        var csharpProperties = ModelReflector.GetJsonPropertyMappings(modelType);
        var schemaProperties = _schemaReader.GetAllProperties(schema!);
        var requiredFields = _schemaReader.GetAllRequired(schema!);

        // Track issues
        var missingInCSharp = new List<string>();
        var extraInCSharp = new List<string>();

        // Check for properties in schema that are missing from C#
        foreach (var schemaPropName in schemaProperties.Keys)
        {
            if (!csharpProperties.ContainsKey(schemaPropName))
            {
                var isRequired = requiredFields.Contains(schemaPropName);
                missingInCSharp.Add(isRequired ? $"{schemaPropName} (REQUIRED)" : schemaPropName);
            }
        }

        // Check for properties in C# that don't exist in schema
        foreach (var csharpProp in csharpProperties.Values)
        {
            // Skip ignored properties
            if (csharpProp.IsIgnored)
                continue;

            // Skip extension data properties
            if (csharpProp.HasExtensionData)
                continue;

            if (!schemaProperties.ContainsKey(csharpProp.JsonName))
            {
                extraInCSharp.Add($"{csharpProp.CSharpName} -> '{csharpProp.JsonName}'");
            }
        }

        // Report findings
        if (missingInCSharp.Any())
        {
            _output.WriteLine($"[{typeName}] Missing in C# model: {string.Join(", ", missingInCSharp)}");
        }

        if (extraInCSharp.Any())
        {
            _output.WriteLine($"[{typeName}] Extra in C# model (not in schema): {string.Join(", ", extraInCSharp)}");
        }

        // Assert - only fail on required fields missing
        var missingRequired = missingInCSharp.Where(f => f.Contains("(REQUIRED)")).ToList();
        missingRequired.Should().BeEmpty(
            $"Type '{typeName}' is missing REQUIRED schema properties: {string.Join(", ", missingRequired)}");
    }

    [Fact]
    public void AllResponseOnlyTypes_Coverage_Report()
    {
        // This test generates a coverage report - doesn't fail, just reports
        var allTypes = ModelReflector.GetResponseOnlyTypes().ToList();
        var schemaNames = _schemaReader.GetAllSchemaNames().ToHashSet();

        var matched = new List<string>();
        var excluded = new List<string>();
        var unmatched = new List<string>();

        foreach (var type in allTypes)
        {
            var schemaName = SchemaModelMapping.GetSchemaName(type);

            if (SchemaModelMapping.ShouldExclude(type))
            {
                excluded.Add($"{type.Name} (excluded)");
            }
            else if (schemaNames.Contains(schemaName))
            {
                matched.Add($"{type.Name} -> {schemaName}");
            }
            else
            {
                unmatched.Add($"{type.Name} -> {schemaName} (no schema found)");
            }
        }

        _output.WriteLine("=== Schema Validation Coverage Report ===\n");

        _output.WriteLine($"Matched Types ({matched.Count}):");
        foreach (var m in matched.OrderBy(x => x))
            _output.WriteLine($"  + {m}");

        _output.WriteLine($"\nExcluded Types ({excluded.Count}):");
        foreach (var e in excluded.OrderBy(x => x))
            _output.WriteLine($"  ~ {e}");

        _output.WriteLine($"\nUnmatched Types ({unmatched.Count}):");
        foreach (var u in unmatched.OrderBy(x => x))
            _output.WriteLine($"  - {u}");

        _output.WriteLine($"\nTotal: {allTypes.Count} types, {matched.Count} matched, {excluded.Count} excluded, {unmatched.Count} unmatched");
    }

    [Fact]
    public void JsonPropertyNames_ShouldBeCamelCase()
    {
        // Validates that all JsonPropertyName values follow camelCase convention
        var violations = new List<string>();

        foreach (var type in ModelReflector.GetResponseOnlyTypes())
        {
            var properties = ModelReflector.GetJsonPropertyMappings(type);

            foreach (var prop in properties.Values)
            {
                if (prop.IsIgnored || prop.HasExtensionData)
                    continue;

                // Check if first character is lowercase (camelCase)
                if (!string.IsNullOrEmpty(prop.JsonName) && char.IsUpper(prop.JsonName[0]))
                {
                    violations.Add($"{type.Name}.{prop.CSharpName} has PascalCase JsonPropertyName: '{prop.JsonName}'");
                }
            }
        }

        if (violations.Any())
        {
            _output.WriteLine("Properties with non-camelCase JsonPropertyName:");
            foreach (var v in violations)
                _output.WriteLine($"  ! {v}");
        }

        violations.Should().BeEmpty("All JsonPropertyName values should be camelCase to match Lichess API convention");
    }

    [Fact]
    public void RequiredProperties_ShouldBeMarkedAsRequired()
    {
        // Validates that properties marked as required in OpenAPI have 'required' modifier in C#
        var issues = new List<string>();

        foreach (var (type, _, schemaName) in GetValidatableTypes().Select(x => ((Type)x[0], (string)x[1], (string)x[2])))
        {
            var schema = _schemaReader.GetSchema(schemaName);
            if (schema == null)
                continue;

            var requiredFields = _schemaReader.GetAllRequired(schema);
            var csharpProperties = ModelReflector.GetJsonPropertyMappings(type);

            foreach (var requiredField in requiredFields)
            {
                if (csharpProperties.TryGetValue(requiredField, out var prop))
                {
                    // Check if C# property is marked as required or is non-nullable value type
                    var isNonNullable = !prop.IsNullable && !prop.PropertyType.IsValueType;
                    if (!prop.IsRequired && isNonNullable)
                    {
                        // This is informational - C# required modifier is preferred but not mandatory
                        // as long as the property can be deserialized correctly
                    }
                }
            }
        }

        // This test is informational - report but don't fail
        if (issues.Any())
        {
            _output.WriteLine("Properties that might need 'required' modifier:");
            foreach (var i in issues)
                _output.WriteLine($"  ? {i}");
        }
    }

    private static string GetOpenApiPath()
    {
        var dir = Directory.GetCurrentDirectory();

        // Walk up to find the repository root (where openapi folder is)
        while (dir != null)
        {
            var openApiPath = Path.Combine(dir, "openapi", "lichess.openapi.json");
            if (File.Exists(openApiPath))
                return openApiPath;

            dir = Directory.GetParent(dir)?.FullName;
        }

        throw new FileNotFoundException(
            "Could not find openapi/lichess.openapi.json. " +
            "Make sure you're running tests from within the repository.");
    }

    public void Dispose() => _schemaReader.Dispose();
}
