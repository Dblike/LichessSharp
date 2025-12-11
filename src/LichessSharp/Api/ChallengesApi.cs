using System.Text;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Challenges API.
/// </summary>
internal sealed class ChallengesApi(ILichessHttpClient httpClient) : IChallengesApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<ChallengeList> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<ChallengeList>("/api/challenge", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ChallengeJson> ShowAsync(string challengeId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(challengeId);

        var endpoint = $"/api/challenge/{Uri.EscapeDataString(challengeId)}/show";
        return await _httpClient.GetAsync<ChallengeJson>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ChallengeJson> CreateAsync(string username, ChallengeCreateOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = $"/api/challenge/{Uri.EscapeDataString(username)}";
        var content = BuildChallengeCreateContent(options);

        return await _httpClient.PostAsync<ChallengeJson>(endpoint, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> AcceptAsync(string challengeId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(challengeId);

        var endpoint = $"/api/challenge/{Uri.EscapeDataString(challengeId)}/accept";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeclineAsync(string challengeId, ChallengeDeclineReason? reason = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(challengeId);

        var endpoint = $"/api/challenge/{Uri.EscapeDataString(challengeId)}/decline";
        HttpContent? content = null;

        if (reason.HasValue)
        {
            var reasonStr = GetDeclineReasonString(reason.Value);
            content = new FormUrlEncodedContent([new KeyValuePair<string, string>("reason", reasonStr)]);
        }

        await _httpClient.PostAsync<OkResponse>(endpoint, content, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CancelAsync(string challengeId, string? opponentToken = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(challengeId);

        var sb = new StringBuilder();
        sb.Append("/api/challenge/");
        sb.Append(Uri.EscapeDataString(challengeId));
        sb.Append("/cancel");

        if (!string.IsNullOrEmpty(opponentToken))
        {
            sb.Append("?opponentToken=");
            sb.Append(Uri.EscapeDataString(opponentToken));
        }

        await _httpClient.PostAsync<OkResponse>(sb.ToString(), null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<ChallengeAiResponse> ChallengeAiAsync(ChallengeAiOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Level < 1 || options.Level > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "AI level must be between 1 and 8.");
        }

        var content = BuildChallengeAiContent(options);
        return await _httpClient.PostAsync<ChallengeAiResponse>("/api/challenge/ai", content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ChallengeOpenJson> CreateOpenAsync(ChallengeOpenOptions? options = null, CancellationToken cancellationToken = default)
    {
        var content = BuildChallengeOpenContent(options);
        return await _httpClient.PostAsync<ChallengeOpenJson>("/api/challenge/open", content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> StartClocksAsync(string gameId, string? token1 = null, string? token2 = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        var sb = new StringBuilder();
        sb.Append("/api/challenge/");
        sb.Append(Uri.EscapeDataString(gameId));
        sb.Append("/start-clocks");

        var hasQuery = false;
        if (!string.IsNullOrEmpty(token1))
        {
            sb.Append("?token1=");
            sb.Append(Uri.EscapeDataString(token1));
            hasQuery = true;
        }
        if (!string.IsNullOrEmpty(token2))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("token2=");
            sb.Append(Uri.EscapeDataString(token2));
        }

        await _httpClient.PostAsync<OkResponse>(sb.ToString(), null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> AddTimeAsync(string gameId, int seconds, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameId);

        if (seconds < 1 || seconds > 60)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), "Seconds must be between 1 and 60.");
        }

        var endpoint = $"/api/round/{Uri.EscapeDataString(gameId)}/add-time/{seconds}";
        await _httpClient.PostAsync<OkResponse>(endpoint, null, cancellationToken).ConfigureAwait(false);
        return true;
    }

    #region Helper Methods

    private static HttpContent? BuildChallengeCreateContent(ChallengeCreateOptions? options)
    {
        if (options == null)
        {
            return null;
        }

        var parameters = new List<KeyValuePair<string, string>>();

        if (options.Rated.HasValue)
        {
            parameters.Add(new("rated", options.Rated.Value.ToString().ToLowerInvariant()));
        }
        if (options.ClockLimit.HasValue)
        {
            parameters.Add(new("clock.limit", options.ClockLimit.Value.ToString()));
        }
        if (options.ClockIncrement.HasValue)
        {
            parameters.Add(new("clock.increment", options.ClockIncrement.Value.ToString()));
        }
        if (options.Days.HasValue)
        {
            parameters.Add(new("days", options.Days.Value.ToString()));
        }
        if (options.Color.HasValue)
        {
            parameters.Add(new("color", GetColorString(options.Color.Value)));
        }
        if (!string.IsNullOrEmpty(options.Variant))
        {
            parameters.Add(new("variant", options.Variant));
        }
        if (!string.IsNullOrEmpty(options.Fen))
        {
            parameters.Add(new("fen", options.Fen));
        }
        if (options.KeepAliveStream.HasValue)
        {
            parameters.Add(new("keepAliveStream", options.KeepAliveStream.Value.ToString().ToLowerInvariant()));
        }
        if (options.Rules.HasValue && options.Rules.Value != ChallengeRules.None)
        {
            parameters.Add(new("rules", GetRulesString(options.Rules.Value)));
        }
        if (!string.IsNullOrEmpty(options.Message))
        {
            parameters.Add(new("message", options.Message));
        }

        return parameters.Count > 0 ? new FormUrlEncodedContent(parameters) : null;
    }

    private static HttpContent BuildChallengeAiContent(ChallengeAiOptions options)
    {
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("level", options.Level.ToString())
        };

        if (options.ClockLimit.HasValue)
        {
            parameters.Add(new("clock.limit", options.ClockLimit.Value.ToString()));
        }
        if (options.ClockIncrement.HasValue)
        {
            parameters.Add(new("clock.increment", options.ClockIncrement.Value.ToString()));
        }
        if (options.Days.HasValue)
        {
            parameters.Add(new("days", options.Days.Value.ToString()));
        }
        if (options.Color.HasValue)
        {
            parameters.Add(new("color", GetColorString(options.Color.Value)));
        }
        if (!string.IsNullOrEmpty(options.Variant))
        {
            parameters.Add(new("variant", options.Variant));
        }
        if (!string.IsNullOrEmpty(options.Fen))
        {
            parameters.Add(new("fen", options.Fen));
        }

        return new FormUrlEncodedContent(parameters);
    }

    private static HttpContent? BuildChallengeOpenContent(ChallengeOpenOptions? options)
    {
        if (options == null)
        {
            return null;
        }

        var parameters = new List<KeyValuePair<string, string>>();

        if (options.Rated.HasValue)
        {
            parameters.Add(new("rated", options.Rated.Value.ToString().ToLowerInvariant()));
        }
        if (options.ClockLimit.HasValue)
        {
            parameters.Add(new("clock.limit", options.ClockLimit.Value.ToString()));
        }
        if (options.ClockIncrement.HasValue)
        {
            parameters.Add(new("clock.increment", options.ClockIncrement.Value.ToString()));
        }
        if (options.Days.HasValue)
        {
            parameters.Add(new("days", options.Days.Value.ToString()));
        }
        if (options.Color.HasValue)
        {
            parameters.Add(new("color", GetColorString(options.Color.Value)));
        }
        if (!string.IsNullOrEmpty(options.Variant))
        {
            parameters.Add(new("variant", options.Variant));
        }
        if (!string.IsNullOrEmpty(options.Fen))
        {
            parameters.Add(new("fen", options.Fen));
        }
        if (options.Rules.HasValue && options.Rules.Value != ChallengeRules.None)
        {
            parameters.Add(new("rules", GetRulesString(options.Rules.Value)));
        }
        if (!string.IsNullOrEmpty(options.Name))
        {
            parameters.Add(new("name", options.Name));
        }
        if (options.Users != null && options.Users.Count > 0)
        {
            parameters.Add(new("users", string.Join(",", options.Users)));
        }
        if (options.ExpiresAt.HasValue)
        {
            parameters.Add(new("expiresAt", options.ExpiresAt.Value.ToString()));
        }

        return parameters.Count > 0 ? new FormUrlEncodedContent(parameters) : null;
    }

    private static string GetColorString(ChallengeColor color) => color switch
    {
        ChallengeColor.White => "white",
        ChallengeColor.Black => "black",
        _ => "random"
    };

    private static string GetDeclineReasonString(ChallengeDeclineReason reason) => reason switch
    {
        ChallengeDeclineReason.Later => "later",
        ChallengeDeclineReason.TooFast => "tooFast",
        ChallengeDeclineReason.TooSlow => "tooSlow",
        ChallengeDeclineReason.TimeControl => "timeControl",
        ChallengeDeclineReason.Rated => "rated",
        ChallengeDeclineReason.Casual => "casual",
        ChallengeDeclineReason.Standard => "standard",
        ChallengeDeclineReason.Variant => "variant",
        ChallengeDeclineReason.NoBot => "noBot",
        ChallengeDeclineReason.OnlyBot => "onlyBot",
        _ => "generic"
    };

    private static string GetRulesString(ChallengeRules rules)
    {
        var rulesList = new List<string>();

        if ((rules & ChallengeRules.NoAbort) != 0) rulesList.Add("noAbort");
        if ((rules & ChallengeRules.NoRematch) != 0) rulesList.Add("noRematch");
        if ((rules & ChallengeRules.NoGiveTime) != 0) rulesList.Add("noGiveTime");
        if ((rules & ChallengeRules.NoClaimWin) != 0) rulesList.Add("noClaimWin");
        if ((rules & ChallengeRules.NoEarlyDraw) != 0) rulesList.Add("noEarlyDraw");

        return string.Join(",", rulesList);
    }

    #endregion
}
