# /release - LichessSharp Release Workflow

Execute the complete release workflow for LichessSharp. This command automates building, testing, changelog generation, wiki updates, and package creation.

## Arguments

- `$ARGUMENTS` - The new version number (e.g., "0.4.0") or "patch"/"minor"/"major" for automatic version bump

## Workflow Steps

Execute each step in order, stopping if any step fails:

### 1. Pre-flight Checks

```
- Verify working directory is clean (no uncommitted changes)
- Get current version from src/LichessSharp/LichessSharp.csproj
- Get last release tag
- If $ARGUMENTS is "patch"/"minor"/"major", calculate new version automatically
- Otherwise use $ARGUMENTS as the new version
```

### 2. Build and Test

```bash
dotnet build LichessSharp.slnx
dotnet test LichessSharp.slnx --filter "Category!=Integration"
```

If tests fail, STOP and report the failures.

### 3. Gather Changes Since Last Tag

```bash
git log <last-tag>..HEAD --oneline
git log <last-tag>..HEAD --pretty=format:"%h %s" --stat
```

Analyze the commits to understand:
- New features (Added)
- Changed behavior (Changed)
- Bug fixes (Fixed)
- Breaking changes (mark as BREAKING)

### 4. Update Version

Edit `src/LichessSharp/LichessSharp.csproj` and update the `<Version>` element to the new version.

### 5. Generate CHANGELOG Entry

Read the existing CHANGELOG.md and insert a new entry at the top (after the header) following the Keep a Changelog format:

```markdown
## [X.Y.Z] - YYYY-MM-DD

### Added
- **Feature name** — Description

### Changed
- **Change description** — Details

### Fixed
- **Bug fix** — Description

### Breaking Changes (if any)
- **BREAKING**: Description of breaking change
```

Also add the version link at the bottom of the file.

### 6. Check Wiki for Updates Needed

Search the wiki folder for any documentation that references changed APIs:
- Check if any code examples use modified types or methods
- Update examples if the API signatures changed
- Push wiki changes to the wiki repository (it's a separate git repo)

### 7. Check Samples for Updates Needed

Search the samples folder for any code that uses modified APIs:
- Verify all samples still compile
- Update any samples that use changed APIs
- Report any samples that need manual review

### 8. Commit and Tag

```bash
git add -A
git commit -m "Release v<version>

<summary of changes>

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>"

git tag -a v<version> -m "Release v<version>"
git push origin main v<version>
```

### 9. Create Package

```bash
pwsh -File scripts/pack.ps1 -SkipTests
```

### 10. Summary

Report:
- New version number
- Package location (artifacts/LichessSharp.X.Y.Z.nupkg)
- Tag pushed
- CHANGELOG entry created
- Any wiki/sample updates made
- Command to publish to NuGet:
  ```
  dotnet nuget push ./artifacts/LichessSharp.<version>.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
  ```

## Error Handling

- If build fails: Report errors and stop
- If tests fail: Report failing tests and stop
- If git operations fail: Report error and stop
- If uncommitted changes exist: Ask user to commit or stash first

## Example Usage

```
/release 0.4.0
/release patch
/release minor
```
