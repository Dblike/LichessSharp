namespace LichessSharp.Coverage;

/// <summary>
/// Metadata about implemented Lichess API endpoints.
/// Used for coverage tracking and comparison with OpenAPI spec.
/// This file should be updated when new endpoints are implemented.
/// </summary>
public static class ImplementedEndpoints
{
    /// <summary>
    /// All implemented endpoints in the library.
    /// </summary>
    public static readonly EndpointInfo[] All =
    [
        // ===== Account API =====
        new("GET", "/api/account", "Account", "GetProfileAsync"),
        new("GET", "/api/account/email", "Account", "GetEmailAsync"),
        new("GET", "/api/account/preferences", "Account", "GetPreferencesAsync"),
        new("GET", "/api/account/kid", "Account", "GetKidModeAsync"),
        new("POST", "/api/account/kid", "Account", "SetKidModeAsync"),
        new("GET", "/api/timeline", "Account", "GetTimelineAsync"),

        // ===== Users API =====
        new("GET", "/api/user/{username}", "Users", "GetAsync"),
        new("POST", "/api/users", "Users", "GetManyAsync"),
        new("GET", "/api/users/status", "Users", "GetRealTimeStatusAsync"),
        new("GET", "/api/player", "Users", "GetAllTop10Async"),
        new("GET", "/api/player/top/{nb}/{perfType}", "Users", "GetLeaderboardAsync"),
        new("GET", "/api/user/{username}/rating-history", "Users", "GetRatingHistoryAsync"),
        new("GET", "/api/user/{username}/perf/{perf}", "Users", "GetPerformanceAsync"),
        new("GET", "/api/user/{username}/activity", "Users", "GetActivityAsync"),
        new("GET", "/api/player/autocomplete", "Users", "AutocompleteAsync"),
        new("GET", "/api/player/autocomplete", "Users", "AutocompletePlayersAsync"),
        new("GET", "/api/crosstable/{user1}/{user2}", "Users", "GetCrosstableAsync"),
        new("GET", "/api/streamer/live", "Users", "GetLiveStreamersAsync"),
        new("GET", "/api/user/{username}/note", "Users", "GetNoteAsync"),
        new("POST", "/api/user/{username}/note", "Users", "WriteNoteAsync"),

        // ===== Relations API =====
        new("POST", "/api/rel/follow/{username}", "Relations", "FollowUserAsync"),
        new("POST", "/api/rel/unfollow/{username}", "Relations", "UnfollowUserAsync"),
        new("POST", "/api/rel/block/{username}", "Relations", "BlockUserAsync"),
        new("POST", "/api/rel/unblock/{username}", "Relations", "UnblockUserAsync"),
        new("GET", "/api/rel/following", "Relations", "StreamFollowingUsersAsync"),

        // ===== Games API =====
        new("GET", "/game/export/{gameId}", "Games", "ExportAsync"),
        new("GET", "/game/export/{gameId}", "Games", "GetPgnAsync"),
        new("GET", "/api/user/{username}/current-game", "Games", "GetCurrentGameByUserAsync"),
        new("GET", "/api/games/user/{username}", "Games", "StreamUserGamesAsync"),
        new("POST", "/api/games/export/_ids", "Games", "StreamByIdsAsync"),
        new("POST", "/api/stream/games-by-users", "Games", "StreamByUsersAsync"),
        new("GET", "/api/account/playing", "Games", "GetOngoingGamesAsync"),
        new("POST", "/api/import", "Games", "ImportAsync"),
        new("GET", "/api/games/export/imports", "Games", "ExportImportedGamesAsync"),
        new("GET", "/api/games/export/bookmarks", "Games", "StreamBookmarkedGamesAsync"),
        new("GET", "/api/stream/game/{id}", "Games", "StreamGameMovesAsync"),
        new("POST", "/api/stream/games/{streamId}", "Games", "StreamByIdsAsync"),
        new("POST", "/api/stream/games/{streamId}/add", "Games", "AddGameIdsToStreamAsync"),

        // ===== TV API =====
        new("GET", "/api/tv/channels", "Tv", "GetCurrentGamesAsync"),
        new("GET", "/api/tv/feed", "Tv", "StreamCurrentGameAsync"),
        new("GET", "/api/tv/{channel}/feed", "Tv", "StreamChannelAsync"),
        new("GET", "/api/tv/{channel}", "Tv", "StreamChannelGamesAsync"),

        // ===== Puzzles API =====
        new("GET", "/api/puzzle/daily", "Puzzles", "GetDailyAsync"),
        new("GET", "/api/puzzle/{id}", "Puzzles", "GetAsync"),
        new("GET", "/api/puzzle/next", "Puzzles", "GetNextAsync"),
        new("GET", "/api/puzzle/activity", "Puzzles", "StreamActivityAsync"),
        new("GET", "/api/puzzle/dashboard/{days}", "Puzzles", "GetDashboardAsync"),
        new("GET", "/api/storm/dashboard/{username}", "Puzzles", "GetStormDashboardAsync"),
        new("POST", "/api/racer", "Puzzles", "CreateRaceAsync"),
        new("GET", "/api/puzzle/batch/{angle}", "Puzzles", "GetBatchAsync"),
        new("POST", "/api/puzzle/batch/{angle}", "Puzzles", "SolveBatchAsync"),
        new("GET", "/api/puzzle/replay/{days}/{theme}", "Puzzles", "GetReplayAsync"),
        new("GET", "/api/racer/{id}", "Puzzles", "GetRaceAsync"),

        // ===== Teams API =====
        new("GET", "/api/team/{teamId}", "Teams", "GetAsync"),
        new("GET", "/api/team/all", "Teams", "GetPopularAsync"),
        new("GET", "/api/team/of/{username}", "Teams", "GetUserTeamsAsync"),
        new("GET", "/api/team/search", "Teams", "SearchAsync"),
        new("GET", "/api/team/{teamId}/users", "Teams", "StreamMembersAsync"),
        new("POST", "/team/{teamId}/join", "Teams", "JoinAsync"),
        new("POST", "/team/{teamId}/quit", "Teams", "LeaveAsync"),
        new("GET", "/api/team/{teamId}/requests", "Teams", "GetJoinRequestsAsync"),
        new("POST", "/api/team/{teamId}/request/{userId}/accept", "Teams", "AcceptJoinRequestAsync"),
        new("POST", "/api/team/{teamId}/request/{userId}/decline", "Teams", "DeclineJoinRequestAsync"),
        new("POST", "/api/team/{teamId}/kick/{userId}", "Teams", "KickMemberAsync"),
        new("POST", "/team/{teamId}/pm-all", "Teams", "MessageAllMembersAsync"),

        // ===== Board API =====
        new("GET", "/api/stream/event", "Board", "StreamEventsAsync"),
        new("GET", "/api/board/game/stream/{gameId}", "Board", "StreamGameAsync"),
        new("POST", "/api/board/game/{gameId}/move/{move}", "Board", "MakeMoveAsync"),
        new("GET", "/api/board/game/{gameId}/chat", "Board", "GetChatAsync"),
        new("POST", "/api/board/game/{gameId}/chat", "Board", "WriteChatAsync"),
        new("POST", "/api/board/game/{gameId}/abort", "Board", "AbortAsync"),
        new("POST", "/api/board/game/{gameId}/resign", "Board", "ResignAsync"),
        new("POST", "/api/board/game/{gameId}/draw/{accept}", "Board", "HandleDrawAsync"),
        new("POST", "/api/board/game/{gameId}/takeback/{accept}", "Board", "HandleTakebackAsync"),
        new("POST", "/api/board/game/{gameId}/claim-victory", "Board", "ClaimVictoryAsync"),
        new("POST", "/api/board/game/{gameId}/claim-draw", "Board", "ClaimDrawAsync"),
        new("POST", "/api/board/game/{gameId}/berserk", "Board", "BerserkAsync"),
        new("POST", "/api/board/seek", "Board", "SeekAsync"),

        // ===== Bot API =====
        new("POST", "/api/bot/account/upgrade", "Bot", "UpgradeAccountAsync"),
        new("GET", "/api/stream/event", "Bot", "StreamEventsAsync"),
        new("GET", "/api/bot/game/stream/{gameId}", "Bot", "StreamGameAsync"),
        new("POST", "/api/bot/game/{gameId}/move/{move}", "Bot", "MakeMoveAsync"),
        new("GET", "/api/bot/game/{gameId}/chat", "Bot", "GetChatAsync"),
        new("POST", "/api/bot/game/{gameId}/chat", "Bot", "WriteChatAsync"),
        new("POST", "/api/bot/game/{gameId}/abort", "Bot", "AbortAsync"),
        new("POST", "/api/bot/game/{gameId}/resign", "Bot", "ResignAsync"),
        new("POST", "/api/bot/game/{gameId}/draw/{accept}", "Bot", "HandleDrawAsync"),
        new("POST", "/api/bot/game/{gameId}/takeback/{accept}", "Bot", "HandleTakebackAsync"),
        new("POST", "/api/bot/game/{gameId}/claim-draw", "Bot", "ClaimDrawAsync"),
        new("POST", "/api/bot/game/{gameId}/claim-victory", "Bot", "ClaimVictoryAsync"),
        new("GET", "/api/bot/online", "Bot", "GetOnlineBotsAsync"),

        // ===== Challenges API =====
        new("GET", "/api/challenge", "Challenges", "GetPendingAsync"),
        new("GET", "/api/challenge/{challengeId}/show", "Challenges", "ShowAsync"),
        new("POST", "/api/challenge/{username}", "Challenges", "CreateAsync"),
        new("POST", "/api/challenge/{challengeId}/accept", "Challenges", "AcceptAsync"),
        new("POST", "/api/challenge/{challengeId}/decline", "Challenges", "DeclineAsync"),
        new("POST", "/api/challenge/{challengeId}/cancel", "Challenges", "CancelAsync"),
        new("POST", "/api/challenge/ai", "Challenges", "ChallengeAiAsync"),
        new("POST", "/api/challenge/open", "Challenges", "CreateOpenAsync"),
        new("POST", "/api/challenge/{gameId}/start-clocks", "Challenges", "StartClocksAsync"),
        new("POST", "/api/round/{gameId}/add-time/{seconds}", "Challenges", "AddTimeAsync"),

        // ===== Analysis API =====
        new("GET", "/api/cloud-eval", "Analysis", "GetCloudEvaluationAsync"),

        // ===== Opening Explorer API =====
        // Note: These endpoints are on explorer.lichess.ovh, not lichess.org
        new("GET", "/masters", "OpeningExplorer", "GetMastersAsync"),
        new("GET", "/lichess", "OpeningExplorer", "GetLichessAsync"),
        new("GET", "/player", "OpeningExplorer", "GetPlayerAsync"),
        new("GET", "/master/pgn/{gameId}", "OpeningExplorer", "GetMasterGamePgnAsync"),

        // ===== Tablebase API =====
        new("GET", "/standard", "Tablebase", "LookupAsync"),
        new("GET", "/atomic", "Tablebase", "LookupAtomicAsync"),
        new("GET", "/antichess", "Tablebase", "LookupAntichessAsync"),

        // ===== FIDE API =====
        new("GET", "/api/fide/player/{playerId}", "Fide", "GetPlayerAsync"),
        new("GET", "/api/fide/player", "Fide", "SearchPlayersAsync"),

        // ===== OAuth API =====
        new("GET", "/oauth", "OAuth", "CreateAuthorizationRequest"),
        new("POST", "/api/token", "OAuth", "GetTokenAsync"),
        new("DELETE", "/api/token", "OAuth", "RevokeTokenAsync"),
        new("POST", "/api/token/test", "OAuth", "TestTokensAsync"),

        // ===== External Engine API =====
        new("GET", "/api/external-engine", "ExternalEngine", "ListAsync"),
        new("POST", "/api/external-engine", "ExternalEngine", "CreateAsync"),
        new("GET", "/api/external-engine/{id}", "ExternalEngine", "GetAsync"),
        new("PUT", "/api/external-engine/{id}", "ExternalEngine", "UpdateAsync"),
        new("DELETE", "/api/external-engine/{id}", "ExternalEngine", "DeleteAsync"),
        new("POST", "/api/external-engine/{id}/analyse", "ExternalEngine", "AnalyseAsync"),
        new("POST", "/api/external-engine/work", "ExternalEngine", "AcquireWorkAsync"),
        new("POST", "/api/external-engine/work/{id}", "ExternalEngine", "SubmitWorkAsync"),

        // ===== Bulk Pairings API =====
        new("GET", "/api/bulk-pairing", "BulkPairings", "GetAllAsync"),
        new("GET", "/api/bulk-pairing/{id}", "BulkPairings", "GetAsync"),
        new("POST", "/api/bulk-pairing", "BulkPairings", "CreateAsync"),
        new("POST", "/api/bulk-pairing/{id}/start-clocks", "BulkPairings", "StartClocksAsync"),
        new("DELETE", "/api/bulk-pairing/{id}", "BulkPairings", "CancelAsync"),
        new("GET", "/api/bulk-pairing/{id}/games", "BulkPairings", "ExportGamesAsync"),
        new("GET", "/api/bulk-pairing/{id}/games", "BulkPairings", "StreamGamesAsync"),

        // ===== Arena Tournaments API =====
        new("GET", "/api/tournament", "ArenaTournaments", "GetCurrentAsync"),
        new("GET", "/api/tournament/{id}", "ArenaTournaments", "GetAsync"),
        new("POST", "/api/tournament", "ArenaTournaments", "CreateAsync"),
        new("POST", "/api/tournament/{id}", "ArenaTournaments", "UpdateAsync"),
        new("POST", "/api/tournament/{id}/join", "ArenaTournaments", "JoinAsync"),
        new("POST", "/api/tournament/{id}/withdraw", "ArenaTournaments", "PauseOrWithdrawAsync"),
        new("POST", "/api/tournament/{id}/terminate", "ArenaTournaments", "TerminateAsync"),
        new("POST", "/api/tournament/team-battle/{id}", "ArenaTournaments", "UpdateTeamBattleAsync"),
        new("GET", "/api/tournament/{id}/games", "ArenaTournaments", "StreamGamesAsync"),
        new("GET", "/api/tournament/{id}/results", "ArenaTournaments", "StreamResultsAsync"),
        new("GET", "/api/tournament/{id}/teams", "ArenaTournaments", "GetTeamStandingAsync"),
        new("GET", "/api/user/{username}/tournament/created", "ArenaTournaments", "StreamCreatedByAsync"),
        new("GET", "/api/user/{username}/tournament/played", "ArenaTournaments", "StreamPlayedByAsync"),
        new("GET", "/api/team/{teamId}/arena", "ArenaTournaments", "StreamTeamTournamentsAsync"),

        // ===== Swiss Tournaments API =====
        new("GET", "/api/swiss/{id}", "SwissTournaments", "GetAsync"),
        new("POST", "/api/swiss/new/{teamId}", "SwissTournaments", "CreateAsync"),
        new("POST", "/api/swiss/{id}/edit", "SwissTournaments", "UpdateAsync"),
        new("POST", "/api/swiss/{id}/schedule-next-round", "SwissTournaments", "ScheduleNextRoundAsync"),
        new("POST", "/api/swiss/{id}/join", "SwissTournaments", "JoinAsync"),
        new("POST", "/api/swiss/{id}/withdraw", "SwissTournaments", "PauseOrWithdrawAsync"),
        new("POST", "/api/swiss/{id}/terminate", "SwissTournaments", "TerminateAsync"),
        new("GET", "/swiss/{id}.trf", "SwissTournaments", "ExportTrfAsync"),
        new("GET", "/api/swiss/{id}/games", "SwissTournaments", "StreamGamesAsync"),
        new("GET", "/api/swiss/{id}/results", "SwissTournaments", "StreamResultsAsync"),
        new("GET", "/api/team/{teamId}/swiss", "SwissTournaments", "StreamTeamTournamentsAsync"),

        // ===== Simuls API =====
        new("GET", "/api/simul", "Simuls", "GetCurrentAsync"),

        // ===== Studies API =====
        new("GET", "/api/study/{studyId}/{chapterId}.pgn", "Studies", "ExportChapterPgnAsync"),
        new("GET", "/api/study/{studyId}.pgn", "Studies", "ExportStudyPgnAsync"),
        new("GET", "/api/study/by/{username}/export.pgn", "Studies", "ExportUserStudiesPgnAsync"),
        new("GET", "/api/study/by/{username}", "Studies", "StreamUserStudiesAsync"),
        new("POST", "/api/study/{studyId}/import-pgn", "Studies", "ImportPgnAsync"),
        new("POST", "/api/study/{studyId}/{chapterId}/tags", "Studies", "UpdateChapterTagsAsync"),
        new("DELETE", "/api/study/{studyId}/{chapterId}", "Studies", "DeleteChapterAsync"),

        // ===== Messaging API =====
        new("POST", "/inbox/{username}", "Messaging", "SendAsync"),

        // ===== Broadcasts API =====
        new("GET", "/api/broadcast", "Broadcasts", "StreamOfficialBroadcastsAsync"),
        new("GET", "/api/broadcast/top", "Broadcasts", "GetTopBroadcastsAsync"),
        new("GET", "/api/broadcast/by/{username}", "Broadcasts", "StreamUserBroadcastsAsync"),
        new("GET", "/api/broadcast/search", "Broadcasts", "SearchBroadcastsAsync"),
        new("GET", "/api/broadcast/{broadcastTournamentId}", "Broadcasts", "GetTournamentAsync"),
        new("GET", "/api/broadcast/{broadcastTournamentSlug}/{broadcastRoundSlug}/{broadcastRoundId}", "Broadcasts", "GetRoundAsync"),
        new("GET", "/api/broadcast/my-rounds", "Broadcasts", "StreamMyRoundsAsync"),
        new("POST", "/broadcast/new", "Broadcasts", "CreateTournamentAsync"),
        new("POST", "/broadcast/{broadcastTournamentId}/edit", "Broadcasts", "UpdateTournamentAsync"),
        new("POST", "/broadcast/{broadcastTournamentId}/new", "Broadcasts", "CreateRoundAsync"),
        new("POST", "/broadcast/round/{broadcastRoundId}/edit", "Broadcasts", "UpdateRoundAsync"),
        new("POST", "/api/broadcast/round/{broadcastRoundId}/reset", "Broadcasts", "ResetRoundAsync"),
        new("POST", "/api/broadcast/round/{broadcastRoundId}/push", "Broadcasts", "PushPgnAsync"),
        new("GET", "/api/broadcast/round/{broadcastRoundId}.pgn", "Broadcasts", "ExportRoundPgnAsync"),
        new("GET", "/api/broadcast/{broadcastTournamentId}.pgn", "Broadcasts", "ExportAllRoundsPgnAsync"),
        new("GET", "/api/stream/broadcast/round/{broadcastRoundId}.pgn", "Broadcasts", "StreamRoundPgnAsync"),
        new("GET", "/broadcast/{broadcastTournamentId}/players", "Broadcasts", "GetPlayersAsync"),
        new("GET", "/broadcast/{broadcastTournamentId}/players/{playerId}", "Broadcasts", "GetPlayerAsync")
    ];

    /// <summary>
    /// Gets the count of implemented endpoints.
    /// </summary>
    public static int Count => All.Length;

    /// <summary>
    /// Gets endpoints grouped by API name.
    /// </summary>
    public static ILookup<string, EndpointInfo> ByApi => All.ToLookup(e => e.ApiName);
}

/// <summary>
/// Represents a single implemented API endpoint.
/// </summary>
/// <param name="Method">HTTP method (GET, POST, PUT, DELETE)</param>
/// <param name="Path">API path template (e.g., "/api/account", "/api/user/{username}")</param>
/// <param name="ApiName">API property name on LichessClient (e.g., "Account", "Users")</param>
/// <param name="MethodName">Implementation method name (e.g., "GetProfileAsync")</param>
public readonly record struct EndpointInfo(string Method, string Path, string ApiName, string MethodName)
{
    /// <summary>
    /// Gets the endpoint key in "METHOD /path" format.
    /// </summary>
    public string Key => $"{Method} {Path}";

    /// <summary>
    /// Gets the full method reference in "Api.Method" format.
    /// </summary>
    public string FullMethodName => $"{ApiName}.{MethodName}";
}
