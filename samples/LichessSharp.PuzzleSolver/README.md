# LichessSharp.PuzzleSolver

An interactive command-line puzzle solving application demonstrating the Puzzles API.

## Features

- **Daily Puzzle**: Fetch and display the daily puzzle with solution
- **Puzzle by ID**: Look up any puzzle by its Lichess ID
- **Random Puzzles**: Get personalized puzzles by difficulty and theme (requires auth)
- **Dashboard**: View your puzzle statistics and performance by theme (requires auth)
- **Activity Stream**: See your recent puzzle history (requires auth)
- **Storm Stats**: View Puzzle Storm high scores for any player

## Running

```bash
# Basic (unauthenticated features only)
dotnet run --project samples/LichessSharp.PuzzleSolver

# With authentication (all features)
export LICHESS_TOKEN="lip_xxxxx"  # or set on Windows
dotnet run --project samples/LichessSharp.PuzzleSolver
```

## Authentication

Some features require a Lichess API token:
- Create a token at https://lichess.org/account/oauth/token
- For full features, enable the `puzzle:read` scope
- Set the `LICHESS_TOKEN` environment variable

## Example Session

```
=== LichessSharp Puzzle Solver ===

Choose an option:
  1. Daily Puzzle
  2. Get Puzzle by ID
  3. Random Puzzle (requires auth)
  4. View Dashboard (requires auth)
  5. Recent Activity (requires auth)
  6. Storm Stats
  q. Quit

> 1

--- Daily Puzzle ---

Puzzle ID: abc123
Rating: 1542
Plays: 45,231
Themes: middlegame, fork, short

From game: player1 vs player2
Game ID: xyz789

Solution (spoiler alert!):
  e4e5 -> d1h5 -> h5f7

Play this puzzle: https://lichess.org/training/abc123
```

## API Methods Demonstrated

| Feature | Method |
|---------|--------|
| Daily Puzzle | `Puzzles.GetDailyAsync()` |
| Get by ID | `Puzzles.GetAsync(id)` |
| Random Puzzle | `Puzzles.GetNextAsync(angle, difficulty)` |
| Dashboard | `Puzzles.GetDashboardAsync(days)` |
| Activity | `Puzzles.StreamActivityAsync(max)` |
| Storm Stats | `Puzzles.GetStormDashboardAsync(username, days)` |

## Puzzle Themes

Common themes you can filter by:
- `mateIn1`, `mateIn2`, `mateIn3`, `mateIn4`, `mateIn5`
- `fork`, `pin`, `skewer`, `discoveredAttack`
- `endgame`, `opening`, `middlegame`
- `sacrifice`, `deflection`, `clearance`
- `backRankMate`, `smotheredMate`, `hookMate`

## See Also

- [Puzzles API Documentation](../../wiki/api/Puzzles.md)
- [LichessSharp.SimpleBot](../LichessSharp.SimpleBot/) - Bot development sample
