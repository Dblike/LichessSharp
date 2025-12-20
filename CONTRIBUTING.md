# Contributing to LichessSharp

Thanks for your interest in contributing! This document explains how to get started.

## Getting Started

1. Fork and clone the repository
2. Install .NET 10 SDK
3. Run `dotnet build` to verify everything compiles
4. Run `dotnet test --filter "Category!=Integration"` to run unit tests

## Making Changes

### Before You Start

- Check existing issues and PRs to avoid duplicate work
- For significant changes, open an issue first to discuss the approach

### Code Style

- Follow existing patterns in the codebase
- All public APIs need XML documentation comments
- Keep methods focused and small
- Use async/await consistently (all I/O operations should be async)

### Testing

- Add unit tests for new functionality
- Ensure all existing tests pass before submitting
- Integration tests require a Lichess API token and hit live servers, so they're excluded from CI

Run tests:
```bash
dotnet test --filter "Category!=Integration"
```

### Commits

- Write clear commit messages explaining what and why
- Keep commits focused on a single change

## Pull Requests

1. Create a branch from `main`
2. Make your changes with tests
3. Ensure `dotnet build` and `dotnet test --filter "Category!=Integration"` pass
4. Push and open a PR against `main`
5. Fill in the PR description explaining your changes

## Project Structure

```
src/LichessSharp/          # Main library
  Api/                     # API implementations (one per Lichess API area)
  Api/Contracts/           # Interfaces for each API
  Models/                  # Request/response DTOs
  Http/                    # HTTP client infrastructure
tests/LichessSharp.Tests/  # Unit and integration tests
samples/                   # Example applications
docs/                      # Documentation
```

## Adding New Endpoints

1. Find the endpoint in `openapi/lichess.openapi.json`
2. Add the method to the appropriate interface in `Api/Contracts/`
3. Implement the method in the corresponding API class
4. Add unit tests
5. Update `docs/api-coverage.md`

## Questions?

Open an issue if you have questions or need guidance.
