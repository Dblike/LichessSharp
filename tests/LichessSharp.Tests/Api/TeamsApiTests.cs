using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models.Common;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class TeamsApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly TeamsApi _teamsApi;

    public TeamsApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _teamsApi = new TeamsApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new TeamsApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task GetAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "lichess-swiss";
        var expectedTeam = CreateTestTeam(teamId);
        _httpClientMock
            .Setup(x => x.GetAsync<Team>($"/api/team/{teamId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTeam);

        // Act
        var result = await _teamsApi.GetAsync(teamId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(teamId);
        _httpClientMock.Verify(x => x.GetAsync<Team>($"/api/team/{teamId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithNullTeamId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.GetAsync(null!));
    }

    [Fact]
    public async Task GetAsync_WithEmptyTeamId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _teamsApi.GetAsync(""));
    }

    [Fact]
    public async Task GetAsync_UrlEncodesTeamId()
    {
        // Arrange
        var teamId = "team with spaces";
        var expectedTeam = CreateTestTeam(teamId);
        _httpClientMock
            .Setup(x => x.GetAsync<Team>(It.Is<string>(s => s.Contains("team%20with%20spaces")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTeam);

        // Act
        await _teamsApi.GetAsync(teamId);

        // Assert
        _httpClientMock.Verify(
            x => x.GetAsync<Team>(It.Is<string>(s => s.Contains("team%20with%20spaces")),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPopularAsync_WithDefaultPage_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResult = CreateTestPaginator();
        _httpClientMock
            .Setup(x => x.GetAsync<TeamPaginator>("/api/team/all", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _teamsApi.GetPopularAsync();

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAsync<TeamPaginator>("/api/team/all", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPopularAsync_WithPage_IncludesQueryParameter()
    {
        // Arrange
        var expectedResult = CreateTestPaginator();
        _httpClientMock
            .Setup(x => x.GetAsync<TeamPaginator>("/api/team/all?page=3", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _teamsApi.GetPopularAsync(3);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAsync<TeamPaginator>("/api/team/all?page=3", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPopularAsync_WithInvalidPage_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await _teamsApi.GetPopularAsync(0));
    }

    [Fact]
    public async Task GetUserTeamsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "DrNykterstein";
        var expectedTeams = new List<Team> { CreateTestTeam("team1"), CreateTestTeam("team2") };
        _httpClientMock
            .Setup(x => x.GetAsync<List<Team>>($"/api/team/of/{username}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTeams);

        // Act
        var result = await _teamsApi.GetUserTeamsAsync(username);

        // Assert
        result.Should().HaveCount(2);
        _httpClientMock.Verify(x => x.GetAsync<List<Team>>($"/api/team/of/{username}", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetUserTeamsAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.GetUserTeamsAsync(null!));
    }

    [Fact]
    public async Task SearchAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var searchText = "chess";
        var expectedResult = CreateTestPaginator();
        _httpClientMock
            .Setup(x => x.GetAsync<TeamPaginator>(It.Is<string>(s => s.Contains("/api/team/search?text=chess")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _teamsApi.SearchAsync(searchText);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(
            x => x.GetAsync<TeamPaginator>(It.Is<string>(s => s.Contains("/api/team/search?text=chess")),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithPage_IncludesPageParameter()
    {
        // Arrange
        var expectedResult = CreateTestPaginator();
        _httpClientMock
            .Setup(x => x.GetAsync<TeamPaginator>(It.Is<string>(s => s.Contains("text=test") && s.Contains("page=2")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        await _teamsApi.SearchAsync("test", 2);

        // Assert
        _httpClientMock.Verify(
            x => x.GetAsync<TeamPaginator>(It.Is<string>(s => s.Contains("text=test") && s.Contains("page=2")),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithNullText_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.SearchAsync(null!));
    }

    [Fact]
    public async Task SearchAsync_WithInvalidPage_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await _teamsApi.SearchAsync("test", 0));
    }

    [Fact]
    public async Task SearchAsync_UrlEncodesSearchText()
    {
        // Arrange
        var searchText = "chess club";
        var expectedResult = CreateTestPaginator();
        _httpClientMock
            .Setup(x => x.GetAsync<TeamPaginator>(It.Is<string>(s => s.Contains("chess%20club")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        await _teamsApi.SearchAsync(searchText);

        // Assert
        _httpClientMock.Verify(
            x => x.GetAsync<TeamPaginator>(It.Is<string>(s => s.Contains("chess%20club")),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StreamMembersAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "lichess-swiss";
        var members = new List<TeamMember>
        {
            new() { Id = "user1", Name = "User1" },
            new() { Id = "user2", Name = "User2" }
        };
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<TeamMember>($"/api/team/{teamId}/users", It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(members));

        // Act
        var results = new List<TeamMember>();
        await foreach (var member in _teamsApi.StreamMembersAsync(teamId)) results.Add(member);

        // Assert
        results.Should().HaveCount(2);
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<TeamMember>($"/api/team/{teamId}/users", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StreamMembersAsync_WithFullTrue_IncludesQueryParameter()
    {
        // Arrange
        var teamId = "lichess-swiss";
        var members = new List<TeamMember>();
        _httpClientMock
            .Setup(x => x.StreamNdjsonAsync<TeamMember>($"/api/team/{teamId}/users?full=true",
                It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(members));

        // Act
        await foreach (var _ in _teamsApi.StreamMembersAsync(teamId, true))
        {
        }

        // Assert
        _httpClientMock.Verify(
            x => x.StreamNdjsonAsync<TeamMember>($"/api/team/{teamId}/users?full=true", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StreamMembersAsync_WithNullTeamId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in _teamsApi.StreamMembersAsync(null!))
            {
            }
        });
    }

    [Fact]
    public async Task JoinAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "lichess-swiss";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/team/{teamId}/join", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _teamsApi.JoinAsync(teamId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/team/{teamId}/join", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinAsync_WithMessage_IncludesFormContent()
    {
        // Arrange
        var teamId = "lichess-swiss";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/team/{teamId}/join", It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _teamsApi.JoinAsync(teamId, "I would like to join!");

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/team/{teamId}/join", It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinAsync_WithNullTeamId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.JoinAsync(null!));
    }

    [Fact]
    public async Task LeaveAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "lichess-swiss";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/team/{teamId}/quit", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _teamsApi.LeaveAsync(teamId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/team/{teamId}/quit", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LeaveAsync_WithNullTeamId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.LeaveAsync(null!));
    }

    [Fact]
    public async Task GetJoinRequestsAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "my-team";
        var expectedRequests = new List<TeamRequestWithUser>();
        _httpClientMock
            .Setup(x => x.GetAsync<List<TeamRequestWithUser>>($"/api/team/{teamId}/requests",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRequests);

        // Act
        var result = await _teamsApi.GetJoinRequestsAsync(teamId);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(
            x => x.GetAsync<List<TeamRequestWithUser>>($"/api/team/{teamId}/requests", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetJoinRequestsAsync_WithDeclinedTrue_IncludesQueryParameter()
    {
        // Arrange
        var teamId = "my-team";
        var expectedRequests = new List<TeamRequestWithUser>();
        _httpClientMock
            .Setup(x => x.GetAsync<List<TeamRequestWithUser>>($"/api/team/{teamId}/requests?declined=true",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRequests);

        // Act
        await _teamsApi.GetJoinRequestsAsync(teamId, true);

        // Assert
        _httpClientMock.Verify(
            x => x.GetAsync<List<TeamRequestWithUser>>($"/api/team/{teamId}/requests?declined=true",
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcceptJoinRequestAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "my-team";
        var userId = "newuser";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/team/{teamId}/request/{userId}/accept", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _teamsApi.AcceptJoinRequestAsync(teamId, userId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/team/{teamId}/request/{userId}/accept", null,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcceptJoinRequestAsync_WithNullTeamId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.AcceptJoinRequestAsync(null!, "user"));
    }

    [Fact]
    public async Task AcceptJoinRequestAsync_WithNullUserId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.AcceptJoinRequestAsync("team", null!));
    }

    [Fact]
    public async Task DeclineJoinRequestAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "my-team";
        var userId = "newuser";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/team/{teamId}/request/{userId}/decline", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _teamsApi.DeclineJoinRequestAsync(teamId, userId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/team/{teamId}/request/{userId}/decline", null,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task KickMemberAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "my-team";
        var userId = "troublemaker";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/team/{teamId}/kick/{userId}", null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _teamsApi.KickMemberAsync(teamId, userId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/api/team/{teamId}/kick/{userId}", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task KickMemberAsync_WithNullTeamId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.KickMemberAsync(null!, "user"));
    }

    [Fact]
    public async Task KickMemberAsync_WithNullUserId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.KickMemberAsync("team", null!));
    }

    [Fact]
    public async Task MessageAllMembersAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var teamId = "my-team";
        var message = "Hello everyone!";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/team/{teamId}/pm-all", It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _teamsApi.MessageAllMembersAsync(teamId, message);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(
            x => x.PostAsync<OkResponse>($"/team/{teamId}/pm-all", It.IsAny<HttpContent>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MessageAllMembersAsync_WithNullTeamId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.MessageAllMembersAsync(null!, "Hello"));
    }

    [Fact]
    public async Task MessageAllMembersAsync_WithNullMessage_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _teamsApi.MessageAllMembersAsync("team", null!));
    }

    private static Team CreateTestTeam(string id)
    {
        return new Team
        {
            Id = id,
            Name = $"Team {id}",
            Description = "A test team",
            NbMembers = 100,
            Open = true
        };
    }

    private static TeamPaginator CreateTestPaginator()
    {
        return new TeamPaginator
        {
            CurrentPage = 1,
            MaxPerPage = 15,
            CurrentPageResults = [CreateTestTeam("team1"), CreateTestTeam("team2")],
            NbResults = 100,
            NbPages = 7
        };
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items) yield return item;
        await Task.CompletedTask;
    }
}