using System.Runtime.CompilerServices;
using System.Text;
using LichessSharp.Http;
using LichessSharp.Models;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the Puzzles API.
/// </summary>
internal sealed class PuzzlesApi : IPuzzlesApi
{
    private readonly ILichessHttpClient _httpClient;

    public PuzzlesApi(ILichessHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<PuzzleWithGame> GetDailyAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<PuzzleWithGame>("/api/puzzle/daily", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PuzzleWithGame> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        return await _httpClient.GetAsync<PuzzleWithGame>($"/api/puzzle/{Uri.EscapeDataString(id)}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PuzzleWithGame> GetNextAsync(string? angle = null, string? difficulty = null, CancellationToken cancellationToken = default)
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

        await foreach (var activity in _httpClient.StreamNdjsonAsync<PuzzleActivity>(endpoint, cancellationToken).ConfigureAwait(false))
        {
            yield return activity;
        }
    }

    /// <inheritdoc />
    public async Task<PuzzleDashboard> GetDashboardAsync(int days = 30, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<PuzzleDashboard>($"/api/puzzle/dashboard/{days}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<StormDashboard> GetStormDashboardAsync(string username, int days = 30, CancellationToken cancellationToken = default)
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
}
