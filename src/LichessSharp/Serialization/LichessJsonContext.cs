using System.Text.Json;
using System.Text.Json.Serialization;
using LichessSharp.Api;
using LichessSharp.Models;

namespace LichessSharp.Serialization;

/// <summary>
/// Source-generated JSON serialization context for AOT compatibility.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true,
    WriteIndented = false)]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(UserExtended))]
[JsonSerializable(typeof(UserStatus))]
[JsonSerializable(typeof(UserStatus[]))]
[JsonSerializable(typeof(List<User>))]
[JsonSerializable(typeof(List<UserStatus>))]
[JsonSerializable(typeof(AccountPreferences))]
[JsonSerializable(typeof(Game))]
[JsonSerializable(typeof(GameJson))]
[JsonSerializable(typeof(GameUser))]
[JsonSerializable(typeof(List<Game>))]
[JsonSerializable(typeof(Puzzle))]
[JsonSerializable(typeof(PuzzleWithGame))]
[JsonSerializable(typeof(OkResponse))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(EmailResponse))]
[JsonSerializable(typeof(KidModeResponse))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(Dictionary<string, List<User>>))]
[JsonSerializable(typeof(List<RatingHistory>))]
[JsonSerializable(typeof(RatingHistory))]
[JsonSerializable(typeof(LeaderboardResponse))]
[JsonSerializable(typeof(CloudEvaluation))]
[JsonSerializable(typeof(PrincipalVariation))]
[JsonSerializable(typeof(ExplorerResult))]
[JsonSerializable(typeof(ExplorerMove))]
[JsonSerializable(typeof(ExplorerGame))]
[JsonSerializable(typeof(ExplorerOpening))]
[JsonSerializable(typeof(TablebaseResult))]
[JsonSerializable(typeof(TablebaseMove))]
[JsonSerializable(typeof(OngoingGamesResponse))]
[JsonSerializable(typeof(OngoingGame))]
[JsonSerializable(typeof(ImportGameResponse))]
[JsonSerializable(typeof(PuzzleActivity))]
[JsonSerializable(typeof(PuzzleActivityPuzzle))]
[JsonSerializable(typeof(PuzzleDashboard))]
[JsonSerializable(typeof(PuzzleThemeResults))]
[JsonSerializable(typeof(PuzzlePerformance))]
[JsonSerializable(typeof(StormDashboard))]
[JsonSerializable(typeof(StormHigh))]
[JsonSerializable(typeof(StormDay))]
[JsonSerializable(typeof(PuzzleRace))]
internal partial class LichessJsonContext : JsonSerializerContext
{
}
