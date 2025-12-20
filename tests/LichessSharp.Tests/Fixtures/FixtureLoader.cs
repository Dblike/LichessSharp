using System.Text.Json;

namespace LichessSharp.Tests.Fixtures;

/// <summary>
/// Loads JSON fixture files for deserialization testing.
/// Fixtures are captured API responses stored in the Fixtures directory.
/// </summary>
public static class FixtureLoader
{
    private static readonly Lazy<string> FixturesBasePath = new(GetFixturesBasePath);

    /// <summary>
    /// Gets the base path for fixture files.
    /// </summary>
    public static string BasePath => FixturesBasePath.Value;

    /// <summary>
    /// Loads a fixture file as a raw JSON string.
    /// </summary>
    /// <param name="relativePath">Path relative to the Fixtures directory (e.g., "Users/user_extended.json")</param>
    public static string LoadRawJson(string relativePath)
    {
        var fullPath = Path.Combine(BasePath, relativePath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Fixture not found: {relativePath}", fullPath);
        return File.ReadAllText(fullPath);
    }

    /// <summary>
    /// Loads and deserializes a fixture file.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="relativePath">Path relative to the Fixtures directory</param>
    /// <param name="options">Optional JSON options (defaults to LichessJsonDefaults.Options)</param>
    public static T Load<T>(string relativePath, JsonSerializerOptions? options = null)
    {
        var json = LoadRawJson(relativePath);
        options ??= LichessJsonDefaults.Options;
        return JsonSerializer.Deserialize<T>(json, options)
               ?? throw new InvalidOperationException($"Failed to deserialize fixture: {relativePath}");
    }

    /// <summary>
    /// Tries to load and deserialize a fixture file.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="relativePath">Path relative to the Fixtures directory</param>
    /// <param name="result">The deserialized object if successful</param>
    /// <param name="options">Optional JSON options</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool TryLoad<T>(string relativePath, out T? result, JsonSerializerOptions? options = null)
    {
        try
        {
            result = Load<T>(relativePath, options);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Checks if a fixture file exists.
    /// </summary>
    /// <param name="relativePath">Path relative to the Fixtures directory</param>
    public static bool Exists(string relativePath)
    {
        var fullPath = Path.Combine(BasePath, relativePath);
        return File.Exists(fullPath);
    }

    /// <summary>
    /// Gets all fixture files matching a pattern.
    /// </summary>
    /// <param name="pattern">Glob pattern (e.g., "*.json", "Users/*.json")</param>
    public static IEnumerable<string> GetFixtureFiles(string pattern = "*.json")
    {
        if (!Directory.Exists(BasePath))
            return Enumerable.Empty<string>();

        return Directory.EnumerateFiles(BasePath, pattern, SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(BasePath, f).Replace('\\', '/'));
    }

    /// <summary>
    /// Gets all fixture files in a subdirectory.
    /// </summary>
    /// <param name="subdirectory">Subdirectory name (e.g., "Users", "Games")</param>
    public static IEnumerable<string> GetFixturesInDirectory(string subdirectory)
    {
        var dirPath = Path.Combine(BasePath, subdirectory);
        if (!Directory.Exists(dirPath))
            return Enumerable.Empty<string>();

        return Directory.EnumerateFiles(dirPath, "*.json", SearchOption.TopDirectoryOnly)
            .Select(f => Path.GetRelativePath(BasePath, f).Replace('\\', '/'));
    }

    /// <summary>
    /// Saves a fixture file (for capturing API responses).
    /// </summary>
    /// <typeparam name="T">The type being serialized</typeparam>
    /// <param name="relativePath">Path relative to the Fixtures directory</param>
    /// <param name="data">The data to save</param>
    /// <param name="options">Optional JSON options</param>
    public static void Save<T>(string relativePath, T data, JsonSerializerOptions? options = null)
    {
        var fullPath = Path.Combine(BasePath, relativePath);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        options ??= LichessJsonDefaults.IndentedOptions;
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(fullPath, json);
    }

    private static string GetFixturesBasePath()
    {
        // Start from current directory and walk up to find Fixtures directory
        var dir = Directory.GetCurrentDirectory();

        while (dir != null)
        {
            // Check for Fixtures directory in tests project
            var fixturesPath = Path.Combine(dir, "tests", "LichessSharp.Tests", "Fixtures");
            if (Directory.Exists(fixturesPath))
                return fixturesPath;

            // Also check if we're already in the tests directory
            fixturesPath = Path.Combine(dir, "Fixtures");
            if (Directory.Exists(fixturesPath))
                return fixturesPath;

            // Check for LichessSharp.Tests project directory
            var testsProjPath = Path.Combine(dir, "LichessSharp.Tests");
            if (Directory.Exists(testsProjPath))
            {
                fixturesPath = Path.Combine(testsProjPath, "Fixtures");
                if (Directory.Exists(fixturesPath))
                    return fixturesPath;
            }

            dir = Directory.GetParent(dir)?.FullName;
        }

        // If not found, create it at a reasonable location
        var repoRoot = GetRepositoryRoot();
        var defaultPath = Path.Combine(repoRoot, "tests", "LichessSharp.Tests", "Fixtures");
        Directory.CreateDirectory(defaultPath);
        return defaultPath;
    }

    private static string GetRepositoryRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir, ".git")))
                return dir;
            dir = Directory.GetParent(dir)?.FullName;
        }
        return Directory.GetCurrentDirectory();
    }
}
