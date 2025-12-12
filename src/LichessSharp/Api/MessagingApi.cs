using LichessSharp.Api.Contracts;
using LichessSharp.Http;

namespace LichessSharp.Api;

/// <summary>
///     Implementation of the Messaging API.
/// </summary>
internal sealed class MessagingApi(ILichessHttpClient httpClient) : IMessagingApi
{
    private readonly ILichessHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public async Task<bool> SendAsync(string username, string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("text", text)
        };

        var content = new FormUrlEncodedContent(parameters);
        var endpoint = $"/inbox/{Uri.EscapeDataString(username)}";

        var response = await _httpClient.PostAsync<MessageSentResponse>(endpoint, content, cancellationToken)
            .ConfigureAwait(false);
        return response.Ok;
    }
}