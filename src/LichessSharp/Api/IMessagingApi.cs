using System.Text.Json.Serialization;

namespace LichessSharp.Api;

/// <summary>
/// Messaging API - Private messages with other players.
/// </summary>
public interface IMessagingApi
{
    /// <summary>
    /// Send a private message to another player.
    /// Requires OAuth with msg:write scope.
    /// </summary>
    /// <param name="username">The username of the recipient.</param>
    /// <param name="text">The message text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the message was sent successfully.</returns>
    Task<bool> SendAsync(string username, string text, CancellationToken cancellationToken = default);
}

#region Response Models

/// <summary>
/// Response from sending a private message.
/// </summary>
public class MessageSentResponse
{
    /// <summary>
    /// Whether the message was sent successfully.
    /// </summary>
    [JsonPropertyName("ok")]
    public bool Ok { get; init; }
}

#endregion
