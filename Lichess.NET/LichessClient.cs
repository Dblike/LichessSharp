using Lichess.NET.Entities;
using Lichess.NET.Exceptions;
using Lichess.NET.Options.Analysis;
using Lichess.NET.Options.Games;

namespace Lichess.NET;

public class LichessClient : BaseClient
{
    private const string LichessApiUrl = "https://lichess.org/api";
    private const string AccountUrl = "https://lichess.org/account/preferences/game-display";
    private const string UsersUrl = "https://lichess.org/player";
    private const string GamesUrl = "https://lichess.org/games";
    private const string TvUrl = "https://lichess.org/tv";
    private const string OpeningExplorerUrl = "https://explorer.lichess.ovh";
    private const string AnalysisUrl = "https://lichess.org/analysis";
    private const string TablebaseUrl = "https://tablebase.lichess.ovh";

    #region Account - OAuth2

    #endregion
    #region Users

    #endregion
    #region Relations - OAuth2

    #endregion
    #region Users

    #endregion
    #region Relations - OAuth2



    #endregion
    #region Games
    /// <summary>
    ///     Download one game in either PGN or JSON format.
    ///     Ongoing games have their last 3 moves omitted after move 5, as to prevent cheat bots from using this API.
    /// </summary>
    /// <param name="gameId">
    ///     The game ID (8 characters)
    /// </param>
    /// <exception cref="LichessClientException">
    ///     Thrown when gameId is not of length 8
    /// </exception>
    public async Task<GameExplorerResult> ExportGameAsync(string gameId, ExportGameOptions? options = null)
    {
        if (gameId.Length != 8)
        {
            throw new LichessClientException("Game ID must have a length of 8");
        }

        var query = $"https://lichess.org/game/export/{gameId}";
        var queryParams = options != null ? new()
        {
            { "moves", options.IncludeMoves.ToString() },
            { "pgnInJson", options.IncludePgnInJson.ToString() },
            { "tags", options.IncludePgnTags.ToString() },
            { "clocks", options.IncludeClockStatus.ToString() },
            { "evals", options.IncludeEvals.ToString() },
            { "accuracy", options.IncludeAccuracy.ToString() },
            { "opening", options.IncludeOpening.ToString() },
            { "literate", options.IncludeAnnotations.ToString() }
        } : ExportGameOptions.QueryParams;

        query = QueryHelpers.AddQueryString(query, queryParams);

        return await SendAndRetryAsync<GameExplorerResult>(HttpMethod.Get, query);
    }

    /// <summary>
    ///     Download the ongoing game, or the last game played, of a user. Available in either PGN or JSON format.
    ///     Ongoing games have their last 3 moves omitted after move 5, as to prevent cheat bots from using this API.
    /// </summary>
    /// <param name="username">
    ///     The username.
    /// </param>
    public async Task<GameExplorerResult> ExportOngoingGameByUserAsync(string username, ExportGameOptions? options = null)
    {
        var query = $"https://lichess.org/api/user/{username}/current-game";
        var queryParams = options != null ? new()
        {
            { "moves", options.IncludeMoves.ToString() },
            { "pgnInJson", options.IncludePgnInJson.ToString() },
            { "tags", options.IncludePgnTags.ToString() },
            { "clocks", options.IncludeClockStatus.ToString() },
            { "evals", options.IncludeEvals.ToString() },
            { "accuracy", options.IncludeAccuracy.ToString() },
            { "opening", options.IncludeOpening.ToString() },
            { "literate", options.IncludeAnnotations.ToString() }
        } : ExportGameOptions.QueryParams;

        query = QueryHelpers.AddQueryString(query, queryParams);

        return await SendAndRetryAsync<GameExplorerResult>(HttpMethod.Get, query);
    }

    public async IAsyncEnumerable<List<GameExplorerResult>> ExportGamesByUserAsync(string username, ExportGamesByUserOptions? options = null)
    {
        var query = $"https://lichess.org/api/games/user/{username}";
        Dictionary<string, string?> queryParams;

        if (options != null)
        {
            queryParams = new()
            {
                { "moves", options.IncludeMoves.ToString() },
                { "pgnInJson", options.IncludePgnInJson.ToString() },
                { "tags", options.IncludePgnTags.ToString() },
                { "clocks", options.IncludeClockStatus.ToString() },
                { "evals", options.IncludeEvals.ToString() },
                { "accuracy", options.IncludeAccuracy.ToString() },
                { "opening", options.IncludeOpening.ToString() },
                { "literate", options.IncludeAnnotations.ToString() },
            };

            if (options.Since != null)
            {
                queryParams["since"] = options.Since.ToString();
            }

            if (options.Until != null)
            {
                queryParams["until"] = options.Until.ToString();
            }

            if (options.MaxGames != null)
            {
                queryParams["max"] = options.MaxGames.ToString();
            }

            if (options.Vs != null)
            {
                queryParams["vs"] = options.Vs;
            }

            if (options.IncludeOnlyRated != null)
            {
                queryParams["rated"] = options.IncludeOnlyRated.ToString()?.ToLower();
            }

            if (options.PerfType != null)
            {
                queryParams["perfType"] = options.PerfType;
            }

            if (options.IncludeOnlyColor != null)
            {
                queryParams["color"] = options.IncludeOnlyColor.ToString()?.ToLower();
            }

            if (options.IncludeOnlyAnalyzed != null)
            {
                queryParams["analysed"] = options.IncludeOnlyAnalyzed.ToString()?.ToLower();
            }

            if (options.PlayerUrl != null)
            {
                queryParams["url"] = options.PlayerUrl;
            }
        }
        else
        {
            queryParams = ExportGamesByUserOptions.QueryParams;
        }

        query = QueryHelpers.AddQueryString(query, queryParams);

        //TODO: Make batch size dependent on authentication method
        var batchSize = 20;
        var batch = new List<GameExplorerResult>(batchSize);
        await foreach (var item in SendAndRetryNdJsonAsync<GameExplorerResult>(HttpMethod.Get, query))
        {
            batch.Add(item);
            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<GameExplorerResult>(batchSize);
            }
        }

        if (batch.Count > 0)
        {
            yield return batch;
        }
    }

    public async Task<List<GameExplorerResult>> ExportGamesByIdsAsync(List<string> gameIds, ExportGameOptions? options = null)
    {
        var query = $"https://lichess.org/api/games/export/_ids";
        var queryParams = options != null ? new Dictionary<string, string?>
        {
            { "moves", options.IncludeMoves.ToString() },
            { "pgnInJson", options.IncludePgnInJson.ToString() },
            { "tags", options.IncludePgnTags.ToString() },
            { "clocks", options.IncludeClockStatus.ToString() },
            { "evals", options.IncludeEvals.ToString() },
            { "accuracy", options.IncludeAccuracy.ToString() },
            { "opening", options.IncludeOpening.ToString() },
            { "literate", options.IncludeAnnotations.ToString() }
        } : ExportGameOptions.QueryParams;
        query = QueryHelpers.AddQueryString(query, queryParams);

        var request = new HttpRequestMessage(HttpMethod.Post, query)
        {
            Content = new StringContent(string.Join(',', gameIds))
        };
        request.Headers.Add("Content", "text/plain");

        var games = new List<GameExplorerResult>();
        await foreach (var game in SendAndRetryNdJsonAsync<GameExplorerResult>(request))
        {
            games.Add(game);
        }

        return games;
    }

    public async IAsyncEnumerable<GameExplorerResult> StreamGamesByIdsAsync(string streamId, List<string> gameIds)
    {
        var query = $"https://lichess.org/api/stream/games/{streamId}";
        var request = new HttpRequestMessage(HttpMethod.Post, query)
        {
            Content = new StringContent(string.Join(',', gameIds))
        };
        request.Headers.Add("Content", "text/plain");

        await foreach (var game in SendAndRetryNdJsonAsync<GameExplorerResult>(request))
        {
            yield return game;
        }
    }

    public async IAsyncEnumerable<GameExplorerResult> AddGameIdsToStreamAsync(string streamId, List<string> gameIds)
    {
        var query = $"https://lichess.org/api/stream/games/{streamId}/add";
        var request = new HttpRequestMessage(HttpMethod.Post, query)
        {
            Content = new StringContent(string.Join(',', gameIds))
        };
        request.Headers.Add("Content", "text/plain");

        await foreach (var game in SendAndRetryNdJsonAsync<GameExplorerResult>(request))
        {
            yield return game;
        }
    }
    #endregion
    #region TV

    

    #endregion
    #region Puzzles

    

    #endregion
    #region Teams

    

    #endregion
    #region Board - OAuth2

    

    #endregion
    #region Bot - OAuth2

    

    #endregion
    #region Challenges - OAuth2

    

    #endregion
    #region Bulk pairings - OAuth2
    

    #endregion
    #region Arena tournaments

    

    #endregion
    #region Swiss tournaments

    

    #endregion
    #region Simuls

    

    #endregion
    #region Studies

    

    #endregion
    #region Messaging - OAuth2

    

    #endregion
    #region Broadcasts

    

    #endregion
    #region Analysis
    public async Task<OpeningExplorerResult> CloudEvaluation(CloudEvaluationOptions? options = null)
    {
        var query = $"{AnalysisUrl}/api/cloud-eval";
        var queryParams = options != null ? new()
        {
            { "multiPv", options.MultiPv.ToString() },
            { "variant", options.Variant.ToString() }
        } : SearchMasterGameOptions.QueryParams;

        query = QueryHelpers.AddQueryString(query, queryParams);

        return await SendAndRetryAsync<OpeningExplorerResult>(HttpMethod.Get, query);
    }

    #endregion
    #region ExternalEngine - OAuth2
    
    #endregion
    #region OpeningExpolorer
    /// <summary>
    ///     Search Master games.
    /// </summary>
    /// <param name="options">
    ///     
    /// </param>
    /// <returns>
    ///     
    /// </returns>
    public async Task<OpeningExplorerResult> SearchMasterGames(SearchMasterGameOptions? options = null)
    {
        var query = $"{OpeningExplorerUrl}/masters";
        var queryParams = options != null ? new()
        {
            { "fen", options.Fen },
            { "play", options.Play },
            { "since", options.Since },
            { "until", options.Until },
            { "moves", options.Moves.ToString() },
            { "topGames", options.TopGames.ToString() }
        } : SearchMasterGameOptions.QueryParams;

        query = QueryHelpers.AddQueryString(query, queryParams);

        return await SendAndRetryAsync<OpeningExplorerResult>(HttpMethod.Get, query);
    }

    /// <summary>
    ///     Search Lichess games.
    /// </summary>
    /// <param name="options">
    ///     
    /// </param>
    /// <returns>
    ///     
    /// </returns>
    public async Task<OpeningExplorerResult> SearchLichessGames(SearchLichessGameOptions? options = null)
    {
        var query = $"{OpeningExplorerUrl}/lichess";
        var queryParams = options != null ? new()
        {
            { "variant", options.Variant.ToString().ToLower() },
            { "fen", options.Fen },
            { "play", options.Play },
            { "speeds", string.Join(",", options.Speeds.Select(speed => speed.ToString().ToLower())) },
            { "ratings", string.Join(",", options.Ratings.Select(rating => rating.ToString().ToLower())) },
            { "since", options.Since },
            { "until", options.Until },
            { "moves", options.Moves.ToString() },
            { "topGames", options.TopGames.ToString() },
            { "recentGames", options.RecentGames.ToString() },
            { "history", options.IncludeHistory.ToString().ToLower() },
        } : SearchLichessGameOptions.QueryParams;

        query = QueryHelpers.AddQueryString(query, queryParams);

        return await SendAndRetryAsync<OpeningExplorerResult>(HttpMethod.Get, query);
    }

    /// <summary>
    ///     Responds with a stream of newline delimited JSON. Will start indexing on demand, immediately respond with the current results, and stream more updates until indexing is complete. 
    ///     The stream is throttled and deduplicated. Empty lines may be sent to avoid timeouts. 
    ///     Will index new games at most once per minute, and revisit previously ongoing games at most once every day.
    /// </summary>
    /// <param name="options">
    ///     
    /// </param>
    /// <returns>
    ///     
    /// </returns>
    public async Task<OpeningExplorerResult> SearchPlayerGames(string playerId, Color color, SearchPlayerGameOptions? options = null)
    {
        var query = $"{OpeningExplorerUrl}/player";
        var queryParams = new Dictionary<string, string?>()
        {
            { "player", playerId },
            { "color", color.ToString().ToLower() }
        };

        var additionalParams = options != null ? new()
        {
            { "variant", options.Variant.ToString().ToLower() },
            { "fen", options.Fen },
            { "play", options.Play },
            { "speeds", string.Join(",", options.Speeds.Select(speed => speed.ToString().ToLower())) },
            { "since", options.Since },
            { "until", options.Until },
            { "moves", options.Moves.ToString() },
            { "recentGames", options.RecentGames.ToString() }
        } : SearchPlayerGameOptions.QueryParams;

        queryParams = queryParams.Union(additionalParams).ToDictionary(pair => pair.Key, pair => pair.Value);
        query = QueryHelpers.AddQueryString(query, queryParams);

        return await SendAndRetryAsync<OpeningExplorerResult>(HttpMethod.Get, query);
    }
    #endregion
    #region Tablebase
    public async Task<TablebaseResult> LookupTablebasePosition(string fen, Variant variant)
    {
        var query = TablebaseUrl;

        query += variant switch
        {
            Variant.Standard => "/standard",
            Variant.Atomic => "/atomic",
            Variant.Antichess => "/antichess",
            _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, null),
        };

        return await SendAndRetryAsync<TablebaseResult>(HttpMethod.Get, $"{query}/{fen}");
    }
    #endregion
    #region OAuth

    

    #endregion
}