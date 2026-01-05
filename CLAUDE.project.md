## Project Overview

Full-featured .NET Lichess API wrapper implementing the official HTTP & streaming APIs.

## Goals

- Feature parity with mature Python/JavaScript clients
- Minimize third-party dependencies
- Support .NET 10 (current target)

## API Reference

- OpenAPI spec: `openapi/lichess.openapi.json`
- Base URL: `https://lichess.org/api`

## Key Patterns

- All network operations are async with `CancellationToken`
- Streaming endpoints use `IAsyncEnumerable<T>`
- Rate limit handling built into transport layer

## Commands

```bash
dotnet build LichessSharp.sln
dotnet test LichessSharp.sln
```
