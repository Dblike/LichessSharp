# Getting Started with LichessSharp

This guide will help you get started with the LichessSharp library.

## Installation

Install the NuGet package:

```bash
dotnet add package LichessSharp
```

Or via the Package Manager Console:

```powershell
Install-Package LichessSharp
```

## Basic Usage

### Creating a Client

The simplest way to create a client is without authentication:

```csharp
using LichessSharp;

using var client = new LichessClient();
```

For authenticated endpoints, provide an access token:

```csharp
using var client = new LichessClient("lip_your_access_token");
```

### Custom Configuration

For more control, use `LichessClientOptions`:

```csharp
var options = new LichessClientOptions
{
    AccessToken = "your-token",
    DefaultTimeout = TimeSpan.FromSeconds(60),
    AutoRetryOnRateLimit = true,
    MaxRateLimitRetries = 5
};

using var client = new LichessClient(new HttpClient(), options);
```

### Using with Dependency Injection

For ASP.NET Core or other DI-enabled applications:

```csharp
services.AddHttpClient<ILichessClient, LichessClient>((sp, httpClient) =>
{
    var options = sp.GetRequiredService<IOptions<LichessClientOptions>>().Value;
    return new LichessClient(httpClient, options);
});
```

## Accessing APIs

The client exposes different API areas through properties:

```csharp
// Account operations
var profile = await client.Account.GetProfileAsync();

// User operations
var user = await client.Users.GetAsync("DrNykterstein");
var statuses = await client.Users.GetStatusAsync(new[] { "DrNykterstein", "Hikaru" });

// Game operations
var game = await client.Games.GetAsync("q7ZvsdUF");

// Puzzle operations
var dailyPuzzle = await client.Puzzles.GetDailyAsync();
```

## Cancellation

All async methods accept a `CancellationToken`:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

try
{
    var user = await client.Users.GetAsync("DrNykterstein", cancellationToken: cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request was cancelled");
}
```

## Streaming

Many endpoints stream data using `IAsyncEnumerable`:

```csharp
await foreach (var game in client.Games.StreamUserGamesAsync("DrNykterstein"))
{
    Console.WriteLine($"Game: {game.Id}");

    // You can break early
    if (someCondition)
        break;
}
```

With cancellation:

```csharp
using var cts = new CancellationTokenSource();

await foreach (var event in client.Tv.StreamCurrentGameAsync(cts.Token))
{
    Console.WriteLine($"FEN: {event.Fen}");
}
```

## Error Handling

Handle specific error types:

```csharp
try
{
    var user = await client.Users.GetAsync("nonexistent");
}
catch (LichessNotFoundException)
{
    // User doesn't exist
}
catch (LichessRateLimitException ex)
{
    // Rate limited - wait and retry
    await Task.Delay(ex.RetryAfter ?? TimeSpan.FromMinutes(1));
}
catch (LichessAuthenticationException)
{
    // Invalid or expired token
}
catch (LichessAuthorizationException ex)
{
    // Missing required OAuth scope
    Console.WriteLine($"Need scope: {ex.RequiredScope}");
}
catch (LichessException ex)
{
    // Other API errors
    Console.WriteLine($"API error: {ex.LichessError}");
}
```

## Next Steps

- [Authentication Guide](authentication.md)
- [Streaming Guide](streaming.md)
- [API Coverage](api-coverage.md)
