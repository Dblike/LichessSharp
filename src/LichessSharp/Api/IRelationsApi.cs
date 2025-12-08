namespace LichessSharp.Api;

/// <summary>
/// Relations API - Follow, unfollow, block, and unblock users.
/// </summary>
public interface IRelationsApi
{
    /// <summary>
    /// Follow a user.
    /// </summary>
    /// <param name="username">The username to follow.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the operation succeeded.</returns>
    Task<bool> FollowAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unfollow a user.
    /// </summary>
    /// <param name="username">The username to unfollow.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the operation succeeded.</returns>
    Task<bool> UnfollowAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Block a user.
    /// </summary>
    /// <param name="username">The username to block.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the operation succeeded.</returns>
    Task<bool> BlockAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unblock a user.
    /// </summary>
    /// <param name="username">The username to unblock.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the operation succeeded.</returns>
    Task<bool> UnblockAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream users who are being followed.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of usernames being followed.</returns>
    IAsyncEnumerable<string> StreamFollowingAsync(CancellationToken cancellationToken = default);
}
