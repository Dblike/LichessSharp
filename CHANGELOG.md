# Changelog

All notable changes to LichessSharp will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2025-12-19

### Added

- **OpenAPI schema validation testing** — Comprehensive test infrastructure to validate C# models against the Lichess OpenAPI specification
  - `OpenApiSchemaReader` for parsing and resolving OpenAPI schemas
  - `ModelReflector` for extracting JSON property metadata from C# types
  - Automated detection of missing or mismatched `[JsonPropertyName]` attributes

- **Fixture-based serialization tests** — Real API responses captured as test fixtures
  - 35+ JSON fixtures covering Users, Games, Puzzles, Tournaments, Teams, Broadcasts, and more
  - Round-trip serialization tests ensuring data preservation
  - Field coverage tests detecting unmapped JSON properties

- **Model property additions**
  - `GameJson`: Added `Source`, `InitialFen`, `DaysPerTurn`, `Tournament`, `Swiss`, `Division`
  - `GameDivision`: New class for middle game/endgame ply markers
  - `UserExtended`: Added `Playing`, `Streaming`, `Streamer`, `Followable`, `Following`, `Blocking`
  - `User`, `LightUser`: Added `PatronColor`
  - `UserActivity`: Added `Storm`, `Racer`, `Streak`, `Simuls`, `Patron`
  - `ActivityStorm`, `ActivityRacer`, `ActivityStreak`, `ActivitySimul`: New activity types
  - `PuzzleRaceResults`: Added `Puzzles`, `StartsAt`, `FinishesAt`

### Changed

- **BREAKING**: `StreamerInfo.Twitch` and `StreamerInfo.YouTube` changed from `string?` to `StreamChannel?` to match actual API response structure

### Fixed

- `StreamerInfo` deserialization now correctly handles nested Twitch/YouTube channel objects

## [0.1.0] - 2025-12-19

### Added

- **Complete Lichess API coverage** — 23 API areas with 176 endpoints
  - Account, Users, Relations, Games, TV, Puzzles
  - Analysis (Cloud Evaluation), Opening Explorer, Tablebase
  - Challenges, Board API, Bot API
  - Arena Tournaments, Swiss Tournaments, Simuls, Bulk Pairings
  - Studies, Broadcasts, Messaging
  - Teams, FIDE, OAuth, External Engine

- **Streaming support** — Real-time NDJSON streams via `IAsyncEnumerable<T>`
  - Game streams, TV channels, tournament results
  - Board/Bot event streams for real-time play

- **Resilient HTTP client**
  - Automatic retry on rate limits (HTTP 429) with configurable max retries
  - Automatic retry on transient network failures (DNS, connection errors)
  - Exponential backoff with jitter

- **Developer experience**
  - Full `CancellationToken` support on all async methods
  - Typed exceptions (`LichessNotFoundException`, `LichessRateLimitException`, etc.)
  - Comprehensive XML documentation
  - Works with `HttpClientFactory` and dependency injection

- **Interactive samples** — 11 scenario-based examples demonstrating common patterns

### Notes

- Targets .NET 10.0
- Uses `System.Text.Json` with AOT preparation (reflection enabled by default)

[0.2.0]: https://github.com/Dblike/lichess-net/releases/tag/v0.2.0
[0.1.0]: https://github.com/Dblike/lichess-net/releases/tag/v0.1.0
