using System.Text.Json;
using FluentAssertions;
using LichessSharp.Api.Contracts;
using LichessSharp.Serialization.Converters;
using Xunit;

namespace LichessSharp.Tests.Serialization;

public class BotGameEventConverterTests
{
    private readonly JsonSerializerOptions _options;

    public BotGameEventConverterTests()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        _options.Converters.Add(new BotGameEventConverter());
    }

    [Fact]
    public void Read_GameFull_ReturnsBotGameFullEvent()
    {
        // Arrange
        var json = """
            {
                "type": "gameFull",
                "id": "abc123",
                "variant": {"key": "standard", "name": "Standard"},
                "speed": "blitz",
                "rated": true,
                "createdAt": 1704067200000,
                "white": {"id": "player1", "name": "Player1", "rating": 1500},
                "black": {"id": "player2", "name": "Player2", "rating": 1600},
                "state": {
                    "type": "gameState",
                    "moves": "e2e4 e7e5",
                    "wtime": 300000,
                    "btime": 300000,
                    "status": "started"
                }
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().BeOfType<BotGameFullEvent>();
        var fullEvent = (BotGameFullEvent)result!;
        fullEvent.Type.Should().Be("gameFull");
        fullEvent.Id.Should().Be("abc123");
        fullEvent.Rated.Should().BeTrue();
        fullEvent.Speed.Should().Be("blitz");
        fullEvent.White.Should().NotBeNull();
        fullEvent.White!.Name.Should().Be("Player1");
        fullEvent.Black.Should().NotBeNull();
        fullEvent.Black!.Name.Should().Be("Player2");
        fullEvent.State.Should().NotBeNull();
        fullEvent.State!.Moves.Should().Be("e2e4 e7e5");
    }

    [Fact]
    public void Read_GameState_ReturnsBotGameStateEvent()
    {
        // Arrange
        var json = """
            {
                "type": "gameState",
                "moves": "e2e4 e7e5 g1f3",
                "wtime": 295000,
                "btime": 298000,
                "winc": 3000,
                "binc": 3000,
                "status": "started"
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().BeOfType<BotGameStateEvent>();
        var stateEvent = (BotGameStateEvent)result!;
        stateEvent.Type.Should().Be("gameState");
        stateEvent.Moves.Should().Be("e2e4 e7e5 g1f3");
        stateEvent.WhiteTime.Should().Be(295000);
        stateEvent.BlackTime.Should().Be(298000);
        stateEvent.WhiteIncrement.Should().Be(3000);
        stateEvent.BlackIncrement.Should().Be(3000);
        stateEvent.Status.Should().Be("started");
    }

    [Fact]
    public void Read_GameState_WithDrawOffer_ReturnsCorrectProperties()
    {
        // Arrange
        var json = """
            {
                "type": "gameState",
                "moves": "e2e4 e7e5",
                "wtime": 300000,
                "btime": 300000,
                "status": "started",
                "wdraw": true,
                "bdraw": false
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().BeOfType<BotGameStateEvent>();
        var stateEvent = (BotGameStateEvent)result!;
        stateEvent.WhiteOfferingDraw.Should().BeTrue();
        stateEvent.BlackOfferingDraw.Should().BeFalse();
    }

    [Fact]
    public void Read_GameState_WithWinner_ReturnsCorrectProperties()
    {
        // Arrange
        var json = """
            {
                "type": "gameState",
                "moves": "e2e4 e7e5 d1h5 b8c6 f1c4 g8f6 h5f7",
                "wtime": 280000,
                "btime": 290000,
                "status": "mate",
                "winner": "white"
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().BeOfType<BotGameStateEvent>();
        var stateEvent = (BotGameStateEvent)result!;
        stateEvent.Status.Should().Be("mate");
        stateEvent.Winner.Should().Be("white");
    }

    [Fact]
    public void Read_ChatLine_ReturnsBotChatLineEvent()
    {
        // Arrange
        var json = """
            {
                "type": "chatLine",
                "room": "player",
                "username": "Player1",
                "text": "Good luck!"
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().BeOfType<BotChatLineEvent>();
        var chatEvent = (BotChatLineEvent)result!;
        chatEvent.Type.Should().Be("chatLine");
        chatEvent.Room.Should().Be("player");
        chatEvent.Username.Should().Be("Player1");
        chatEvent.Text.Should().Be("Good luck!");
    }

    [Fact]
    public void Read_ChatLine_SpectatorRoom_ReturnsCorrectRoom()
    {
        // Arrange
        var json = """
            {
                "type": "chatLine",
                "room": "spectator",
                "username": "Spectator1",
                "text": "Nice move!"
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().BeOfType<BotChatLineEvent>();
        var chatEvent = (BotChatLineEvent)result!;
        chatEvent.Room.Should().Be("spectator");
    }

    [Fact]
    public void Read_OpponentGone_ReturnsBotOpponentGoneEvent()
    {
        // Arrange
        var json = """
            {
                "type": "opponentGone",
                "gone": true,
                "claimWinInSeconds": 30
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().BeOfType<BotOpponentGoneEvent>();
        var goneEvent = (BotOpponentGoneEvent)result!;
        goneEvent.Type.Should().Be("opponentGone");
        goneEvent.Gone.Should().BeTrue();
        goneEvent.ClaimWinInSeconds.Should().Be(30);
    }

    [Fact]
    public void Read_OpponentGone_NotGone_ReturnsCorrectProperties()
    {
        // Arrange
        var json = """
            {
                "type": "opponentGone",
                "gone": false
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().BeOfType<BotOpponentGoneEvent>();
        var goneEvent = (BotOpponentGoneEvent)result!;
        goneEvent.Gone.Should().BeFalse();
        goneEvent.ClaimWinInSeconds.Should().BeNull();
    }

    [Fact]
    public void Read_UnknownType_ReturnsBaseEvent()
    {
        // Arrange
        var json = """
            {
                "type": "futureEventType",
                "someField": "value"
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BotGameEvent>();
        result!.Type.Should().Be("futureEventType");
    }

    [Fact]
    public void Read_MissingType_ReturnsBaseEventWithUnknown()
    {
        // Arrange
        var json = """
            {
                "someField": "value"
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BotGameEvent>();
        result!.Type.Should().Be("unknown");
    }

    [Fact]
    public void Read_NullValue_ReturnsNull()
    {
        // Arrange
        var json = "null";

        // Act
        var result = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Read_InvalidTokenType_ThrowsJsonException()
    {
        // Arrange
        var json = "\"not an object\"";

        // Act
        var act = () => JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        act.Should().Throw<JsonException>().WithMessage("*Expected StartObject*");
    }

    [Fact]
    public void Write_GameFullEvent_SerializesCorrectly()
    {
        // Arrange
        var evt = new BotGameFullEvent
        {
            Type = "gameFull",
            Id = "abc123",
            Rated = true,
            Speed = "blitz"
        };

        // Act
        var json = JsonSerializer.Serialize<BotGameEvent>(evt, _options);

        // Assert
        json.Should().Contain("\"type\":\"gameFull\"");
        json.Should().Contain("\"id\":\"abc123\"");
        json.Should().Contain("\"rated\":true");
    }

    [Fact]
    public void Write_GameStateEvent_SerializesCorrectly()
    {
        // Arrange
        var evt = new BotGameStateEvent
        {
            Type = "gameState",
            Moves = "e2e4 e7e5",
            WhiteTime = 300000,
            BlackTime = 300000
        };

        // Act
        var json = JsonSerializer.Serialize<BotGameEvent>(evt, _options);

        // Assert
        json.Should().Contain("\"type\":\"gameState\"");
        json.Should().Contain("\"moves\":\"e2e4 e7e5\"");
    }

    [Fact]
    public void RoundTrip_GameFullEvent_PreservesData()
    {
        // Arrange
        var original = new BotGameFullEvent
        {
            Type = "gameFull",
            Id = "test123",
            Rated = true,
            Speed = "rapid",
            CreatedAt = 1704067200000
        };

        // Act
        var json = JsonSerializer.Serialize<BotGameEvent>(original, _options);
        var deserialized = JsonSerializer.Deserialize<BotGameEvent>(json, _options);

        // Assert
        deserialized.Should().BeOfType<BotGameFullEvent>();
        var result = (BotGameFullEvent)deserialized!;
        result.Id.Should().Be(original.Id);
        result.Rated.Should().Be(original.Rated);
        result.Speed.Should().Be(original.Speed);
        result.CreatedAt.Should().Be(original.CreatedAt);
    }
}
