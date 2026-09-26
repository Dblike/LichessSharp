# Changelog

All notable changes to LichessSharp will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-09-26

Catches the library up with Lichess API v2.0.174 (from v2.0.130, via the previously unreleased v2.0.152 work). Major version because several response types changed shape.

### Added

- **OpenAPI spec updated to v2.0.174** (from v2.0.152; snapshots and diff reports in `docs/openapi/`)
- **`ITeamsApi.GetUpdatesAsync` / `ITeamsApi.GetTeamUpdatesAsync`** — new methods for `GET /team/updates` and `GET /team/updates/{teamId}` (require `team:read`): paginated updates posted by the leaders of teams you have joined, plus a per-team unread summary. New models `TeamUpdates`, `TeamUpdatesOfTeam`, `TeamUpdatesPager`, `TeamUpdate`, `TeamUpdateMessage`, `TeamUpdatesByTeamEntry` (namespace `LichessSharp.Models.Teams`) and `LightTeam` (`LichessSharp.Models.Common`).
- **`IGamesApi.BookmarkAsync`** — new method for `POST /bookmark/{gameId}` (requires `preference:write`): toggles a game bookmark, or sets it explicitly via the optional `bookmarked` parameter.
- **`IBroadcastsApi.StreamTourPgnAsync`** — new method for `GET /api/stream/broadcast/tour/{broadcastTourId}.pgn`, streaming all ongoing rounds of a broadcast tournament as PGN (verified live: 200 `application/x-chess-pgn` for a real tournament, 404 for an unknown id). Same single-PGN streaming limitation as `StreamRoundPgnAsync`.
- **`GamePlayer.Berserk`** — whether the player berserked (Arena games only), new in v2.0.174.
- **`StudyImportResult.Error`** — error message returned by `ImportPgnAsync` when some games could not be imported, new in v2.0.174.
- **OpenAPI spec updated to v2.0.152** (from v2.0.130)
- **`GameJson.ArenaTour` / `GameJson.SwissTour`** — new tournament-reference objects (`GameTournamentRef`, with `Id` and, for arenas, `Name`). The Lichess API now returns these objects for tournament games instead of the former `tournament`/`swiss` ID strings.
- **`BroadcastWithFullGroup` and `BroadcastPhoto` models** — the get-tournament endpoint now returns full group details and player photos (keyed by FIDE ID).
- **`StreamOfficialBroadcastsAsync` `live` filter** — new optional `live` parameter restricting results to broadcasts with a currently-ongoing round.
- **`FidePlayer.Gender`** — FIDE-recorded binary gender ("M"/"F").
- **`BroadcastTour.ShowTeamScores`**, **`BroadcastTourInfo.Regulations`** (official regulations URL), and **`BroadcastPlayerGame.Ongoing`** — new response fields added in v2.0.152.
- **`ImplementedEndpoints.IntentionallyNotImplemented`** — records endpoints deliberately excluded from the library, with rationale. Seeded with `POST /api/token/admin-challenge` (a Lichess-admin-only endpoint, not usable by regular API consumers — will not be implemented). Coverage tooling now reports these separately from genuine gaps.
- **`IStudiesApi.UpdateChapterMovesAsync`** — new method for `POST /api/study/{studyId}/{chapterId}/moves`, replacing a study chapter's moves from PGN (requires `study:write` scope).
- **`IBroadcastsApi.StreamGroupPgnAsync`** — new method for `GET /api/stream/broadcast/group/{broadcastGroupId}.pgn`, streaming a broadcast group's ongoing rounds as PGN.

### Changed

- **BREAKING: `BroadcastRoundInfo.CreatedAt` is now `long?` and `BroadcastRoundInfo.Rated` is now `bool?`** (were `long` / `bool`). Both became optional in the spec in v2.0.174; a missing value now reads as `null` instead of `0` / `false`.
- **`ITeamsApi.MessageAllMembersAsync`** — `POST /team/{teamId}/pm-all` is now documented by Lichess as "Send a team update" (requires the leader "Updates" permission); the messages it posts are what `GetUpdatesAsync` returns. Signature and behavior unchanged.
- **`GamePlayer`** — documented as covering both spec shapes for a side: `GamePlayerUser` (human) and the new `GamePlayerAi` (`aiLevel` + `analysis`, no `user`/`rating`).
- **Microsoft.SourceLink.GitHub 10.0.102 → 10.0.401** (build-time only; drops the transitive Microsoft.Build.Tasks.Git 10.0.102 flagged by NU1902).
- **BREAKING: `IBroadcastsApi.GetTournamentAsync` now returns `BroadcastWithFullGroup`** (was `BroadcastWithRounds`). As of Lichess API v2.0.152 this endpoint's response includes the full `BroadcastGroup` object and player photos, so it maps to a distinct type. The common `Tour` and `Rounds` members are unchanged.
- **BREAKING: `BroadcastWithRounds.Group` is now `string?`** (was `BroadcastGroup?`). The streaming (`StreamOfficialBroadcastsAsync`) and create endpoints now return the group as a plain name string.
- **BREAKING: `IBroadcastsApi.UpdateTournamentAsync` now returns `Task<bool>`** (was `Task<BroadcastWithRounds>`). The `POST /broadcast/{id}/edit` endpoint returns only an acknowledgement (`{"ok":true}`), not the tournament — the previous return type would fail to deserialize. Read back with `GetTournamentAsync` if you need the updated tournament.
- **`BroadcastTourInfo.FideTimeControl`** JSON mapping aligned to the spec's `fideTC` casing (was `fideTc`; behavior unchanged as matching is case-insensitive).

### Deprecated

- **`IAccountApi.GetTimelineAsync`** — `GET /api/timeline` was removed from the Lichess API in v2.0.174 (upstream: unused, and no OAuth scope grants access to it, so calls with an access token fail with 401). Marked `[Obsolete]` and dropped from `ImplementedEndpoints`; will be removed in a future release together with the `Timeline` models.
- **`UserPreferences.Dark` and `UserPreferences.TransparentBackground`** — removed from the Lichess API in v2.0.174 and always `null` now. Marked `[Obsolete]`.
- **`BroadcastRoundInfo.Delay`** — removed from the round response in v2.0.174 and always `null` now. Marked `[Obsolete]`. (The `delay` option on round create/update forms is unaffected.)
- **`GameJson.Tournament` and `GameJson.Swiss`** — removed from the Lichess API in v2.0.152 and always `null` now. Use `ArenaTour.Id` / `SwissTour.Id` instead. Marked `[Obsolete]`; will be removed in a future release.

### Fixed

- **`GetSpectatorChatAsync` was non-functional and now works.** It targeted `GET /game/{gameId}/chat`, which returns 404 (no redirect); the endpoint is `GET /api/game/{gameId}/chat`. It also deserializes the actual `{ "lines": [...] }` response object (the OpenAPI spec declared a bare array until v2.0.174, which now matches the real response). The public signature (`Task<IReadOnlyList<ChatMessage>>`) is unchanged.

## [1.0.0] - 2026-03-23

### Added

- **`GameSource` enum** — Replaces stringly-typed `Source` properties on `GameJson`, `MoveStreamEvent`, and `OngoingGame` with a strongly-typed enum (13 values: Lobby, Friend, Ai, Api, Tournament, Position, Import, ImportLive, Simul, Relay, Pool, Arena, Swiss)
- **`JudgmentName` enum** — Replaces `Judgment.Name` string with Inaccuracy, Mistake, Blunder enum values
- **`StreamingService` enum** — Replaces `StreamInfo.Service` string with Twitch, YouTube enum values
- **Rate-limit-aware integration test framework** — `LichessTestFixture` with 1.5s request throttling and sequential test execution via xUnit `[Collection]`

### Changed

- **BREAKING: Stringly-typed response properties replaced with enums** — All `Title`, `Color`, and `Source` properties on response models now use strongly-typed enums instead of `string?`. This affects 30+ properties across all API contracts. Consumers must update string comparisons (e.g., `game.Source == "pool"` → `game.Source == GameSource.Pool`, `player.Title == "GM"` → `player.Title == Title.GM`)
- **`ChallengeJson.Color` now uses `ChallengeColor` enum** — Supports `Random`, `White`, and `Black` values (previously `string?`)
- **`ChallengeColor` enum now has `[JsonConverter]`** — Enables lowercase JSON deserialization
- **Integration tests run sequentially** — All tests share `LichessTestFixture` via `[Collection("Lichess API")]`, eliminating parallel rate limit collisions
- **Integration test timeout reduced** — From 10 minutes to 60 seconds per test (throttling prevents rate limits)

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

[2.0.0]: https://github.com/Dblike/LichessSharp/releases/tag/v2.0.0
[1.0.0]: https://github.com/Dblike/LichessSharp/releases/tag/v1.0.0
[0.5.1]: https://github.com/Dblike/LichessSharp/releases/tag/v0.5.1
[0.5.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.5.0
[0.4.1]: https://github.com/Dblike/LichessSharp/releases/tag/v0.4.1
[0.4.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.4.0
[0.3.1]: https://github.com/Dblike/LichessSharp/releases/tag/v0.3.1
[0.3.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.3.0
[0.2.1]: https://github.com/Dblike/LichessSharp/releases/tag/v0.2.1
[0.2.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.2.0
[0.1.0]: https://github.com/Dblike/LichessSharp/releases/tag/v0.1.0
