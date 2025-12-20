using System.Text.Json;
using FluentAssertions;
using LichessSharp.Api.Contracts;
using LichessSharp.Models.Games;
using LichessSharp.Models.Puzzles;
using LichessSharp.Models.Users;
using LichessSharp.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace LichessSharp.Tests.Serialization;

/// <summary>
/// Tests round-trip serialization (deserialize -> serialize -> deserialize)
/// to ensure no data loss during JSON processing.
/// </summary>
public class RoundTripSerializationTests
{
    private readonly ITestOutputHelper _output;
    private readonly JsonSerializerOptions _options = LichessJsonDefaults.Options;

    public RoundTripSerializationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Provides test data for round-trip tests with type information.
    /// </summary>
    public static IEnumerable<object[]> GetRoundTripTestCases()
    {
        // User fixtures
        if (FixtureLoader.Exists("Users/user_extended_thibault.json"))
            yield return new object[] { "Users/user_extended_thibault.json", typeof(UserExtended) };

        if (FixtureLoader.Exists("Users/user_status_multiple.json"))
            yield return new object[] { "Users/user_status_multiple.json", typeof(UserStatus[]) };

        if (FixtureLoader.Exists("Users/rating_history_thibault.json"))
            yield return new object[] { "Users/rating_history_thibault.json", typeof(List<RatingHistory>) };

        // Game fixtures
        if (FixtureLoader.Exists("Games/game_json_full.json"))
            yield return new object[] { "Games/game_json_full.json", typeof(GameJson) };

        // Puzzle fixtures
        if (FixtureLoader.Exists("Puzzles/puzzle_daily.json"))
            yield return new object[] { "Puzzles/puzzle_daily.json", typeof(PuzzleWithGame) };
    }

    [Theory]
    [MemberData(nameof(GetRoundTripTestCases))]
    public void RoundTrip_PreservesAllFields(string fixturePath, Type modelType)
    {
        // Arrange
        var originalJson = FixtureLoader.LoadRawJson(fixturePath);

        // Act - deserialize
        var deserialized = JsonSerializer.Deserialize(originalJson, modelType, _options);
        deserialized.Should().NotBeNull($"Failed to deserialize {fixturePath}");

        // Act - reserialize
        var reserialized = JsonSerializer.Serialize(deserialized, modelType, _options);

        // Act - deserialize again
        var roundTripped = JsonSerializer.Deserialize(reserialized, modelType, _options);

        // Assert - compare objects
        roundTripped.Should().BeEquivalentTo(deserialized,
            options => options.Using<DateTimeOffset>(ctx =>
                ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
                .WhenTypeIs<DateTimeOffset>(),
            because: "round-trip serialization should preserve all data");

        _output.WriteLine($"Round-trip test passed for {fixturePath}");
    }

    [Fact]
    public void UserExtended_RoundTrip_PreservesNestedObjects()
    {
        const string fixturePath = "Users/user_extended_thibault.json";
        if (!FixtureLoader.Exists(fixturePath))
        {
            _output.WriteLine($"Fixture not found: {fixturePath}");
            return;
        }

        // Arrange
        var original = FixtureLoader.Load<UserExtended>(fixturePath);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var roundTripped = JsonSerializer.Deserialize<UserExtended>(json, _options);

        // Assert specific nested properties
        roundTripped.Should().NotBeNull();
        roundTripped!.Id.Should().Be(original.Id);
        roundTripped.Username.Should().Be(original.Username);

        if (original.Profile != null)
        {
            roundTripped.Profile.Should().NotBeNull();
            roundTripped.Profile!.Country.Should().Be(original.Profile.Country);
        }

        if (original.Perfs != null)
        {
            roundTripped.Perfs.Should().NotBeNull();
            roundTripped.Perfs!.Keys.Should().BeEquivalentTo(original.Perfs.Keys);

            foreach (var key in original.Perfs.Keys)
            {
                roundTripped.Perfs[key].Rating.Should().Be(original.Perfs[key].Rating);
                roundTripped.Perfs[key].Games.Should().Be(original.Perfs[key].Games);
            }
        }

        _output.WriteLine($"UserExtended round-trip verified with nested objects");
    }

    [Fact]
    public void GameJson_RoundTrip_PreservesPlayerData()
    {
        const string fixturePath = "Games/game_json_full.json";
        if (!FixtureLoader.Exists(fixturePath))
        {
            _output.WriteLine($"Fixture not found: {fixturePath}");
            return;
        }

        // Arrange
        var original = FixtureLoader.Load<GameJson>(fixturePath);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var roundTripped = JsonSerializer.Deserialize<GameJson>(json, _options);

        // Assert
        roundTripped.Should().NotBeNull();
        roundTripped!.Id.Should().Be(original.Id);
        roundTripped.Variant.Should().Be(original.Variant);
        roundTripped.Speed.Should().Be(original.Speed);
        roundTripped.Status.Should().Be(original.Status);
        roundTripped.Moves.Should().Be(original.Moves);

        if (original.Players != null)
        {
            roundTripped.Players.Should().NotBeNull();
            roundTripped.Players!.White?.Rating.Should().Be(original.Players.White?.Rating);
            roundTripped.Players!.Black?.Rating.Should().Be(original.Players.Black?.Rating);
        }

        _output.WriteLine($"GameJson round-trip verified");
    }

    [Fact]
    public void Serialization_HandlesNullFields_Correctly()
    {
        // Create a minimal user with many null fields
        var user = new UserExtended
        {
            Id = "testuser",
            Username = "TestUser"
            // All optional fields are null
        };

        // Act
        var json = JsonSerializer.Serialize(user, _options);
        var roundTripped = JsonSerializer.Deserialize<UserExtended>(json, _options);

        // Assert
        roundTripped.Should().NotBeNull();
        roundTripped!.Id.Should().Be("testuser");
        roundTripped.Username.Should().Be("TestUser");
        roundTripped.Profile.Should().BeNull();
        roundTripped.Perfs.Should().BeNull();

        _output.WriteLine($"Null field handling verified");
        _output.WriteLine($"Serialized JSON: {json}");
    }
}
