using Lichess.NET.Entities;

namespace Lichess.NET.Test;

public class LichessClientTests
{
    private readonly LichessClient _client;

    public LichessClientTests()
    {
        _client = new LichessClient();
    }

    [Fact]
    public async Task ExportGamesByUserAsync()
    {
        var batches = new List<List<GameExplorerResult>>();

        await foreach (var batch in _client.ExportGamesByUserAsync("german11", new Games.ExportGamesByUserOptions()
        {
            MaxGames = 40
        }))
        {
            batches.Add(batch);
        }

        Assert.True(batches[0].Count == 20);
        Assert.True(batches[1].Count == 20);
    }

    [Fact]
    public async Task ExportGamesByIdsAsync()
    {
        var gamesIds = new List<string> { "TJxUmbWK", "4OtIh2oh", "ILwozzRZ" };
        var games = await _client.ExportGamesByIdsAsync(gamesIds, new Games.ExportGameOptions());

        Assert.True(games.Count == gamesIds.Count);
    }
}