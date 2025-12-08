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
        Teams = new NotImplementedApi<ITeamsApi>();
        Board = new NotImplementedApi<IBoardApi>();
        Bot = new NotImplementedApi<IBotApi>();
        Challenges = new NotImplementedApi<IChallengesApi>();
        BulkPairings = new NotImplementedApi<IBulkPairingsApi>();
        ArenaTournaments = new NotImplementedApi<IArenaTournamentsApi>();
        SwissTournaments = new NotImplementedApi<ISwissTournamentsApi>();
        Simuls = new NotImplementedApi<ISimulsApi>();
        Studies = new NotImplementedApi<IStudiesApi>();
        Messaging = new NotImplementedApi<IMessagingApi>();
        Broadcasts = new NotImplementedApi<IBroadcastsApi>();
        Analysis = new AnalysisApi(_httpClient);
        OpeningExplorer = new OpeningExplorerApi(_httpClient, _options.ExplorerBaseAddress);
        Tablebase = new TablebaseApi(_httpClient, _options.TablebaseBaseAddress);
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
        ITeamsApi, IBoardApi, IBotApi, IChallengesApi, IBulkPairingsApi,
        IArenaTournamentsApi, ISwissTournamentsApi, ISimulsApi, IStudiesApi,
        IMessagingApi, IBroadcastsApi, IAnalysisApi, IOpeningExplorerApi, ITablebaseApi
    {
        private static NotImplementedException NotImplemented() =>
            new($"The {typeof(T).Name} is not yet implemented. Implementation coming soon!");

        // IAccountApi
        Task<Models.UserExtended> IAccountApi.GetProfileAsync(CancellationToken ct) => throw NotImplemented();
        Task<string> IAccountApi.GetEmailAsync(CancellationToken ct) => throw NotImplemented();
        Task<Models.AccountPreferences> IAccountApi.GetPreferencesAsync(CancellationToken ct) => throw NotImplemented();
        Task<bool> IAccountApi.GetKidModeAsync(CancellationToken ct) => throw NotImplemented();
        Task<bool> IAccountApi.SetKidModeAsync(bool enabled, CancellationToken ct) => throw NotImplemented();

        // IUsersApi
        Task<Models.UserExtended> IUsersApi.GetAsync(string username, ApiOptions.GetUserOptions? options, CancellationToken ct) => throw NotImplemented();
        Task<IReadOnlyList<Models.User>> IUsersApi.GetManyAsync(IEnumerable<string> userIds, CancellationToken ct) => throw NotImplemented();
        Task<IReadOnlyList<Models.UserStatus>> IUsersApi.GetStatusAsync(IEnumerable<string> userIds, ApiOptions.GetUserStatusOptions? options, CancellationToken ct) => throw NotImplemented();
        Task<Dictionary<string, List<Models.User>>> IUsersApi.GetAllTop10Async(CancellationToken ct) => throw NotImplemented();
        Task<IReadOnlyList<Models.User>> IUsersApi.GetLeaderboardAsync(string perfType, int count, CancellationToken ct) => throw NotImplemented();
        Task<IReadOnlyList<RatingHistory>> IUsersApi.GetRatingHistoryAsync(string username, CancellationToken ct) => throw NotImplemented();

        // IRelationsApi
        Task<bool> IRelationsApi.FollowAsync(string username, CancellationToken ct) => throw NotImplemented();
        Task<bool> IRelationsApi.UnfollowAsync(string username, CancellationToken ct) => throw NotImplemented();
        Task<bool> IRelationsApi.BlockAsync(string username, CancellationToken ct) => throw NotImplemented();
        Task<bool> IRelationsApi.UnblockAsync(string username, CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<Models.UserExtended> IRelationsApi.StreamFollowingAsync(CancellationToken ct) => throw NotImplemented();

        // IGamesApi
        Task<Models.GameJson> IGamesApi.GetAsync(string gameId, ApiOptions.ExportGameOptions? options, CancellationToken ct) => throw NotImplemented();
        Task<string> IGamesApi.GetPgnAsync(string gameId, ApiOptions.ExportGameOptions? options, CancellationToken ct) => throw NotImplemented();
        Task<Models.GameJson> IGamesApi.GetCurrentGameAsync(string username, ApiOptions.ExportGameOptions? options, CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> IGamesApi.StreamUserGamesAsync(string username, ApiOptions.ExportUserGamesOptions? options, CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> IGamesApi.StreamByIdsAsync(IEnumerable<string> gameIds, ApiOptions.ExportGameOptions? options, CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> IGamesApi.StreamGamesByUsersAsync(IEnumerable<string> userIds, bool withCurrentGames, CancellationToken ct) => throw NotImplemented();
        Task<IReadOnlyList<Models.OngoingGame>> IGamesApi.GetOngoingGamesAsync(int count, CancellationToken ct) => throw NotImplemented();
        Task<Models.ImportGameResponse> IGamesApi.ImportPgnAsync(string pgn, CancellationToken ct) => throw NotImplemented();

        // ITvApi
        Task<TvChannels> ITvApi.GetCurrentGamesAsync(CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<TvFeedEvent> ITvApi.StreamCurrentGameAsync(CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<TvFeedEvent> ITvApi.StreamChannelAsync(string channel, CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<Models.GameJson> ITvApi.StreamChannelGamesAsync(string channel, TvChannelGamesOptions? options, CancellationToken ct) => throw NotImplemented();

        // IPuzzlesApi
        Task<Models.PuzzleWithGame> IPuzzlesApi.GetDailyAsync(CancellationToken ct) => throw NotImplemented();
        Task<Models.PuzzleWithGame> IPuzzlesApi.GetAsync(string id, CancellationToken ct) => throw NotImplemented();
        Task<Models.PuzzleWithGame> IPuzzlesApi.GetNextAsync(string? angle, string? difficulty, CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<PuzzleActivity> IPuzzlesApi.StreamActivityAsync(int? max, DateTimeOffset? before, CancellationToken ct) => throw NotImplemented();
        Task<PuzzleDashboard> IPuzzlesApi.GetDashboardAsync(int days, CancellationToken ct) => throw NotImplemented();
        Task<StormDashboard> IPuzzlesApi.GetStormDashboardAsync(string username, int days, CancellationToken ct) => throw NotImplemented();
        Task<PuzzleRace> IPuzzlesApi.CreateRaceAsync(CancellationToken ct) => throw NotImplemented();

        // ITeamsApi
        Task<Team> ITeamsApi.GetAsync(string teamId, CancellationToken ct) => throw NotImplemented();
        Task<IReadOnlyList<Team>> ITeamsApi.GetUserTeamsAsync(string username, CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<TeamMember> ITeamsApi.StreamMembersAsync(string teamId, CancellationToken ct) => throw NotImplemented();
        Task<bool> ITeamsApi.JoinAsync(string teamId, string? message, string? password, CancellationToken ct) => throw NotImplemented();
        Task<bool> ITeamsApi.LeaveAsync(string teamId, CancellationToken ct) => throw NotImplemented();
        Task<TeamSearchResult> ITeamsApi.SearchAsync(string text, int page, CancellationToken ct) => throw NotImplemented();

        // IBoardApi
        IAsyncEnumerable<BoardEvent> IBoardApi.StreamEventsAsync(CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<BoardGameEvent> IBoardApi.StreamGameAsync(string gameId, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBoardApi.MakeMoveAsync(string gameId, string move, bool? offeringDraw, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBoardApi.WriteChatAsync(string gameId, string room, string text, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBoardApi.AbortAsync(string gameId, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBoardApi.ResignAsync(string gameId, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBoardApi.HandleDrawOfferAsync(string gameId, bool accept, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBoardApi.HandleTakebackAsync(string gameId, bool accept, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBoardApi.ClaimVictoryAsync(string gameId, CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<BoardSeekEvent> IBoardApi.SeekAsync(BoardSeekOptions options, CancellationToken ct) => throw NotImplemented();

        // IBotApi
        Task<bool> IBotApi.UpgradeAccountAsync(CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<BotEvent> IBotApi.StreamEventsAsync(CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<BotGameEvent> IBotApi.StreamGameAsync(string gameId, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBotApi.MakeMoveAsync(string gameId, string move, bool? offeringDraw, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBotApi.WriteChatAsync(string gameId, string room, string text, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBotApi.AbortAsync(string gameId, CancellationToken ct) => throw NotImplemented();
        Task<bool> IBotApi.ResignAsync(string gameId, CancellationToken ct) => throw NotImplemented();
        IAsyncEnumerable<BotInfo> IBotApi.GetOnlineBotsAsync(int? count, CancellationToken ct) => throw NotImplemented();

        // IChallengesApi
        Task<ChallengeList> IChallengesApi.GetPendingAsync(CancellationToken ct) => throw NotImplemented();
        Task<Challenge> IChallengesApi.CreateAsync(string username, ChallengeOptions options, CancellationToken ct) => throw NotImplemented();
        Task<bool> IChallengesApi.AcceptAsync(string challengeId, CancellationToken ct) => throw NotImplemented();
        Task<bool> IChallengesApi.DeclineAsync(string challengeId, string? reason, CancellationToken ct) => throw NotImplemented();
        Task<bool> IChallengesApi.CancelAsync(string challengeId, CancellationToken ct) => throw NotImplemented();
        Task<Challenge> IChallengesApi.ChallengeAiAsync(ChallengeAiOptions options, CancellationToken ct) => throw NotImplemented();
        Task<Challenge> IChallengesApi.CreateOpenAsync(ChallengeOptions options, CancellationToken ct) => throw NotImplemented();
        Task<bool> IChallengesApi.StartClocksAsync(string gameId, string? token1, string? token2, CancellationToken ct) => throw NotImplemented();
        Task<bool> IChallengesApi.AddTimeAsync(string gameId, int seconds, CancellationToken ct) => throw NotImplemented();

        // IAnalysisApi
        Task<CloudEvaluation?> IAnalysisApi.GetCloudEvaluationAsync(string fen, int? multiPv, string? variant, CancellationToken ct) => throw NotImplemented();

        // IOpeningExplorerApi
        Task<ExplorerResult> IOpeningExplorerApi.GetMastersAsync(string fen, ExplorerOptions? options, CancellationToken ct) => throw NotImplemented();
        Task<ExplorerResult> IOpeningExplorerApi.GetLichessAsync(string fen, ExplorerOptions? options, CancellationToken ct) => throw NotImplemented();
        Task<ExplorerResult> IOpeningExplorerApi.GetPlayerAsync(string fen, string player, ExplorerOptions? options, CancellationToken ct) => throw NotImplemented();

        // ITablebaseApi
        Task<TablebaseResult> ITablebaseApi.LookupAsync(string fen, CancellationToken ct) => throw NotImplemented();
        Task<TablebaseResult> ITablebaseApi.LookupAtomicAsync(string fen, CancellationToken ct) => throw NotImplemented();
        Task<TablebaseResult> ITablebaseApi.LookupAntichessAsync(string fen, CancellationToken ct) => throw NotImplemented();
    }
}
