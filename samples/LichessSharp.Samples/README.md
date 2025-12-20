# LichessSharp Samples

Interactive sample application demonstrating common usage patterns for the LichessSharp library.

## Running the Samples

```bash
cd samples/LichessSharp.Samples
dotnet run
```

This launches an interactive menu where you can explore different API features.

## Authentication

Some samples require authentication. Set the `LICHESS_TEST_TOKEN` environment variable with a [personal access token](https://lichess.org/account/oauth/token):

```bash
# Linux/macOS
export LICHESS_TEST_TOKEN="lip_your_token_here"

# Windows (PowerShell)
$env:LICHESS_TEST_TOKEN = "lip_your_token_here"

# Windows (Command Prompt)
set LICHESS_TEST_TOKEN=lip_your_token_here
```

## Sample Scenarios

The samples are numbered to suggest a learning path. Browse them in the `Scenarios/` folder:

| # | File | Description |
|---|------|-------------|
| 1 | `01_GettingStarted.cs` | Client setup, configuration options, available APIs |
| 2 | `02_UserProfiles.cs` | Fetch user data, status, leaderboards, rating history |
| 3 | `03_GameExport.cs` | Export games (JSON/PGN), stream user games, filters |
| 4 | `04_LiveTV.cs` | Watch live games on Lichess TV channels |
| 5 | `05_Puzzles.cs` | Daily puzzle, dashboard, storm, activity |
| 6 | `06_Tournaments.cs` | Arena and Swiss tournaments, streaming results |
| 7 | `07_Broadcasts.cs` | Live event broadcasts, rounds, PGN export |
| 8 | `08_ChessAnalysis.cs` | Cloud evaluation, opening explorer, tablebase |
| 9 | `09_BoardPlay.cs` | Play games via Board API (external boards/apps) |
| 10 | `10_BotDevelopment.cs` | Build and run a Lichess bot |
| 11 | `11_ErrorHandling.cs` | Exception types, rate limits, best practices |

## Key Patterns Demonstrated

### Streaming with IAsyncEnumerable

```csharp
await foreach (var game in client.Games.StreamUserGamesAsync("DrNykterstein"))
{
    Console.WriteLine($"Game: {game.Id}");
    if (someCondition) break; // Early exit supported
}
```

### Cancellation

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
await foreach (var evt in client.Tv.StreamCurrentGameAsync(cts.Token))
{
    // Process events until timeout or cancellation
}
```

### Error Handling

```csharp
try
{
    var user = await client.Users.GetAsync("username");
}
catch (LichessNotFoundException) { /* User doesn't exist */ }
catch (LichessRateLimitException ex) { /* Wait ex.RetryAfter */ }
catch (LichessAuthorizationException ex) { /* Missing scope */ }
```

## See Also

- [Getting Started Guide](../../docs/getting-started.md)
- [Authentication Guide](../../docs/authentication.md)
- [API Coverage](../../docs/api-coverage.md)
