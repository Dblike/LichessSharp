using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models.Puzzles;
using LichessSharp.Serialization;

namespace LichessSharp.Api;

/// <summary>
///     Implementation of the Puzzles API.
/// </summary>
internal sealed class PuzzlesApi(ILichessHttpClient httpClient) : IPuzzlesApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<PuzzleWithGame> GetDailyAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<PuzzleWithGame>("/api/puzzle/daily", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PuzzleWithGame> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        return await _httpClient.GetAsync<PuzzleWithGame>($"/api/puzzle/{Uri.EscapeDataString(id)}", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PuzzleWithGame> GetNextAsync(string? angle = null, string? difficulty = null,
        CancellationToken cancellationToken = default)
    {
        var endpoint = BuildNextPuzzleEndpoint(angle, difficulty);
        return await _httpClient.GetAsync<PuzzleWithGame>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<PuzzleActivity> StreamActivityAsync(
        int? max = null,
        DateTimeOffset? before = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var endpoint = BuildActivityEndpoint(max, before);

        await foreach (var activity in _httpClient.StreamNdjsonAsync<PuzzleActivity>(endpoint, cancellationToken)
                           .ConfigureAwait(false)) yield return activity;
    }

    /// <inheritdoc />
    public async Task<PuzzleDashboard> GetDashboardAsync(int days = 30, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<PuzzleDashboard>($"/api/puzzle/dashboard/{days}", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<StormDashboard> GetStormDashboardAsync(string username, int days = 30,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var endpoint = BuildStormDashboardEndpoint(username, days);
        return await _httpClient.GetAsync<StormDashboard>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PuzzleRace> CreateRaceAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<PuzzleRace>("/api/racer", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PuzzleBatch> GetBatchAsync(string angle, int? nb = null, string? difficulty = null,
        string? color = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(angle);

        var endpoint = BuildBatchEndpoint(angle, nb, difficulty, color);
        return await _httpClient.GetAsync<PuzzleBatch>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PuzzleBatchResult> SolveBatchAsync(string angle, IEnumerable<PuzzleSolution> solutions,
        int? nb = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(angle);
        ArgumentNullException.ThrowIfNull(solutions);

        var endpoint = BuildSolveBatchEndpoint(angle, nb);
        var request = new PuzzleBatchSolveRequest { Solutions = solutions };
        var json = JsonSerializer.Serialize(request, LichessJsonContext.Default.PuzzleBatchSolveRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await _httpClient.PostAsync<PuzzleBatchResult>(endpoint, content, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PuzzleReplay> GetReplayAsync(int days, string theme,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme);

        var endpoint = $"/api/puzzle/replay/{days}/{Uri.EscapeDataString(theme)}";
        return await _httpClient.GetAsync<PuzzleReplay>(endpoint, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PuzzleRaceResults> GetRaceAsync(string raceId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(raceId);

        return await _httpClient
            .GetAsync<PuzzleRaceResults>($"/api/racer/{Uri.EscapeDataString(raceId)}", cancellationToken)
            .ConfigureAwait(false);
    }

    private static string BuildNextPuzzleEndpoint(string? angle, string? difficulty)
    {
        var sb = new StringBuilder("/api/puzzle/next");
        var hasQuery = false;

        if (!string.IsNullOrWhiteSpace(angle))
        {
            sb.Append('?');
            sb.Append("angle=");
            sb.Append(Uri.EscapeDataString(angle));
            hasQuery = true;
        }

        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("difficulty=");
            sb.Append(Uri.EscapeDataString(difficulty));
        }

        return sb.ToString();
    }

    private static string BuildActivityEndpoint(int? max, DateTimeOffset? before)
    {
        var sb = new StringBuilder("/api/puzzle/activity");
        var hasQuery = false;

        if (max.HasValue)
        {
            sb.Append('?');
            sb.Append("max=");
            sb.Append(max.Value);
            hasQuery = true;
        }

        if (before.HasValue)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("before=");
            sb.Append(before.Value.ToUnixTimeMilliseconds());
        }

        return sb.ToString();
    }

    private static string BuildStormDashboardEndpoint(string username, int days)
    {
        var sb = new StringBuilder("/api/storm/dashboard/");
        sb.Append(Uri.EscapeDataString(username));

        if (days != 30)
        {
            sb.Append("?days=");
            sb.Append(days);
        }

        return sb.ToString();
    }

    private static string BuildBatchEndpoint(string angle, int? nb, string? difficulty, string? color)
    {
        var sb = new StringBuilder("/api/puzzle/batch/");
        sb.Append(Uri.EscapeDataString(angle));

        var hasQuery = false;

        if (nb.HasValue)
        {
            sb.Append('?');
            sb.Append("nb=");
            sb.Append(nb.Value);
            hasQuery = true;
        }

        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("difficulty=");
            sb.Append(Uri.EscapeDataString(difficulty));
            hasQuery = true;
        }

        if (!string.IsNullOrWhiteSpace(color))
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append("color=");
            sb.Append(Uri.EscapeDataString(color));
        }

        return sb.ToString();
    }

    private static string BuildSolveBatchEndpoint(string angle, int? nb)
    {
        var sb = new StringBuilder("/api/puzzle/batch/");
        sb.Append(Uri.EscapeDataString(angle));

        if (nb.HasValue && nb.Value > 0)
        {
            sb.Append("?nb=");
            sb.Append(nb.Value);
        }

        return sb.ToString();
    }
}