namespace LichessSharp.Api.Options;

/// <summary>
/// Options for exporting bookmarked games.
/// </summary>
public class ExportBookmarksOptions : ExportGameOptions
{
    /// <summary>
    /// Download games bookmarked since this timestamp.
    /// Defaults to account creation date.
    /// </summary>
    public DateTimeOffset? Since { get; set; }

    /// <summary>
    /// Download games bookmarked until this timestamp.
    /// Defaults to now.
    /// </summary>
    public DateTimeOffset? Until { get; set; }

    /// <summary>
    /// Maximum number of bookmarked games to download.
    /// Leave empty to download all bookmarked games.
    /// </summary>
    public int? Max { get; set; }

    /// <summary>
    /// Include the last position FEN.
    /// </summary>
    public bool? LastFen { get; set; }

    /// <summary>
    /// Sort order: "dateAsc" or "dateDesc".
    /// Defaults to reverse chronological order (most recent first).
    /// </summary>
    public string? Sort { get; set; }
}
