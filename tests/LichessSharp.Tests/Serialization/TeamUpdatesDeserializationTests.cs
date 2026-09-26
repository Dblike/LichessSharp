using System.Text.Json;
using FluentAssertions;
using LichessSharp.Models.Teams;
using LichessSharp.Tests.Fixtures;
using Xunit;

namespace LichessSharp.Tests.Serialization;

/// <summary>
/// Tests for the team-updates models introduced in Lichess API v2.0.174
/// (GET /team/updates and GET /team/updates/{teamId}). The JSON is taken from the
/// spec examples teams-updates.json and teams-updates-of-team.json.
/// </summary>
public class TeamUpdatesDeserializationTests
{
    private readonly JsonSerializerOptions _options = LichessJsonDefaults.Options;

    private const string UpdatesJson = """
        {
          "updates": {
            "currentPage": 1,
            "maxPerPage": 6,
            "currentPageResults": [
              {
                "msg": {
                  "id": "AAAAAAAI",
                  "team": { "id": "the-fischermen", "name": "The Fischermen", "flair": "people.older-person-medium-skin-tone" },
                  "text": "Let's get everyone to at least one classical game this week.",
                  "sender": { "name": "Guang", "id": "guang" },
                  "date": 1789801389222
                },
                "seen": false
              },
              {
                "msg": {
                  "id": "AAAAAAAq",
                  "team": { "id": "the-prep-leaks-for-itself", "name": "The prep leaks for itself" },
                  "text": "Quiet week ahead, use it to study some endgames!",
                  "sender": { "name": "Marcel", "patron": true, "patronColor": 1, "id": "marcel" },
                  "date": 1789637365558
                },
                "seen": true
              }
            ],
            "previousPage": null,
            "nextPage": 2,
            "nbResults": 66,
            "nbPages": 11
          },
          "byTeam": [
            {
              "team": { "id": "the-fischermen", "name": "The Fischermen", "flair": "people.older-person-medium-skin-tone" },
              "unread": 2,
              "last": 1789801389222
            },
            {
              "team": { "id": "open-chess-club", "name": "Open Chess Club" },
              "unread": 0,
              "last": 1789662367160
            }
          ]
        }
        """;

    private const string UpdatesOfTeamJson = """
        {
          "team": { "id": "open-chess-club", "name": "Open Chess Club" },
          "subscribed": true,
          "updates": {
            "currentPage": 1,
            "maxPerPage": 6,
            "currentPageResults": [
              {
                "msg": {
                  "id": "AAAAAAC4",
                  "team": { "id": "open-chess-club", "name": "Open Chess Club" },
                  "text": "Nice work in the last league match, on to the next round!",
                  "sender": { "name": "Mary", "flair": "nature.crab", "id": "mary" },
                  "date": 1789662367160
                },
                "seen": false
              }
            ],
            "previousPage": null,
            "nextPage": null,
            "nbResults": 3,
            "nbPages": 1
          },
          "byTeam": [
            {
              "team": { "id": "open-chess-club", "name": "Open Chess Club" },
              "unread": 2,
              "last": 1789662367160
            }
          ]
        }
        """;

    [Fact]
    public void Deserialize_TeamUpdates_PopulatesPagerAndByTeam()
    {
        var result = JsonSerializer.Deserialize<TeamUpdates>(UpdatesJson, _options);

        result.Should().NotBeNull();
        result!.Updates.CurrentPage.Should().Be(1);
        result.Updates.MaxPerPage.Should().Be(6);
        result.Updates.PreviousPage.Should().BeNull();
        result.Updates.NextPage.Should().Be(2);
        result.Updates.NbResults.Should().Be(66);
        result.Updates.NbPages.Should().Be(11);
        result.Updates.CurrentPageResults.Should().HaveCount(2);

        var first = result.Updates.CurrentPageResults[0];
        first.Seen.Should().BeFalse();
        first.Message.Id.Should().Be("AAAAAAAI");
        first.Message.Text.Should().StartWith("Let's get everyone");
        first.Message.Date.Should().Be(DateTimeOffset.FromUnixTimeMilliseconds(1789801389222));
        first.Message.Sender.Id.Should().Be("guang");
        first.Message.Sender.Name.Should().Be("Guang");
        first.Message.Team.Should().NotBeNull();
        first.Message.Team!.Id.Should().Be("the-fischermen");
        first.Message.Team.Flair.Should().Be("people.older-person-medium-skin-tone");

        var second = result.Updates.CurrentPageResults[1];
        second.Seen.Should().BeTrue();
        second.Message.Sender.Patron.Should().BeTrue();
        second.Message.Sender.PatronColor.Should().Be(1);
        second.Message.Team!.Flair.Should().BeNull();

        result.ByTeam.Should().HaveCount(2);
        result.ByTeam[0].Team.Id.Should().Be("the-fischermen");
        result.ByTeam[0].Unread.Should().Be(2);
        result.ByTeam[0].Last.Should().Be(DateTimeOffset.FromUnixTimeMilliseconds(1789801389222));
        result.ByTeam[1].Unread.Should().Be(0);
    }

    [Fact]
    public void Deserialize_TeamUpdatesOfTeam_PopulatesTeamAndSubscription()
    {
        var result = JsonSerializer.Deserialize<TeamUpdatesOfTeam>(UpdatesOfTeamJson, _options);

        result.Should().NotBeNull();
        result!.Team.Id.Should().Be("open-chess-club");
        result.Team.Name.Should().Be("Open Chess Club");
        result.Subscribed.Should().BeTrue();
        result.Updates.NextPage.Should().BeNull();
        result.Updates.CurrentPageResults.Should().ContainSingle()
            .Which.Message.Sender.Flair.Should().Be("nature.crab");
        result.ByTeam.Should().ContainSingle().Which.Unread.Should().Be(2);
    }
}
