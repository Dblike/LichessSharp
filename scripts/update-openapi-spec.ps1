<#
.SYNOPSIS
    Fetches, bundles, and installs the latest Lichess OpenAPI spec.

.DESCRIPTION
    This script:
    1. Checks if a newer spec version is available
    2. Clones the lichess-org/api repo (shallow)
    3. Bundles the multi-file YAML spec into a single JSON using Redocly CLI
    4. Archives the current spec as a versioned snapshot
    5. Installs the new bundled spec
    6. Updates the README badge version
    7. Runs the diff script to show what changed

    Requires: Node.js (for npx), git

.PARAMETER Force
    Update even if local version matches remote.

.PARAMETER SkipArchive
    Don't create a snapshot of the current spec before replacing it.

.PARAMETER SkipDiff
    Don't run the diff report after updating.

.PARAMETER CheckOnly
    Only check versions, don't perform any updates.

.PARAMETER OpenApiPath
    Path to the local OpenAPI JSON file. Defaults to docs/openapi/lichess.openapi.json

.EXAMPLE
    ./scripts/update-openapi-spec.ps1

.EXAMPLE
    ./scripts/update-openapi-spec.ps1 -Force

.EXAMPLE
    ./scripts/update-openapi-spec.ps1 -CheckOnly
#>

param(
    [switch]$Force,
    [switch]$SkipArchive,
    [switch]$SkipDiff,
    [switch]$CheckOnly,
    [string]$OpenApiPath = "docs/openapi/lichess.openapi.json"
)

$ErrorActionPreference = "Stop"

# Resolve paths
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$OpenApiFullPath = Join-Path $RepoRoot $OpenApiPath
$SnapshotsDir = Join-Path $RepoRoot "docs/openapi/snapshots"
$ReadmePath = Join-Path $RepoRoot "README.md"
$DiffScript = Join-Path $ScriptDir "diff-openapi-specs.ps1"

$RemoteRepoUrl = "https://github.com/lichess-org/api.git"
$RemoteRawUrl = "https://raw.githubusercontent.com/lichess-org/api/master/doc/specs/lichess-api.yaml"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Lichess OpenAPI Spec Updater" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ─── Step 1: Read local version ───

Write-Host "[1/7] Reading local spec version..." -ForegroundColor Yellow

if (-not (Test-Path $OpenApiFullPath)) {
    Write-Error "Local OpenAPI spec not found: $OpenApiFullPath"
    exit 1
}

$localSpec = Get-Content $OpenApiFullPath -Raw | ConvertFrom-Json
$localVersion = $localSpec.info.version
Write-Host "  Local version: $localVersion" -ForegroundColor White

# ─── Step 2: Fetch remote version ───

Write-Host "[2/7] Checking remote version..." -ForegroundColor Yellow

try {
    $response = Invoke-WebRequest -Uri $RemoteRawUrl -UseBasicParsing -ErrorAction Stop
    $yamlContent = $response.Content

    if ($yamlContent -match 'version:\s*[''"]?(\d+\.\d+\.\d+)[''"]?') {
        $remoteVersion = $Matches[1]
    } else {
        Write-Error "Could not parse version from remote spec"
        exit 1
    }
} catch {
    Write-Error "Failed to fetch remote spec: $_"
    exit 1
}

Write-Host "  Remote version: $remoteVersion" -ForegroundColor White

# ─── Step 3: Compare ───

if ($localVersion -eq $remoteVersion -and -not $Force) {
    Write-Host ""
    Write-Host "  Already up to date (v$localVersion)." -ForegroundColor Green
    if ($CheckOnly) { exit 0 } else { exit 0 }
}

if ($CheckOnly) {
    Write-Host ""
    Write-Host "  Update available: $localVersion -> $remoteVersion" -ForegroundColor Yellow
    exit 2
}

Write-Host ""
Write-Host "  Updating: $localVersion -> $remoteVersion" -ForegroundColor Cyan
Write-Host ""

# ─── Step 4: Prerequisites check ───

Write-Host "[3/7] Checking prerequisites..." -ForegroundColor Yellow

# Check Node.js
$nodeVersion = & node --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Error "Node.js is required but not found. Install from https://nodejs.org"
    exit 1
}
Write-Host "  Node.js: $nodeVersion" -ForegroundColor Gray

# Check git
$gitVersion = & git --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Error "Git is required but not found."
    exit 1
}
Write-Host "  Git: $($gitVersion -replace 'git version ','')" -ForegroundColor Gray

# ─── Step 5: Clone and bundle ───

Write-Host "[4/7] Fetching and bundling spec..." -ForegroundColor Yellow

$tempDir = Join-Path ([System.IO.Path]::GetTempPath()) "lichess-api-$(Get-Date -Format 'yyyyMMddHHmmss')"

try {
    # Shallow clone just the spec files
    Write-Host "  Cloning lichess-org/api (shallow)..." -ForegroundColor Gray
    & git clone --depth 1 --filter=blob:none --sparse $RemoteRepoUrl $tempDir 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to clone repository"
    }

    Push-Location $tempDir
    & git sparse-checkout set doc/specs 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to set sparse checkout"
    }
    Pop-Location

    # Bundle with Redocly CLI
    $bundledPath = Join-Path $tempDir "bundled.json"
    $specEntryPoint = Join-Path $tempDir "doc/specs/lichess-api.yaml"

    if (-not (Test-Path $specEntryPoint)) {
        throw "Spec entry point not found at: $specEntryPoint"
    }

    Write-Host "  Bundling with Redocly CLI..." -ForegroundColor Gray
    & npx @redocly/cli bundle $specEntryPoint -o $bundledPath 2>&1 | ForEach-Object {
        if ($_ -match "error|Error") {
            Write-Host "  $_" -ForegroundColor Red
        }
    }

    if (-not (Test-Path $bundledPath)) {
        throw "Bundling failed: output file not created"
    }

    $bundledSize = (Get-Item $bundledPath).Length
    Write-Host "  Bundled spec: $([math]::Round($bundledSize / 1KB)) KB" -ForegroundColor Gray

    # Verify the bundled spec is valid JSON with the expected version
    $bundledSpec = Get-Content $bundledPath -Raw | ConvertFrom-Json
    $bundledVersion = $bundledSpec.info.version

    if ($bundledVersion -ne $remoteVersion) {
        Write-Host "  Warning: Bundled version ($bundledVersion) differs from expected ($remoteVersion)" -ForegroundColor Yellow
    }

    # ─── Step 6: Archive current spec ───

    if (-not $SkipArchive) {
        Write-Host "[5/7] Archiving current spec..." -ForegroundColor Yellow

        if (-not (Test-Path $SnapshotsDir)) {
            New-Item -ItemType Directory -Path $SnapshotsDir -Force | Out-Null
        }

        $snapshotPath = Join-Path $SnapshotsDir "lichess.openapi.$localVersion.json"
        if (Test-Path $snapshotPath) {
            Write-Host "  Snapshot already exists: $snapshotPath" -ForegroundColor Gray
        } else {
            Copy-Item -Path $OpenApiFullPath -Destination $snapshotPath
            Write-Host "  Archived: lichess.openapi.$localVersion.json" -ForegroundColor Gray
        }
    } else {
        Write-Host "[5/7] Skipping archive (-SkipArchive)" -ForegroundColor DarkGray
    }

    # ─── Step 7: Install new spec ───

    Write-Host "[6/7] Installing new spec..." -ForegroundColor Yellow
    Copy-Item -Path $bundledPath -Destination $OpenApiFullPath -Force
    Write-Host "  Updated: $OpenApiPath" -ForegroundColor Gray

    # Update README badge
    if (Test-Path $ReadmePath) {
        $readmeContent = Get-Content $ReadmePath -Raw
        $oldBadge = "Lichess%20API-v$localVersion"
        $newBadge = "Lichess%20API-v$remoteVersion"

        if ($readmeContent -match [regex]::Escape($oldBadge)) {
            $readmeContent = $readmeContent -replace [regex]::Escape($oldBadge), $newBadge
            Set-Content -Path $ReadmePath -Value $readmeContent -NoNewline
            Write-Host "  Updated README badge: v$localVersion -> v$remoteVersion" -ForegroundColor Gray
        } else {
            Write-Host "  Warning: Could not find badge to update in README" -ForegroundColor Yellow
        }
    }

    # ─── Step 8: Diff report ───

    if (-not $SkipDiff) {
        Write-Host "[7/7] Generating diff report..." -ForegroundColor Yellow
        Write-Host ""

        $snapshotForDiff = Join-Path $SnapshotsDir "lichess.openapi.$localVersion.json"
        $diffReportPath = Join-Path $RepoRoot "docs/openapi/diff-$localVersion-to-$remoteVersion.txt"
        if (Test-Path $snapshotForDiff) {
            & pwsh -File $DiffScript -OldSpecPath $snapshotForDiff -NewSpecPath $OpenApiFullPath -OutputPath $diffReportPath
        } else {
            Write-Host "  Cannot generate diff: old snapshot not available" -ForegroundColor Yellow
        }
    } else {
        Write-Host "[7/7] Skipping diff (-SkipDiff)" -ForegroundColor DarkGray
    }

} finally {
    # Clean up temp directory
    if (Test-Path $tempDir) {
        Remove-Item -Path $tempDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# ─── Summary ───

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Spec update complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "  Version: $localVersion -> $remoteVersion" -ForegroundColor White
if (-not $SkipArchive) {
    Write-Host "  Snapshot: docs/openapi/snapshots/lichess.openapi.$localVersion.json" -ForegroundColor White
}
Write-Host "  Spec:     $OpenApiPath" -ForegroundColor White
Write-Host "  README:   Badge updated" -ForegroundColor White
if (-not $SkipDiff) {
    Write-Host "  Report:   docs/openapi/diff-$localVersion-to-$remoteVersion.txt" -ForegroundColor White
}
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Review the diff report above" -ForegroundColor White
Write-Host "  2. Run: pwsh -File scripts/generate-endpoint-coverage.ps1" -ForegroundColor White
Write-Host "  3. Run: pwsh -File scripts/find-missing-endpoints.ps1" -ForegroundColor White
Write-Host "  4. Run: dotnet build LichessSharp.slnx" -ForegroundColor White
Write-Host "  5. Run: dotnet test LichessSharp.slnx" -ForegroundColor White
Write-Host ""

exit 0
