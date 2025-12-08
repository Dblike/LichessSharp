# LichessSharp

A fully-featured .NET client library for the [Lichess API](https://lichess.org/api).

[![NuGet](https://img.shields.io/nuget/v/LichessSharp.svg)](https://www.nuget.org/packages/LichessSharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Features

- Complete coverage of the Lichess API (20+ API surface areas)
- Async-first design with `CancellationToken` support on all methods
- Streaming support via `IAsyncEnumerable<T>` for real-time data
- Strong typing with comprehensive models
- Built-in rate limiting and retry logic
- AOT-compatible with `System.Text.Json` source generators
- Multi-targeting: .NET 10.0

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

// Stream TV feed
await foreach (var event in client.Tv.StreamCurrentGameAsync())
{
    Console.WriteLine($"FEN: {event.Fen}");
}
```

## API Coverage

| API | Status | Description |
|-----|--------|-------------|
| Account | Planned | Profile, email, preferences |
| Users | Planned | User profiles, status, leaderboards |
| Relations | Planned | Follow, block users |
| Games | Planned | Export, stream, import games |
| TV | Planned | TV channels and streams |
| Puzzles | Planned | Daily puzzle, dashboard, storm |
| Teams | Planned | Team management |
| Board | Planned | Physical board API |
| Bot | Planned | Bot account API |
| Challenges | Planned | Send/receive challenges |
| Bulk Pairings | Planned | Bulk game creation |
| Arena Tournaments | Planned | Arena management |
| Swiss Tournaments | Planned | Swiss management |
| Simuls | Planned | Simultaneous exhibitions |
| Studies | Planned | Lichess studies |
| Messaging | Planned | Private messages |
| Broadcasts | Planned | Event broadcasts |
| Analysis | Planned | Cloud evaluations |
| Opening Explorer | Planned | Position lookups |
| Tablebase | Planned | Endgame tablebases |

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
