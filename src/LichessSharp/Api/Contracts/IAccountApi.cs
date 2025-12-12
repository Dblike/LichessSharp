using LichessSharp.Models.Account;
using LichessSharp.Models.Users;

namespace LichessSharp.Api.Contracts;

/// <summary>
/// Account API - Read and write account information and preferences.
/// </summary>
public interface IAccountApi
{
    /// <summary>
    /// Get the profile of the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authenticated user's profile.</returns>
    Task<UserExtended> GetProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the email address of the authenticated user.
    /// Requires the email:read OAuth scope.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The email address.</returns>
    Task<string> GetEmailAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the preferences of the authenticated user.
    /// Requires the preference:read OAuth scope.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's preferences.</returns>
    Task<AccountPreferences> GetPreferencesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the kid mode status of the authenticated user.
    /// Requires the preference:read OAuth scope.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether kid mode is enabled.</returns>
    Task<bool> GetKidModeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the kid mode status of the authenticated user.
    /// Requires the preference:write OAuth scope.
    /// </summary>
    /// <param name="enabled">Whether to enable kid mode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the operation succeeded.</returns>
    Task<bool> SetKidModeAsync(bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the timeline of the authenticated user.
    /// Requires OAuth.
    /// </summary>
    /// <param name="nb">Maximum number of entries to return (default 15, max 30).</param>
    /// <param name="since">Only return entries after this timestamp.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's timeline.</returns>
    Task<Timeline> GetTimelineAsync(int? nb = null, DateTimeOffset? since = null, CancellationToken cancellationToken = default);
}
