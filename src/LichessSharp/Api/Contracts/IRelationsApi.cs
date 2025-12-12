using LichessSharp.Models.Users;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Relations API - Follow, unfollow, block, and unblock users.
/// </summary>
public interface IRelationsApi
{
    /// <summary>
    /// Stream users who are being followed by the logged in user.
    /// Requires the follow:read OAuth scope.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream of users being followed.</returns>
    IAsyncEnumerable<UserExtended> StreamFollowingUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Follow a user.
    /// Requires the follow:write OAuth scope.
    /// </summary>
    /// <param name="username">The username to follow.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the operation succeeded.</returns>
    Task<bool> FollowUserAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unfollow a user.
    /// Requires the follow:write OAuth scope.
    /// </summary>
    /// <param name="username">The username to unfollow.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the operation succeeded.</returns>
    Task<bool> UnfollowUserAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Block a user.
    /// Requires the follow:write OAuth scope.
    /// </summary>
    /// <param name="username">The username to block.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the operation succeeded.</returns>
    Task<bool> BlockUserAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unblock a user.
    /// Requires the follow:write OAuth scope.
    /// </summary>
    /// <param name="username">The username to unblock.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the operation succeeded.</returns>
    Task<bool> UnblockUserAsync(string username, CancellationToken cancellationToken = default);
}
