# Changelog

All notable changes to LichessSharp will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.5.1] - 2026-03-23

### Added

- **Spectator game chat endpoint** — New `GetSpectatorChatAsync()` on `IGamesApi` for fetching public spectator chat messages (`GET /game/{gameId}/chat`, no auth required)
- **`BroadcastPlayerGame.FideTimeControl` deserialization tests** — Validates required field and all accepted values (standard, rapid, blitz)

### Changed

- **OpenAPI spec updated to v2.0.130** (from v2.0.129)
- **Board/Bot `GetChatAsync` documentation** — Clarified as "player chat" (private between 2 players) to distinguish from the new spectator chat endpoint

## [0.5.0] - 2026-03-13

### Added

- **Create Study endpoint** — New `CreateStudyAsync()` on `IStudiesApi` for creating studies via `POST /api/study` (requires `study:write` scope)

### Changed

- **OpenAPI spec updated to v2.0.129** (from v2.0.125)
- **Opening Explorer now requires authentication** — All `IOpeningExplorerApi` endpoints require a valid access token following Lichess infrastructure changes
- **Explorer domain migrated** — `explorer.lichess.ovh` → `explorer.lichess.org`
- **Tablebase domain migrated** — `tablebase.lichess.ovh` → `tablebase.lichess.org`
- **`BroadcastGameEntry.FideTimeControl` is now required** — Property changed from `string?` to `required string` per upstream schema change

### Fixed

- **Masters PGN endpoint path** — Fixed `/master/pgn/{gameId}` → `/masters/pgn/{gameId}` to match upstream rename

## [0.4.1] - 2026-03-03

### Added

- **Puzzle activity `since` parameter** — Added `DateTimeOffset? since` parameter to `StreamActivityAsync()` for filtering puzzle activity from a given timestamp (Lichess API v2.0.125)

### Changed

- **OpenAPI spec updated to v2.0.125** (from v2.0.123)

## [0.4.0] - 2026-02-23

### Added

- **Lichess API spec update pipeline** — Automated scripts for fetching, archiving, and diffing OpenAPI spec updates (`check-api-version.ps1`, `diff-openapi-specs.ps1`, `update-openapi-spec.ps1`)
- **`/update-api` Claude command** — Orchestrates the full spec update workflow including version check, fetch, diff, coverage analysis, and implementation guidance
- **GitHub Actions version monitoring** — Weekly cron workflow (`check-api-version.yml`) that creates issues when a new Lichess API version is available
- **FIDE player rating history** — New `GetPlayerRatingsAsync()` endpoint on `IFideApi` returning standard/rapid/blitz rating history
- **Broadcast team standings** — New `GetTeamStandingsAsync()` endpoint on `IBroadcastsApi` returning team match and player data
- **PGN export options** — Added `clocks` and `comments` parameters to `ExportRoundPgnAsync()`, `ExportAllRoundsPgnAsync()`, and `StreamRoundPgnAsync()`
- **Users API FIDE ID option** — Added `FideId` option to `GetUserOptions` for including FIDE IDs in user profiles
- **Lichess API version badge** — README now displays the tracked Lichess API version

### Changed

- **OpenAPI spec updated to v2.0.123** (from v2.0.112) — 11 versions of upstream changes incorporated
- **`GetUserTeamsAsync` now requires OAuth** — Matches upstream API change (since v2.0.123). Teams that hide their player list are only included if the caller also belongs to the team.

### Fixed

- **`GetUserTeamsAsync` integration test** — Updated to expect authentication requirement matching the current API behavior

## [0.3.1] - 2026-01-20

### Fixed

- **UserExtended.Streamer type** — Changed `UserExtended.Streamer` from `StreamerInfo?` to new `UserStreamer?` type. The Lichess API uses different schemas: `StreamerInfo` (string URLs) for `/api/streamer/live`, and `UserStreamer` (nested `StreamChannel` objects) for user profiles. This fixes deserialization of `UserExtended` responses.

### Added

- **UserStreamer and StreamChannel types** — New types to correctly model the nested streamer structure in user profiles per OpenAPI spec

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

[0.5.1]: https://github.com/Dblike/LichessSharp/releases/tag/v0.5.1
[0.5.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.5.0
[0.4.1]: https://github.com/Dblike/LichessSharp/releases/tag/v0.4.1
[0.4.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.4.0
[0.3.1]: https://github.com/Dblike/LichessSharp/releases/tag/v0.3.1
[0.3.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.3.0
[0.2.1]: https://github.com/Dblike/LichessSharp/releases/tag/v0.2.1
[0.2.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.2.0
[0.1.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.1.0
