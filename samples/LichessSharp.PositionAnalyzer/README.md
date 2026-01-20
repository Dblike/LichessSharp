# LichessSharp.PositionAnalyzer

A chess position analysis tool using cloud evaluation, opening explorer, and tablebase APIs.

## Features

- Cloud evaluation lookup with multiple principal variations
- Opening explorer (Masters, Lichess, and Player databases)
- Endgame tablebase lookup (up to 7 pieces)
- ASCII board display
- Famous position presets

## Usage

```bash
# Interactive mode
dotnet run --project samples/LichessSharp.PositionAnalyzer
```

## Sample Output

```
=== LichessSharp Position Analyzer ===

Current FEN: rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1

  +---+---+---+---+---+---+---+---+
8 | r | n | b | q | k | b | n | r |
  +---+---+---+---+---+---+---+---+
7 | p | p | p | p | p | p | p | p |
  +---+---+---+---+---+---+---+---+
6 |   |   |   |   |   |   |   |   |
  +---+---+---+---+---+---+---+---+
5 |   |   |   |   |   |   |   |   |
  +---+---+---+---+---+---+---+---+
4 |   |   |   |   |   |   |   |   |
  +---+---+---+---+---+---+---+---+
3 |   |   |   |   |   |   |   |   |
  +---+---+---+---+---+---+---+---+
2 | P | P | P | P | P | P | P | P |
  +---+---+---+---+---+---+---+---+
1 | R | N | B | Q | K | B | N | R |
  +---+---+---+---+---+---+---+---+
    a   b   c   d   e   f   g   h

Options:
  1. Cloud evaluation
  2. Opening explorer (Masters)
  3. Opening explorer (Lichess)
  4. Opening explorer (Player)
  5. Tablebase lookup
  6. Enter new FEN
  7. Use famous positions
  q. Quit
```

### Opening Explorer Output

```
Database: Masters
Total games: 1,234,567
  White wins: 456,789 (37.0%)
  Draws:      432,100 (35.0%)
  Black wins: 345,678 (28.0%)

Opening: C50 Italian Game

Moves played:
  Move        Games   White%    Draw%   Black%  AvgElo
  -------------------------------------------------------
  e4        654,321    37.2%    35.1%    27.7%   2450
  d4        234,567    38.1%    36.2%    25.7%   2480
  Nf3       123,456    36.5%    34.8%    28.7%   2420
```

### Tablebase Output

```
Category: win
DTZ (Distance to Zero/conversion): 15
DTM (Distance to Mate): 23

Best moves:
  Kc7      -> win, mate in 11
  Rc2+     -> win, mate in 12
  Rb1      -> draw, dtz 0
```

## Famous Positions

The tool includes preset positions for quick testing:

| # | Position | Description |
|---|----------|-------------|
| 1 | Starting position | Standard chess opening |
| 2 | Italian Game | After 1.e4 e5 2.Nf3 Nc6 3.Bc4 |
| 3 | Sicilian Dragon | Dragon variation setup |
| 4 | King's Indian Attack | KIA formation |
| 5 | Immortal Game | Anderssen vs Kieseritzky, 1851 |
| 6 | Lucena position | Famous rook endgame |
| 7 | Philidor position | Defensive rook endgame |
| 8 | KQvK | Queen vs King endgame |

## API Methods Demonstrated

| Feature | Method |
|---------|--------|
| Cloud evaluation | `Analysis.GetCloudEvaluationAsync(fen)` |
| Masters explorer | `OpeningExplorer.GetMastersAsync(fen)` |
| Lichess explorer | `OpeningExplorer.GetLichessAsync(fen)` |
| Player explorer | `OpeningExplorer.GetPlayerAsync(fen, player)` |
| Tablebase | `Tablebase.LookupAsync(fen)` |

## See Also

- [Analysis API Documentation](../../wiki/api/Analysis.md)
- [Opening Explorer API Documentation](../../wiki/api/OpeningExplorer.md)
- [Tablebase API Documentation](../../wiki/api/Tablebase.md)
- [LichessSharp.PuzzleSolver](../LichessSharp.PuzzleSolver/) - Puzzle solving
- [LichessSharp.GameArchiver](../LichessSharp.GameArchiver/) - Game export
