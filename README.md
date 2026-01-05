# LichessSharp

A fully-featured .NET client library for the [Lichess API](https://lichess.org/api).

[![NuGet](https://img.shields.io/nuget/v/LichessSharp.svg)](https://www.nuget.org/packages/LichessSharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Features

- **Complete API coverage** - 23 API areas with 176 endpoints fully implemented
- **Async-first design** - All methods return `Task<T>` with full `CancellationToken` support
- **Real-time streaming** - `IAsyncEnumerable<T>` for NDJSON streams (games, TV, tournaments, events)
- **Strong typing** - Comprehensive models and enums matching Lichess semantics
- **Resilient by default** - Built-in retry logic for rate limits (HTTP 429) and transient network failures
- **DI-friendly** - Works seamlessly with `HttpClientFactory` and dependency injection
- **Modern .NET** - Targets .NET 10.0, uses `System.Text.Json` with AOT preparation

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

**[Full Documentation](../../wiki)** - Complete guides and API reference

- [Getting Started](../../wiki/Getting-Started) - Installation, configuration, basic usage
- [Authentication](../../wiki/Authentication) - OAuth tokens, scopes, security
- [API Reference](../../wiki/API-Reference) - Complete endpoint coverage

## Samples

Run the interactive samples to explore common usage patterns:

```bash
cd samples/LichessSharp.Samples
dotnet run
```

Includes examples for client setup, streaming, Board API, bot development, and error handling.

## Error Handling

```csharp
try
{
    var user = await client.Users.GetAsync("nonexistent");
}
catch (LichessNotFoundException) { /* User not found */ }
catch (LichessRateLimitException ex) { /* Retry after ex.RetryAfter */ }
catch (LichessAuthorizationException ex) { /* Missing scope: ex.RequiredScope */ }
```

## Contributing

Contributions welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

MIT License - see [LICENSE](LICENSE) for details.

## Links

- [Lichess API Documentation](https://lichess.org/api)
- [Lichess](https://lichess.org)
