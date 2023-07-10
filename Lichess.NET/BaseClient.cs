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

    public BaseClient()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    }

    protected async Task<T> SendAndRetryAsync<T>(HttpMethod method, string query)
        where T : class, new()
    {
        var needToRetry = true;
        HttpResponseMessage response = new();

        while (needToRetry)
        {
            response = await _client.SendAsync(new HttpRequestMessage(method, query));

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                Thread.Sleep(TimeSpan.FromSeconds(75));
            }
            else
            {
                needToRetry = false;
            }
        }

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
}