# LichessSharp

A fully-featured .NET client library for the [Lichess API](https://lichess.org/api).

[![NuGet](https://img.shields.io/nuget/v/LichessSharp.svg)](https://www.nuget.org/packages/LichessSharp/)
[![Lichess API](https://img.shields.io/badge/Lichess%20API-v2.0.125-blue.svg)](https://lichess.org/api)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Installation

```bash
dotnet add package LichessSharp
```

## Quick Start

```csharp
using LichessSharp;

// Create a client (unauthenticated for public API)
using var client = new LichessClient();

// Or with an access token for authenticated endpoints
using var authenticatedClient = new LichessClient("your-access-token");

// Access different API areas
var user = await client.Users.GetAsync("DrNykterstein");
var game = await client.Games.GetAsync("q7ZvsdUF");
var puzzle = await client.Puzzles.GetDailyAsync();
```

## Documentation

**[Full Documentation](../../wiki/Getting-Started)** - Installation, configuration, and usage guides

- [Authentication](../../wiki/Authentication) - OAuth tokens, scopes, security
- [Quick Reference](../../wiki/Quick-Reference) - Code snippets for all API areas

## Samples

| Sample | Description |
|--------|-------------|
| [LichessSharp.Samples](samples/LichessSharp.Samples) | Interactive demos covering all API areas |
| [LichessSharp.SimpleBot](samples/LichessSharp.SimpleBot) | Complete bot implementation |
| [LichessSharp.PuzzleSolver](samples/LichessSharp.PuzzleSolver) | Puzzle dashboard and solver CLI |
| [LichessSharp.GameArchiver](samples/LichessSharp.GameArchiver) | Export games to PGN files |
| [LichessSharp.TvViewer](samples/LichessSharp.TvViewer) | Live TV streaming viewer |
| [LichessSharp.UserStats](samples/LichessSharp.UserStats) | Player statistics tool |
| [LichessSharp.PositionAnalyzer](samples/LichessSharp.PositionAnalyzer) | Position analysis with cloud eval, opening explorer, tablebase |

## Contributing

Contributions welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

MIT License - see [LICENSE](LICENSE) for details.

## Links

- [Lichess API Documentation](https://lichess.org/api)
- [Lichess](https://lichess.org)
