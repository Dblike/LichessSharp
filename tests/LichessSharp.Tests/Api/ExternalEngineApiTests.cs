using FluentAssertions;

using LichessSharp.Api;
using LichessSharp.Http;

using Moq;

using Xunit;

namespace LichessSharp.Tests.Api;

public class ExternalEngineApiTests
{
    private readonly Mock<ILichessHttpClient> _httpClientMock;
    private readonly ExternalEngineApi _api;
    private readonly Uri _engineBaseAddress = new("https://engine.lichess.ovh");

    public ExternalEngineApiTests()
    {
        _httpClientMock = new Mock<ILichessHttpClient>();
        _api = new ExternalEngineApi(_httpClientMock.Object, _engineBaseAddress);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new ExternalEngineApi(null!, _engineBaseAddress);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClient");
    }

    [Fact]
    public void Constructor_WithNullEngineBaseAddress_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new ExternalEngineApi(_httpClientMock.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("engineBaseAddress");
    }

    #endregion

    #region ListAsync Tests

    [Fact]
    public async Task ListAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var expectedEngines = new List<ExternalEngine>
        {
            CreateTestEngine("engine1"),
            CreateTestEngine("engine2")
        };
        _httpClientMock
            .Setup(x => x.GetAsync<List<ExternalEngine>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEngines);

        // Act
        var result = await _api.ListAsync();

        // Assert
        result.Should().HaveCount(2);
        _httpClientMock.Verify(x => x.GetAsync<List<ExternalEngine>>(
            "/api/external-engine",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidRegistration_CallsCorrectEndpoint()
    {
        // Arrange
        var registration = CreateValidRegistration();
        var expectedEngine = CreateTestEngine("new-engine");

        _httpClientMock
            .Setup(x => x.PostJsonAsync<ExternalEngine>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEngine);

        // Act
        var result = await _api.CreateAsync(registration);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("new-engine");
        _httpClientMock.Verify(x => x.PostJsonAsync<ExternalEngine>(
            "/api/external-engine",
            It.IsAny<object>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullRegistration_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _api.CreateAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("registration");
    }

    [Fact]
    public async Task CreateAsync_WithTooShortName_ThrowsArgumentException()
    {
        // Arrange
        var registration = new ExternalEngineRegistration
        {
            Name = "AB",
            MaxThreads = 8,
            MaxHash = 2048,
            ProviderSecret = "this-is-a-long-enough-secret"
        };

        // Act
        var act = () => _api.CreateAsync(registration);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateAsync_WithInvalidMaxThreads_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var registration = new ExternalEngineRegistration
        {
            Name = "Test Engine",
            MaxThreads = 0,
            MaxHash = 2048,
            ProviderSecret = "this-is-a-long-enough-secret"
        };

        // Act
        var act = () => _api.CreateAsync(registration);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task CreateAsync_WithInvalidMaxHash_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var registration = new ExternalEngineRegistration
        {
            Name = "Test Engine",
            MaxThreads = 8,
            MaxHash = 0,
            ProviderSecret = "this-is-a-long-enough-secret"
        };

        // Act
        var act = () => _api.CreateAsync(registration);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task CreateAsync_WithTooShortProviderSecret_ThrowsArgumentException()
    {
        // Arrange
        var registration = new ExternalEngineRegistration
        {
            Name = "Test Engine",
            MaxThreads = 8,
            MaxHash = 2048,
            ProviderSecret = "short"
        };

        // Act
        var act = () => _api.CreateAsync(registration);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithValidId_CallsCorrectEndpoint()
    {
        // Arrange
        var engineId = "test-engine-id";
        var expectedEngine = CreateTestEngine(engineId);

        _httpClientMock
            .Setup(x => x.GetAsync<ExternalEngine>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEngine);

        // Act
        var result = await _api.GetAsync(engineId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(engineId);
        _httpClientMock.Verify(x => x.GetAsync<ExternalEngine>(
            $"/api/external-engine/{engineId}",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithNullEngineId_ThrowsArgumentException()
    {
        // Act
        var act = () => _api.GetAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_WithEmptyEngineId_ThrowsArgumentException()
    {
        // Act
        var act = () => _api.GetAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_CallsCorrectEndpoint()
    {
        // Arrange
        var engineId = "test-engine-id";
        var registration = CreateValidRegistration();
        var expectedEngine = CreateTestEngine(engineId);

        _httpClientMock
            .Setup(x => x.PutJsonAsync<ExternalEngine>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEngine);

        // Act
        var result = await _api.UpdateAsync(engineId, registration);

        // Assert
        result.Should().NotBeNull();
        _httpClientMock.Verify(x => x.PutJsonAsync<ExternalEngine>(
            $"/api/external-engine/{engineId}",
            It.IsAny<object>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNullEngineId_ThrowsArgumentException()
    {
        // Act
        var act = () => _api.UpdateAsync(null!, CreateValidRegistration());

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_WithNullRegistration_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _api.UpdateAsync("engine-id", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("registration");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_CallsCorrectEndpoint()
    {
        // Arrange
        var engineId = "test-engine-id";

        _httpClientMock
            .Setup(x => x.DeleteNoContentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _api.DeleteAsync(engineId);

        // Assert
        _httpClientMock.Verify(x => x.DeleteNoContentAsync(
            $"/api/external-engine/{engineId}",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNullEngineId_ThrowsArgumentException()
    {
        // Act
        var act = () => _api.DeleteAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region AnalyseAsync Tests

    [Fact]
    public async Task AnalyseAsync_WithNullEngineId_ThrowsArgumentException()
    {
        // Arrange
        var request = CreateValidAnalysisRequest();

        // Act
        var act = async () => await _api.AnalyseAsync(null!, request).ToListAsync();

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AnalyseAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act
        var act = async () => await _api.AnalyseAsync("engine-id", null!).ToListAsync();

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AnalyseAsync_WithNullClientSecret_ThrowsArgumentException()
    {
        // Arrange
        var request = new EngineAnalysisRequest
        {
            ClientSecret = null!,
            Work = new EngineAnalysisWork { InitialFen = "startpos" }
        };

        // Act
        var act = async () => await _api.AnalyseAsync("engine-id", request).ToListAsync();

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region AcquireWorkAsync Tests

    [Fact]
    public async Task AcquireWorkAsync_WithNullProviderSecret_ThrowsArgumentException()
    {
        // Act
        var act = () => _api.AcquireWorkAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AcquireWorkAsync_WithEmptyProviderSecret_ThrowsArgumentException()
    {
        // Act
        var act = () => _api.AcquireWorkAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region SubmitWorkAsync Tests

    [Fact]
    public async Task SubmitWorkAsync_WithNullWorkId_ThrowsArgumentException()
    {
        // Arrange
        var lines = AsyncEnumerable.Empty<string>();

        // Act
        var act = () => _api.SubmitWorkAsync(null!, lines);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SubmitWorkAsync_WithNullLines_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _api.SubmitWorkAsync("work-id", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region Helper Methods

    private static ExternalEngine CreateTestEngine(string id) => new()
    {
        Id = id,
        Name = "Test Engine",
        ClientSecret = "test-client-secret",
        UserId = "testuser",
        MaxThreads = 8,
        MaxHash = 2048,
        Variants = ["chess"],
        ProviderData = null
    };

    private static ExternalEngineRegistration CreateValidRegistration() => new()
    {
        Name = "Test Engine",
        MaxThreads = 8,
        MaxHash = 2048,
        ProviderSecret = "this-is-a-long-enough-secret"
    };

    private static EngineAnalysisRequest CreateValidAnalysisRequest() => new()
    {
        ClientSecret = "test-client-secret",
        Work = new EngineAnalysisWork
        {
            InitialFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        }
    };

    #endregion
}
