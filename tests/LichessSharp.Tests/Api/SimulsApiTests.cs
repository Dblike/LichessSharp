using FluentAssertions;
using LichessSharp.Api;
using LichessSharp.Api.Contracts;
using LichessSharp.Http;
using Moq;
using Xunit;

namespace LichessSharp.Tests.Api;

public class SimulsApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly SimulsApi _simulsApi;

    public SimulsApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _simulsApi = new SimulsApi(_httpClientMock.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new SimulsApi(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public async Task GetCurrentAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedResult = CreateTestSimulList();
        _httpClientMock
            .Setup(x => x.GetAsync<SimulList>("/api/simul", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _simulsApi.GetCurrentAsync();

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.GetAsync<SimulList>("/api/simul", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCurrentAsync_ReturnsPendingSimuls()
    {
        // Arrange
        var expectedResult = new SimulList
        {
            Pending = new List<Simul> { CreateTestSimul("pending1") },
            Created = new List<Simul>(),
            Started = new List<Simul>(),
            Finished = new List<Simul>()
        };
        _httpClientMock
            .Setup(x => x.GetAsync<SimulList>("/api/simul", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _simulsApi.GetCurrentAsync();

        // Assert
        result.Pending.Should().HaveCount(1);
        result.Pending[0].Id.Should().Be("pending1");
    }

    [Fact]
    public async Task GetCurrentAsync_ReturnsCreatedSimuls()
    {
        // Arrange
        var expectedResult = new SimulList
        {
            Pending = new List<Simul>(),
            Created = new List<Simul> { CreateTestSimul("created1"), CreateTestSimul("created2") },
            Started = new List<Simul>(),
            Finished = new List<Simul>()
        };
        _httpClientMock
            .Setup(x => x.GetAsync<SimulList>("/api/simul", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _simulsApi.GetCurrentAsync();

        // Assert
        result.Created.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCurrentAsync_ReturnsStartedSimuls()
    {
        // Arrange
        var simul = CreateTestSimul("started1");
        var expectedResult = new SimulList
        {
            Pending = new List<Simul>(),
            Created = new List<Simul>(),
            Started = new List<Simul> { simul },
            Finished = new List<Simul>()
        };
        _httpClientMock
            .Setup(x => x.GetAsync<SimulList>("/api/simul", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _simulsApi.GetCurrentAsync();

        // Assert
        result.Started.Should().HaveCount(1);
        result.Started[0].IsRunning.Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrentAsync_ReturnsFinishedSimuls()
    {
        // Arrange
        var expectedResult = new SimulList
        {
            Pending = new List<Simul>(),
            Created = new List<Simul>(),
            Started = new List<Simul>(),
            Finished = new List<Simul> { CreateTestSimul("finished1") }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<SimulList>("/api/simul", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _simulsApi.GetCurrentAsync();

        // Assert
        result.Finished.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetCurrentAsync_WithCancellationToken_PassesToken()
    {
        // Arrange
        var expectedResult = CreateTestSimulList();
        var cts = new CancellationTokenSource();
        _httpClientMock
            .Setup(x => x.GetAsync<SimulList>("/api/simul", cts.Token))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _simulsApi.GetCurrentAsync(cts.Token);

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<SimulList>("/api/simul", cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetCurrentAsync_ReturnsSimulWithCorrectHostInfo()
    {
        // Arrange
        var simul = new Simul
        {
            Id = "test123",
            Name = "Test Simul",
            FullName = "GM Test Host Test Simul",
            Host = new SimulHost
            {
                Id = "testhost",
                Name = "TestHost",
                Title = "GM",
                Rating = 2800,
                Online = true
            },
            Variants = new List<SimulVariant>
            {
                new() { Key = "standard", Name = "Standard" }
            },
            IsCreated = false,
            IsRunning = true,
            IsFinished = false,
            NbApplicants = 10,
            NbPairings = 5
        };
        var expectedResult = new SimulList
        {
            Pending = new List<Simul>(),
            Created = new List<Simul>(),
            Started = new List<Simul> { simul },
            Finished = new List<Simul>()
        };
        _httpClientMock
            .Setup(x => x.GetAsync<SimulList>("/api/simul", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _simulsApi.GetCurrentAsync();

        // Assert
        result.Started.Should().HaveCount(1);
        result.Started[0].Host.Title.Should().Be("GM");
        result.Started[0].Host.Rating.Should().Be(2800);
        result.Started[0].Host.Online.Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrentAsync_ReturnsSimulWithMultipleVariants()
    {
        // Arrange
        var simul = new Simul
        {
            Id = "test123",
            Name = "Multi-Variant Simul",
            FullName = "Multi-Variant Simul",
            Host = new SimulHost { Id = "host", Name = "Host" },
            Variants = new List<SimulVariant>
            {
                new() { Key = "standard", Name = "Standard" },
                new() { Key = "chess960", Name = "Chess960" }
            },
            IsCreated = true,
            IsRunning = false,
            IsFinished = false,
            NbApplicants = 0,
            NbPairings = 0
        };
        var expectedResult = new SimulList
        {
            Pending = new List<Simul>(),
            Created = new List<Simul> { simul },
            Started = new List<Simul>(),
            Finished = new List<Simul>()
        };
        _httpClientMock
            .Setup(x => x.GetAsync<SimulList>("/api/simul", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _simulsApi.GetCurrentAsync();

        // Assert
        result.Created[0].Variants.Should().HaveCount(2);
        result.Created[0].Variants[0].Key.Should().Be("standard");
        result.Created[0].Variants[1].Key.Should().Be("chess960");
    }

    private static SimulList CreateTestSimulList()
    {
        return new SimulList
        {
            Pending = new List<Simul>(),
            Created = new List<Simul> { CreateTestSimul("created1") },
            Started = new List<Simul> { CreateTestSimul("started1") },
            Finished = new List<Simul> { CreateTestSimul("finished1") }
        };
    }

    private static Simul CreateTestSimul(string id)
    {
        return new Simul
        {
            Id = id,
            Name = $"Test Simul {id}",
            FullName = $"Test Simul {id}",
            Host = new SimulHost
            {
                Id = "testhost",
                Name = "TestHost"
            },
            Variants = new List<SimulVariant>
            {
                new() { Key = "standard", Name = "Standard" }
            },
            IsCreated = id.StartsWith("created"),
            IsRunning = id.StartsWith("started"),
            IsFinished = id.StartsWith("finished"),
            NbApplicants = 5,
            NbPairings = 3
        };
    }
}