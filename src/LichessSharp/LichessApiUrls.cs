namespace LichessSharp;

/// <summary>
/// Contains the base URLs for Lichess API services.
/// </summary>
public static class LichessApiUrls
{
    /// <summary>
    /// The base address for the main Lichess API.
    /// </summary>
    public static readonly Uri BaseAddress = new("https://lichess.org");

    /// <summary>
    /// The base address for the Opening Explorer API.
    /// </summary>
    public static readonly Uri ExplorerBaseAddress = new("https://explorer.lichess.ovh");

    /// <summary>
    /// The base address for the Tablebase API.
    /// </summary>
    public static readonly Uri TablebaseBaseAddress = new("https://tablebase.lichess.ovh");

    /// <summary>
    /// The base address for the External Engine API (analysis endpoints only).
    /// Note: CRUD operations use the main BaseAddress.
    /// </summary>
    public static readonly Uri EngineBaseAddress = new("https://engine.lichess.ovh");
}
