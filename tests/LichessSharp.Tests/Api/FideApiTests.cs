using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Http;
using LichessSharp.Models;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class FideApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly FideApi _fideApi;

    public FideApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _fideApi = new FideApi(_httpClientMock.Object);
    }


    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new FideApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }



    [Fact]
    public async Task GetPlayerAsync_WithValidId_CallsCorrectEndpoint()
    {
        // Arrange
        var playerId = 1503014;
        var expectedPlayer = CreateTestFidePlayer(playerId, "Carlsen, Magnus");
        _httpClientMock
            .Setup(x => x.GetAsync<FidePlayer>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPlayer);

        // Act
        var result = await _fideApi.GetPlayerAsync(playerId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(playerId);
        result.Name.Should().Be("Carlsen, Magnus");
        _httpClientMock.Verify(x => x.GetAsync<FidePlayer>(
            $"/api/fide/player/{playerId}",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPlayerAsync_WithZeroId_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => _fideApi.GetPlayerAsync(0);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("playerId");
    }

    [Fact]
    public async Task GetPlayerAsync_WithNegativeId_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => _fideApi.GetPlayerAsync(-1);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("playerId");
    }

    [Fact]
    public async Task GetPlayerAsync_ReturnsPlayerWithAllFields()
    {
        // Arrange
        var playerId = 1503014;
        var expectedPlayer = new FidePlayer
        {
            Id = playerId,
            Name = "Carlsen, Magnus",
            Title = "GM",
            Federation = "NOR",
            Year = 1990,
            Standard = 2830,
            Rapid = 2823,
            Blitz = 2886
        };
        _httpClientMock
            .Setup(x => x.GetAsync<FidePlayer>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPlayer);

        // Act
        var result = await _fideApi.GetPlayerAsync(playerId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(playerId);
        result.Name.Should().Be("Carlsen, Magnus");
        result.Title.Should().Be("GM");
        result.Federation.Should().Be("NOR");
        result.Year.Should().Be(1990);
        result.Standard.Should().Be(2830);
        result.Rapid.Should().Be(2823);
        result.Blitz.Should().Be(2886);
    }

    [Fact]
    public async Task GetPlayerAsync_WithCancellationToken_PassesTokenToHttpClient()
    {
        // Arrange
        var playerId = 1503014;
        var cts = new CancellationTokenSource();
        var expectedPlayer = CreateTestFidePlayer(playerId, "Carlsen, Magnus");
        _httpClientMock
            .Setup(x => x.GetAsync<FidePlayer>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPlayer);

        // Act
        await _fideApi.GetPlayerAsync(playerId, cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<FidePlayer>(
            It.IsAny<string>(),
            cts.Token), Times.Once);
    }



    [Fact]
    public async Task SearchPlayersAsync_WithValidQuery_CallsCorrectEndpoint()
    {
        // Arrange
        var query = "Carlsen";
        var expectedPlayers = new List<FidePlayer>
        {
            CreateTestFidePlayer(1503014, "Carlsen, Magnus"),
            CreateTestFidePlayer(1234567, "Carlsen, Henrik")
        };
        _httpClientMock
            .Setup(x => x.GetAsync<List<FidePlayer>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPlayers);

        // Act
        var result = await _fideApi.SearchPlayersAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _httpClientMock.Verify(x => x.GetAsync<List<FidePlayer>>(
            $"/api/fide/player?q={Uri.EscapeDataString(query)}",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchPlayersAsync_WithNullQuery_ThrowsArgumentException()
    {
        // Act
        var act = () => _fideApi.SearchPlayersAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SearchPlayersAsync_WithEmptyQuery_ThrowsArgumentException()
    {
        // Act
        var act = () => _fideApi.SearchPlayersAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SearchPlayersAsync_WithWhitespaceQuery_ThrowsArgumentException()
    {
        // Act
        var act = () => _fideApi.SearchPlayersAsync("   ");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SearchPlayersAsync_UrlEncodesQuery()
    {
        // Arrange
        var query = "Ding Liren";
        _httpClientMock
            .Setup(x => x.GetAsync<List<FidePlayer>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _fideApi.SearchPlayersAsync(query);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<List<FidePlayer>>(
            It.Is<string>(s => s.Contains("Ding%20Liren")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchPlayersAsync_WhenNoResults_ReturnsEmptyList()
    {
        // Arrange
        var query = "NonExistentPlayer12345";
        _httpClientMock
            .Setup(x => x.GetAsync<List<FidePlayer>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _fideApi.SearchPlayersAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchPlayersAsync_WithCancellationToken_PassesTokenToHttpClient()
    {
        // Arrange
        var query = "Carlsen";
        var cts = new CancellationTokenSource();
        _httpClientMock
            .Setup(x => x.GetAsync<List<FidePlayer>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _fideApi.SearchPlayersAsync(query, cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<List<FidePlayer>>(
            It.IsAny<string>(),
            cts.Token), Times.Once);
    }

    [Fact]
    public async Task SearchPlayersAsync_WithSpecialCharacters_UrlEncodesCorrectly()
    {
        // Arrange
        var query = "Müller & Sons";
        _httpClientMock
            .Setup(x => x.GetAsync<List<FidePlayer>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _fideApi.SearchPlayersAsync(query);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<List<FidePlayer>>(
            It.Is<string>(s =>
                s.Contains("%C3%BC") && // ü encoded
                s.Contains("%26")),     // & encoded
            It.IsAny<CancellationToken>()), Times.Once);
    }



    private static FidePlayer CreateTestFidePlayer(int id, string name) => new()
    {
        Id = id,
        Name = name,
        Title = "GM",
        Federation = "NOR",
        Year = 1990,
        Standard = 2830,
        Rapid = 2823,
        Blitz = 2886
    };

}
