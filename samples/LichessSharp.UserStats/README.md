# LichessSharp.UserStats

A player statistics and comparison tool.

## Features

- View detailed player profiles
- Rating history across all time controls
- Head-to-head comparison with crosstable
- Real-time online status checking
- Win/loss/draw statistics

## Usage

```bash
# Interactive mode
dotnet run --project samples/LichessSharp.UserStats

# Direct lookup
dotnet run --project samples/LichessSharp.UserStats DrNykterstein
dotnet run --project samples/LichessSharp.UserStats Hikaru
```

## Sample Output

```
=== GM DrNykterstein ===

Profile:
  ID: drnykterstein
  Created: 2014-12-09
  Last seen: 2026-01-20 14:30
  Play time: 4,521 hours

Ratings:
  bullet        : 3234  - 5,421 games
  blitz         : 3271  - 12,543 games
  rapid         : 2901  - 1,234 games

Game Statistics:
  Total games: 24,532
  Wins: 18,234
  Losses: 4,123
  Draws: 2,175
  Win rate: 74.3%

Options:
  1. View rating history
  2. Compare with another player
  3. Check online status
  4. Look up another user
  q. Quit
```

## API Methods Demonstrated

| Feature | Method |
|---------|--------|
| Get profile | `Users.GetAsync(username)` |
| Get multiple | `Users.GetManyAsync(usernames)` |
| Rating history | `Users.GetRatingHistoryAsync(username)` |
| Crosstable | `Users.GetCrosstableAsync(user1, user2)` |
| Online status | `Users.GetRealTimeStatusAsync(usernames)` |

## See Also

- [Users API Documentation](../../wiki/api/Users.md)
- [LichessSharp.GameArchiver](../LichessSharp.GameArchiver/) - Game export
- [LichessSharp.TvViewer](../LichessSharp.TvViewer/) - Watch live games
