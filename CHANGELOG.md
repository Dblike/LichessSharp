# Changelog

All notable changes to LichessSharp will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.0] - 2026-01-20

### Added

- **OAuth API enhancements** — Added `GetAuthorizationUrlAsync()` and `GetAuthorizationUrl()` methods to `IOAuthApi` for streamlined PKCE authorization flows

- **New Bot API endpoint** — Added `GetOnlineBotUsersAsync()` to retrieve currently online bot accounts

- **6 new sample projects** demonstrating real-world usage patterns:
  - `LichessSharp.SimpleBot` — Bot API integration with move generation
  - `LichessSharp.PuzzleSolver` — Interactive puzzle solving with the Puzzles API
  - `LichessSharp.GameArchiver` — Export and archive games to PGN files
  - `LichessSharp.TvViewer` — Live TV channel streaming
  - `LichessSharp.UserStats` — User profile and statistics display
  - `LichessSharp.PositionAnalyzer` — Cloud evaluation and opening explorer

- **OAuth types registered for AOT** — Added `OAuthToken`, `OAuthTokenInfo`, and `List<OAuthTokenInfo>` to source-generated JSON context

### Changed

- **BREAKING**: `StreamerInfo.Twitch` and `StreamerInfo.YouTube` reverted from `StreamChannel?` back to `string?` to match current Lichess API response format (API changed since 0.2.0)

- **Studies API** — `ExportUserStudiesAsync()` now requires `order` parameter (defaults to "newest") per Lichess API requirements

### Fixed

- **Studies API 404 errors** — Fixed endpoint path to use correct `/api/study/by/` prefix

- **Games API streaming test** — Fixed `StreamGameMovesAsync` test expectations (first event contains game metadata with `Id`, subsequent events contain `Fen`)

### Documentation

- Consolidated README with wiki documentation
- Updated sample scenarios to favor runnable code over printed examples

## [0.2.1] - 2026-01-19

### Fixed

- **Bot/Board API polymorphic deserialization** — `StreamGameAsync()` now correctly deserializes events to their proper types (`BotGameFullEvent`, `BotGameStateEvent`, `BotChatLineEvent`, `BotOpponentGoneEvent` for Bot API; `GameFullEvent`, `GameStateEvent`, `ChatLineEvent`, `OpponentGoneEvent` for Board API). Previously all events were returned as the base class, losing subclass-specific properties. ([#3](https://github.com/Dblike/LichessSharp/issues/3))

- **OpenAPI schema test path** — Fixed test file path for relocated OpenAPI schema (`docs/openapi/`)

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

[0.3.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.3.0
[0.2.1]: https://github.com/Dblike/LichessSharp/releases/tag/v0.2.1
[0.2.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.2.0
[0.1.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.1.0
