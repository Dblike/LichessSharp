# Phased Implementation Plan: Missing Lichess API Endpoints

This document outlines a comprehensive phased plan for implementing the ~39 missing Lichess API endpoints, including unit testing and integration testing strategies.

## Table of Contents

1. [Overview](#overview)
2. [Testing Strategy](#testing-strategy)
3. [Phase 1: Users API Extensions](#phase-1-users-api-extensions)
4. [Phase 2: FIDE API](#phase-2-fide-api)
5. [Phase 3: Games API Extensions](#phase-3-games-api-extensions)
6. [Phase 4: Puzzles API Extensions](#phase-4-puzzles-api-extensions)
7. [Phase 5: Board & Bot API Extensions](#phase-5-board--bot-api-extensions)
8. [Phase 6: Broadcasts API Extensions](#phase-6-broadcasts-api-extensions)
9. [Phase 7: Opening Explorer Extensions](#phase-7-opening-explorer-extensions)
10. [Phase 8: Arena Tournaments Extensions](#phase-8-arena-tournaments-extensions)
11. [Phase 9: OAuth API](#phase-9-oauth-api)
12. [Phase 10: External Engine API](#phase-10-external-engine-api)
13. [Implementation Checklist](#implementation-checklist)

---

## Overview

### Missing Endpoint Summary

| Category | Missing Count | Priority | Auth Required |
|----------|---------------|----------|---------------|
| Users API | 8 | High | Mixed |
| FIDE API | 2 | High | No |
| Games API | 5 | High | Yes |
| Puzzles API | 4 | Medium | Mixed |
| Board/Bot API | 2 | Medium | Yes |
| Broadcasts API | 2 | Medium | No |
| Opening Explorer | 1 | Low | No |
| Arena Tournaments | 1 | Low | No |
| OAuth API | 4 | Low | Mixed |
| External Engine API | 8 | Low | Yes (Alpha API) |

### Implementation Order Rationale

1. **High-priority, unauthenticated endpoints first** - Easier to test, high user value
2. **Authenticated extensions to existing APIs** - Builds on existing patterns
3. **New API categories** - More infrastructure needed
4. **Alpha/experimental APIs last** - May change

---

## Testing Strategy

### Unit Test Structure

All unit tests follow the established pattern:
- Mock `ILichessHttpClient` using Moq
- Test correct endpoint URL construction
- Test query parameter building
- Test request body serialization
- Test response deserialization
- Test input validation (null checks, range checks)
- Test edge cases (empty results, optional parameters)

```csharp
// Example unit test pattern
[Fact]
public async Task MethodAsync_WithValidInput_CallsCorrectEndpoint()
{
    // Arrange
    var expectedResult = CreateTestModel();
    _httpClientMock
        .Setup(x => x.GetAsync<ExpectedType>("/api/endpoint", It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResult);

    // Act
    var result = await _api.MethodAsync("param");

    // Assert
    result.Should().BeEquivalentTo(expectedResult);
    _httpClientMock.Verify(x => x.GetAsync<ExpectedType>("/api/endpoint", It.IsAny<CancellationToken>()), Times.Once);
}
```

### Integration Test Categories

#### 1. Unauthenticated Integration Tests
- Inherit from `IntegrationTestBase`
- Mark with `[Trait("Category", "Integration")]`
- Run with: `dotnet test --filter "Category=Integration"`
- Skip with: `dotnet test --filter "Category!=Integration"`

```csharp
[Trait("Category", "Integration")]
public class FideApiIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task GetPlayerAsync_WithKnownFideId_ReturnsPlayer()
    {
        var player = await Client.Fide.GetPlayerAsync(1503014); // Magnus Carlsen
        player.Should().NotBeNull();
        player.Name.Should().Contain("Carlsen");
    }
}
```

#### 2. Authenticated Integration Tests
- Inherit from `AuthenticatedTestBase`
- Mark with `[AuthenticatedTest]` attribute
- Requires `LICHESS_TEST_TOKEN` environment variable
- Run with: `dotnet test --filter "Category=Authenticated"`

```csharp
[AuthenticatedTest]
public class UsersApiAuthenticatedTests : AuthenticatedTestBase
{
    [Fact]
    public async Task GetTimelineAsync_ReturnsAuthenticatedUserTimeline()
    {
        var timeline = await Client.Users.GetTimelineAsync();
        timeline.Should().NotBeNull();
    }
}
```

#### 3. Dangerous/Mutating Tests (Manual Only)
- Tests that create, modify, or delete data
- Mark with `[Trait("Category", "Manual")]`
- Never run in CI automatically
- Document prerequisites clearly

```csharp
[Trait("Category", "Manual")]
public class DangerousTests : AuthenticatedTestBase
{
    [Fact(Skip = "Requires manual execution - creates real data")]
    public async Task WriteNoteAsync_CreatesNote()
    {
        // This test creates real data on Lichess
    }
}
```

---

## Phase 1: Users API Extensions

**Priority: High**
**Estimated Effort: Medium**
**Dependencies: None**

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| Get user performance stats | `GET /api/user/{username}/perf/{perf}` | No | `apiUserPerf` |
| Get user activity | `GET /api/user/{username}/activity` | No | `apiUserActivity` |
| Player autocomplete | `GET /api/player/autocomplete` | No | `apiPlayerAutocomplete` |
| Get crosstable | `GET /api/crosstable/{user1}/{user2}` | No | `apiCrosstable` |
| Get live streamers | `GET /api/streamer/live` | No | `streamerLive` |
| Read user note | `GET /api/user/{username}/note` | Yes | `readNote` |
| Write user note | `POST /api/user/{username}/note` | Yes | `writeNote` |
| Get timeline | `GET /api/timeline` | Yes | `timeline` |

### Implementation Steps

#### Step 1.1: Add Interface Methods

```csharp
// Add to IUsersApi.cs

/// <summary>
/// Get performance statistics for a user in a specific variant.
/// </summary>
Task<UserPerformance> GetPerformanceAsync(string username, string perfType, CancellationToken cancellationToken = default);

/// <summary>
/// Get activity feed for a user.
/// </summary>
Task<IReadOnlyList<UserActivity>> GetActivityAsync(string username, CancellationToken cancellationToken = default);

/// <summary>
/// Autocomplete player usernames.
/// </summary>
Task<IReadOnlyList<string>> AutocompleteAsync(string term, bool @object = false, string? friend = null, CancellationToken cancellationToken = default);

/// <summary>
/// Get crosstable (head-to-head) stats between two users.
/// </summary>
Task<Crosstable> GetCrosstableAsync(string user1, string user2, bool matchup = false, CancellationToken cancellationToken = default);

/// <summary>
/// Get currently live streamers.
/// </summary>
Task<IReadOnlyList<Streamer>> GetLiveStreamersAsync(CancellationToken cancellationToken = default);

/// <summary>
/// Read your note about a user. Requires OAuth with follow:read scope.
/// </summary>
Task<string?> GetNoteAsync(string username, CancellationToken cancellationToken = default);

/// <summary>
/// Write a note about a user. Requires OAuth with follow:write scope.
/// </summary>
Task<bool> WriteNoteAsync(string username, string text, CancellationToken cancellationToken = default);

/// <summary>
/// Get your timeline. Requires OAuth.
/// </summary>
Task<Timeline> GetTimelineAsync(int? nb = null, DateTime? since = null, CancellationToken cancellationToken = default);
```

#### Step 1.2: Add Models

```csharp
// Models/UserPerformance.cs
public class UserPerformance
{
    [JsonPropertyName("perf")]
    public PerfStats? Perf { get; init; }

    [JsonPropertyName("rank")]
    public int? Rank { get; init; }

    [JsonPropertyName("percentile")]
    public double? Percentile { get; init; }

    [JsonPropertyName("stat")]
    public PerfStatistics? Stat { get; init; }
}

// Models/UserActivity.cs
public class UserActivity
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("interval")]
    public ActivityInterval? Interval { get; init; }

    // Activity-specific properties based on type
}

// Models/Crosstable.cs
public class Crosstable
{
    [JsonPropertyName("users")]
    public Dictionary<string, double>? Users { get; init; }

    [JsonPropertyName("nbGames")]
    public int NbGames { get; init; }

    [JsonPropertyName("matchup")]
    public CrosstableMatchup? Matchup { get; init; }
}

// Models/Streamer.cs
public class Streamer
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("stream")]
    public StreamInfo? Stream { get; init; }
}

// Models/Timeline.cs
public class Timeline
{
    [JsonPropertyName("entries")]
    public IReadOnlyList<TimelineEntry>? Entries { get; init; }

    [JsonPropertyName("users")]
    public Dictionary<string, LightUser>? Users { get; init; }
}
```

#### Step 1.3: Implement API Methods

```csharp
// Add to UsersApi.cs

public async Task<UserPerformance> GetPerformanceAsync(string username, string perfType, CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(username);
    ArgumentException.ThrowIfNullOrWhiteSpace(perfType);

    return await _httpClient.GetAsync<UserPerformance>(
        $"/api/user/{Uri.EscapeDataString(username)}/perf/{Uri.EscapeDataString(perfType)}",
        cancellationToken);
}

public async Task<IReadOnlyList<UserActivity>> GetActivityAsync(string username, CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(username);

    return await _httpClient.GetAsync<List<UserActivity>>(
        $"/api/user/{Uri.EscapeDataString(username)}/activity",
        cancellationToken) ?? [];
}

public async Task<IReadOnlyList<string>> AutocompleteAsync(string term, bool @object = false, string? friend = null, CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(term);

    var url = new StringBuilder($"/api/player/autocomplete?term={Uri.EscapeDataString(term)}");
    if (@object) url.Append("&object=true");
    if (!string.IsNullOrEmpty(friend)) url.Append($"&friend={Uri.EscapeDataString(friend)}");

    return await _httpClient.GetAsync<List<string>>(url.ToString(), cancellationToken) ?? [];
}

public async Task<Crosstable> GetCrosstableAsync(string user1, string user2, bool matchup = false, CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(user1);
    ArgumentException.ThrowIfNullOrWhiteSpace(user2);

    var url = $"/api/crosstable/{Uri.EscapeDataString(user1)}/{Uri.EscapeDataString(user2)}";
    if (matchup) url += "?matchup=true";

    return await _httpClient.GetAsync<Crosstable>(url, cancellationToken);
}

public async Task<IReadOnlyList<Streamer>> GetLiveStreamersAsync(CancellationToken cancellationToken = default)
{
    return await _httpClient.GetAsync<List<Streamer>>("/api/streamer/live", cancellationToken) ?? [];
}

public async Task<string?> GetNoteAsync(string username, CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(username);

    var response = await _httpClient.GetAsync<NoteResponse>(
        $"/api/user/{Uri.EscapeDataString(username)}/note",
        cancellationToken);
    return response?.Text;
}

public async Task<bool> WriteNoteAsync(string username, string text, CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(username);
    ArgumentNullException.ThrowIfNull(text);

    var formData = new Dictionary<string, string> { ["text"] = text };
    var response = await _httpClient.PostFormAsync<OkResponse>(
        $"/api/user/{Uri.EscapeDataString(username)}/note",
        formData,
        cancellationToken);
    return response?.Ok == true;
}

public async Task<Timeline> GetTimelineAsync(int? nb = null, DateTime? since = null, CancellationToken cancellationToken = default)
{
    var url = new StringBuilder("/api/timeline");
    var hasQuery = false;

    if (nb.HasValue)
    {
        url.Append(hasQuery ? '&' : '?').Append($"nb={nb.Value}");
        hasQuery = true;
    }
    if (since.HasValue)
    {
        var timestamp = new DateTimeOffset(since.Value).ToUnixTimeMilliseconds();
        url.Append(hasQuery ? '&' : '?').Append($"since={timestamp}");
    }

    return await _httpClient.GetAsync<Timeline>(url.ToString(), cancellationToken);
}
```

### Unit Tests

```csharp
// Tests/Api/UsersApiTests.cs - Add these test methods


[Fact]
public async Task GetPerformanceAsync_WithValidParams_CallsCorrectEndpoint()
{
    // Arrange
    var expected = new UserPerformance { Rank = 100 };
    _httpClientMock
        .Setup(x => x.GetAsync<UserPerformance>("/api/user/thibault/perf/bullet", It.IsAny<CancellationToken>()))
        .ReturnsAsync(expected);

    // Act
    var result = await _usersApi.GetPerformanceAsync("thibault", "bullet");

    // Assert
    result.Rank.Should().Be(100);
    _httpClientMock.Verify(x => x.GetAsync<UserPerformance>("/api/user/thibault/perf/bullet", It.IsAny<CancellationToken>()), Times.Once);
}

[Fact]
public async Task GetPerformanceAsync_WithNullUsername_ThrowsArgumentException()
{
    var act = () => _usersApi.GetPerformanceAsync(null!, "bullet");
    await act.Should().ThrowAsync<ArgumentException>();
}

[Fact]
public async Task GetPerformanceAsync_WithNullPerfType_ThrowsArgumentException()
{
    var act = () => _usersApi.GetPerformanceAsync("thibault", null!);
    await act.Should().ThrowAsync<ArgumentException>();
}



[Fact]
public async Task GetActivityAsync_WithValidUsername_CallsCorrectEndpoint()
{
    // Arrange
    var expected = new List<UserActivity> { new() { Type = "games" } };
    _httpClientMock
        .Setup(x => x.GetAsync<List<UserActivity>>("/api/user/thibault/activity", It.IsAny<CancellationToken>()))
        .ReturnsAsync(expected);

    // Act
    var result = await _usersApi.GetActivityAsync("thibault");

    // Assert
    result.Should().HaveCount(1);
}



[Fact]
public async Task AutocompleteAsync_WithTerm_CallsCorrectEndpoint()
{
    // Arrange
    _httpClientMock
        .Setup(x => x.GetAsync<List<string>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string> { "thibault", "thib" });

    // Act
    var result = await _usersApi.AutocompleteAsync("thib");

    // Assert
    result.Should().HaveCount(2);
    _httpClientMock.Verify(x => x.GetAsync<List<string>>(
        It.Is<string>(s => s.StartsWith("/api/player/autocomplete?term=thib")),
        It.IsAny<CancellationToken>()), Times.Once);
}

[Fact]
public async Task AutocompleteAsync_WithObjectAndFriend_BuildsCorrectQueryString()
{
    // Arrange
    _httpClientMock
        .Setup(x => x.GetAsync<List<string>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string>());

    // Act
    await _usersApi.AutocompleteAsync("thib", @object: true, friend: "thibault");

    // Assert
    _httpClientMock.Verify(x => x.GetAsync<List<string>>(
        It.Is<string>(s => s.Contains("object=true") && s.Contains("friend=thibault")),
        It.IsAny<CancellationToken>()), Times.Once);
}



[Fact]
public async Task GetCrosstableAsync_WithTwoUsers_CallsCorrectEndpoint()
{
    // Arrange
    var expected = new Crosstable { NbGames = 10 };
    _httpClientMock
        .Setup(x => x.GetAsync<Crosstable>("/api/crosstable/user1/user2", It.IsAny<CancellationToken>()))
        .ReturnsAsync(expected);

    // Act
    var result = await _usersApi.GetCrosstableAsync("user1", "user2");

    // Assert
    result.NbGames.Should().Be(10);
}

[Fact]
public async Task GetCrosstableAsync_WithMatchup_AddsQueryParam()
{
    // Arrange
    _httpClientMock
        .Setup(x => x.GetAsync<Crosstable>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new Crosstable());

    // Act
    await _usersApi.GetCrosstableAsync("user1", "user2", matchup: true);

    // Assert
    _httpClientMock.Verify(x => x.GetAsync<Crosstable>(
        "/api/crosstable/user1/user2?matchup=true",
        It.IsAny<CancellationToken>()), Times.Once);
}



[Fact]
public async Task GetLiveStreamersAsync_ReturnsStreamers()
{
    // Arrange
    var expected = new List<Streamer> { new() { Id = "streamer1", Name = "Streamer" } };
    _httpClientMock
        .Setup(x => x.GetAsync<List<Streamer>>("/api/streamer/live", It.IsAny<CancellationToken>()))
        .ReturnsAsync(expected);

    // Act
    var result = await _usersApi.GetLiveStreamersAsync();

    // Assert
    result.Should().HaveCount(1);
}



[Fact]
public async Task GetNoteAsync_WithValidUsername_CallsCorrectEndpoint()
{
    // Arrange
    _httpClientMock
        .Setup(x => x.GetAsync<NoteResponse>("/api/user/thibault/note", It.IsAny<CancellationToken>()))
        .ReturnsAsync(new NoteResponse { Text = "Test note" });

    // Act
    var result = await _usersApi.GetNoteAsync("thibault");

    // Assert
    result.Should().Be("Test note");
}



[Fact]
public async Task WriteNoteAsync_WithValidParams_CallsCorrectEndpoint()
{
    // Arrange
    _httpClientMock
        .Setup(x => x.PostFormAsync<OkResponse>(
            "/api/user/thibault/note",
            It.Is<Dictionary<string, string>>(d => d["text"] == "My note"),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new OkResponse { Ok = true });

    // Act
    var result = await _usersApi.WriteNoteAsync("thibault", "My note");

    // Assert
    result.Should().BeTrue();
}



[Fact]
public async Task GetTimelineAsync_WithoutParams_CallsBaseEndpoint()
{
    // Arrange
    _httpClientMock
        .Setup(x => x.GetAsync<Timeline>("/api/timeline", It.IsAny<CancellationToken>()))
        .ReturnsAsync(new Timeline());

    // Act
    await _usersApi.GetTimelineAsync();

    // Assert
    _httpClientMock.Verify(x => x.GetAsync<Timeline>("/api/timeline", It.IsAny<CancellationToken>()), Times.Once);
}

[Fact]
public async Task GetTimelineAsync_WithParams_BuildsCorrectQueryString()
{
    // Arrange
    _httpClientMock
        .Setup(x => x.GetAsync<Timeline>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new Timeline());

    // Act
    await _usersApi.GetTimelineAsync(nb: 15, since: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    // Assert
    _httpClientMock.Verify(x => x.GetAsync<Timeline>(
        It.Is<string>(s => s.Contains("nb=15") && s.Contains("since=")),
        It.IsAny<CancellationToken>()), Times.Once);
}

```

### Integration Tests

```csharp
// Tests/Integration/UsersApiIntegrationTests.cs - Add these tests


[Fact]
public async Task GetPerformanceAsync_WithValidParams_ReturnsPerformance()
{
    // Act
    var perf = await Client.Users.GetPerformanceAsync(ThibaultUsername, "bullet");

    // Assert
    perf.Should().NotBeNull();
}



[Fact]
public async Task GetActivityAsync_WithValidUsername_ReturnsActivity()
{
    // Act
    var activity = await Client.Users.GetActivityAsync(ThibaultUsername);

    // Assert
    activity.Should().NotBeNull();
    // Activity list may be empty for inactive users
}



[Fact]
public async Task AutocompleteAsync_WithValidTerm_ReturnsSuggestions()
{
    // Act
    var suggestions = await Client.Users.AutocompleteAsync("magn");

    // Assert
    suggestions.Should().NotBeNull();
    suggestions.Should().NotBeEmpty();
}



[Fact]
public async Task GetCrosstableAsync_WithTwoUsers_ReturnsCrosstable()
{
    // Act
    var crosstable = await Client.Users.GetCrosstableAsync(ThibaultUsername, Maia1Username);

    // Assert
    crosstable.Should().NotBeNull();
}

[Fact]
public async Task GetCrosstableAsync_WithMatchup_ReturnsMatchupData()
{
    // Act
    var crosstable = await Client.Users.GetCrosstableAsync(ThibaultUsername, Maia1Username, matchup: true);

    // Assert
    crosstable.Should().NotBeNull();
}



[Fact]
public async Task GetLiveStreamersAsync_ReturnsStreamers()
{
    // Act
    var streamers = await Client.Users.GetLiveStreamersAsync();

    // Assert
    streamers.Should().NotBeNull();
    // List may be empty if no one is streaming
}

```

### Authenticated Integration Tests

```csharp
// Tests/Integration/Authenticated/UsersApiAuthenticatedTests.cs

[AuthenticatedTest]
public class UsersApiAuthenticatedTests : AuthenticatedTestBase
{
    private const string TestNoteUsername = "maia1"; // Safe test target

    [Fact]
    public async Task GetTimelineAsync_ReturnsTimeline()
    {
        // Act
        var timeline = await Client.Users.GetTimelineAsync(nb: 5);

        // Assert
        timeline.Should().NotBeNull();
    }

    [Fact]
    public async Task GetNoteAsync_WithValidUsername_ReturnsNoteOrNull()
    {
        // Act
        var note = await Client.Users.GetNoteAsync(TestNoteUsername);

        // Assert
        // Note may be null if no note exists
        // This test just verifies the endpoint works
    }

    [Fact]
    [Trait("Category", "Manual")]
    public async Task WriteNoteAsync_WritesAndReadsNote()
    {
        // This test modifies real data - run manually
        var testNote = $"Test note from LichessSharp at {DateTime.UtcNow}";

        // Act
        var writeResult = await Client.Users.WriteNoteAsync(TestNoteUsername, testNote);

        // Assert
        writeResult.Should().BeTrue();

        // Verify by reading back
        var readNote = await Client.Users.GetNoteAsync(TestNoteUsername);
        readNote.Should().Be(testNote);
    }
}
```

---

## Phase 2: FIDE API

**Priority: High**
**Estimated Effort: Low**
**Dependencies: None**

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| Get FIDE player | `GET /api/fide/player/{playerId}` | No | `fidePlayerGet` |
| Search FIDE players | `GET /api/fide/player?q={query}` | No | `fidePlayerSearch` |

### Implementation Steps

#### Step 2.1: Create Interface

```csharp
// Api/IFideApi.cs
namespace LichessSharp.Api;

/// <summary>
/// FIDE API - Access FIDE player data from Lichess.
/// </summary>
public interface IFideApi
{
    /// <summary>
    /// Get information about a FIDE player by their FIDE ID.
    /// </summary>
    /// <param name="playerId">The FIDE player ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>FIDE player information.</returns>
    Task<FidePlayer> GetPlayerAsync(int playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for FIDE players by name.
    /// </summary>
    /// <param name="query">Search query (player name).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching FIDE players.</returns>
    Task<IReadOnlyList<FidePlayer>> SearchPlayersAsync(string query, CancellationToken cancellationToken = default);
}
```

#### Step 2.2: Create Models

```csharp
// Models/FidePlayer.cs
namespace LichessSharp.Models;

/// <summary>
/// A FIDE chess player.
/// </summary>
public class FidePlayer
{
    /// <summary>
    /// FIDE player ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Player's name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Player's title (GM, IM, FM, etc.).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Federation (3-letter country code).
    /// </summary>
    [JsonPropertyName("fed")]
    public string? Federation { get; init; }

    /// <summary>
    /// Year of birth.
    /// </summary>
    [JsonPropertyName("year")]
    public int? Year { get; init; }

    /// <summary>
    /// Standard (classical) rating.
    /// </summary>
    [JsonPropertyName("standard")]
    public int? Standard { get; init; }

    /// <summary>
    /// Rapid rating.
    /// </summary>
    [JsonPropertyName("rapid")]
    public int? Rapid { get; init; }

    /// <summary>
    /// Blitz rating.
    /// </summary>
    [JsonPropertyName("blitz")]
    public int? Blitz { get; init; }
}
```

#### Step 2.3: Create Implementation

```csharp
// Api/FideApi.cs
namespace LichessSharp.Api;

/// <summary>
/// Implementation of the FIDE API.
/// </summary>
internal sealed class FideApi : IFideApi
{
    private readonly ILichessHttpClient _httpClient;

    public FideApi(ILichessHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<FidePlayer> GetPlayerAsync(int playerId, CancellationToken cancellationToken = default)
    {
        if (playerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(playerId), "FIDE player ID must be positive.");

        return await _httpClient.GetAsync<FidePlayer>($"/api/fide/player/{playerId}", cancellationToken);
    }

    public async Task<IReadOnlyList<FidePlayer>> SearchPlayersAsync(string query, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        return await _httpClient.GetAsync<List<FidePlayer>>(
            $"/api/fide/player?q={Uri.EscapeDataString(query)}",
            cancellationToken) ?? [];
    }
}
```

#### Step 2.4: Add to LichessClient

```csharp
// Add to ILichessClient.cs
IFideApi Fide { get; }

// Add to LichessClient.cs
public IFideApi Fide { get; }

// In constructor:
Fide = new FideApi(_httpClient);
```

### Unit Tests

```csharp
// Tests/Api/FideApiTests.cs
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
        var act = () => new FideApi(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("httpClient");
    }



    [Fact]
    public async Task GetPlayerAsync_WithValidId_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new FidePlayer { Id = 1503014, Name = "Carlsen, Magnus" };
        _httpClientMock
            .Setup(x => x.GetAsync<FidePlayer>("/api/fide/player/1503014", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _fideApi.GetPlayerAsync(1503014);

        // Assert
        result.Name.Should().Be("Carlsen, Magnus");
        _httpClientMock.Verify(x => x.GetAsync<FidePlayer>("/api/fide/player/1503014", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPlayerAsync_WithZeroId_ThrowsArgumentOutOfRangeException()
    {
        var act = () => _fideApi.GetPlayerAsync(0);
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task GetPlayerAsync_WithNegativeId_ThrowsArgumentOutOfRangeException()
    {
        var act = () => _fideApi.GetPlayerAsync(-1);
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }



    [Fact]
    public async Task SearchPlayersAsync_WithValidQuery_CallsCorrectEndpoint()
    {
        // Arrange
        var expected = new List<FidePlayer>
        {
            new() { Id = 1503014, Name = "Carlsen, Magnus" },
            new() { Id = 12345678, Name = "Carlsen, Someone" }
        };
        _httpClientMock
            .Setup(x => x.GetAsync<List<FidePlayer>>(It.Is<string>(s => s.StartsWith("/api/fide/player?q=")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _fideApi.SearchPlayersAsync("Carlsen");

        // Assert
        result.Should().HaveCount(2);
        _httpClientMock.Verify(x => x.GetAsync<List<FidePlayer>>("/api/fide/player?q=Carlsen", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchPlayersAsync_WithSpecialCharacters_EncodesQuery()
    {
        // Arrange
        _httpClientMock
            .Setup(x => x.GetAsync<List<FidePlayer>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FidePlayer>());

        // Act
        await _fideApi.SearchPlayersAsync("O'Connor");

        // Assert
        _httpClientMock.Verify(x => x.GetAsync<List<FidePlayer>>(
            It.Is<string>(s => s.Contains("O%27Connor") || s.Contains("O'Connor")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchPlayersAsync_WithNullQuery_ThrowsArgumentException()
    {
        var act = () => _fideApi.SearchPlayersAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SearchPlayersAsync_WithEmptyQuery_ThrowsArgumentException()
    {
        var act = () => _fideApi.SearchPlayersAsync("");
        await act.Should().ThrowAsync<ArgumentException>();
    }

}
```

### Integration Tests

```csharp
// Tests/Integration/FideApiIntegrationTests.cs
namespace LichessSharp.Tests.Integration;

[Trait("Category", "Integration")]
public class FideApiIntegrationTests : IntegrationTestBase
{
    // Well-known FIDE IDs
    private const int MagnusCarlsenFideId = 1503014;
    private const int HikaruNakamuraFideId = 2016192;


    [Fact]
    public async Task GetPlayerAsync_WithMagnusCarlsen_ReturnsCorrectPlayer()
    {
        // Act
        var player = await Client.Fide.GetPlayerAsync(MagnusCarlsenFideId);

        // Assert
        player.Should().NotBeNull();
        player.Id.Should().Be(MagnusCarlsenFideId);
        player.Name.Should().Contain("Carlsen");
        player.Title.Should().Be("GM");
        player.Federation.Should().Be("NOR");
    }

    [Fact]
    public async Task GetPlayerAsync_WithHikaruNakamura_ReturnsCorrectPlayer()
    {
        // Act
        var player = await Client.Fide.GetPlayerAsync(HikaruNakamuraFideId);

        // Assert
        player.Should().NotBeNull();
        player.Id.Should().Be(HikaruNakamuraFideId);
        player.Name.Should().Contain("Nakamura");
        player.Title.Should().Be("GM");
    }



    [Fact]
    public async Task SearchPlayersAsync_WithCarlsen_ReturnsResults()
    {
        // Act
        var players = await Client.Fide.SearchPlayersAsync("Carlsen Magnus");

        // Assert
        players.Should().NotBeNull();
        players.Should().NotBeEmpty();
        players.Should().Contain(p => p.Id == MagnusCarlsenFideId);
    }

    [Fact]
    public async Task SearchPlayersAsync_WithErigaisi_ReturnsResults()
    {
        // From OpenAPI spec example
        // Act
        var players = await Client.Fide.SearchPlayersAsync("Erigaisi Arjun");

        // Assert
        players.Should().NotBeNull();
        players.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SearchPlayersAsync_WithNonExistentName_ReturnsEmptyList()
    {
        // Act
        var players = await Client.Fide.SearchPlayersAsync("xyznonexistent12345");

        // Assert
        players.Should().NotBeNull();
        players.Should().BeEmpty();
    }

}
```

---

## Phase 3: Games API Extensions

**Priority: High**
**Estimated Effort: Medium**
**Dependencies: None**

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| Export imported games | `GET /api/games/export/imports` | Yes | `apiImportedGamesUser` |
| Export bookmarked games | `GET /api/games/export/bookmarks` | Yes | `apiExportBookmarks` |
| Stream game by ID | `GET /api/stream/game/{id}` | No | `streamGame` |
| Add IDs to stream | `POST /api/stream/games/{streamId}/add` | No | `gamesByIdsAdd` |
| Create stream by IDs | `POST /api/stream/games/{streamId}` | No | `gamesByIds` |

### Implementation Steps

#### Step 3.1: Add Interface Methods

```csharp
// Add to IGamesApi.cs

/// <summary>
/// Export all games you have imported. Requires OAuth.
/// </summary>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>Async enumerable of PGN games.</returns>
IAsyncEnumerable<string> StreamImportedGamesPgnAsync(CancellationToken cancellationToken = default);

/// <summary>
/// Export all games you have bookmarked. Requires OAuth.
/// </summary>
/// <param name="options">Export options.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>Async enumerable of games.</returns>
IAsyncEnumerable<GameJson> StreamBookmarkedGamesAsync(ExportBookmarksOptions? options = null, CancellationToken cancellationToken = default);

/// <summary>
/// Stream a game's updates. Useful for watching live games.
/// </summary>
/// <param name="gameId">The game ID.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>Async enumerable of game events.</returns>
IAsyncEnumerable<GameStreamEvent> StreamGameAsync(string gameId, CancellationToken cancellationToken = default);

/// <summary>
/// Create a stream to receive updates for specific game IDs.
/// </summary>
/// <param name="streamId">Your unique stream identifier.</param>
/// <param name="gameIds">Initial game IDs to watch.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>Async enumerable of game events.</returns>
IAsyncEnumerable<GameStreamEvent> CreateGameStreamAsync(string streamId, IEnumerable<string> gameIds, CancellationToken cancellationToken = default);

/// <summary>
/// Add game IDs to an existing stream.
/// </summary>
/// <param name="streamId">The stream identifier.</param>
/// <param name="gameIds">Game IDs to add to the stream.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>True if successful.</returns>
Task<bool> AddToGameStreamAsync(string streamId, IEnumerable<string> gameIds, CancellationToken cancellationToken = default);
```

#### Step 3.2: Add Models

```csharp
// Models/ExportBookmarksOptions.cs
public class ExportBookmarksOptions
{
    public long? Since { get; set; }
    public long? Until { get; set; }
    public int? Max { get; set; }
    public bool? Moves { get; set; }
    public bool? PgnInJson { get; set; }
    public bool? Tags { get; set; }
    public bool? Clocks { get; set; }
    public bool? Evals { get; set; }
    public bool? Accuracy { get; set; }
    public bool? Opening { get; set; }
    public bool? Division { get; set; }
    public bool? Literate { get; set; }
    public bool? LastFen { get; set; }
    public string? Sort { get; set; } // "dateAsc" or "dateDesc"
}

// Models/GameStreamEvent.cs
public class GameStreamEvent
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("fen")]
    public string? Fen { get; init; }

    [JsonPropertyName("lm")]
    public string? LastMove { get; init; }

    [JsonPropertyName("wc")]
    public int? WhiteClock { get; init; }

    [JsonPropertyName("bc")]
    public int? BlackClock { get; init; }
}
```

### Unit Tests (abbreviated)

```csharp
// Tests/Api/GamesApiTests.cs - Add tests

[Fact]
public async Task StreamImportedGamesPgnAsync_CallsCorrectEndpoint()
{
    // Arrange & Act & Assert
    // Test streaming endpoint is called correctly
}

[Fact]
public async Task StreamBookmarkedGamesAsync_WithOptions_BuildsCorrectQueryString()
{
    // Test query string building
}

[Fact]
public async Task StreamGameAsync_WithValidId_CallsCorrectEndpoint()
{
    // Test /api/stream/game/{id} endpoint
}
```

### Integration Tests

```csharp
// Tests/Integration/GamesApiIntegrationTests.cs - Add tests

[Fact]
public async Task StreamGameAsync_WithRecentGame_StreamsUpdates()
{
    // First get a recent game ID from TV
    var tvGames = await Client.Tv.GetCurrentGamesAsync();
    var gameId = tvGames.Values.FirstOrDefault()?.GameId;

    if (gameId == null)
    {
        // No games currently on TV, skip
        return;
    }

    // Act
    var eventCount = 0;
    await foreach (var evt in Client.Games.StreamGameAsync(gameId).WithCancellation(new CancellationTokenSource(5000).Token))
    {
        eventCount++;
        evt.Should().NotBeNull();
        if (eventCount >= 1) break; // Just verify we get at least one event
    }

    // Assert
    eventCount.Should().BeGreaterThan(0);
}
```

### Authenticated Integration Tests

```csharp
// Tests/Integration/Authenticated/GamesApiAuthenticatedTests.cs

[AuthenticatedTest]
public class GamesApiAuthenticatedTests : AuthenticatedTestBase
{
    [Fact]
    public async Task StreamImportedGamesPgnAsync_ReturnsGamesOrEmpty()
    {
        // Act
        var games = new List<string>();
        await foreach (var pgn in Client.Games.StreamImportedGamesPgnAsync()
            .WithCancellation(new CancellationTokenSource(5000).Token))
        {
            games.Add(pgn);
            if (games.Count >= 5) break; // Limit for test
        }

        // Assert - may be empty if user hasn't imported games
        games.Should().NotBeNull();
    }

    [Fact]
    public async Task StreamBookmarkedGamesAsync_ReturnsGamesOrEmpty()
    {
        // Arrange
        var options = new ExportBookmarksOptions { Max = 5 };

        // Act
        var games = new List<GameJson>();
        await foreach (var game in Client.Games.StreamBookmarkedGamesAsync(options)
            .WithCancellation(new CancellationTokenSource(5000).Token))
        {
            games.Add(game);
        }

        // Assert - may be empty if user hasn't bookmarked games
        games.Should().NotBeNull();
    }
}
```

---

## Phase 4: Puzzles API Extensions

**Priority: Medium**
**Estimated Effort: Medium**
**Dependencies: None**

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| Get puzzles batch | `GET /api/puzzle/batch/{angle}` | Yes | `apiPuzzleBatchSelect` |
| Solve puzzles batch | `POST /api/puzzle/batch/{angle}` | Yes | `apiPuzzleBatchSolve` |
| Replay puzzles | `GET /api/puzzle/replay/{days}/{theme}` | Yes | `apiPuzzleReplay` |
| Get puzzle race | `GET /api/racer/{id}` | No | `racerGet` |

### Implementation (Interface Methods)

```csharp
// Add to IPuzzlesApi.cs

/// <summary>
/// Get a batch of puzzles to solve offline. Requires OAuth with puzzle:read scope.
/// </summary>
Task<PuzzleBatch> GetBatchAsync(string angle, int? nb = null, CancellationToken cancellationToken = default);

/// <summary>
/// Submit solutions for a batch of puzzles. Requires OAuth with puzzle:read scope.
/// </summary>
Task<PuzzleBatchResult> SolveBatchAsync(string angle, IEnumerable<PuzzleSolution> solutions, CancellationToken cancellationToken = default);

/// <summary>
/// Get puzzles to replay (review incorrect puzzles). Requires OAuth with puzzle:read scope.
/// </summary>
IAsyncEnumerable<PuzzleWithGame> StreamReplayAsync(int days, string theme, CancellationToken cancellationToken = default);

/// <summary>
/// Get information about a puzzle race.
/// </summary>
Task<PuzzleRace> GetRaceAsync(string raceId, CancellationToken cancellationToken = default);
```

---

## Phase 5: Board & Bot API Extensions

**Priority: Medium**
**Estimated Effort: Low**
**Dependencies: None**

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| Board: Claim draw | `POST /api/board/game/{gameId}/claim-draw` | Yes | `boardGameClaimDraw` |
| Bot: Claim draw | `POST /api/bot/game/{gameId}/claim-draw` | Yes | `botGameClaimDraw` |

### Implementation

```csharp
// Add to IBoardApi.cs
/// <summary>
/// Claim a draw by the 50-move rule, or by threefold repetition.
/// Requires OAuth with board:play scope.
/// </summary>
Task<bool> ClaimDrawAsync(string gameId, CancellationToken cancellationToken = default);

// Add to IBotApi.cs
/// <summary>
/// Claim a draw by the 50-move rule, or by threefold repetition.
/// Requires OAuth with bot:play scope.
/// </summary>
Task<bool> ClaimDrawAsync(string gameId, CancellationToken cancellationToken = default);

// Implementation in BoardApi.cs
public async Task<bool> ClaimDrawAsync(string gameId, CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(gameId);
    var response = await _httpClient.PostAsync<OkResponse>($"/api/board/game/{gameId}/claim-draw", cancellationToken);
    return response?.Ok == true;
}

// Implementation in BotApi.cs
public async Task<bool> ClaimDrawAsync(string gameId, CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(gameId);
    var response = await _httpClient.PostAsync<OkResponse>($"/api/bot/game/{gameId}/claim-draw", cancellationToken);
    return response?.Ok == true;
}
```

---

## Phase 6: Broadcasts API Extensions

**Priority: Medium**
**Estimated Effort: Low**
**Dependencies: None**

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| Get players | `GET /broadcast/{tournamentId}/players` | No | `broadcastPlayersGet` |
| Get player | `GET /broadcast/{tournamentId}/players/{playerId}` | No | `broadcastPlayerGet` |

### Implementation

```csharp
// Add to IBroadcastsApi.cs

/// <summary>
/// Get all players in a broadcast tournament.
/// </summary>
Task<BroadcastPlayers> GetPlayersAsync(string tournamentId, CancellationToken cancellationToken = default);

/// <summary>
/// Get a specific player from a broadcast tournament.
/// </summary>
Task<BroadcastPlayer> GetPlayerAsync(string tournamentId, string playerId, CancellationToken cancellationToken = default);
```

---

## Phase 7: Opening Explorer Extensions

**Priority: Low**
**Estimated Effort: Low**
**Dependencies: None**

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| Get master game | `GET /masters/pgn/{gameId}` | No | `openingExplorerMasterGame` |

### Implementation

```csharp
// Add to IOpeningExplorerApi.cs

/// <summary>
/// Get a specific game from the masters database as PGN.
/// </summary>
/// <param name="gameId">The game ID from the masters database.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>PGN string of the game.</returns>
Task<string> GetMasterGamePgnAsync(string gameId, CancellationToken cancellationToken = default);
```

---

## Phase 8: Arena Tournaments Extensions

**Priority: Low**
**Estimated Effort: Low**
**Dependencies: None**

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| Stream team arenas | `GET /api/team/{teamId}/arena` | No | `apiTeamArena` |

### Implementation

```csharp
// Add to IArenaTournamentsApi.cs

/// <summary>
/// Stream arena tournaments for a team.
/// </summary>
IAsyncEnumerable<ArenaTournament> StreamTeamArenasAsync(string teamId, int? max = null, CancellationToken cancellationToken = default);
```

---

## Phase 9: OAuth API

**Priority: Low**
**Estimated Effort: Medium**
**Dependencies: None**

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| Obtain token | `POST /api/token` | No | `apiToken` |
| Revoke token | `DELETE /api/token` | Yes | `apiTokenDelete` |
| Test tokens | `POST /api/token/test` | No | `tokenTest` |

Note: The `GET /oauth` endpoint is typically handled by browser redirect and not directly by the API client.

### Implementation

```csharp
// Api/IOAuthApi.cs
namespace LichessSharp.Api;

/// <summary>
/// OAuth API - Token management for the Lichess API.
/// </summary>
public interface IOAuthApi
{
    /// <summary>
    /// Exchange an authorization code for an access token (PKCE flow).
    /// </summary>
    Task<OAuthToken> GetTokenAsync(OAuthTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke the current access token.
    /// </summary>
    Task RevokeTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Test multiple OAuth tokens to check their validity and scopes.
    /// </summary>
    Task<Dictionary<string, OAuthTokenInfo?>> TestTokensAsync(IEnumerable<string> tokens, CancellationToken cancellationToken = default);
}
```

---

## Phase 10: External Engine API

**Priority: Low (Alpha API)**
**Estimated Effort: High**
**Dependencies: None**

> **Note**: This API is marked as "alpha and subject to change" in the Lichess documentation.

### Endpoints to Implement

| Endpoint | Route | Auth | OpenAPI operationId |
|----------|-------|------|---------------------|
| List engines | `GET /api/external-engine` | Yes | `apiExternalEngineList` |
| Create engine | `POST /api/external-engine` | Yes | `apiExternalEngineCreate` |
| Get engine | `GET /api/external-engine/{id}` | Yes | `apiExternalEngineGet` |
| Update engine | `PUT /api/external-engine/{id}` | Yes | `apiExternalEnginePut` |
| Delete engine | `DELETE /api/external-engine/{id}` | Yes | `apiExternalEngineDelete` |
| Request analysis* | `POST /api/external-engine/{id}/analyse` | Yes | `apiExternalEngineAnalyse` |
| Acquire work* | `POST /api/external-engine/work` | No | `apiExternalEngineAcquire` |
| Submit analysis* | `POST /api/external-engine/work/{id}` | No | `apiExternalEngineSubmit` |

*These endpoints use `engine.lichess.ovh` as the base URL.

### Implementation

```csharp
// Api/IExternalEngineApi.cs
namespace LichessSharp.Api;

/// <summary>
/// External Engine API - Register and manage external analysis engines.
/// WARNING: This API is in alpha and subject to change.
/// </summary>
public interface IExternalEngineApi
{
    /// <summary>
    /// List all registered external engines. Requires OAuth with engine:read scope.
    /// </summary>
    Task<IReadOnlyList<ExternalEngine>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Register a new external engine. Requires OAuth with engine:write scope.
    /// </summary>
    Task<ExternalEngine> CreateAsync(ExternalEngineRegistration registration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get an external engine by ID. Requires OAuth with engine:read scope.
    /// </summary>
    Task<ExternalEngine> GetAsync(string engineId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an external engine. Requires OAuth with engine:write scope.
    /// </summary>
    Task<ExternalEngine> UpdateAsync(string engineId, ExternalEngineRegistration registration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an external engine. Requires OAuth with engine:write scope.
    /// </summary>
    Task DeleteAsync(string engineId, CancellationToken cancellationToken = default);

    // Note: Analysis endpoints use engine.lichess.ovh and may require separate client configuration
}
```

---

## Implementation Checklist

### Phase 1: Users API Extensions ✅ COMPLETED
- [x] Add 8 interface methods to `IUsersApi.cs`
- [x] Add 8 models (`UserPerformance`, `UserActivity`, `Crosstable`, `Streamer`, `Timeline`, etc.)
- [x] Implement 8 methods in `UsersApi.cs`
- [x] Add unit tests for all 8 methods
- [x] Add integration tests (8 unauthenticated tests)
- [x] Update `docs/api-coverage.md`
- [x] Update XML documentation

### Phase 2: FIDE API ✅ COMPLETED
- [x] Create `IFideApi.cs` interface
- [x] Create `FidePlayer.cs` model
- [x] Create `FideApi.cs` implementation
- [x] Add to `ILichessClient` and `LichessClient`
- [x] Add unit tests (14 tests)
- [x] Add integration tests (5 tests)
- [x] Update `docs/api-coverage.md`

### Phase 3: Games API Extensions
- [ ] Add 5 interface methods to `IGamesApi.cs`
- [ ] Add models (`ExportBookmarksOptions`, `GameStreamEvent`)
- [ ] Implement 5 methods in `GamesApi.cs`
- [ ] Add unit tests for all 5 methods
- [ ] Add integration tests (2 unauthenticated, 2 authenticated)
- [ ] Update `docs/api-coverage.md`

### Phase 4: Puzzles API Extensions
- [ ] Add 4 interface methods to `IPuzzlesApi.cs`
- [ ] Add models (`PuzzleBatch`, `PuzzleBatchResult`, `PuzzleSolution`, `PuzzleRace`)
- [ ] Implement 4 methods in `PuzzlesApi.cs`
- [ ] Add unit tests
- [ ] Add integration tests
- [ ] Update `docs/api-coverage.md`

### Phase 5: Board & Bot API Extensions
- [ ] Add `ClaimDrawAsync` to `IBoardApi.cs`
- [ ] Add `ClaimDrawAsync` to `IBotApi.cs`
- [ ] Implement in both APIs
- [ ] Add unit tests
- [ ] Update `docs/api-coverage.md`

### Phase 6: Broadcasts API Extensions
- [ ] Add 2 interface methods to `IBroadcastsApi.cs`
- [ ] Add models (`BroadcastPlayers`, `BroadcastPlayer`)
- [ ] Implement 2 methods
- [ ] Add unit tests
- [ ] Add integration tests
- [ ] Update `docs/api-coverage.md`

### Phase 7: Opening Explorer Extensions ✅ COMPLETED
- [x] Add `GetMasterGamePgnAsync` to `IOpeningExplorerApi.cs`
- [x] Implement in `OpeningExplorerApi.cs`
- [x] Add unit tests
- [x] Add integration tests
- [x] Update `docs/api-coverage.md`

### Phase 8: Arena Tournaments Extensions ✅ COMPLETED
- [x] Add `StreamTeamTournamentsAsync` to `IArenaTournamentsApi.cs`
- [x] Implement in `ArenaTournamentsApi.cs`
- [x] Add unit tests
- [x] Add integration tests
- [x] Update `docs/api-coverage.md`

### Phase 9: OAuth API ✅ COMPLETED
- [x] Create `IOAuthApi.cs` interface
- [x] Create models (`OAuthToken`, `OAuthTokenRequest`, `OAuthTokenInfo`)
- [x] Create `OAuthApi.cs` implementation
- [x] Add to client
- [x] Add unit tests (17 tests)
- [ ] Add integration tests (OAuth flow requires real authorization)
- [x] Update `docs/api-coverage.md`

### Phase 10: External Engine API ✅ COMPLETED
- [x] Create `IExternalEngineApi.cs` interface
- [x] Create models (`ExternalEngine`, `ExternalEngineRegistration`, `EngineWork`, `EngineAnalysisLine`, etc.)
- [x] Create `ExternalEngineApi.cs` implementation
- [x] Add to client (with separate engine.lichess.ovh configuration via `EngineBaseAddress`)
- [x] Add unit tests (26 tests)
- [ ] Add integration tests (manual/authenticated - requires engine:read/write scopes)
- [x] Update `docs/api-coverage.md`
- [x] Add warning about alpha status in documentation

---

## Summary

| Phase | Endpoints | Est. Unit Tests | Est. Integration Tests | Priority |
|-------|-----------|-----------------|------------------------|----------|
| 1. Users API | 8 | 24+ | 10+ | High |
| 2. FIDE API | 2 | 6+ | 5+ | High |
| 3. Games API | 5 | 15+ | 6+ | High |
| 4. Puzzles API | 4 | 12+ | 6+ | Medium |
| 5. Board/Bot API | 2 | 4+ | 2+ | Medium |
| 6. Broadcasts API | 2 | 6+ | 4+ | Medium |
| 7. Opening Explorer | 1 | 3+ | 2+ | Low |
| 8. Arena Tournaments | 1 | 3+ | 2+ | Low |
| 9. OAuth API | 3 | 9+ | 4+ | Low |
| 10. External Engine | 8 | 24+ | 8+ | Low |
| **Total** | **36** | **106+** | **49+** | - |
