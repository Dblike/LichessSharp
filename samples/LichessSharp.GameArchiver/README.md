# LichessSharp.GameArchiver

A command-line tool for exporting Lichess games to PGN files.

## Features

- Export all games for any Lichess user
- Filter by variant, time period, and rated/casual
- Include optional clock times and engine evaluations
- Real-time progress tracking
- Graceful cancellation with Ctrl+C

## Usage

```bash
dotnet run --project samples/LichessSharp.GameArchiver -- <username> [options]
```

### Options

| Option | Description |
|--------|-------------|
| `-o, --output <file>` | Output PGN file (default: `<username>_games.pgn`) |
| `-n, --max <count>` | Maximum number of games to export |
| `-v, --variant <type>` | Filter by variant: bullet, blitz, rapid, classical, etc. |
| `-r, --rated` | Only export rated games |
| `-m, --months <count>` | Only games from the last N months |
| `--clocks` | Include clock times in PGN |
| `--evals` | Include engine evaluations |

### Examples

```bash
# Export last 100 games
dotnet run -- DrNykterstein -n 100

# Export blitz games from the last 3 months
dotnet run -- DrNykterstein -v blitz -m 3

# Export all rated rapid games with clocks and evals
dotnet run -- DrNykterstein -v rapid -r --clocks --evals -o magnus_rapid.pgn

# Export all games (warning: may take a while for active players)
dotnet run -- DrNykterstein -o complete_archive.pgn
```

## Output

The tool writes standard PGN format that can be imported into:
- Lichess studies
- Chess.com game explorer
- ChessBase
- SCID
- Any PGN-compatible analysis software

### Sample Output

```
=== LichessSharp Game Archiver ===

Exporting games for: DrNykterstein
Variant filter: blitz
Rated games only
Since: 2026-01-01
Maximum games: 100
Output file: DrNykterstein_games.pgn

Exported 100 games (15.2/s) - W:62 L:28 D:10

=== Export Complete ===
Total games: 100
Wins: 62 (62.0%)
Losses: 28 (28.0%)
Draws: 10 (10.0%)
Output: C:\Users\user\DrNykterstein_games.pgn
```

## API Methods Demonstrated

| Feature | Method |
|---------|--------|
| Stream games | `Games.StreamUserGamesAsync(username, options)` |
| Export options | `ExportUserGamesOptions` with filters |
| Cancellation | `CancellationToken` support |

## Notes

- No authentication required (exports are public)
- Rate limits are handled automatically by LichessSharp
- Large exports may take time; progress is shown in real-time
- Press Ctrl+C to stop early and keep the games already exported

## See Also

- [Games API Documentation](../../wiki/api/Games.md)
- [LichessSharp.SimpleBot](../LichessSharp.SimpleBot/) - Bot development sample
- [LichessSharp.PuzzleSolver](../LichessSharp.PuzzleSolver/) - Puzzle solving sample
