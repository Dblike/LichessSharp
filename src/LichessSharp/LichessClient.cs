using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LichessSharp;

/// <summary>
/// The main client for interacting with the Lichess API.
/// </summary>
public sealed class LichessClient : ILichessClient
{
    private readonly LichessHttpClient _httpClient;
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

    /// <inheritdoc />
    public IOAuthApi OAuth { get; private set; } = null!;

    /// <inheritdoc />
    public IExternalEngineApi ExternalEngine { get; private set; } = null!;

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
        OpeningExplorer = new OpeningExplorerApi(_httpClient, LichessApiUrls.ExplorerBaseAddress);
        Tablebase = new TablebaseApi(_httpClient, LichessApiUrls.TablebaseBaseAddress);
        Fide = new FideApi(_httpClient);
        OAuth = new OAuthApi(_httpClient);
        ExternalEngine = new ExternalEngineApi(_httpClient, LichessApiUrls.EngineBaseAddress);
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
}
