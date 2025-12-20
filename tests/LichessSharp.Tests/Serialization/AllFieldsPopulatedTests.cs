using System.Text.Json;
using FluentAssertions;
using LichessSharp.Models.Games;
using LichessSharp.Models.Puzzles;
using LichessSharp.Models.Users;
using LichessSharp.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace LichessSharp.Tests.Serialization;

/// <summary>
/// Tests that verify all JSON fields in fixtures are captured by C# model properties.
/// Detects missing JsonPropertyName attributes or missing properties.
/// </summary>
public class AllFieldsPopulatedTests
{
    private readonly ITestOutputHelper _output;
    private readonly JsonSerializerOptions _options = LichessJsonDefaults.Options;

    public AllFieldsPopulatedTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Provides test data for field coverage tests.
    /// </summary>
    public static IEnumerable<object[]> GetFieldCoverageTestCases()
    {
        if (FixtureLoader.Exists("Users/user_extended_thibault.json"))
            yield return new object[] { "Users/user_extended_thibault.json", typeof(UserExtended) };

        if (FixtureLoader.Exists("Games/game_json_full.json"))
            yield return new object[] { "Games/game_json_full.json", typeof(GameJson) };

        if (FixtureLoader.Exists("Puzzles/puzzle_daily.json"))
            yield return new object[] { "Puzzles/puzzle_daily.json", typeof(PuzzleWithGame) };
    }

    [Theory]
    [MemberData(nameof(GetFieldCoverageTestCases))]
    public void AllJsonFields_AreMappedToProperties(string fixturePath, Type modelType)
    {
        // Arrange
        var originalJson = FixtureLoader.LoadRawJson(fixturePath);
        using var originalDoc = JsonDocument.Parse(originalJson);

        // Deserialize and reserialize
        var deserialized = JsonSerializer.Deserialize(originalJson, modelType, _options);
        var reserialized = JsonSerializer.Serialize(deserialized, modelType, _options);
        using var reserializedDoc = JsonDocument.Parse(reserialized);

        // Find missing fields
        var missingFields = FindMissingFields(originalDoc.RootElement, reserializedDoc.RootElement, "");
        var unmappedFields = missingFields.Where(f => !IsExpectedMissing(f)).ToList();

        // Report
        if (missingFields.Any())
        {
            _output.WriteLine($"Fields in {fixturePath} not captured by {modelType.Name}:");
            foreach (var field in missingFields)
            {
                var status = IsExpectedMissing(field) ? " (expected - custom converter)" : " (!!)";
                _output.WriteLine($"  - {field}{status}");
            }
        }
        else
        {
            _output.WriteLine($"All fields in {fixturePath} are captured by {modelType.Name}");
        }

        // Assert - only fail on unexpected missing fields
        unmappedFields.Should().BeEmpty(
            $"Some JSON fields are not mapped to C# properties in {modelType.Name}. " +
            $"Missing: {string.Join(", ", unmappedFields)}");
    }

    [Fact]
    public void DetailedFieldAnalysis_UserExtended()
    {
        const string fixturePath = "Users/user_extended_thibault.json";
        if (!FixtureLoader.Exists(fixturePath))
        {
            _output.WriteLine($"Fixture not found: {fixturePath}");
            return;
        }

        var originalJson = FixtureLoader.LoadRawJson(fixturePath);
        using var doc = JsonDocument.Parse(originalJson);

        _output.WriteLine("=== Field Analysis for UserExtended ===\n");

        AnalyzeElement(doc.RootElement, "", 0);
    }

    [Fact]
    public void DetailedFieldAnalysis_GameJson()
    {
        const string fixturePath = "Games/game_json_full.json";
        if (!FixtureLoader.Exists(fixturePath))
        {
            _output.WriteLine($"Fixture not found: {fixturePath}");
            return;
        }

        var originalJson = FixtureLoader.LoadRawJson(fixturePath);
        using var doc = JsonDocument.Parse(originalJson);

        _output.WriteLine("=== Field Analysis for GameJson ===\n");

        AnalyzeElement(doc.RootElement, "", 0);
    }

    private List<string> FindMissingFields(JsonElement original, JsonElement reserialized, string path)
    {
        var missing = new List<string>();

        if (original.ValueKind != JsonValueKind.Object)
            return missing;

        foreach (var prop in original.EnumerateObject())
        {
            var fieldPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";

            if (!reserialized.TryGetProperty(prop.Name, out var reserializedValue))
            {
                missing.Add(fieldPath);
            }
            else if (prop.Value.ValueKind == JsonValueKind.Object &&
                     reserializedValue.ValueKind == JsonValueKind.Object)
            {
                // Recursively check nested objects
                missing.AddRange(FindMissingFields(prop.Value, reserializedValue, fieldPath));
            }
            else if (prop.Value.ValueKind == JsonValueKind.Array &&
                     reserializedValue.ValueKind == JsonValueKind.Array)
            {
                // For arrays, check the first element if it's an object
                var origArray = prop.Value.EnumerateArray().ToList();
                var reserArray = reserializedValue.EnumerateArray().ToList();

                if (origArray.Any() && reserArray.Any() &&
                    origArray[0].ValueKind == JsonValueKind.Object &&
                    reserArray[0].ValueKind == JsonValueKind.Object)
                {
                    missing.AddRange(FindMissingFields(origArray[0], reserArray[0], $"{fieldPath}[0]"));
                }
            }
        }

        return missing;
    }

    private static bool IsExpectedMissing(string fieldPath)
    {
        // Fields that are expected to be missing due to custom converters or intentional exclusion
        var expectedMissing = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Add fields that are handled by custom converters and may not round-trip exactly
            // Example: "perfs.bullet.prov" -> "perfs.bullet.provisional"
        };

        return expectedMissing.Contains(fieldPath);
    }

    private void AnalyzeElement(JsonElement element, string path, int depth)
    {
        var indent = new string(' ', depth * 2);

        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                var fieldPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
                var typeDesc = GetTypeDescription(prop.Value);

                _output.WriteLine($"{indent}{prop.Name}: {typeDesc}");

                if (prop.Value.ValueKind == JsonValueKind.Object && depth < 3)
                {
                    AnalyzeElement(prop.Value, fieldPath, depth + 1);
                }
                else if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    var firstItem = prop.Value.EnumerateArray().FirstOrDefault();
                    if (firstItem.ValueKind == JsonValueKind.Object && depth < 2)
                    {
                        _output.WriteLine($"{indent}  [0]:");
                        AnalyzeElement(firstItem, $"{fieldPath}[0]", depth + 2);
                    }
                }
            }
        }
    }

    private static string GetTypeDescription(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => "object",
            JsonValueKind.Array => $"array[{element.GetArrayLength()}]",
            JsonValueKind.String => $"string = \"{Truncate(element.GetString(), 30)}\"",
            JsonValueKind.Number => $"number = {element.GetRawText()}",
            JsonValueKind.True => "bool = true",
            JsonValueKind.False => "bool = false",
            JsonValueKind.Null => "null",
            _ => element.ValueKind.ToString()
        };
    }

    private static string Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return "";
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }
}
