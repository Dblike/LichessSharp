# API Coverage

This document tracks the implementation status of each Lichess API endpoint.

## Legend

- Implemented - Endpoint is fully implemented
- Partial - Some functionality implemented
- Planned - Not yet implemented

## Account API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get my profile | Planned | `Account.GetProfileAsync()` |
| Get my email | Planned | `Account.GetEmailAsync()` |
| Get my preferences | Planned | `Account.GetPreferencesAsync()` |
| Get kid mode status | Planned | `Account.GetKidModeAsync()` |
| Set kid mode status | Planned | `Account.SetKidModeAsync()` |

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
| Follow a player | Planned | `Relations.FollowAsync()` |
| Unfollow a player | Planned | `Relations.UnfollowAsync()` |
| Block a player | Planned | `Relations.BlockAsync()` |
| Unblock a player | Planned | `Relations.UnblockAsync()` |

## Games API

| Endpoint | Status | Method |
|----------|--------|--------|
| Export one game | Planned | `Games.GetAsync()` |
| Export one game (PGN) | Planned | `Games.GetPgnAsync()` |
| Export ongoing game | Planned | `Games.GetCurrentGameAsync()` |
| Export games of a user | Planned | `Games.StreamUserGamesAsync()` |
| Export games by IDs | Planned | `Games.StreamByIdsAsync()` |
| Stream games by users | Planned | `Games.StreamGamesByUsersAsync()` |
| Get ongoing games | Planned | `Games.GetOngoingGamesAsync()` |
| Import one PGN game | Planned | `Games.ImportPgnAsync()` |

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
| Get daily puzzle | Planned | `Puzzles.GetDailyAsync()` |
| Get puzzle by ID | Planned | `Puzzles.GetAsync()` |
| Get next puzzle | Planned | `Puzzles.GetNextAsync()` |
| Get puzzle activity | Planned | `Puzzles.StreamActivityAsync()` |
| Get puzzle dashboard | Planned | `Puzzles.GetDashboardAsync()` |
| Get storm dashboard | Planned | `Puzzles.GetStormDashboardAsync()` |
| Create puzzle race | Planned | `Puzzles.CreateRaceAsync()` |

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
| Standard tablebase | Planned | `Tablebase.LookupAsync()` |
| Atomic tablebase | Planned | `Tablebase.LookupAtomicAsync()` |
| Antichess tablebase | Planned | `Tablebase.LookupAntichessAsync()` |
