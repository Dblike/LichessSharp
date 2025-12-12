using FluentAssertions;
using LichessSharp.Api.Contracts;
using LichessSharp.Exceptions;
using Xunit;

namespace LichessSharp.Tests.Integration;

/// <summary>
///     Integration tests for the Teams API.
///     These tests make real HTTP calls to Lichess.
/// </summary>
[IntegrationTest]
[Trait("Category", "Integration")]
public class TeamsApiIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task GetAsync_WithValidTeamId_ReturnsTeamInfo()
    {
        // Arrange
        var teamId = "lichess-swiss"; // A well-known Lichess team

        // Act
        var team = await Client.Teams.GetAsync(teamId);

        // Assert
        team.Should().NotBeNull();
        team.Id.Should().Be(teamId);
        team.Name.Should().NotBeNullOrWhiteSpace();
        team.NbMembers.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAsync_WithInvalidTeamId_ThrowsLichessException()
    {
        // Arrange
        var invalidTeamId = "nonexistent-team-12345";

        // Act & Assert
        var act = async () => await Client.Teams.GetAsync(invalidTeamId);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task GetPopularAsync_ReturnsTeams()
    {
        // Act
        var result = await Client.Teams.GetPopularAsync();

        // Assert
        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(1);
        result.CurrentPageResults.Should().NotBeNull();
        result.CurrentPageResults.Should().NotBeEmpty();
        result.NbResults.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetPopularAsync_WithPage2_ReturnsDifferentTeams()
    {
        // Act
        var page1 = await Client.Teams.GetPopularAsync(1);
        var page2 = await Client.Teams.GetPopularAsync(2);

        // Assert
        page1.Should().NotBeNull();
        page2.Should().NotBeNull();
        page2.CurrentPage.Should().Be(2);

        // The teams should be different (unless there are very few total teams)
        if (page1.NbPages > 1)
            page1.CurrentPageResults!.First().Id.Should().NotBe(page2.CurrentPageResults!.First().Id);
    }

    [Fact]
    public async Task GetUserTeamsAsync_WithValidUser_ReturnsTeams()
    {
        // Arrange
        var username = "thibault"; // Lichess founder, member of many teams

        // Act
        var teams = await Client.Teams.GetUserTeamsAsync(username);

        // Assert
        teams.Should().NotBeNull();
        // thibault should be part of at least one team
        teams.Should().NotBeEmpty("thibault should be a member of at least one team");
    }

    [Fact]
    public async Task GetUserTeamsAsync_WithInvalidUser_ThrowsLichessException()
    {
        // Arrange
        var invalidUsername = "nonexistent_user_12345";

        // Act & Assert
        var act = async () => await Client.Teams.GetUserTeamsAsync(invalidUsername);

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task SearchAsync_WithValidTerm_ReturnsResults()
    {
        // Arrange
        var searchTerm = "chess";

        // Act
        var result = await Client.Teams.SearchAsync(searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.CurrentPageResults.Should().NotBeNull();
        result.CurrentPageResults.Should().NotBeEmpty("searching for 'chess' should return some teams");
        result.NbResults.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SearchAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var searchTerm = "chess";

        // Act
        var result = await Client.Teams.SearchAsync(searchTerm, 2);

        // Assert
        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(2);
    }

    [Fact]
    public async Task StreamMembersAsync_WithValidTeam_ReturnsMembers()
    {
        // Arrange
        var teamId = "lichess-swiss";
        var members = new List<TeamMember>();

        // Act
        await foreach (var member in Client.Teams.StreamMembersAsync(teamId))
        {
            members.Add(member);
            if (members.Count >= 5) break; // Just get a few members for the test
        }

        // Assert
        members.Should().NotBeEmpty("lichess-swiss should have members");
        members[0].Id.Should().NotBeNullOrWhiteSpace();
        members[0].Name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task JoinAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var teamId = "lichess-swiss";

        // Act & Assert
        var act = async () => await Client.Teams.JoinAsync(teamId);

        await act.Should().ThrowAsync<LichessException>()
            .WithMessage("Access denied. Your token may not have the required scope for this operation.");
    }

    [Fact]
    public async Task LeaveAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var teamId = "lichess-swiss";

        // Act & Assert
        var act = async () => await Client.Teams.LeaveAsync(teamId);

        await act.Should().ThrowAsync<LichessException>()
            .WithMessage("Access denied. Your token may not have the required scope for this operation.");
    }

    [Fact]
    public async Task GetJoinRequestsAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Arrange
        var teamId = "lichess-swiss";

        // Act & Assert
        var act = async () => await Client.Teams.GetJoinRequestsAsync(teamId);

        await act.Should().ThrowAsync<LichessException>()
            .WithMessage("Authentication failed. Please check your access token.");
    }

    [Fact]
    public async Task AcceptJoinRequestAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Act & Assert
        var act = async () => await Client.Teams.AcceptJoinRequestAsync("some-team", "some-user");

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task DeclineJoinRequestAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Act & Assert
        var act = async () => await Client.Teams.DeclineJoinRequestAsync("some-team", "some-user");

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task KickMemberAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Act & Assert
        var act = async () => await Client.Teams.KickMemberAsync("some-team", "some-user");

        await act.Should().ThrowAsync<LichessException>();
    }

    [Fact]
    public async Task MessageAllMembersAsync_WithoutAuthentication_ThrowsLichessException()
    {
        // Act & Assert
        var act = async () => await Client.Teams.MessageAllMembersAsync("some-team", "Hello!");

        await act.Should().ThrowAsync<LichessException>()
            .WithMessage("Access denied. Your token may not have the required scope for this operation.");
    }
}