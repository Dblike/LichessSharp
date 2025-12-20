# LichessSharp

A fully-featured .NET client library for the [Lichess API](https://lichess.org/api).

[![NuGet](https://img.shields.io/nuget/v/LichessSharp.svg)](https://www.nuget.org/packages/LichessSharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Features

- **Complete API coverage** — 23 API areas with 176 endpoints fully implemented
- **Async-first design** — All methods return `Task<T>` with full `CancellationToken` support
- **Real-time streaming** — `IAsyncEnumerable<T>` for NDJSON streams (games, TV, tournaments, events)
- **Strong typing** — Comprehensive models and enums matching Lichess semantics
- **Resilient by default** — Built-in retry logic for rate limits (HTTP 429) and transient network failures
- **DI-friendly** — Works seamlessly with `HttpClientFactory` and dependency injection
- **Modern .NET** — Targets .NET 10.0, uses `System.Text.Json` with AOT preparation

### Staying Current

This library tracks the official [Lichess OpenAPI specification](https://lichess.org/api). The spec is stored in [`openapi/lichess.openapi.json`](openapi/lichess.openapi.json) and used to identify coverage gaps and API changes. See [docs/api-coverage.md](docs/api-coverage.md) for detailed endpoint-level status.

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
    // Automatic rate limit retry
    AutoRetryOnRateLimit = true,
    MaxRateLimitRetries = 3,
    // Automatic retry on transient network failures (DNS, connection errors)
    EnableTransientRetry = true,
    MaxTransientRetries = 3,
    TransientRetryBaseDelay = TimeSpan.FromSeconds(1)
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

All 23 Lichess API areas are fully implemented (176 endpoints):

**Core:** Account, Users, Relations, Games, TV, Puzzles
**Analysis:** Cloud Evaluation, Opening Explorer, Tablebase
**Play:** Challenges, Board API, Bot API
**Competition:** Arena Tournaments, Swiss Tournaments, Simuls, Bulk Pairings
**Content:** Studies, Broadcasts, Messaging
**Other:** Teams, FIDE, OAuth, External Engine

See [docs/api-coverage.md](docs/api-coverage.md) for detailed endpoint-level coverage.

## Samples

The [`samples/LichessSharp.Samples`](samples/LichessSharp.Samples) project contains interactive examples demonstrating common usage patterns. Run it to explore:

- Client setup and configuration
- User profiles and leaderboards
- Game export and streaming
- Live TV, puzzles, and tournaments
- Board API and bot development
- Error handling best practices

See the [samples README](samples/LichessSharp.Samples/README.md) for details.

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
