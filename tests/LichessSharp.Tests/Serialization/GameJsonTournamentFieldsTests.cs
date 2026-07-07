using System.Text.Json;
using FluentAssertions;
using LichessSharp.Models.Games;
using LichessSharp.Tests.Fixtures;
using Xunit;

namespace LichessSharp.Tests.Serialization;

/// <summary>
/// Tests for the arenaTour/swissTour fields introduced in Lichess API v2.0.152,
/// which replaced the former string-valued tournament/swiss fields on GameJson.
/// </summary>
public class GameJsonTournamentFieldsTests
{
    private readonly JsonSerializerOptions _options = LichessJsonDefaults.Options;

    [Fact]
    public void Deserialize_WithArenaTour_PopulatesArenaTour()
    {
        const string json = """
            {
              "id": "p2uYFdtm",
              "rated": true,
              "arenaTour": { "id": "59x1tmRl", "name": "Hourly UltraBullet Arena" }
            }
            """;

        var game = JsonSerializer.Deserialize<GameJson>(json, _options);

        game.Should().NotBeNull();
        game!.ArenaTour.Should().NotBeNull();
        game.ArenaTour!.Id.Should().Be("59x1tmRl");
        game.ArenaTour.Name.Should().Be("Hourly UltraBullet Arena");
        game.SwissTour.Should().BeNull();
    }

    [Fact]
    public void Deserialize_WithSwissTour_PopulatesSwissTour()
    {
        const string json = """
            {
              "id": "abcd1234",
              "rated": true,
              "swissTour": { "id": "swiss99" }
            }
            """;

        var game = JsonSerializer.Deserialize<GameJson>(json, _options);

        game.Should().NotBeNull();
        game!.SwissTour.Should().NotBeNull();
        game.SwissTour!.Id.Should().Be("swiss99");
        game.SwissTour.Name.Should().BeNull();
        game.ArenaTour.Should().BeNull();
    }

    [Fact]
    public void Deserialize_WithoutTournamentFields_LeavesBothNull()
    {
        const string json = """{ "id": "abcd1234", "rated": false }""";

        var game = JsonSerializer.Deserialize<GameJson>(json, _options);

        game.Should().NotBeNull();
        game!.ArenaTour.Should().BeNull();
        game.SwissTour.Should().BeNull();
    }
}
