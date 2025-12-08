# Authentication

This guide covers authentication with the Lichess API.

## Overview

The Lichess API uses OAuth2 with Bearer tokens. Most read operations work without authentication, but write operations and private data require a token.

## Getting a Token

### Personal Access Token

For development or personal use, generate a personal access token:

1. Go to [lichess.org/account/oauth/token](https://lichess.org/account/oauth/token)
2. Select the required scopes
3. Generate the token
4. Store it securely

### OAuth2 Authorization Code Flow

For applications that need to authenticate users, use the OAuth2 PKCE flow. See the [Lichess OAuth documentation](https://lichess.org/api#section/Introduction/Authentication).

## Using Tokens

### Simple Usage

```csharp
using LichessSharp;

using var client = new LichessClient("lip_your_token_here");
```

### With Options

```csharp
using LichessSharp;

var options = new LichessClientOptions
{
    AccessToken = Environment.GetEnvironmentVariable("LICHESS_TOKEN")
};

using var client = new LichessClient(new HttpClient(), options);
```

## OAuth Scopes

Different API endpoints require different scopes:

| Scope | Description |
|-------|-------------|
| `email:read` | Read email address |
| `preference:read` | Read preferences |
| `preference:write` | Change preferences |
| `challenge:read` | Read incoming challenges |
| `challenge:write` | Create/accept/decline challenges |
| `puzzle:read` | Read puzzle data |
| `puzzle:write` | Submit puzzle solutions |
| `tournament:write` | Create/manage tournaments |
| `team:read` | Read team information |
| `team:write` | Join/leave teams |
| `team:lead` | Manage teams you lead |
| `follow:read` | Read followed users |
| `follow:write` | Follow/unfollow users |
| `msg:write` | Send private messages |
| `board:play` | Play games via Board API |
| `bot:play` | Play games as a bot |
| `racer:write` | Create puzzle races |
| `study:read` | Read studies |
| `study:write` | Create/modify studies |

## Security Best Practices

1. **Never hardcode tokens** - Use environment variables or secure configuration
2. **Use minimal scopes** - Only request scopes you actually need
3. **Rotate tokens** - Periodically regenerate tokens
4. **Revoke compromised tokens** - If a token is exposed, revoke it immediately

```csharp
// Good: Environment variable
var token = Environment.GetEnvironmentVariable("LICHESS_TOKEN");

// Good: User secrets (development)
var token = configuration["Lichess:AccessToken"];

// Bad: Hardcoded
var token = "lip_xxxxx"; // Never do this!
```

## Handling Authentication Errors

```csharp
try
{
    var email = await client.Account.GetEmailAsync();
}
catch (LichessAuthenticationException)
{
    // Token is invalid or expired
    // Prompt user to re-authenticate
}
catch (LichessAuthorizationException ex)
{
    // Token doesn't have required scope
    Console.WriteLine($"Missing scope: {ex.RequiredScope}");
}
```
