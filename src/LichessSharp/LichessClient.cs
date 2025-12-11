using LichessSharp.Api;
using LichessSharp.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ApiOptions = LichessSharp.Api.Options;

namespace LichessSharp;

/// <summary>
/// The main client for interacting with the Lichess API.
/// </summary>
public sealed class LichessClient : ILichessClient
{
    private readonly LichessHttpClient _httpClient;
    private readonly LichessClientOptions _options;
    private bool _disposed;

    /// <summary>
    /// Creates a new Lichess client with the specified options.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    /// <param name="options">The client options.</param>
    /// <param name="logger">Optional logger.</param>
    public LichessClient(
        HttpClient httpClient,
        LichessClientOptions options,
        ILogger<LichessClient>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _options = options;

        var httpLogger = logger != null
            ? new LoggerFactory().CreateLogger<LichessHttpClient>()
            : NullLogger<LichessHttpClient>.Instance;

        _httpClient = new LichessHttpClient(
            httpClient,
            Microsoft.Extensions.Options.Options.Create(options),
            httpLogger);

        InitializeApis();
    }

    /// <summary>
    /// Creates a new Lichess client with default options.
    /// </summary>
    /// <param name="accessToken">Optional access token for authenticated requests.</param>
    public LichessClient(string? accessToken = null)
        : this(new HttpClient(), new LichessClientOptions { AccessToken = accessToken })
    {
    }

    /// <inheritdoc />
    public IAccountApi Account { get; private set; } = null!;

    /// <inheritdoc />
    public IUsersApi Users { get; private set; } = null!;

    /// <inheritdoc />
    public IRelationsApi Relations { get; private set; } = null!;

    /// <inheritdoc />
    public IGamesApi Games { get; private set; } = null!;

    /// <inheritdoc />
    public ITvApi Tv { get; private set; } = null!;

    /// <inheritdoc />
    public IPuzzlesApi Puzzles { get; private set; } = null!;

    /// <inheritdoc />
    public ITeamsApi Teams { get; private set; } = null!;

    /// <inheritdoc />
    public IBoardApi Board { get; private set; } = null!;

    /// <inheritdoc />
    public IBotApi Bot { get; private set; } = null!;

    /// <inheritdoc />
    public IChallengesApi Challenges { get; private set; } = null!;

    /// <inheritdoc />
    public IBulkPairingsApi BulkPairings { get; private set; } = null!;

    /// <inheritdoc />
    public IArenaTournamentsApi ArenaTournaments { get; private set; } = null!;

    /// <inheritdoc />
    public ISwissTournamentsApi SwissTournaments { get; private set; } = null!;

    /// <inheritdoc />
    public ISimulsApi Simuls { get; private set; } = null!;

    /// <inheritdoc />
    public IStudiesApi Studies { get; private set; } = null!;

    /// <inheritdoc />
    public IMessagingApi Messaging { get; private set; } = null!;

    /// <inheritdoc />
    public IBroadcastsApi Broadcasts { get; private set; } = null!;

    /// <inheritdoc />
    public IAnalysisApi Analysis { get; private set; } = null!;

    /// <inheritdoc />
    public IOpeningExplorerApi OpeningExplorer { get; private set; } = null!;

    /// <inheritdoc />
    public ITablebaseApi Tablebase { get; private set; } = null!;

    /// <inheritdoc />
    public IFideApi Fide { get; private set; } = null!;

    private void InitializeApis()
    {
        // Initialize API implementations
        // These will be implemented incrementally
        Account = new AccountApi(_httpClient);
        Users = new UsersApi(_httpClient);
        Relations = new RelationsApi(_httpClient);
        Games = new GamesApi(_httpClient);
        Tv = new TvApi(_httpClient);
        Puzzles = new PuzzlesApi(_httpClient);
        Teams = new TeamsApi(_httpClient);
        Board = new BoardApi(_httpClient);
        Bot = new BotApi(_httpClient);
        Challenges = new ChallengesApi(_httpClient);
        BulkPairings = new BulkPairingsApi(_httpClient);
        ArenaTournaments = new ArenaTournamentsApi(_httpClient);
        SwissTournaments = new SwissTournamentsApi(_httpClient);
        Simuls = new SimulsApi(_httpClient);
        Studies = new StudiesApi(_httpClient);
        Messaging = new MessagingApi(_httpClient);
        Broadcasts = new BroadcastsApi(_httpClient);
        Analysis = new AnalysisApi(_httpClient);
        OpeningExplorer = new OpeningExplorerApi(_httpClient, _options.ExplorerBaseAddress);
        Tablebase = new TablebaseApi(_httpClient, _options.TablebaseBaseAddress);
        Fide = new FideApi(_httpClient);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
    }

    /// <summary>
    /// Placeholder for APIs that are not yet implemented.
    /// </summary>
    private sealed class NotImplementedApi<T> :
        IAccountApi, IUsersApi, IRelationsApi, IGamesApi, ITvApi, IPuzzlesApi,
        ITeamsApi, IBoardApi, IBotApi, IChallengesApi,
        IArenaTournamentsApi, ISwissTournamentsApi, ISimulsApi, IStudiesApi,
        IBroadcastsApi, IAnalysisApi, IOpeningExplorerApi, ITablebaseApi, IFideApi
    {
        private static NotImplementedException NotImplemented() =>
            new($"The {typeof(T).Name} is not yet implemented. Implementation coming soon!");

        // IAccountApi
        Task<Models.UserExtended> IAccountApi.GetProfileAsync(CancellationToken cancellationToken) => throw NotImplemented();
        Task<string> IAccountApi.GetEmailAsync(CancellationToken cancellationToken) => throw NotImplemented();
        Task<Models.AccountPreferences> IAccountApi.GetPreferencesAsync(CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IAccountApi.GetKidModeAsync(CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IAccountApi.SetKidModeAsync(bool enabled, CancellationToken cancellationToken) => throw NotImplemented();

        // IUsersApi
        Task<Models.UserExtended> IUsersApi.GetAsync(string username, ApiOptions.GetUserOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<Models.User>> IUsersApi.GetManyAsync(IEnumerable<string> userIds, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<Models.UserStatus>> IUsersApi.GetStatusAsync(IEnumerable<string> userIds, ApiOptions.GetUserStatusOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<Dictionary<string, List<Models.User>>> IUsersApi.GetAllTop10Async(CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<Models.User>> IUsersApi.GetLeaderboardAsync(string perfType, int count, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<RatingHistory>> IUsersApi.GetRatingHistoryAsync(string username, CancellationToken cancellationToken) => throw NotImplemented();
        Task<Models.UserPerformance> IUsersApi.GetPerformanceAsync(string username, string perfType, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<Models.UserActivity>> IUsersApi.GetActivityAsync(string username, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<string>> IUsersApi.AutocompleteAsync(string term, bool asObject, string? friend, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<Models.AutocompletePlayer>> IUsersApi.AutocompletePlayersAsync(string term, string? friend, CancellationToken cancellationToken) => throw NotImplemented();
        Task<Models.Crosstable> IUsersApi.GetCrosstableAsync(string user1, string user2, bool matchup, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<Models.Streamer>> IUsersApi.GetLiveStreamersAsync(CancellationToken cancellationToken) => throw NotImplemented();
        Task<string?> IUsersApi.GetNoteAsync(string username, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IUsersApi.WriteNoteAsync(string username, string text, CancellationToken cancellationToken) => throw NotImplemented();
        Task<Models.Timeline> IUsersApi.GetTimelineAsync(int? nb, DateTimeOffset? since, CancellationToken cancellationToken) => throw NotImplemented();

        // IRelationsApi
        Task<bool> IRelationsApi.FollowAsync(string username, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IRelationsApi.UnfollowAsync(string username, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IRelationsApi.BlockAsync(string username, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IRelationsApi.UnblockAsync(string username, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.UserExtended> IRelationsApi.StreamFollowingAsync(CancellationToken cancellationToken) => throw NotImplemented();

        // IGamesApi
        Task<Models.GameJson> IGamesApi.GetAsync(string gameId, ApiOptions.ExportGameOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<string> IGamesApi.GetPgnAsync(string gameId, ApiOptions.ExportGameOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<Models.GameJson> IGamesApi.GetCurrentGameAsync(string username, ApiOptions.ExportGameOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> IGamesApi.StreamUserGamesAsync(string username, ApiOptions.ExportUserGamesOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> IGamesApi.StreamByIdsAsync(IEnumerable<string> gameIds, ApiOptions.ExportGameOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> IGamesApi.StreamGamesByUsersAsync(IEnumerable<string> userIds, bool withCurrentGames, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<Models.OngoingGame>> IGamesApi.GetOngoingGamesAsync(int count, CancellationToken cancellationToken) => throw NotImplemented();
        Task<Models.ImportGameResponse> IGamesApi.ImportPgnAsync(string pgn, CancellationToken cancellationToken) => throw NotImplemented();
        Task<string> IGamesApi.GetImportedGamesPgnAsync(CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> IGamesApi.StreamBookmarkedGamesAsync(ApiOptions.ExportBookmarksOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.MoveStreamEvent> IGamesApi.StreamGameMovesAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.GameStreamEvent> IGamesApi.StreamGamesByIdsAsync(string streamId, IEnumerable<string> gameIds, CancellationToken cancellationToken) => throw NotImplemented();
        Task IGamesApi.AddGameIdsToStreamAsync(string streamId, IEnumerable<string> gameIds, CancellationToken cancellationToken) => throw NotImplemented();

        // ITvApi
        Task<TvChannels> ITvApi.GetCurrentGamesAsync(CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<TvFeedEvent> ITvApi.StreamCurrentGameAsync(CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<TvFeedEvent> ITvApi.StreamChannelAsync(string channel, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> ITvApi.StreamChannelGamesAsync(string channel, TvChannelGamesOptions? options, CancellationToken cancellationToken) => throw NotImplemented();

        // IPuzzlesApi
        Task<Models.PuzzleWithGame> IPuzzlesApi.GetDailyAsync(CancellationToken cancellationToken) => throw NotImplemented();
        Task<Models.PuzzleWithGame> IPuzzlesApi.GetAsync(string id, CancellationToken cancellationToken) => throw NotImplemented();
        Task<Models.PuzzleWithGame> IPuzzlesApi.GetNextAsync(string? angle, string? difficulty, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<PuzzleActivity> IPuzzlesApi.StreamActivityAsync(int? max, DateTimeOffset? before, CancellationToken cancellationToken) => throw NotImplemented();
        Task<PuzzleDashboard> IPuzzlesApi.GetDashboardAsync(int days, CancellationToken cancellationToken) => throw NotImplemented();
        Task<StormDashboard> IPuzzlesApi.GetStormDashboardAsync(string username, int days, CancellationToken cancellationToken) => throw NotImplemented();
        Task<PuzzleRace> IPuzzlesApi.CreateRaceAsync(CancellationToken cancellationToken) => throw NotImplemented();

        // ITeamsApi
        Task<Team> ITeamsApi.GetAsync(string teamId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<TeamPaginator> ITeamsApi.GetPopularAsync(int page, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<Team>> ITeamsApi.GetUserTeamsAsync(string username, CancellationToken cancellationToken) => throw NotImplemented();
        Task<TeamPaginator> ITeamsApi.SearchAsync(string text, int page, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<TeamMember> ITeamsApi.StreamMembersAsync(string teamId, bool full, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ITeamsApi.JoinAsync(string teamId, string? message, string? password, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ITeamsApi.LeaveAsync(string teamId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<TeamRequestWithUser>> ITeamsApi.GetJoinRequestsAsync(string teamId, bool declined, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ITeamsApi.AcceptJoinRequestAsync(string teamId, string userId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ITeamsApi.DeclineJoinRequestAsync(string teamId, string userId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ITeamsApi.KickMemberAsync(string teamId, string userId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ITeamsApi.MessageAllMembersAsync(string teamId, string message, CancellationToken cancellationToken) => throw NotImplemented();

        // IBoardApi
        IAsyncEnumerable<BoardAccountEvent> IBoardApi.StreamEventsAsync(CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<BoardGameEvent> IBoardApi.StreamGameAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBoardApi.MakeMoveAsync(string gameId, string move, bool? offeringDraw, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<ChatMessage>> IBoardApi.GetChatAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBoardApi.WriteChatAsync(string gameId, ChatRoom room, string text, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBoardApi.AbortAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBoardApi.ResignAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBoardApi.HandleDrawAsync(string gameId, bool accept, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBoardApi.HandleTakebackAsync(string gameId, bool accept, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBoardApi.ClaimVictoryAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBoardApi.BerserkAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<SeekResult> IBoardApi.SeekAsync(SeekOptions options, CancellationToken cancellationToken) => throw NotImplemented();

        // IBotApi
        Task<bool> IBotApi.UpgradeAccountAsync(CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<BotAccountEvent> IBotApi.StreamEventsAsync(CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<BotGameEvent> IBotApi.StreamGameAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBotApi.MakeMoveAsync(string gameId, string move, bool? offeringDraw, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<ChatMessage>> IBotApi.GetChatAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBotApi.WriteChatAsync(string gameId, ChatRoom room, string text, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBotApi.AbortAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBotApi.ResignAsync(string gameId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBotApi.HandleDrawAsync(string gameId, bool accept, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBotApi.HandleTakebackAsync(string gameId, bool accept, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<BotUser> IBotApi.GetOnlineBotsAsync(int? count, CancellationToken cancellationToken) => throw NotImplemented();

        // IChallengesApi
        Task<ChallengeList> IChallengesApi.GetPendingAsync(CancellationToken cancellationToken) => throw NotImplemented();
        Task<ChallengeJson> IChallengesApi.ShowAsync(string challengeId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<ChallengeJson> IChallengesApi.CreateAsync(string username, ChallengeCreateOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IChallengesApi.AcceptAsync(string challengeId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IChallengesApi.DeclineAsync(string challengeId, ChallengeDeclineReason? reason, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IChallengesApi.CancelAsync(string challengeId, string? opponentToken, CancellationToken cancellationToken) => throw NotImplemented();
        Task<ChallengeAiResponse> IChallengesApi.ChallengeAiAsync(ChallengeAiOptions options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<ChallengeOpenJson> IChallengesApi.CreateOpenAsync(ChallengeOpenOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IChallengesApi.StartClocksAsync(string gameId, string? token1, string? token2, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IChallengesApi.AddTimeAsync(string gameId, int seconds, CancellationToken cancellationToken) => throw NotImplemented();

        // IArenaTournamentsApi
        Task<ArenaTournamentList> IArenaTournamentsApi.GetCurrentAsync(CancellationToken cancellationToken) => throw NotImplemented();
        Task<ArenaTournament> IArenaTournamentsApi.GetAsync(string id, int page, CancellationToken cancellationToken) => throw NotImplemented();
        Task<ArenaTournament> IArenaTournamentsApi.CreateAsync(ArenaCreateOptions options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<ArenaTournament> IArenaTournamentsApi.UpdateAsync(string id, ArenaUpdateOptions options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IArenaTournamentsApi.JoinAsync(string id, string? password, string? team, bool? pairMeAsap, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IArenaTournamentsApi.WithdrawAsync(string id, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IArenaTournamentsApi.TerminateAsync(string id, CancellationToken cancellationToken) => throw NotImplemented();
        Task<ArenaTournament> IArenaTournamentsApi.UpdateTeamBattleAsync(string id, string teams, int? nbLeaders, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> IArenaTournamentsApi.StreamGamesAsync(string id, ArenaGamesExportOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<ArenaPlayerResult> IArenaTournamentsApi.StreamResultsAsync(string id, int? nb, bool sheet, CancellationToken cancellationToken) => throw NotImplemented();
        Task<ArenaTeamStanding> IArenaTournamentsApi.GetTeamStandingAsync(string id, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<ArenaTournamentSummary> IArenaTournamentsApi.StreamCreatedByAsync(string username, ArenaStatusFilter? status, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<ArenaPlayedTournament> IArenaTournamentsApi.StreamPlayedByAsync(string username, int? nb, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<ArenaTournamentSummary> IArenaTournamentsApi.StreamTeamTournamentsAsync(string teamId, int? max, CancellationToken cancellationToken) => throw NotImplemented();

        // ISwissTournamentsApi
        Task<SwissTournament> ISwissTournamentsApi.GetAsync(string id, CancellationToken cancellationToken) => throw NotImplemented();
        Task<SwissTournament> ISwissTournamentsApi.CreateAsync(string teamId, SwissCreateOptions options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<SwissTournament> ISwissTournamentsApi.UpdateAsync(string id, SwissUpdateOptions options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ISwissTournamentsApi.ScheduleNextRoundAsync(string id, long date, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ISwissTournamentsApi.JoinAsync(string id, string? password, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ISwissTournamentsApi.WithdrawAsync(string id, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> ISwissTournamentsApi.TerminateAsync(string id, CancellationToken cancellationToken) => throw NotImplemented();
        Task<string> ISwissTournamentsApi.ExportTrfAsync(string id, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> ISwissTournamentsApi.StreamGamesAsync(string id, SwissGamesExportOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<SwissPlayerResult> ISwissTournamentsApi.StreamResultsAsync(string id, int? nb, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<SwissTournament> ISwissTournamentsApi.StreamTeamTournamentsAsync(string teamId, int? max, CancellationToken cancellationToken) => throw NotImplemented();

        // ISimulsApi
        Task<SimulList> ISimulsApi.GetCurrentAsync(CancellationToken cancellationToken) => throw NotImplemented();

        // IStudiesApi
        Task<string> IStudiesApi.ExportChapterPgnAsync(string studyId, string chapterId, StudyExportOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<string> IStudiesApi.ExportStudyPgnAsync(string studyId, StudyExportOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<string> IStudiesApi.ExportUserStudiesPgnAsync(string username, StudyExportOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<StudyMetadata> IStudiesApi.StreamUserStudiesAsync(string username, CancellationToken cancellationToken) => throw NotImplemented();
        Task<StudyImportResult> IStudiesApi.ImportPgnAsync(string studyId, string pgn, StudyImportOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IStudiesApi.UpdateChapterTagsAsync(string studyId, string chapterId, string pgnTags, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IStudiesApi.DeleteChapterAsync(string studyId, string chapterId, CancellationToken cancellationToken) => throw NotImplemented();

        // IBroadcastsApi
        IAsyncEnumerable<BroadcastWithRounds> IBroadcastsApi.StreamOfficialBroadcastsAsync(int? nb, bool? html, CancellationToken cancellationToken) => throw NotImplemented();
        Task<BroadcastTopPage> IBroadcastsApi.GetTopBroadcastsAsync(int? page, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<BroadcastByUser> IBroadcastsApi.StreamUserBroadcastsAsync(string username, int? nb, bool? html, CancellationToken cancellationToken) => throw NotImplemented();
        Task<BroadcastSearchPage> IBroadcastsApi.SearchBroadcastsAsync(string query, int? page, CancellationToken cancellationToken) => throw NotImplemented();
        Task<BroadcastWithRounds> IBroadcastsApi.GetTournamentAsync(string broadcastTournamentId, bool? html, CancellationToken cancellationToken) => throw NotImplemented();
        Task<BroadcastRound> IBroadcastsApi.GetRoundAsync(string broadcastTournamentSlug, string broadcastRoundSlug, string broadcastRoundId, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<BroadcastMyRound> IBroadcastsApi.StreamMyRoundsAsync(int? nb, CancellationToken cancellationToken) => throw NotImplemented();
        Task<BroadcastWithRounds> IBroadcastsApi.CreateTournamentAsync(BroadcastTournamentOptions options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<BroadcastWithRounds> IBroadcastsApi.UpdateTournamentAsync(string broadcastTournamentId, BroadcastTournamentOptions options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<BroadcastRoundNew> IBroadcastsApi.CreateRoundAsync(string broadcastTournamentId, BroadcastRoundOptions options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<BroadcastRoundNew> IBroadcastsApi.UpdateRoundAsync(string broadcastRoundId, BroadcastRoundOptions options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<bool> IBroadcastsApi.ResetRoundAsync(string broadcastRoundId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<BroadcastPgnPushResult> IBroadcastsApi.PushPgnAsync(string broadcastRoundId, string pgn, CancellationToken cancellationToken) => throw NotImplemented();
        Task<string> IBroadcastsApi.ExportRoundPgnAsync(string broadcastRoundId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<string> IBroadcastsApi.ExportAllRoundsPgnAsync(string broadcastTournamentId, CancellationToken cancellationToken) => throw NotImplemented();
        IAsyncEnumerable<string> IBroadcastsApi.StreamRoundPgnAsync(string broadcastRoundId, CancellationToken cancellationToken) => throw NotImplemented();

        // IAnalysisApi
        Task<CloudEvaluation?> IAnalysisApi.GetCloudEvaluationAsync(string fen, int? multiPv, string? variant, CancellationToken cancellationToken) => throw NotImplemented();

        // IOpeningExplorerApi
        Task<ExplorerResult> IOpeningExplorerApi.GetMastersAsync(string fen, ExplorerOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<ExplorerResult> IOpeningExplorerApi.GetLichessAsync(string fen, ExplorerOptions? options, CancellationToken cancellationToken) => throw NotImplemented();
        Task<ExplorerResult> IOpeningExplorerApi.GetPlayerAsync(string fen, string player, ExplorerOptions? options, CancellationToken cancellationToken) => throw NotImplemented();

        // ITablebaseApi
        Task<TablebaseResult> ITablebaseApi.LookupAsync(string fen, CancellationToken cancellationToken) => throw NotImplemented();
        Task<TablebaseResult> ITablebaseApi.LookupAtomicAsync(string fen, CancellationToken cancellationToken) => throw NotImplemented();
        Task<TablebaseResult> ITablebaseApi.LookupAntichessAsync(string fen, CancellationToken cancellationToken) => throw NotImplemented();

        // IFideApi
        Task<Models.FidePlayer> IFideApi.GetPlayerAsync(int playerId, CancellationToken cancellationToken) => throw NotImplemented();
        Task<IReadOnlyList<Models.FidePlayer>> IFideApi.SearchPlayersAsync(string query, CancellationToken cancellationToken) => throw NotImplemented();
    }
}
