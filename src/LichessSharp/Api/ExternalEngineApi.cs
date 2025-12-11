using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using LichessSharp.Api.Contracts;
using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
/// Implementation of the External Engine API.
/// WARNING: This API is in alpha and subject to change.
/// </summary>
internal sealed class ExternalEngineApi : IExternalEngineApi
{
    private readonly ILichessHttpClient _httpClient;
    private readonly Uri _engineBaseAddress;

    public ExternalEngineApi(ILichessHttpClient httpClient, Uri engineBaseAddress)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _engineBaseAddress = engineBaseAddress ?? throw new ArgumentNullException(nameof(engineBaseAddress));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExternalEngine>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<List<ExternalEngine>>("/api/external-engine", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ExternalEngine> CreateAsync(ExternalEngineRegistration registration, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(registration);
        ValidateRegistration(registration);

        var request = CreateRegistrationRequest(registration);
        return await _httpClient.PostJsonAsync<ExternalEngine>("/api/external-engine", request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ExternalEngine> GetAsync(string engineId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(engineId);

        return await _httpClient.GetAsync<ExternalEngine>($"/api/external-engine/{Uri.EscapeDataString(engineId)}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ExternalEngine> UpdateAsync(string engineId, ExternalEngineRegistration registration, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(engineId);
        ArgumentNullException.ThrowIfNull(registration);
        ValidateRegistration(registration);

        var request = CreateRegistrationRequest(registration);
        return await _httpClient.PutJsonAsync<ExternalEngine>($"/api/external-engine/{Uri.EscapeDataString(engineId)}", request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string engineId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(engineId);

        await _httpClient.DeleteNoContentAsync($"/api/external-engine/{Uri.EscapeDataString(engineId)}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<EngineAnalysisLine> AnalyseAsync(
        string engineId,
        EngineAnalysisRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(engineId);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ClientSecret);
        ArgumentNullException.ThrowIfNull(request.Work);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Work.InitialFen);

        var url = new Uri(_engineBaseAddress, $"/api/external-engine/{Uri.EscapeDataString(engineId)}/analyse");
        var body = CreateAnalysisRequestBody(request);

        await foreach (var line in _httpClient.StreamAbsoluteNdjsonPostAsync<EngineAnalysisLine>(url, body, cancellationToken).ConfigureAwait(false))
        {
            yield return line;
        }
    }

    /// <inheritdoc />
    public async Task<EngineWork?> AcquireWorkAsync(string providerSecret, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerSecret);

        var url = new Uri(_engineBaseAddress, "/api/external-engine/work");
        var body = new EngineWorkRequest { ProviderSecret = providerSecret };

        try
        {
            return await _httpClient.PostAbsoluteJsonAsync<EngineWork>(url, body, cancellationToken).ConfigureAwait(false);
        }
        catch (Exceptions.LichessException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            // No work available
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SubmitWorkAsync(string workId, IAsyncEnumerable<string> uciLines, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workId);
        ArgumentNullException.ThrowIfNull(uciLines);

        var url = new Uri(_engineBaseAddress, $"/api/external-engine/work/{Uri.EscapeDataString(workId)}");
        await _httpClient.PostAbsoluteStreamAsync(url, uciLines, cancellationToken).ConfigureAwait(false);
    }

    private static void ValidateRegistration(ExternalEngineRegistration registration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registration.Name);
        if (registration.Name.Length < 3 || registration.Name.Length > 200)
        {
            throw new ArgumentException("Name must be between 3 and 200 characters.", nameof(registration));
        }

        if (registration.MaxThreads < 1 || registration.MaxThreads > 65536)
        {
            throw new ArgumentOutOfRangeException(nameof(registration), "MaxThreads must be between 1 and 65536.");
        }

        if (registration.MaxHash < 1 || registration.MaxHash > 1048576)
        {
            throw new ArgumentOutOfRangeException(nameof(registration), "MaxHash must be between 1 and 1048576.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(registration.ProviderSecret);
        if (registration.ProviderSecret.Length < 16)
        {
            throw new ArgumentException("ProviderSecret must be at least 16 characters.", nameof(registration));
        }
    }

    private static EngineRegistrationRequest CreateRegistrationRequest(ExternalEngineRegistration registration)
    {
        return new EngineRegistrationRequest
        {
            Name = registration.Name,
            MaxThreads = registration.MaxThreads,
            MaxHash = registration.MaxHash,
            ProviderSecret = registration.ProviderSecret,
            Variants = registration.Variants,
            ProviderData = registration.ProviderData
        };
    }

    private static EngineAnalysisRequestBody CreateAnalysisRequestBody(EngineAnalysisRequest request)
    {
        return new EngineAnalysisRequestBody
        {
            ClientSecret = request.ClientSecret,
            Work = new EngineAnalysisWorkBody
            {
                SessionId = request.Work.SessionId,
                Threads = request.Work.Threads,
                Hash = request.Work.Hash,
                MultiPv = request.Work.MultiPv,
                Variant = request.Work.Variant,
                InitialFen = request.Work.InitialFen,
                Moves = request.Work.Moves,
                InfiniteDepth = request.Work.InfiniteDepth,
                Nodes = request.Work.Nodes,
                MoveTime = request.Work.MoveTime
            }
        };
    }

    /// <summary>
    /// Internal request body for engine registration.
    /// </summary>
    private sealed class EngineRegistrationRequest
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("maxThreads")]
        public int MaxThreads { get; init; }

        [JsonPropertyName("maxHash")]
        public int MaxHash { get; init; }

        [JsonPropertyName("providerSecret")]
        public required string ProviderSecret { get; init; }

        [JsonPropertyName("variants")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string>? Variants { get; init; }

        [JsonPropertyName("providerData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ProviderData { get; init; }
    }

    /// <summary>
    /// Internal request body for engine analysis.
    /// </summary>
    private sealed class EngineAnalysisRequestBody
    {
        [JsonPropertyName("clientSecret")]
        public required string ClientSecret { get; init; }

        [JsonPropertyName("work")]
        public required EngineAnalysisWorkBody Work { get; init; }
    }

    /// <summary>
    /// Internal analysis work body.
    /// </summary>
    private sealed class EngineAnalysisWorkBody
    {
        [JsonPropertyName("sessionId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SessionId { get; init; }

        [JsonPropertyName("threads")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Threads { get; init; }

        [JsonPropertyName("hash")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Hash { get; init; }

        [JsonPropertyName("multiPv")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MultiPv { get; init; }

        [JsonPropertyName("variant")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Variant { get; init; }

        [JsonPropertyName("initialFen")]
        public required string InitialFen { get; init; }

        [JsonPropertyName("moves")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string>? Moves { get; init; }

        [JsonPropertyName("infiniteDepth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? InfiniteDepth { get; init; }

        [JsonPropertyName("nodes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? Nodes { get; init; }

        [JsonPropertyName("movetime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MoveTime { get; init; }
    }
}
