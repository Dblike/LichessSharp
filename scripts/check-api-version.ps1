<#
.SYNOPSIS
    Checks if a newer Lichess API spec version is available.

.DESCRIPTION
    Compares the local OpenAPI spec version with the remote version from the
    lichess-org/api GitHub repository. Designed for both interactive use and
    CI automation.

    Exit codes:
      0 = Local spec is up to date
      1 = Error occurred
      2 = New version available

.PARAMETER Quiet
    Machine-readable output: "local=X.Y.Z remote=A.B.C"

.PARAMETER OpenApiPath
    Path to the local OpenAPI JSON file. Defaults to docs/openapi/lichess.openapi.json

.EXAMPLE
    ./scripts/check-api-version.ps1

.EXAMPLE
    ./scripts/check-api-version.ps1 -Quiet
#>

param(
    [switch]$Quiet,
    [string]$OpenApiPath = "docs/openapi/lichess.openapi.json"
)

$ErrorActionPreference = "Stop"

# Resolve repo root
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$OpenApiFullPath = Join-Path $RepoRoot $OpenApiPath

$RemoteUrl = "https://raw.githubusercontent.com/lichess-org/api/master/doc/specs/lichess-api.yaml"

try {
    # Read local version
    if (-not (Test-Path $OpenApiFullPath)) {
        if (-not $Quiet) {
            Write-Error "Local OpenAPI spec not found: $OpenApiFullPath"
        }
        exit 1
    }

    $localSpec = Get-Content $OpenApiFullPath -Raw | ConvertFrom-Json
    $localVersion = $localSpec.info.version

    if (-not $localVersion) {
        if (-not $Quiet) {
            Write-Error "Could not read version from local spec"
        }
        exit 1
    }

    # Fetch remote version (just the header, not the full spec)
    if (-not $Quiet) {
        Write-Host "Checking Lichess API version..." -ForegroundColor Cyan
        Write-Host "  Local:  $localVersion" -ForegroundColor White
    }

    $response = Invoke-WebRequest -Uri $RemoteUrl -UseBasicParsing -ErrorAction Stop
    $content = $response.Content

    # Extract version from YAML header using regex
    if ($content -match 'version:\s*[''"]?(\d+\.\d+\.\d+)[''"]?') {
        $remoteVersion = $Matches[1]
    } else {
        if (-not $Quiet) {
            Write-Error "Could not parse version from remote spec"
        }
        exit 1
    }

    if (-not $Quiet) {
        Write-Host "  Remote: $remoteVersion" -ForegroundColor White
    }

    # Compare versions
    if ($localVersion -eq $remoteVersion) {
        if ($Quiet) {
            Write-Output "local=$localVersion remote=$remoteVersion"
        } else {
            Write-Host ""
            Write-Host "  Up to date." -ForegroundColor Green
        }
        exit 0
    } else {
        if ($Quiet) {
            Write-Output "local=$localVersion remote=$remoteVersion"
        } else {
            Write-Host ""
            Write-Host "  New version available: $localVersion -> $remoteVersion" -ForegroundColor Yellow
        }
        exit 2
    }

} catch {
    if (-not $Quiet) {
        Write-Error "Failed to check API version: $_"
    }
    exit 1
}
