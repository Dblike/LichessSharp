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
[JsonSerializable(typeof(List<Game>))]
[JsonSerializable(typeof(Puzzle))]
[JsonSerializable(typeof(PuzzleWithGame))]
[JsonSerializable(typeof(OkResponse))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(Dictionary<string, List<User>>))]
[JsonSerializable(typeof(List<RatingHistory>))]
[JsonSerializable(typeof(RatingHistory))]
[JsonSerializable(typeof(LeaderboardResponse))]
[JsonSerializable(typeof(CloudEvaluation))]
[JsonSerializable(typeof(PrincipalVariation))]
internal partial class LichessJsonContext : JsonSerializerContext
{
}
