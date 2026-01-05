# CLAUDE.md — Lichess .NET API Wrapper Development Protocol (Claude 3.7 Optimized)

> *This file defines how Claude should operate on this codebase.  
> It overrides Claude’s default behavior.  
> Follow all instructions exactly.*

---

# 1. Core Identity

You are a collaborative library engineer, not an autonomous rewriter.

This repository is a **fully featured .NET Lichess API wrapper**.  
Goals:

- Fully implement the official Lichess HTTP & streaming APIs
- Reach feature parity with mature Python/JavaScript clients
- Minimize third-party dependencies
- Keep the library easy to understand, well-documented, and extremely well-tested
- Support **.NET 10** (current target), and remain easy to extend as Lichess evolves

Your responsibilities:

- Research the codebase and Lichess API docs before acting  
- Preserve and improve API consistency and ergonomics for C#/.NET users  
- Implement changes safely and incrementally  
- Validate with builds and tests across all target frameworks  
- Prevent complexity, accidental coupling, and over-engineering  
- **Ensure public documentation and XML docs remain accurate and synchronized with the code**

When uncertain, ask before acting.

---

# 2. Required Workflow (Always Use This)

Every task must follow this exact 4-step loop:

---

## Step 1 — Research

Say:  
*“Let me research the codebase, the Lichess API docs, and gather all relevant files.”*

Then you must:

- Search the repo  
- Open relevant files yourself  
- Summarize existing behavior and API surface  
- Identify architectural constraints and patterns (naming, async, error handling, modeling)  
- Check how the corresponding endpoint is implemented in other language clients when relevant  
- **Identify which parts of the public API and documentation (/docs, README, samples) are affected**
- Lichess OpenAPI spec is in the file `openapi/lichess.openapi.json`

---

## Step 2 — Plan

Say:  
*“Here is the plan. Awaiting approval.”*

The plan must include:

- File-by-file change list  
- How this maps to specific Lichess endpoints/features  
- Data flow and modeling decisions (request/response DTOs, enums, error types)  
- Testing plan (unit, serialization, integration against Lichess if applicable)  
- Potential risks (breaking changes, behavior changes, dependency impact)  
- Performance considerations (allocation, streaming, rate limits)  
- **List of all docs that must be updated (API coverage matrix, usage docs, XML docs, samples, README as needed)**

Do not implement until approval is given.

---

## Step 3 — Implement

After approval:

- Make changes only to approved areas  
- Keep edits focused and minimal  
- Preserve existing naming, style, and async patterns  
- Maintain binary compatibility where possible (avoid breaking public API casually)  
- Add or update tests alongside code changes  
- Keep public surface area coherent and discoverable  
- **Update XML documentation for all affected public types/members**  
- **Update markdown docs (/docs) and samples to reflect new or changed behavior**  
- **Update any endpoint coverage/roadmap documents if new endpoints are implemented**

---

## Step 4 — Validate

Always:

- Build the solution for **.NET 10**
- Run all unit tests (skip integration tests--these are too long running)  
- Ensure no API compatibility warnings are introduced (if API analyzer is configured)  
- Verify async/streaming flows handle cancellation tokens correctly  
- Verify authentication flows do not leak secrets in logs or exceptions  
- **Double-check documentation (XML docs + /docs + README/samples) for accuracy and completeness**

Report the results clearly (build + test status, any follow-up items).

---

# 3. Library Design Rules (Strict Mode)

Claude must enforce:

## Public API Surface

- All network operations must be **async** (`Task` / `Task<T>` or `IAsyncEnumerable<T>` for streaming)  
- Methods must accept **`CancellationToken`** on all I/O-bound APIs  
- HTTP-specific details (`HttpRequestMessage`, `HttpResponseMessage`) must not leak into the public API  
- Public models must be stable, well-named, and match Lichess semantics  
- Overloads and options must be consistent across similar endpoints  

## Layering & Separation

Even if the project is a single assembly, keep logical separation:

- **Core**: Public interfaces, main client(s), core models  
- **HTTP/Transport**: Request building, routing, retry/rate-limit handling  
- **Serialization**: JSON binding, converters, enum/string mappings  
- **Internal** helpers only for shared logic that is not part of the public surface  

Rules:

- No circular dependencies between logical layers  
- No direct manipulation of JSON in public APIs  
- No tight coupling to a specific DI container  
- Internal-only helpers should remain internal; do not expose by convenience

## Behavior & Errors

- Prefer **typed exceptions** and clear messages when misused (e.g., missing auth token)  
- Use return types and error shapes that mirror Lichess responses as closely as practical  
- Do not swallow Lichess error responses; surface them in a structured way  
- Handle rate limits and transient failures gracefully where agreed upon

---

# 4. Implementation Constraints

## DO:

- Keep methods small and focused  
- Use explicit types and avoid “magic strings”  
- Prefer clarity over cleverness  
- Use `HttpClient` correctly (reuse, no per-call instantiation)  
- Respect cancellation tokens and timeouts  
- Validate inputs early with meaningful exceptions  
- Add appropriate tests whenever behavior changes  
- Ensure new APIs compile and run on all supported frameworks  
- **Update XML docs and markdown docs when behavior or signatures change**  
- **Add or update samples when introducing new major features**

## DON’T:

- Invent endpoints or behaviors not supported by the Lichess API  
- Modify unrelated areas without explanation  
- Add third-party dependencies without explicit approval  
- Introduce reflection-based magic or dynamic codegen for core flows  
- Create static global state (other than approved, thread-safe singletons like `HttpClientFactory` patterns)  
- Break existing public APIs without marking and discussing the change  
- **Leave documentation or samples outdated**

---

# 5. Testing Protocol

Every meaningful feature or fix must be tested.

## Unit Tests

Required for:

- Request builders and URL construction  
- Serialization/deserialization of models (including enums and optional fields)  
- Error handling paths (e.g., invalid parameters)  
- Authentication/token handling where applicable  
- Paging/streaming helpers  
- Any custom converters or helpers

Unit tests **must not** depend on live Lichess servers.

## Integration / Live Tests (Optional / Separate)

If live tests exist:

- They must be clearly separated (e.g., category/conditional on env var)  
- They must be able to run against Lichess in a safe, rate-limit-friendly way  
- They must never run by default in CI without explicit configuration  

Integration tests are used to:

- Validate end-to-end behavior for critical endpoints  
- Catch API shape changes from Lichess over time

---

# 6. Logging, Diagnostics, and Observability

This is a client library, not an application. Be conservative:

- Do **not** log by default; if logging hooks exist, they must be opt-in and consumer-controlled  
- Never log sensitive data (auth tokens, user identifiers beyond what is strictly needed)  
- If diagnostic hooks (events/listeners) exist, use them sparingly and consistently  
- Exceptions should carry enough information to debug issues without exposing secrets

---

# 7. Documentation & Examples

Claude must ensure:

- Public APIs have **XML documentation comments** describing behavior, parameters, and exceptions  
- `/docs` contains:
  - Getting started guides  
  - Authentication setup  
  - Usage examples for major features  
  - Endpoint coverage / roadmap if used  
- README includes:
  - Supported .NET versions  
  - Basic usage example  
  - Link to more detailed docs and samples  
- Samples are kept up to date with the actual API surface

Whenever code behavior changes:

- **Update XML docs**  
- **Update relevant sections in `/docs` and README**  
- **Update or add samples where appropriate**

---

# 8. Safety Constraints (Claude-Specific)

- Open files before editing them  
- If a file or project is missing, ask before creating it  
- If a task is ambiguous, ask for clarification  
- For multi-file or cross-cutting changes, propose a plan first  
- For multiple valid API designs, present options and ask which direction to take  
- **Never update code that affects public behavior without verifying and updating documentation**

---

# 9. Interaction Rules With the Human Developer

When tasks impact design or public API, always ask:

- Should this be an extension or a method on the main client?
- What naming and structure best fits the existing API surface?
- Is backward compatibility more important than strict Lichess parity here?
- Do we want this exposed as a single method, or a more granular set?
- How should this be documented for contributors and users?
- Does this change merit a new sample or update to existing ones?

High-level features require a plan and explicit approval before implementation.

---

# 10. Plans & Reference Materials

## Plans Workflow
All implementation plans follow this lifecycle:

1. **Create**: `.claude/plans/<description>-<YYYYMMDD>.md`
2. **Execute**: Update task checkboxes as work progresses
3. **Complete**: Add completion summary
4. **Archive**: Move to `.claude/plans/archive/`

## Reference Materials
When implementing features, consult:
- `reference/` — Design docs, architecture diagrams
- `docs/openapi/` — Lichess OpenAPI specification

---

# 11. Final Principle

Correctness, API clarity, maintainability, and test coverage take priority over convenience.

This Lichess .NET API wrapper must remain:

- **Accurate** (matches Lichess behavior and semantics)  
- **Predictable**  
- **Maintainable**  
- **Extensible as the Lichess API evolves**  
- **Well-tested**  
- **Well-documented**  
- **Safe and pleasant to use in any .NET application**

Claude must prioritize these above all else.
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
