# API Coverage

This document tracks the implementation status of each Lichess API endpoint.

## Summary

| Status | Count | APIs |
|--------|-------|------|
| **Implemented** | 8 APIs (38 endpoints) | Account, Users, Relations, Games, Puzzles, Analysis, Opening Explorer, Tablebase |
| **Planned** | 11 APIs | TV, Teams, Board, Bot, Challenges, Bulk Pairings, Arena, Swiss, Simuls, Studies, Messaging, Broadcasts |

## Legend

- Implemented - Endpoint is fully implemented and tested
- Partial - Some functionality implemented
- Planned - Not yet implemented

## Account API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get my profile | Implemented | `Account.GetProfileAsync()` |
| Get my email | Implemented | `Account.GetEmailAsync()` |
| Get my preferences | Implemented | `Account.GetPreferencesAsync()` |
| Get kid mode status | Implemented | `Account.GetKidModeAsync()` |
| Set kid mode status | Implemented | `Account.SetKidModeAsync()` |

## Users API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get user public data | Implemented | `Users.GetAsync()` |
| Get users by ID | Implemented | `Users.GetManyAsync()` |
| Get real-time user status | Implemented | `Users.GetStatusAsync()` |
| Get all top 10 | Implemented | `Users.GetAllTop10Async()` |
| Get one leaderboard | Implemented | `Users.GetLeaderboardAsync()` |
| Get rating history | Implemented | `Users.GetRatingHistoryAsync()` |

## Relations API

| Endpoint | Status | Method |
|----------|--------|--------|
| Follow a player | Implemented | `Relations.FollowAsync()` |
| Unfollow a player | Implemented | `Relations.UnfollowAsync()` |
| Block a player | Implemented | `Relations.BlockAsync()` |
| Unblock a player | Implemented | `Relations.UnblockAsync()` |
| Stream following | Implemented | `Relations.StreamFollowingAsync()` |

## Games API

| Endpoint | Status | Method |
|----------|--------|--------|
| Export one game | Implemented | `Games.GetAsync()` |
| Export one game (PGN) | Implemented | `Games.GetPgnAsync()` |
| Export ongoing game | Implemented | `Games.GetCurrentGameAsync()` |
| Export games of a user | Implemented | `Games.StreamUserGamesAsync()` |
| Export games by IDs | Implemented | `Games.StreamByIdsAsync()` |
| Stream games by users | Implemented | `Games.StreamGamesByUsersAsync()` |
| Get ongoing games | Implemented | `Games.GetOngoingGamesAsync()` |
| Import one PGN game | Implemented | `Games.ImportPgnAsync()` |

## TV API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get current TV games | Planned | `Tv.GetCurrentGamesAsync()` |
| Stream current TV game | Planned | `Tv.StreamCurrentGameAsync()` |
| Stream TV channel | Planned | `Tv.StreamChannelAsync()` |
| Get channel games | Planned | `Tv.GetChannelGamesAsync()` |

## Puzzles API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get daily puzzle | Implemented | `Puzzles.GetDailyAsync()` |
| Get puzzle by ID | Implemented | `Puzzles.GetAsync()` |
| Get next puzzle | Implemented | `Puzzles.GetNextAsync()` |
| Get puzzle activity | Implemented | `Puzzles.StreamActivityAsync()` |
| Get puzzle dashboard | Implemented | `Puzzles.GetDashboardAsync()` |
| Get storm dashboard | Implemented | `Puzzles.GetStormDashboardAsync()` |
| Create puzzle race | Implemented | `Puzzles.CreateRaceAsync()` |

## Teams API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get team info | Planned | `Teams.GetAsync()` |
| Get user's teams | Planned | `Teams.GetUserTeamsAsync()` |
| Get team members | Planned | `Teams.StreamMembersAsync()` |
| Join a team | Planned | `Teams.JoinAsync()` |
| Leave a team | Planned | `Teams.LeaveAsync()` |
| Search teams | Planned | `Teams.SearchAsync()` |

## Board API

| Endpoint | Status | Method |
|----------|--------|--------|
| Stream events | Planned | `Board.StreamEventsAsync()` |
| Stream game state | Planned | `Board.StreamGameAsync()` |
| Make a move | Planned | `Board.MakeMoveAsync()` |
| Write in chat | Planned | `Board.WriteChatAsync()` |
| Abort game | Planned | `Board.AbortAsync()` |
| Resign game | Planned | `Board.ResignAsync()` |
| Handle draw | Planned | `Board.HandleDrawOfferAsync()` |
| Handle takeback | Planned | `Board.HandleTakebackAsync()` |
| Claim victory | Planned | `Board.ClaimVictoryAsync()` |
| Seek game | Planned | `Board.SeekAsync()` |

## Bot API

| Endpoint | Status | Method |
|----------|--------|--------|
| Upgrade to bot | Planned | `Bot.UpgradeAccountAsync()` |
| Stream events | Planned | `Bot.StreamEventsAsync()` |
| Stream game state | Planned | `Bot.StreamGameAsync()` |
| Make a move | Planned | `Bot.MakeMoveAsync()` |
| Write in chat | Planned | `Bot.WriteChatAsync()` |
| Abort game | Planned | `Bot.AbortAsync()` |
| Resign game | Planned | `Bot.ResignAsync()` |
| Get online bots | Planned | `Bot.GetOnlineBotsAsync()` |

## Challenges API

| Endpoint | Status | Method |
|----------|--------|--------|
| List challenges | Planned | `Challenges.GetPendingAsync()` |
| Create challenge | Planned | `Challenges.CreateAsync()` |
| Accept challenge | Planned | `Challenges.AcceptAsync()` |
| Decline challenge | Planned | `Challenges.DeclineAsync()` |
| Cancel challenge | Planned | `Challenges.CancelAsync()` |
| Challenge AI | Planned | `Challenges.ChallengeAiAsync()` |
| Open challenge | Planned | `Challenges.CreateOpenAsync()` |

## Analysis API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get cloud evaluation | Implemented | `Analysis.GetCloudEvaluationAsync()` |

## Opening Explorer API

| Endpoint | Status | Method |
|----------|--------|--------|
| Masters database | Implemented | `OpeningExplorer.GetMastersAsync()` |
| Lichess database | Implemented | `OpeningExplorer.GetLichessAsync()` |
| Player database | Implemented | `OpeningExplorer.GetPlayerAsync()` |

## Tablebase API

| Endpoint | Status | Method |
|----------|--------|--------|
| Standard tablebase | Implemented | `Tablebase.LookupAsync()` |
| Atomic tablebase | Implemented | `Tablebase.LookupAtomicAsync()` |
| Antichess tablebase | Implemented | `Tablebase.LookupAntichessAsync()` |

## Bulk Pairings API

| Endpoint | Status | Method |
|----------|--------|--------|
| Create bulk pairing | Planned | `BulkPairings.CreateAsync()` |
| Get bulk pairing | Planned | `BulkPairings.GetAsync()` |
| Start clocks | Planned | `BulkPairings.StartClocksAsync()` |
| Cancel bulk pairing | Planned | `BulkPairings.CancelAsync()` |

## Arena Tournaments API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get current tournaments | Planned | `ArenaTournaments.GetCurrentAsync()` |
| Get tournament | Planned | `ArenaTournaments.GetAsync()` |
| Create tournament | Planned | `ArenaTournaments.CreateAsync()` |
| Update tournament | Planned | `ArenaTournaments.UpdateAsync()` |
| Join tournament | Planned | `ArenaTournaments.JoinAsync()` |
| Withdraw from tournament | Planned | `ArenaTournaments.WithdrawAsync()` |
| Terminate tournament | Planned | `ArenaTournaments.TerminateAsync()` |
| Get standings | Planned | `ArenaTournaments.GetStandingsAsync()` |
| Stream tournament games | Planned | `ArenaTournaments.StreamGamesAsync()` |
| Get team battle results | Planned | `ArenaTournaments.GetTeamBattleResultsAsync()` |

## Swiss Tournaments API

| Endpoint | Status | Method |
|----------|--------|--------|
| Create Swiss tournament | Planned | `SwissTournaments.CreateAsync()` |
| Get Swiss tournament | Planned | `SwissTournaments.GetAsync()` |
| Update Swiss tournament | Planned | `SwissTournaments.UpdateAsync()` |
| Join Swiss tournament | Planned | `SwissTournaments.JoinAsync()` |
| Withdraw from Swiss | Planned | `SwissTournaments.WithdrawAsync()` |
| Terminate Swiss | Planned | `SwissTournaments.TerminateAsync()` |
| Schedule next round | Planned | `SwissTournaments.ScheduleNextRoundAsync()` |
| Stream Swiss games | Planned | `SwissTournaments.StreamGamesAsync()` |
| Get Swiss results | Planned | `SwissTournaments.GetResultsAsync()` |

## Simuls API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get simul | Planned | `Simuls.GetAsync()` |
| Get current simuls | Planned | `Simuls.GetCurrentAsync()` |

## Studies API

| Endpoint | Status | Method |
|----------|--------|--------|
| Export study chapters | Planned | `Studies.ExportChaptersAsync()` |
| Export study chapter | Planned | `Studies.ExportChapterAsync()` |
| Export all user studies | Planned | `Studies.ExportUserStudiesAsync()` |
| List study metadata | Planned | `Studies.ListMetadataAsync()` |
| Delete study chapter | Planned | `Studies.DeleteChapterAsync()` |

## Messaging API

| Endpoint | Status | Method |
|----------|--------|--------|
| Send private message | Planned | `Messaging.SendAsync()` |

## Broadcasts API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get official broadcasts | Planned | `Broadcasts.GetOfficialAsync()` |
| Create broadcast | Planned | `Broadcasts.CreateAsync()` |
| Get broadcast | Planned | `Broadcasts.GetAsync()` |
| Update broadcast | Planned | `Broadcasts.UpdateAsync()` |
| Create round | Planned | `Broadcasts.CreateRoundAsync()` |
| Get round | Planned | `Broadcasts.GetRoundAsync()` |
| Update round | Planned | `Broadcasts.UpdateRoundAsync()` |
| Push PGN | Planned | `Broadcasts.PushPgnAsync()` |
| Stream round | Planned | `Broadcasts.StreamRoundAsync()` |

---

## Implementation Roadmap

This section outlines the recommended order for implementing remaining APIs based on user value and complexity.

### Phase 1: Real-time Viewing (High Value, Low Complexity)

**TV API** - 4 endpoints
- Enables watching live games
- Read-only, no authentication required
- Streaming patterns already established in Games API

### Phase 2: Game Play (High Value, Medium Complexity)

**Challenges API** - 9 endpoints
- Core functionality for initiating games
- Required for Board/Bot APIs
- Mix of authenticated read/write operations

**Board API** - 10 endpoints
- Physical board and third-party client support
- Long-lived streaming connections
- Real-time move submission

**Bot API** - 8 endpoints
- Engine-assisted play
- Similar patterns to Board API
- Requires bot account upgrade

### Phase 3: Social & Competition (Medium Value, Medium Complexity)

**Teams API** - 6+ endpoints
- Team management and membership
- Pagination patterns
- Mix of read/write operations

**Arena Tournaments API** - 10+ endpoints
- Tournament creation and management
- Complex state management
- Streaming standings

**Swiss Tournaments API** - 9+ endpoints
- Similar to Arena but different format
- Round scheduling

### Phase 4: Content & Broadcasting (Medium Value, Higher Complexity)

**Studies API** - 5+ endpoints
- Study and chapter management
- PGN import/export

**Broadcasts API** - 9+ endpoints
- Live event broadcasting
- PGN push functionality
- Multi-round management

**Simuls API** - 2 endpoints
- Simultaneous exhibitions
- Read-only operations

### Phase 5: Utility & Admin (Lower Priority)

**Bulk Pairings API** - 4 endpoints
- Tournament organizer functionality
- Admin-level operations

**Messaging API** - 1 endpoint
- Private messaging
- Simple implementation

### Implementation Notes

Each phase should include:
1. Interface definition (`I*Api.cs`)
2. Implementation (`*Api.cs`)
3. Models and DTOs
4. Unit tests with mocked HTTP client
5. Integration tests (optional, separate project)
6. Documentation updates
