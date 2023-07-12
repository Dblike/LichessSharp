using Ndjson.AsyncStreams.Net.Http;

using System;
using System.Text.Json;

namespace Lichess.NET;

public class BaseClient
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private static readonly TimeSpan TooManyRequestsDelay = TimeSpan.FromSeconds(75);

    public BaseClient()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    }

    protected async Task<T> SendAndRetryAsync<T>(HttpMethod method, string query)
        where T : class, new()
    {
        return await SendAndRetryAsync<T>(new HttpRequestMessage(method, query));
    }

    protected async Task<T> SendAndRetryAsync<T>(HttpRequestMessage request)
        where T : class, new()
    {
        var response = await SendAndRetryAsync(request);
        using var content = await response.Content.ReadAsStreamAsync();

        try
        {
            var result = JsonSerializer.Deserialize<T>(content, jsonSerializerOptions);
            return result ?? new T();
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected async IAsyncEnumerable<T> SendAndRetryNdJsonAsync<T>(HttpMethod method, string query)
        where T : class, new()
    {
        await foreach (var item in SendAndRetryNdJsonAsync<T>(new HttpRequestMessage(method, query)))
        {
            yield return item;
        }
    }

    protected async IAsyncEnumerable<T> SendAndRetryNdJsonAsync<T>(HttpRequestMessage request)
        where T : class, new()
    {
        request.Headers.Add("Accept", "application/x-ndjson");
        var response = await SendAndRetryAsync(request);

        await foreach (var item in response.Content.ReadFromNdjsonAsync<T>())
        {
            yield return item;
        }
    }

    private async Task<HttpResponseMessage> SendAndRetryAsync(HttpRequestMessage request)
    {
        var needToRetry = true;
        HttpResponseMessage response = new();

        while (needToRetry)
        {
            response = await _client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                Thread.Sleep(TooManyRequestsDelay);
            }
            else
            {
                needToRetry = false;
            }
        }

        return response;
    }
}