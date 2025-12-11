namespace LichessSharp.Samples.Helpers;

/// <summary>
/// Helper utilities for running samples with consistent formatting.
/// </summary>
public static class SampleRunner
{
    /// <summary>
    /// Prints a section header.
    /// </summary>
    public static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();
    }

    /// <summary>
    /// Prints a subsection header.
    /// </summary>
    public static void PrintSubHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"--- {title} ---");
        Console.WriteLine();
    }

    /// <summary>
    /// Prints an info message.
    /// </summary>
    public static void PrintInfo(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }

    /// <summary>
    /// Prints a success message.
    /// </summary>
    public static void PrintSuccess(string message)
    {
        Console.WriteLine($"[OK] {message}");
    }

    /// <summary>
    /// Prints a warning message.
    /// </summary>
    public static void PrintWarning(string message)
    {
        Console.WriteLine($"[WARN] {message}");
    }

    /// <summary>
    /// Prints an error message.
    /// </summary>
    public static void PrintError(string message)
    {
        Console.WriteLine($"[ERROR] {message}");
    }

    /// <summary>
    /// Prints a key-value pair.
    /// </summary>
    public static void PrintKeyValue(string key, object? value)
    {
        Console.WriteLine($"  {key}: {value ?? "(null)"}");
    }

    /// <summary>
    /// Gets the Lichess test token from environment variable.
    /// </summary>
    public static string? GetToken()
    {
        return Environment.GetEnvironmentVariable("LICHESS_TEST_TOKEN");
    }

    /// <summary>
    /// Checks if authentication is available and prints appropriate message.
    /// </summary>
    public static bool CheckAuthentication(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            PrintWarning("No LICHESS_TEST_TOKEN environment variable found.");
            PrintInfo("Some features require authentication. Set the token to enable all features.");
            return false;
        }

        PrintSuccess("Authentication token found.");
        return true;
    }

    /// <summary>
    /// Waits for user to press a key to continue.
    /// </summary>
    public static void WaitForKey(string message = "Press any key to continue...")
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.ReadKey(intercept: true);
    }

    /// <summary>
    /// Asks user a yes/no question.
    /// </summary>
    public static bool AskYesNo(string question)
    {
        Console.Write($"{question} (y/n): ");
        var key = Console.ReadKey(intercept: false);
        Console.WriteLine();
        return key.KeyChar is 'y' or 'Y';
    }
}
