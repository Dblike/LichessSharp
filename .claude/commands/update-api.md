# /update-api - Lichess API Spec Update Workflow

Check for and apply Lichess OpenAPI specification updates, then guide through implementation of any changes.

## Arguments

- `$ARGUMENTS` - Optional: "check" (version check only), "force" (update even if same version)

## Workflow Steps

Execute each step in order, stopping if any step fails:

### 1. Check for Updates

```bash
pwsh -File scripts/check-api-version.ps1
```

If `$ARGUMENTS` is "check", report the version status and stop.

If no new version is available and `$ARGUMENTS` is not "force", report that the spec is up to date and stop.

### 2. Fetch, Bundle, and Install New Spec

Run the update script which handles: shallow clone, Redocly bundling, archiving, README badge update, and diff report:

```bash
pwsh -File scripts/update-openapi-spec.ps1
```

If `$ARGUMENTS` is "force", add the `-Force` flag.

Report the version transition (e.g., v2.0.112 → v2.0.123).

### 3. Review Diff Report

The update script outputs a diff report. Read and present it to the user organized by:

- **New endpoints** — grouped by API tag, with summaries
- **Removed endpoints** — flag any that are currently implemented
- **Changed endpoints** — highlight those marked [IMPLEMENTED]
- **Schema changes** — new/removed schemas, property-level modifications

### 4. Run Endpoint Coverage

```bash
pwsh -File scripts/generate-endpoint-coverage.ps1
```

Present the updated coverage numbers by API tag.

### 5. Find Missing Endpoints

```bash
pwsh -File scripts/find-missing-endpoints.ps1
```

Present the list of unimplemented endpoints, highlighting any that are **newly added** in this spec version.

### 6. Impact Assessment

Analyze the diff to determine:

- Which **implemented endpoints** have breaking changes (parameter changes, deprecation)
- Which **models** in `src/LichessSharp/Models/` may need updating based on schema changes
- Whether any **tests** may fail with the new spec (particularly schema validation tests)
- Priority ranking: breaking changes > schema updates requiring model changes > new endpoints > deprecations

Present a prioritized action list to the user.

### 7. Build and Test

```bash
dotnet build LichessSharp.slnx
dotnet test LichessSharp.slnx --filter "Category!=Integration"
```

If build or tests fail:
- List the failures
- Analyze whether they're caused by the spec update
- Propose fixes

### 8. Commit Spec Update

If build and tests pass (or after applying fixes):

```bash
git add docs/openapi/lichess.openapi.json
git add docs/openapi/snapshots/
git add README.md
```

Commit with message:

```
Update Lichess API spec v{oldVersion} → v{newVersion}

- Archived previous spec as snapshot
- Updated README badge
- {N} new endpoints, {N} changed, {N} removed
- {N} schema changes

Co-Authored-By: Claude Opus 4.6 <noreply@anthropic.com>
```

### 9. Implementation Guidance

For each category of change found in the diff, offer to:

- **New endpoints**: Create interface methods, implementation stubs, models, and ImplementedEndpoints.cs entries following existing patterns
- **Schema changes**: Update affected model classes in `src/LichessSharp/Models/`
- **Deprecations**: Add `[Obsolete("Deprecated in Lichess API vX.Y.Z")]` attributes
- **Removals**: Flag for review, discuss whether to keep for backwards compatibility or remove

Ask the user which changes they want to implement now vs. defer to a future session.

### 10. Summary

Report:
- Spec version: {oldVersion} → {newVersion}
- Snapshot archived: `docs/openapi/snapshots/lichess.openapi.{oldVersion}.json`
- Endpoint changes: {added} new, {changed} changed, {removed} removed
- Schema changes: {added} new, {modified} modified, {removed} removed
- Build status: Pass/Fail
- Test status: Pass/Fail
- Implementation status: what was done, what remains
- Remaining work items for follow-up

## Error Handling

- If version check fails (network error): Report and stop
- If bundling fails: Report error, suggest checking Node.js/npx installation
- If build fails after update: Analyze failures, propose fixes
- If tests fail: List failing tests, analyze if spec-related

## Example Usage

```
/update-api              # Check and update if new version available
/update-api check        # Just check version, no changes
/update-api force        # Re-fetch and update even if version matches
```
