using Lichess.NET.Entities;
using Lichess.NET.Exceptions;

using Microsoft.AspNetCore.WebUtilities;

namespace Lichess.NET;

public class LichessClient : BaseClient
{
    private const string OpeningExplorerUrl = "https://explorer.lichess.ovh";

    //Account
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

        options ??= ExportGameOptions.Default;

        var query = "https://lichess.org/game/export";
        var queryParams = new Dictionary<string, string>
        {
            { "moves", options.IncludeMoves.ToString() },
            { "pgnInJson", options.IncludePgnInJson.ToString() },
            { "tags", options.IncludePgnTags.ToString() },
            { "clocks", options.IncludeClockStatus.ToString() },
            { "evals", options.IncludeEvals.ToString() },
            { "accuracy", options.IncludeAccuracy.ToString() },
            { "opening", options.IncludeOpening.ToString() },
            { "literate", options.IncludeAnnotations.ToString() }
        };
        query = QueryHelpers.AddQueryString($"{query}/{gameId}", queryParams);

        return await SendAndRetryAsync<GameExplorerResult>(HttpMethod.Get, query);
    }

    //ExportOngoingGamesAsync
    //ExportGamesAsync
    //ExportGamesByIdsAsync
    //StreamGamesAsync
    //AddGameIdsToStreamAsync
    //GetOngoingGamesAsync
    //StreamGameMovesAsync
    //ImportGameAsync

    //Users
    //Relations
    //Games
    //TV
    //Puzzles
    //Teams
    //Board
    //Bot
    //Challenges
    //Bulk pairings
    //Arena tournaments
    //Swiss tournaments
    //Simuls
    //Studies
    //Messaging
    //Broadcasting
    //Analysis
    //External engine
    //Opening Explorer
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
        options ??= SearchMasterGameOptions.Default;

        var query = $"{OpeningExplorerUrl}/masters";
        var queryParams = new Dictionary<string, string>
        {
            { "fen", options.Fen },
            { "play", options.Play },
            { "since", options.Since },
            { "until", options.Until },
            { "moves", options.Moves.ToString() },
            { "topGames", options.TopGames.ToString() }
        };
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
        options ??= SearchLichessGameOptions.Default;

        var query = $"{OpeningExplorerUrl}/lichess";
        var queryParams = new Dictionary<string, string>
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
        };
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
        options ??= SearchPlayerGameOptions.Default;

        var query = $"{OpeningExplorerUrl}/player";
        var queryParams = new Dictionary<string, string>
        {
            { "player", playerId },
            { "color", color.ToString().ToLower() },
            { "variant", options.Variant.ToString().ToLower() },
            { "fen", options.Fen },
            { "play", options.Play },
            { "speeds", string.Join(",", options.Speeds.Select(speed => speed.ToString().ToLower())) },
            { "since", options.Since },
            { "until", options.Until },
            { "moves", options.Moves.ToString() },
            { "recentGames", options.RecentGames.ToString() }
        };
        query = QueryHelpers.AddQueryString(query, queryParams);

        return await SendAndRetryAsync<OpeningExplorerResult>(HttpMethod.Get, query);
    }
    //Tablebase
    //OAuth
}