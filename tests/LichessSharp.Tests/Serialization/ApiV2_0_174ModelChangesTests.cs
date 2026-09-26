using System.Text.Json;
using FluentAssertions;
using LichessSharp.Api.Contracts;
using LichessSharp.Models.Games;
using LichessSharp.Tests.Fixtures;
using Xunit;

namespace LichessSharp.Tests.Serialization;

/// <summary>
/// Tests for the schema changes introduced in Lichess API v2.0.174:
/// GamePlayerUser.berserk, the GamePlayerAi shape, StudyImportPgnChapters.error,
/// and BroadcastRoundInfo.createdAt / rated becoming optional.
/// </summary>
public class ApiV2_0_174ModelChangesTests
{
    private readonly JsonSerializerOptions _options = LichessJsonDefaults.Options;

    [Fact]
    public void GamePlayer_WithBerserk_PopulatesBerserk()
    {
        const string json = """
            { "user": { "id": "thibault", "name": "Thibault" }, "rating": 1500, "berserk": true }
            """;

        var player = JsonSerializer.Deserialize<GamePlayer>(json, _options);

        player.Should().NotBeNull();
        player!.Berserk.Should().BeTrue();
        player.User!.Id.Should().Be("thibault");
        player.AiLevel.Should().BeNull();
    }

    [Fact]
    public void GamePlayer_AiShape_PopulatesAiLevelWithoutUser()
    {
        const string json = """
            { "aiLevel": 3, "analysis": { "inaccuracy": 1, "mistake": 2, "blunder": 0, "acpl": 25 } }
            """;

        var player = JsonSerializer.Deserialize<GamePlayer>(json, _options);

        player.Should().NotBeNull();
        player!.AiLevel.Should().Be(3);
        player.User.Should().BeNull();
        player.Rating.Should().BeNull();
        player.Berserk.Should().BeNull();
        player.Analysis.Should().NotBeNull();
    }

    [Fact]
    public void StudyImportResult_WithError_PopulatesError()
    {
        const string json = """
            { "chapters": [ { "id": "abcd1234", "name": "Chapter 1" } ], "error": "Some games could not be imported" }
            """;

        var result = JsonSerializer.Deserialize<StudyImportResult>(json, _options);

        result.Should().NotBeNull();
        result!.Chapters.Should().ContainSingle().Which.Id.Should().Be("abcd1234");
        result.Error.Should().Be("Some games could not be imported");
    }

    [Fact]
    public void StudyImportResult_WithNullError_LeavesErrorNull()
    {
        const string json = """
            { "chapters": [], "error": null }
            """;

        var result = JsonSerializer.Deserialize<StudyImportResult>(json, _options);

        result.Should().NotBeNull();
        result!.Error.Should().BeNull();
    }

    [Fact]
    public void BroadcastRoundInfo_WithoutCreatedAtOrRated_DeserializesWithNulls()
    {
        const string json = """
            { "id": "WCYaLfnP", "name": "Round 9", "slug": "round-9", "url": "https://lichess.org/broadcast/x/round-9/WCYaLfnP", "ongoing": true }
            """;

        var round = JsonSerializer.Deserialize<BroadcastRoundInfo>(json, _options);

        round.Should().NotBeNull();
        round!.Id.Should().Be("WCYaLfnP");
        round.CreatedAt.Should().BeNull();
        round.Rated.Should().BeNull();
        round.Ongoing.Should().BeTrue();
    }

    [Fact]
    public void BroadcastRoundInfo_WithCreatedAtAndRated_PopulatesValues()
    {
        const string json = """
            { "id": "WCYaLfnP", "name": "Round 9", "slug": "round-9", "url": "https://lichess.org/broadcast/x/round-9/WCYaLfnP", "createdAt": 1789801389222, "rated": true }
            """;

        var round = JsonSerializer.Deserialize<BroadcastRoundInfo>(json, _options);

        round.Should().NotBeNull();
        round!.CreatedAt.Should().Be(1789801389222);
        round.Rated.Should().BeTrue();
    }
}
