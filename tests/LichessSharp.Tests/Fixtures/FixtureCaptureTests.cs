using LichessSharp.Tests.Integration;
using Xunit;
using Xunit.Abstractions;

namespace LichessSharp.Tests.Fixtures;

/// <summary>
/// Utility tests for capturing real API responses as fixtures.
/// These tests are marked with Skip and should be run manually when fixtures need updating.
///
/// To run: dotnet test --filter "Category=FixtureCapture" -- RunConfiguration.SkipTestExecution=false
/// Or remove the Skip attribute temporarily.
/// </summary>
[IntegrationTest]
[Trait("Category", "FixtureCapture")]
public class FixtureCaptureTests : IntegrationTestBase
{
    private readonly ITestOutputHelper _output;

    public FixtureCaptureTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(Skip = "Run manually to capture fixtures")]
    public async Task Capture_UserExtended_Thibault()
    {
        var user = await Client.Users.GetAsync("thibault");
        FixtureLoader.Save("Users/user_extended_thibault.json", user);
        _output.WriteLine($"Saved user fixture for: {user.Username}");
    }

    [Fact(Skip = "Run manually to capture fixtures")]
    public async Task Capture_UserStatus_Multiple()
    {
        var statuses = await Client.Users.GetRealTimeStatusAsync(new[] { "thibault", "maia1", "maia5" });
        FixtureLoader.Save("Users/user_status_multiple.json", statuses);
        _output.WriteLine($"Saved {statuses.Count} user statuses");
    }

    [Fact(Skip = "Run manually to capture fixtures")]
    public async Task Capture_RatingHistory()
    {
        var history = await Client.Users.GetRatingHistoryAsync("thibault");
        FixtureLoader.Save("Users/rating_history_thibault.json", history);
        _output.WriteLine($"Saved {history.Count} rating history entries");
    }

    [Fact(Skip = "Run manually to capture fixtures")]
    public async Task Capture_GameJson()
    {
        // Use a well-known game ID from the OpenAPI examples
        var game = await Client.Games.ExportAsync("q7ZvsdUF");
        FixtureLoader.Save("Games/game_json_full.json", game);
        _output.WriteLine($"Saved game fixture: {game.Id}");
    }

    [Fact(Skip = "Run manually to capture fixtures")]
    public async Task Capture_DailyPuzzle()
    {
        var puzzle = await Client.Puzzles.GetDailyAsync();
        FixtureLoader.Save("Puzzles/puzzle_daily.json", puzzle);
        _output.WriteLine($"Saved daily puzzle fixture");
    }

    [Fact(Skip = "Run manually to capture fixtures")]
    public async Task Capture_Leaderboard()
    {
        // Get bullet leaderboard as an example
        var leaderboard = await Client.Users.GetLeaderboardAsync("bullet");
        FixtureLoader.Save("Users/leaderboard_bullet.json", leaderboard);
        _output.WriteLine($"Saved leaderboard fixture with {leaderboard.Count} players");
    }

    [Fact(Skip = "Run manually to capture fixtures")]
    public async Task Capture_Crosstable()
    {
        var crosstable = await Client.Users.GetCrosstableAsync("thibault", "DrNykterstein");
        FixtureLoader.Save("Users/crosstable.json", crosstable);
        _output.WriteLine($"Saved crosstable fixture");
    }

    [Fact(Skip = "Run manually to capture fixtures")]
    public async Task Capture_AllFixtures()
    {
        _output.WriteLine("Capturing all fixtures...\n");

        await CaptureWithReport("Users/user_extended_thibault.json", async () =>
            await Client.Users.GetAsync("thibault"));

        await CaptureWithReport("Users/user_status_multiple.json", async () =>
            await Client.Users.GetRealTimeStatusAsync(new[] { "thibault", "maia1", "maia5" }));

        await CaptureWithReport("Users/rating_history_thibault.json", async () =>
            await Client.Users.GetRatingHistoryAsync("thibault"));

        await CaptureWithReport("Users/leaderboard_bullet.json", async () =>
            await Client.Users.GetLeaderboardAsync("bullet"));

        await CaptureWithReport("Games/game_json_full.json", async () =>
            await Client.Games.ExportAsync("q7ZvsdUF"));

        await CaptureWithReport("Puzzles/puzzle_daily.json", async () =>
            await Client.Puzzles.GetDailyAsync());

        _output.WriteLine("\nAll fixtures captured successfully!");
    }

    private async Task CaptureWithReport<T>(string path, Func<Task<T>> captureFunc)
    {
        try
        {
            _output.WriteLine($"Capturing: {path}...");
            var data = await captureFunc();
            FixtureLoader.Save(path, data);
            _output.WriteLine($"  OK: Saved {path}");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"  FAILED: {path} - {ex.Message}");
        }
    }
}
