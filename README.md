# LichessSharp

A fully-featured .NET client library for the [Lichess API](https://lichess.org/api).

[![NuGet](https://img.shields.io/nuget/v/LichessSharp.svg)](https://www.nuget.org/packages/LichessSharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Features

- **13 API areas fully implemented** with 84+ endpoints (Account, Users, Relations, Games, TV, Puzzles, Analysis, Opening Explorer, Tablebase, Challenges, Board, Bot, Teams)
- Async-first design with `CancellationToken` support on all methods
- Streaming support via `IAsyncEnumerable<T>` for real-time NDJSON data
- Strong typing with comprehensive models and enums
- Built-in rate limiting and automatic retry logic
- AOT-compatible with `System.Text.Json` source generators
- Targets **.NET 10.0**

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
var profile = await client.Account.GetProfileAsync();
var user = await client.Users.GetAsync("DrNykterstein");
var game = await client.Games.GetAsync("q7ZvsdUF");
var puzzle = await client.Puzzles.GetDailyAsync();
```

## Configuration

```csharp
var options = new LichessClientOptions
{
    AccessToken = "your-token",
    DefaultTimeout = TimeSpan.FromSeconds(30),
    AutoRetryOnRateLimit = true,
    MaxRateLimitRetries = 3
};

using var client = new LichessClient(new HttpClient(), options);
```

## Streaming

Many Lichess endpoints stream data in real-time using newline-delimited JSON (ndjson). LichessSharp handles this natively with `IAsyncEnumerable`:

```csharp
// Stream user games
await foreach (var game in client.Games.StreamUserGamesAsync("DrNykterstein"))
{
    Console.WriteLine($"Game: {game.Id}");
}

// Stream TV feed (real-time positions and moves)
await foreach (var evt in client.Tv.StreamCurrentGameAsync())
{
    Console.WriteLine($"Type: {evt.Type}, FEN: {evt.Data?.Fen}");
}
```

## API Coverage

| API | Status | Description |
|-----|--------|-------------|
| Account | ✅ Implemented | Profile, email, preferences, kid mode |
| Users | ✅ Implemented | User profiles, status, leaderboards, rating history |
| Relations | ✅ Implemented | Follow, unfollow, block, unblock users |
| Games | ✅ Implemented | Export, stream, import games (JSON/PGN) |
| Puzzles | ✅ Implemented | Daily puzzle, dashboard, storm, activity |
| Analysis | ✅ Implemented | Cloud evaluations |
| Opening Explorer | ✅ Implemented | Masters, Lichess, and player databases |
| Tablebase | ✅ Implemented | Standard, Atomic, Antichess tablebases |
| TV | ✅ Implemented | TV channels, streams, and channel games |
| Challenges | ✅ Implemented | Create, accept, decline challenges, AI, open challenges |
| Board | ✅ Implemented | Play games from external boards/apps |
| Bot | ✅ Implemented | Bot account management and game play |
| Teams | ✅ Implemented | Team info, search, membership, management |
| Bulk Pairings | 🔜 Planned | Bulk game creation |
| Arena Tournaments | 🔜 Planned | Arena management |
| Swiss Tournaments | 🔜 Planned | Swiss management |
| Simuls | 🔜 Planned | Simultaneous exhibitions |
| Studies | 🔜 Planned | Lichess studies |
| Messaging | 🔜 Planned | Private messages |
| Broadcasts | 🔜 Planned | Event broadcasts |

See [docs/api-coverage.md](docs/api-coverage.md) for detailed endpoint-level coverage.

## Authentication

Most read operations work without authentication. For write operations or accessing private data, you need a [personal access token](https://lichess.org/account/oauth/token).

## Error Handling

LichessSharp provides typed exceptions for different error scenarios:

```csharp
try
{
    var user = await client.Users.GetAsync("nonexistent");
}
catch (LichessNotFoundException ex)
{
    Console.WriteLine("User not found");
}
catch (LichessRateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}");
}
catch (LichessAuthorizationException ex)
{
    Console.WriteLine($"Missing scope: {ex.RequiredScope}");
}
```

## Contributing

Contributions are welcome! Please read the [CLAUDE.md](CLAUDE.md) file for development guidelines.

## License

MIT License - see [LICENSE](LICENSE) for details.

## Links

- [Lichess API Documentation](https://lichess.org/api)
- [Lichess](https://lichess.org)
