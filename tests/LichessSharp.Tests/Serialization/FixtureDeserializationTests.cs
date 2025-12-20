using FluentAssertions;
using LichessSharp.Models.Games;
using LichessSharp.Models.Puzzles;
using LichessSharp.Models.Users;
using LichessSharp.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace LichessSharp.Tests.Serialization;

/// <summary>
/// Tests deserialization of JSON fixtures captured from real API responses.
/// These tests validate that model classes correctly deserialize actual API data.
/// </summary>
public class FixtureDeserializationTests
{
    private readonly ITestOutputHelper _output;

    public FixtureDeserializationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Provides all available user fixture paths for parameterized testing.
    /// </summary>
    public static IEnumerable<object[]> GetUserFixtures()
    {
        foreach (var fixture in FixtureLoader.GetFixturesInDirectory("Users"))
        {
            yield return new object[] { fixture };
        }
    }

    /// <summary>
    /// Provides all available game fixture paths for parameterized testing.
    /// </summary>
    public static IEnumerable<object[]> GetGameFixtures()
    {
        foreach (var fixture in FixtureLoader.GetFixturesInDirectory("Games"))
        {
            yield return new object[] { fixture };
        }
    }

    /// <summary>
    /// Provides all available puzzle fixture paths for parameterized testing.
    /// </summary>
    public static IEnumerable<object[]> GetPuzzleFixtures()
    {
        foreach (var fixture in FixtureLoader.GetFixturesInDirectory("Puzzles"))
        {
            yield return new object[] { fixture };
        }
    }

    [Theory]
    [MemberData(nameof(GetUserFixtures))]
    public void UserExtended_DeserializesFromFixture(string fixturePath)
    {
        // Skip if fixture doesn't exist yet
        if (!FixtureLoader.Exists(fixturePath))
        {
            _output.WriteLine($"Fixture not found (run FixtureCaptureTests first): {fixturePath}");
            return;
        }

        // Skip non-user_extended fixtures
        if (!fixturePath.Contains("user_extended"))
            return;

        // Act
        var user = FixtureLoader.Load<UserExtended>(fixturePath);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeNullOrEmpty("Id is a required field");
        user.Username.Should().NotBeNullOrEmpty("Username is a required field");

        _output.WriteLine($"Deserialized user: {user.Username} (id: {user.Id})");

        if (user.CreatedAt != default)
            _output.WriteLine($"  Created: {user.CreatedAt:O}");

        if (user.Perfs != null)
            _output.WriteLine($"  Performance types: {string.Join(", ", user.Perfs.Keys)}");
    }

    [Theory]
    [MemberData(nameof(GetGameFixtures))]
    public void GameJson_DeserializesFromFixture(string fixturePath)
    {
        if (!FixtureLoader.Exists(fixturePath))
        {
            _output.WriteLine($"Fixture not found: {fixturePath}");
            return;
        }

        // Act
        var game = FixtureLoader.Load<GameJson>(fixturePath);

        // Assert
        game.Should().NotBeNull();
        game.Id.Should().NotBeNullOrEmpty("Id is a required field");

        _output.WriteLine($"Deserialized game: {game.Id}");
        _output.WriteLine($"  Variant: {game.Variant}, Speed: {game.Speed}");
        _output.WriteLine($"  Status: {game.Status}, Rated: {game.Rated}");

        if (game.Players != null)
        {
            _output.WriteLine($"  White: {game.Players.White?.User?.Name ?? "anonymous"}");
            _output.WriteLine($"  Black: {game.Players.Black?.User?.Name ?? "anonymous"}");
        }

        if (!string.IsNullOrEmpty(game.Moves))
            _output.WriteLine($"  Moves: {game.Moves.Split(' ').Length} moves");
    }

    [Theory]
    [MemberData(nameof(GetPuzzleFixtures))]
    public void Puzzle_DeserializesFromFixture(string fixturePath)
    {
        if (!FixtureLoader.Exists(fixturePath))
        {
            _output.WriteLine($"Fixture not found: {fixturePath}");
            return;
        }

        // Act
        var puzzle = FixtureLoader.Load<PuzzleWithGame>(fixturePath);

        // Assert
        puzzle.Should().NotBeNull();
        puzzle.Puzzle.Should().NotBeNull();
        puzzle.Game.Should().NotBeNull();

        _output.WriteLine($"Deserialized puzzle from game: {puzzle.Game.Id}");
        _output.WriteLine($"  Rating: {puzzle.Puzzle.Rating}");
        _output.WriteLine($"  Themes: {string.Join(", ", puzzle.Puzzle.Themes ?? Array.Empty<string>())}");
    }

    [Fact]
    public void ListAvailableFixtures()
    {
        var fixtures = FixtureLoader.GetFixtureFiles().ToList();

        _output.WriteLine($"Available fixtures ({fixtures.Count}):");
        foreach (var fixture in fixtures.OrderBy(x => x))
        {
            _output.WriteLine($"  - {fixture}");
        }

        if (!fixtures.Any())
        {
            _output.WriteLine("No fixtures found. Run FixtureCaptureTests to create fixtures.");
        }
    }
}
