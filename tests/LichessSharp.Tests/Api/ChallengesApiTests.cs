using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using LichessSharp.Models.Common;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class ChallengesApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly ChallengesApi _challengesApi;

    public ChallengesApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _challengesApi = new ChallengesApi(_httpClientMock.Object);
    }
    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new ChallengesApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task GetPendingAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResult = new ChallengeList
        {
            In = new List<ChallengeJson> { CreateTestChallenge("in1") },
            Out = new List<ChallengeJson> { CreateTestChallenge("out1") }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<ChallengeList>("/api/challenge", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _challengesApi.GetPendingAsync();

        // Assert
        result.Should().NotBeNull();
        result.In.Should().HaveCount(1);
        result.Out.Should().HaveCount(1);
        _httpClientMock.Verify(x => x.GetAsync<ChallengeList>("/api/challenge", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShowAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var challengeId = "abc123";
        var expectedResult = CreateTestChallenge(challengeId);
        _httpClientMock
            .Setup(x => x.GetAsync<ChallengeJson>($"/api/challenge/{challengeId}/show", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _challengesApi.ShowAsync(challengeId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(challengeId);
        _httpClientMock.Verify(x => x.GetAsync<ChallengeJson>($"/api/challenge/{challengeId}/show", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShowAsync_WithNullChallengeId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _challengesApi.ShowAsync(null!));
    }

    [Fact]
    public async Task ShowAsync_WithEmptyChallengeId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _challengesApi.ShowAsync(""));
    }

    [Fact]
    public async Task CreateAsync_WithUsername_CallsCorrectEndpoint()
    {
        // Arrange
        var username = "opponent";
        var expectedResult = CreateTestChallenge("newchallenge");
        _httpClientMock
            .Setup(x => x.PostAsync<ChallengeJson>($"/api/challenge/{username}", It.IsAny<HttpContent?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _challengesApi.CreateAsync(username);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.PostAsync<ChallengeJson>($"/api/challenge/{username}", It.IsAny<HttpContent?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullUsername_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _challengesApi.CreateAsync(null!));
    }

    [Fact]
    public async Task CreateAsync_UrlEncodesUsername()
    {
        // Arrange
        var username = "user name";
        var expectedResult = CreateTestChallenge("newchallenge");
        _httpClientMock
            .Setup(x => x.PostAsync<ChallengeJson>(It.Is<string>(s => s.Contains("user%20name")), It.IsAny<HttpContent?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        await _challengesApi.CreateAsync(username);

        // Assert
        _httpClientMock.Verify(x => x.PostAsync<ChallengeJson>(It.Is<string>(s => s.Contains("user%20name")), It.IsAny<HttpContent?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcceptAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var challengeId = "abc123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/challenge/{challengeId}/accept", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _challengesApi.AcceptAsync(challengeId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/challenge/{challengeId}/accept", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcceptAsync_WithNullChallengeId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _challengesApi.AcceptAsync(null!));
    }

    [Fact]
    public async Task DeclineAsync_WithoutReason_CallsCorrectEndpoint()
    {
        // Arrange
        var challengeId = "abc123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/challenge/{challengeId}/decline", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _challengesApi.DeclineAsync(challengeId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/challenge/{challengeId}/decline", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeclineAsync_WithReason_IncludesReasonInContent()
    {
        // Arrange
        var challengeId = "abc123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/challenge/{challengeId}/decline", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _challengesApi.DeclineAsync(challengeId, ChallengeDeclineReason.TooFast);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/challenge/{challengeId}/decline", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeclineAsync_WithNullChallengeId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _challengesApi.DeclineAsync(null!));
    }

    [Fact]
    public async Task CancelAsync_WithoutOpponentToken_CallsCorrectEndpoint()
    {
        // Arrange
        var challengeId = "abc123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/challenge/{challengeId}/cancel", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _challengesApi.CancelAsync(challengeId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/challenge/{challengeId}/cancel", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_WithOpponentToken_IncludesTokenInQuery()
    {
        // Arrange
        var challengeId = "abc123";
        var opponentToken = "token123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("opponentToken=token123")), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _challengesApi.CancelAsync(challengeId, opponentToken);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("opponentToken=token123")), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChallengeAiAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var options = new ChallengeAiOptions { Level = 5 };
        var expectedResult = new ChallengeAiResponse { Id = "aigame123" };
        _httpClientMock
            .Setup(x => x.PostAsync<ChallengeAiResponse>("/api/challenge/ai", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _challengesApi.ChallengeAiAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("aigame123");
        _httpClientMock.Verify(x => x.PostAsync<ChallengeAiResponse>("/api/challenge/ai", It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChallengeAiAsync_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _challengesApi.ChallengeAiAsync(null!));
    }

    [Fact]
    public async Task ChallengeAiAsync_WithInvalidLevel_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new ChallengeAiOptions { Level = 0 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await _challengesApi.ChallengeAiAsync(options));
    }

    [Fact]
    public async Task ChallengeAiAsync_WithLevelAbove8_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new ChallengeAiOptions { Level = 9 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await _challengesApi.ChallengeAiAsync(options));
    }

    [Fact]
    public async Task CreateOpenAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResult = new ChallengeOpenJson { Id = "open123", Url = "https://lichess.org/open123", Status = "created", Rated = false };
        _httpClientMock
            .Setup(x => x.PostAsync<ChallengeOpenJson>("/api/challenge/open", It.IsAny<HttpContent?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _challengesApi.CreateOpenAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("open123");
        _httpClientMock.Verify(x => x.PostAsync<ChallengeOpenJson>("/api/challenge/open", It.IsAny<HttpContent?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StartClocksAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/challenge/{gameId}/start-clocks", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _challengesApi.StartClocksAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/challenge/{gameId}/start-clocks", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StartClocksAsync_WithTokens_IncludesTokensInQuery()
    {
        // Arrange
        var gameId = "game123";
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("token1=t1") && s.Contains("token2=t2")), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _challengesApi.StartClocksAsync(gameId, "t1", "t2");

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>(It.Is<string>(s => s.Contains("token1=t1") && s.Contains("token2=t2")), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddTimeAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var gameId = "game123";
        var seconds = 15;
        _httpClientMock
            .Setup(x => x.PostAsync<OkResponse>($"/api/round/{gameId}/add-time/{seconds}", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResponse { Ok = true });

        // Act
        var result = await _challengesApi.AddTimeAsync(gameId, seconds);

        // Assert
        result.Should().BeTrue();
        _httpClientMock.Verify(x => x.PostAsync<OkResponse>($"/api/round/{gameId}/add-time/{seconds}", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddTimeAsync_WithSecondsBelowRange_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await _challengesApi.AddTimeAsync("game123", 0));
    }

    [Fact]
    public async Task AddTimeAsync_WithSecondsAboveRange_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await _challengesApi.AddTimeAsync("game123", 61));
    }

    private static ChallengeJson CreateTestChallenge(string id) => new()
    {
        Id = id,
        Url = $"https://lichess.org/{id}",
        Status = "created",
        Rated = false
    };

}
