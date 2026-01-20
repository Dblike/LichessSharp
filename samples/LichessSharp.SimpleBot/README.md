# LichessSharp SimpleBot

A minimal Lichess bot example using LichessSharp. This bot accepts challenges and makes moves (using a very simple pseudo-random move generator).

## Prerequisites

1. **Create a Lichess account** for your bot (separate from your main account)
2. **Upgrade to BOT account** - This is irreversible and the account can no longer play manually
3. **Generate an API token** with the required scopes

### Upgrading to BOT Account

1. Log into Lichess with your bot account
2. Go to https://lichess.org/account/oauth/token
3. Create a token with at least these scopes:
   - `bot:play` - Required to play as a bot
   - `challenge:write` - Required to accept/decline challenges
4. Make the upgrade API call:

```bash
curl -X POST https://lichess.org/api/bot/account/upgrade \
  -H "Authorization: Bearer YOUR_TOKEN"
```

> **Warning**: Once upgraded, an account cannot be downgraded back to a regular account.

## Running the Bot

1. Set your token as an environment variable:

```bash
# Linux/macOS
export LICHESS_BOT_TOKEN="lip_xxxxxxxxxxxxx"

# Windows PowerShell
$env:LICHESS_BOT_TOKEN="lip_xxxxxxxxxxxxx"

# Windows CMD
set LICHESS_BOT_TOKEN=lip_xxxxxxxxxxxxx
```

2. Run the bot:

```bash
cd path/to/LichessSharp
dotnet run --project samples/LichessSharp.SimpleBot
```

3. Challenge your bot on Lichess!

## How It Works

The bot uses these LichessSharp APIs:

1. **Bot.StreamEventsAsync()** - Listens for incoming challenges and game starts
2. **Challenges.AcceptAsync()** / **DeclineAsync()** - Handles challenges
3. **Bot.StreamGameAsync()** - Follows game state with polymorphic events
4. **Bot.MakeMoveAsync()** - Plays moves
5. **Bot.WriteChatAsync()** - Sends chat messages

### Event Handling

```csharp
await foreach (var evt in client.Bot.StreamEventsAsync(cancellationToken))
{
    switch (evt.Type)
    {
        case "challenge":
            await HandleChallengeAsync(evt.Challenge!);
            break;
        case "gameStart":
            StartGame(evt.Game!.GameId!);
            break;
        case "gameFinish":
            StopGame(evt.Game!.GameId!);
            break;
    }
}
```

### Polymorphic Game Events

```csharp
await foreach (var evt in client.Bot.StreamGameAsync(gameId, cancellationToken))
{
    switch (evt)
    {
        case BotGameFullEvent full:
            // Game just started - set up initial state
            break;
        case BotGameStateEvent state:
            // A move was made - check if it's our turn
            break;
        case BotChatLineEvent chat:
            // Chat message received
            break;
        case BotOpponentGoneEvent gone:
            // Opponent disconnected
            break;
    }
}
```

## Making It Smarter

This example uses a very simple move generator. For a real bot, integrate a chess engine:

### Using Stockfish

1. Install Stockfish: https://stockfishchess.org/download/
2. Use a UCI library like [Stockfish.NET](https://github.com/Oremiro/Stockfish.NET)

```csharp
// Example integration (not included)
using var engine = new Stockfish("path/to/stockfish");
engine.SetPosition(moves);
var bestMove = engine.GetBestMove();
await client.Bot.MakeMoveAsync(gameId, bestMove);
```

### Using a Chess Library

For move validation and board state, use a library like:
- [Chess.NET](https://github.com/ProgramFOX/Chess.NET)
- [ChessDotNet](https://github.com/thomas-daniels/Chess.NET)

## Configuration

The bot accepts these challenge types by default:
- **Speeds**: bullet, blitz, rapid, classical
- **Variants**: standard, chess960

Modify `HandleChallengeAsync` to customize which challenges to accept.

## Troubleshooting

### "This account is not a BOT account"

You need to upgrade your account. See [Upgrading to BOT Account](#upgrading-to-bot-account).

### "403 Forbidden"

Your token doesn't have the required scopes. Generate a new token with `bot:play` and `challenge:write`.

### "429 Too Many Requests"

The bot is rate limited. LichessSharp handles this automatically with retries.

### Bot doesn't respond to challenges

1. Check the console for error messages
2. Verify the token is set correctly
3. Ensure the account is upgraded to BOT

## See Also

- [Bot API Documentation](../../wiki/api/Bot.md)
- [Streaming Events Guide](../../wiki/guides/Streaming-Events.md)
- [Error Handling Guide](../../wiki/guides/Error-Handling.md)
- [Lichess Bot API](https://lichess.org/api#tag/Bot)
