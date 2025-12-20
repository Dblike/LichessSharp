# API Coverage

This document tracks the implementation status of each Lichess API endpoint.

## Summary

| Status | Count | APIs |
|--------|-------|------|
| **Implemented** | 23 APIs (176 endpoints) | Account, Users, Relations, Games, TV, Puzzles, Analysis, Opening Explorer, Tablebase, Challenges, Board, Bot, Teams, Arena Tournaments, Swiss Tournaments, Simuls, Studies, Broadcasts, Bulk Pairings, Messaging, FIDE, OAuth, External Engine |
| **Planned** | 0 APIs | - |

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
| Get performance statistics | Implemented | `Users.GetPerformanceAsync()` |
| Get user activity | Implemented | `Users.GetActivityAsync()` |
| Autocomplete usernames | Implemented | `Users.AutocompleteAsync()` |
| Autocomplete players (object) | Implemented | `Users.AutocompletePlayersAsync()` |
| Get crosstable | Implemented | `Users.GetCrosstableAsync()` |
| Get live streamers | Implemented | `Users.GetLiveStreamersAsync()` |
| Get note about user | Implemented | `Users.GetNoteAsync()` |
| Write note about user | Implemented | `Users.WriteNoteAsync()` |
| Get timeline | Implemented | `Users.GetTimelineAsync()` |

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
| Export imported games (PGN) | Implemented | `Games.GetImportedGamesPgnAsync()` |
| Export bookmarked games | Implemented | `Games.StreamBookmarkedGamesAsync()` |
| Stream game moves | Implemented | `Games.StreamGameMovesAsync()` |
| Stream games by IDs | Implemented | `Games.StreamGamesByIdsAsync()` |
| Add IDs to stream | Implemented | `Games.AddGameIdsToStreamAsync()` |

## TV API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get current TV games | Implemented | `Tv.GetCurrentGamesAsync()` |
| Stream current TV game | Implemented | `Tv.StreamCurrentGameAsync()` |
| Stream TV channel | Implemented | `Tv.StreamChannelAsync()` |
| Stream channel games | Implemented | `Tv.StreamChannelGamesAsync()` |

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
| Get puzzles batch | Implemented | `Puzzles.GetBatchAsync()` |
| Solve puzzles batch | Implemented | `Puzzles.SolveBatchAsync()` |
| Get puzzles to replay | Implemented | `Puzzles.GetReplayAsync()` |
| Get puzzle race results | Implemented | `Puzzles.GetRaceAsync()` |

## Teams API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get team info | Implemented | `Teams.GetAsync()` |
| Get popular teams | Implemented | `Teams.GetPopularAsync()` |
| Get user's teams | Implemented | `Teams.GetUserTeamsAsync()` |
| Search teams | Implemented | `Teams.SearchAsync()` |
| Get team members (stream) | Implemented | `Teams.StreamMembersAsync()` |
| Join a team | Implemented | `Teams.JoinAsync()` |
| Leave a team | Implemented | `Teams.LeaveAsync()` |
| Get join requests | Implemented | `Teams.GetJoinRequestsAsync()` |
| Accept join request | Implemented | `Teams.AcceptJoinRequestAsync()` |
| Decline join request | Implemented | `Teams.DeclineJoinRequestAsync()` |
| Kick team member | Implemented | `Teams.KickMemberAsync()` |
| Message all members | Implemented | `Teams.MessageAllMembersAsync()` |

## Board API

| Endpoint | Status | Method |
|----------|--------|--------|
| Stream incoming events | Implemented | `Board.StreamEventsAsync()` |
| Stream game state | Implemented | `Board.StreamGameAsync()` |
| Make a move | Implemented | `Board.MakeMoveAsync()` |
| Fetch chat | Implemented | `Board.GetChatAsync()` |
| Write in chat | Implemented | `Board.WriteChatAsync()` |
| Abort game | Implemented | `Board.AbortAsync()` |
| Resign game | Implemented | `Board.ResignAsync()` |
| Handle draw | Implemented | `Board.HandleDrawAsync()` |
| Handle takeback | Implemented | `Board.HandleTakebackAsync()` |
| Claim victory | Implemented | `Board.ClaimVictoryAsync()` |
| Claim draw | Implemented | `Board.ClaimDrawAsync()` |
| Berserk a game | Implemented | `Board.BerserkAsync()` |
| Create a seek | Implemented | `Board.SeekAsync()` |

## Bot API

| Endpoint | Status | Method |
|----------|--------|--------|
| Upgrade to bot | Implemented | `Bot.UpgradeAccountAsync()` |
| Stream incoming events | Implemented | `Bot.StreamEventsAsync()` |
| Stream game state | Implemented | `Bot.StreamGameAsync()` |
| Make a move | Implemented | `Bot.MakeMoveAsync()` |
| Fetch chat | Implemented | `Bot.GetChatAsync()` |
| Write in chat | Implemented | `Bot.WriteChatAsync()` |
| Abort game | Implemented | `Bot.AbortAsync()` |
| Resign game | Implemented | `Bot.ResignAsync()` |
| Handle draw | Implemented | `Bot.HandleDrawAsync()` |
| Handle takeback | Implemented | `Bot.HandleTakebackAsync()` |
| Claim draw | Implemented | `Bot.ClaimDrawAsync()` |
| Get online bots | Implemented | `Bot.GetOnlineBotsAsync()` |

## Challenges API

| Endpoint | Status | Method |
|----------|--------|--------|
| List pending challenges | Implemented | `Challenges.GetPendingAsync()` |
| Show a challenge | Implemented | `Challenges.ShowAsync()` |
| Create a challenge | Implemented | `Challenges.CreateAsync()` |
| Accept a challenge | Implemented | `Challenges.AcceptAsync()` |
| Decline a challenge | Implemented | `Challenges.DeclineAsync()` |
| Cancel a challenge | Implemented | `Challenges.CancelAsync()` |
| Challenge the AI | Implemented | `Challenges.ChallengeAiAsync()` |
| Open-ended challenge | Implemented | `Challenges.CreateOpenAsync()` |
| Start clocks | Implemented | `Challenges.StartClocksAsync()` |
| Add time to opponent's clock | Implemented | `Challenges.AddTimeAsync()` |

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
| Get master game PGN | Implemented | `OpeningExplorer.GetMasterGamePgnAsync()` |

## Tablebase API

| Endpoint | Status | Method |
|----------|--------|--------|
| Standard tablebase | Implemented | `Tablebase.LookupAsync()` |
| Atomic tablebase | Implemented | `Tablebase.LookupAtomicAsync()` |
| Antichess tablebase | Implemented | `Tablebase.LookupAntichessAsync()` |

## FIDE API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get FIDE player | Implemented | `Fide.GetPlayerAsync()` |
| Search FIDE players | Implemented | `Fide.SearchPlayersAsync()` |

## OAuth API

| Endpoint | Status | Method |
|----------|--------|--------|
| Obtain access token | Implemented | `OAuth.GetTokenAsync()` |
| Revoke access token | Implemented | `OAuth.RevokeTokenAsync()` |
| Test multiple tokens | Implemented | `OAuth.TestTokensAsync()` |

## External Engine API (Alpha)

> **WARNING:** This API is in alpha and subject to change.

| Endpoint | Status | Method |
|----------|--------|--------|
| List external engines | Implemented | `ExternalEngine.ListAsync()` |
| Create external engine | Implemented | `ExternalEngine.CreateAsync()` |
| Get external engine | Implemented | `ExternalEngine.GetAsync()` |
| Update external engine | Implemented | `ExternalEngine.UpdateAsync()` |
| Delete external engine | Implemented | `ExternalEngine.DeleteAsync()` |
| Analyse with engine | Implemented | `ExternalEngine.AnalyseAsync()` |
| Acquire analysis work | Implemented | `ExternalEngine.AcquireWorkAsync()` |
| Submit analysis work | Implemented | `ExternalEngine.SubmitWorkAsync()` |

## Bulk Pairings API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get all bulk pairings | Implemented | `BulkPairings.GetAllAsync()` |
| Get bulk pairing | Implemented | `BulkPairings.GetAsync()` |
| Create bulk pairing | Implemented | `BulkPairings.CreateAsync()` |
| Start clocks | Implemented | `BulkPairings.StartClocksAsync()` |
| Cancel bulk pairing | Implemented | `BulkPairings.CancelAsync()` |
| Export games (PGN) | Implemented | `BulkPairings.ExportGamesPgnAsync()` |
| Stream games (NDJSON) | Implemented | `BulkPairings.StreamGamesAsync()` |

## Arena Tournaments API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get current tournaments | Implemented | `ArenaTournaments.GetCurrentAsync()` |
| Get tournament | Implemented | `ArenaTournaments.GetAsync()` |
| Create tournament | Implemented | `ArenaTournaments.CreateAsync()` |
| Update tournament | Implemented | `ArenaTournaments.UpdateAsync()` |
| Join tournament | Implemented | `ArenaTournaments.JoinAsync()` |
| Withdraw from tournament | Implemented | `ArenaTournaments.WithdrawAsync()` |
| Terminate tournament | Implemented | `ArenaTournaments.TerminateAsync()` |
| Update team battle | Implemented | `ArenaTournaments.UpdateTeamBattleAsync()` |
| Stream tournament games | Implemented | `ArenaTournaments.StreamGamesAsync()` |
| Stream tournament results | Implemented | `ArenaTournaments.StreamResultsAsync()` |
| Get team standing | Implemented | `ArenaTournaments.GetTeamStandingAsync()` |
| Stream created by user | Implemented | `ArenaTournaments.StreamCreatedByAsync()` |
| Stream played by user | Implemented | `ArenaTournaments.StreamPlayedByAsync()` |
| Stream team tournaments | Implemented | `ArenaTournaments.StreamTeamTournamentsAsync()` |

## Swiss Tournaments API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get Swiss tournament | Implemented | `SwissTournaments.GetAsync()` |
| Create Swiss tournament | Implemented | `SwissTournaments.CreateAsync()` |
| Update Swiss tournament | Implemented | `SwissTournaments.UpdateAsync()` |
| Schedule next round | Implemented | `SwissTournaments.ScheduleNextRoundAsync()` |
| Join Swiss tournament | Implemented | `SwissTournaments.JoinAsync()` |
| Withdraw from Swiss | Implemented | `SwissTournaments.WithdrawAsync()` |
| Terminate Swiss | Implemented | `SwissTournaments.TerminateAsync()` |
| Export TRF | Implemented | `SwissTournaments.ExportTrfAsync()` |
| Stream Swiss games | Implemented | `SwissTournaments.StreamGamesAsync()` |
| Stream Swiss results | Implemented | `SwissTournaments.StreamResultsAsync()` |
| Stream team tournaments | Implemented | `SwissTournaments.StreamTeamTournamentsAsync()` |

## Simuls API

| Endpoint | Status | Method |
|----------|--------|--------|
| Get current simuls | Implemented | `Simuls.GetCurrentAsync()` |

## Studies API

| Endpoint | Status | Method |
|----------|--------|--------|
| Export one chapter (PGN) | Implemented | `Studies.ExportChapterPgnAsync()` |
| Export all chapters (PGN) | Implemented | `Studies.ExportStudyPgnAsync()` |
| Export user's studies (PGN) | Implemented | `Studies.ExportUserStudiesPgnAsync()` |
| Stream user's study metadata | Implemented | `Studies.StreamUserStudiesAsync()` |
| Import PGN to study | Implemented | `Studies.ImportPgnAsync()` |
| Update chapter tags | Implemented | `Studies.UpdateChapterTagsAsync()` |
| Delete study chapter | Implemented | `Studies.DeleteChapterAsync()` |

## Messaging API

| Endpoint | Status | Method |
|----------|--------|--------|
| Send private message | Implemented | `Messaging.SendAsync()` |

## Broadcasts API

| Endpoint | Status | Method |
|----------|--------|--------|
| Stream official broadcasts | Implemented | `Broadcasts.StreamOfficialBroadcastsAsync()` |
| Get top broadcasts | Implemented | `Broadcasts.GetTopBroadcastsAsync()` |
| Stream user broadcasts | Implemented | `Broadcasts.StreamUserBroadcastsAsync()` |
| Search broadcasts | Implemented | `Broadcasts.SearchBroadcastsAsync()` |
| Get tournament | Implemented | `Broadcasts.GetTournamentAsync()` |
| Get round | Implemented | `Broadcasts.GetRoundAsync()` |
| Stream my rounds | Implemented | `Broadcasts.StreamMyRoundsAsync()` |
| Create tournament | Implemented | `Broadcasts.CreateTournamentAsync()` |
| Update tournament | Implemented | `Broadcasts.UpdateTournamentAsync()` |
| Create round | Implemented | `Broadcasts.CreateRoundAsync()` |
| Update round | Implemented | `Broadcasts.UpdateRoundAsync()` |
| Reset round | Implemented | `Broadcasts.ResetRoundAsync()` |
| Push PGN | Implemented | `Broadcasts.PushPgnAsync()` |
| Export round PGN | Implemented | `Broadcasts.ExportRoundPgnAsync()` |
| Export all rounds PGN | Implemented | `Broadcasts.ExportAllRoundsPgnAsync()` |
| Stream round PGN | Implemented | `Broadcasts.StreamRoundPgnAsync()` |
| Get players | Implemented | `Broadcasts.GetPlayersAsync()` |
| Get player | Implemented | `Broadcasts.GetPlayerAsync()` |

