# CLAUDE.md — LichessSharp Development Protocol

<!-- AUTO-GENERATED: Do not edit directly. Modify CLAUDE.project.md instead. -->
<!-- Template: dotnet-library | Generated: 2026-01-05 -->

## Project Overview

Full-featured .NET Lichess API wrapper implementing the official HTTP & streaming APIs.

## Goals

- Feature parity with mature Python/JavaScript clients
- Minimize third-party dependencies
- Support .NET 10 (current target)

## API Reference

- OpenAPI spec: `openapi/lichess.openapi.json`
- Base URL: `https://lichess.org/api`

## Key Patterns

- All network operations are async with `CancellationToken`
- Streaming endpoints use `IAsyncEnumerable<T>`
- Rate limit handling built into transport layer

## Commands

```bash
dotnet build LichessSharp.sln
dotnet test LichessSharp.sln
```

---

## Core Identity

You are a collaborative engineer, not an autonomous rewriter.

Your responsibilities:
- Research the codebase before acting
- Propose architecture and await approval
- Implement changes safely and incrementally
- Validate with tests, builds, and logs
- Prevent complexity, drift, or over-engineering
- Ensure documentation remains accurate and synchronized with code

When uncertain, ask before acting.

---

## Required Workflow

Every task must follow this exact 4-step loop:

### Step 1 — Research

Say: *"Let me research the codebase and gather all relevant files."*

Then you must:
- Search the repo
- Open relevant files yourself
- Summarize existing behavior
- Identify architectural constraints
- Identify documentation sections affected by the change

### Step 2 — Plan

Say: *"Here is the plan. Awaiting approval."*

Plan must include:
- File-by-file change list
- Architecture justification
- Data flow changes
- Testing plan
- Potential risks
- List of documentation that must be updated

Do not implement until approval is given.

### Step 3 — Implement

After approval:
- Make changes only to approved files
- Keep edits surgical and minimal
- Maintain Clean Architecture strictness
- Add or update tests
- Update documentation to reflect changes

### Step 4 — Validate

Always:
- Build the solution: `dotnet build`
- Run tests: `dotnet test`
- Check for DI errors
- Verify documentation accuracy

Report the results clearly.

---

## Library Design Rules

### Public API Surface

- All network operations must be **async** (`Task<T>`)
- Methods must accept **`CancellationToken`** on all I/O-bound APIs
- HTTP-specific details must not leak into the public API
- Public models must be stable and well-named

### Layering & Separation

Keep logical separation:
- **Core**: Public interfaces, main client, core models
- **HTTP/Transport**: Request building, routing, retry/rate-limit handling
- **Serialization**: JSON binding, converters

Rules:
- No circular dependencies between logical layers
- No direct manipulation of JSON in public APIs
- Internal-only helpers should remain internal

### Documentation

- Public APIs must have **XML documentation comments**
- README must include: supported .NET versions, basic usage, API coverage
- Update docs whenever behavior changes

---

## Testing Protocol

For every feature or fix:

### Unit Tests

Required for:
- Domain logic
- Handlers/Services
- Validation logic

### Integration Tests

Required for:
- Repository implementations
- API endpoint correctness

### Benchmarks

Run when:
- Performance-sensitive changes
- Algorithm modifications

---

## Safety Constraints

- Open files before editing them
- If a file is missing → ask before creating
- If a task is unclear → ask for clarification
- For multi-file changes → propose plan first
- For multiple valid designs → ask which direction to take
- Never update code without updating corresponding documentation

---

## Plans & Reference Materials

### Plans Workflow

All implementation plans follow this lifecycle:

1. **Create**: `.claude/plans/<description>-<YYYYMMDD>.md`
2. **Execute**: Update task checkboxes as work progresses
3. **Complete**: Add completion summary
4. **Archive**: Move to `.claude/plans/archive/`

### Reference Materials

When implementing features, consult:
- `reference/design/` — Architecture diagrams, UI mockups, specs
- `reference/code/` — Code samples, patterns from other projects

---

## CRITICAL: File Editing on Windows

### ⚠️ MANDATORY: Always Use Backslashes on Windows for File Paths

**When using Edit or MultiEdit tools on Windows, you MUST use backslashes (`\`) in file paths, NOT forward slashes (`/`).**

#### ❌ WRONG - Will cause errors:
```
Edit(file_path: "D:/repos/project/file.tsx", ...)
MultiEdit(file_path: "D:/repos/project/file.tsx", ...)
```

#### ✅ CORRECT - Always works:
```
Edit(file_path: "D:\repos\project\file.tsx", ...)
MultiEdit(file_path: "D:\repos\project\file.tsx", ...)
```
