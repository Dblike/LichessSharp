# LichessSharp.TvViewer

A live chess TV streaming viewer for your terminal.

## Features

- Watch live games from any Lichess TV channel
- Real-time position updates with ASCII board
- Clock display and move tracking
- Channel selection menu
- Featured game streaming

## Usage

```bash
# Interactive menu
dotnet run --project samples/LichessSharp.TvViewer

# Direct channel
dotnet run --project samples/LichessSharp.TvViewer bullet
dotnet run --project samples/LichessSharp.TvViewer blitz
```

## Available Channels

| Channel | Description |
|---------|-------------|
| `bullet` | 1-2 minute games |
| `blitz` | 3-5 minute games |
| `rapid` | 10-15 minute games |
| `classical` | 30+ minute games |
| `ultraBullet` | Under 1 minute games |
| `chess960` | Fischer Random Chess |
| `computer` | Games vs Stockfish |
| `bot` | Games between bots |

## Sample Output

```
=== BLITZ TV ===
Streaming live games. Press Ctrl+C to stop.

White: DrNykterstein (2850)
Black: Hikaru (2800)

  +---+---+---+---+---+---+---+---+
8 | r |   | b | q | k | b | n | r |
  +---+---+---+---+---+---+---+---+
7 | p | p | p | p |   | p | p | p |
  +---+---+---+---+---+---+---+---+
6 |   |   | n |   |   |   |   |   |
  +---+---+---+---+---+---+---+---+
5 |   |   |   |   | p |   |   |   |
  +---+---+---+---+---+---+---+---+
4 |   |   |   |   | P |   |   |   |
  +---+---+---+---+---+---+---+---+
3 |   |   |   |   |   | N |   |   |
  +---+---+---+---+---+---+---+---+
2 | P | P | P | P |   | P | P | P |
  +---+---+---+---+---+---+---+---+
1 | R | N | B | Q | K | B |   | R |
  +---+---+---+---+---+---+---+---+
    a   b   c   d   e   f   g   h

Last move: g1f3
Time: White 2:45 | Black 2:52

Updates: 5
```

## API Methods Demonstrated

| Feature | Method |
|---------|--------|
| Get current games | `Tv.GetCurrentGamesAsync()` |
| Stream featured game | `Tv.StreamCurrentGameAsync()` |
| Stream channel | `Tv.StreamChannelAsync(channel)` |

## See Also

- [TV API Documentation](../../wiki/api/Tv.md)
- [LichessSharp.SimpleBot](../LichessSharp.SimpleBot/) - Bot development
- [LichessSharp.GameArchiver](../LichessSharp.GameArchiver/) - Game export
