<#
.SYNOPSIS
    Builds and packages LichessSharp for NuGet.

.DESCRIPTION
    This script builds the LichessSharp library in Release configuration,
    runs tests, and creates the NuGet package (.nupkg) and symbol package (.snupkg).

.PARAMETER SkipTests
    Skip running unit tests before packaging.

.PARAMETER Version
    Override the package version (default: uses version from .csproj).

.PARAMETER OutputDir
    Output directory for packages (default: ./artifacts).

.EXAMPLE
    ./scripts/pack.ps1

.EXAMPLE
    ./scripts/pack.ps1 -SkipTests -Version "0.2.0"

.EXAMPLE
    ./scripts/pack.ps1 -OutputDir "./release"
#>

param(
    [switch]$SkipTests,
    [string]$Version,
    [string]$OutputDir = "./artifacts"
)

$ErrorActionPreference = "Stop"
$ProjectPath = "src/LichessSharp/LichessSharp.csproj"

# Navigate to repository root
$RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if (Test-Path (Join-Path $PSScriptRoot "../$ProjectPath")) {
    $RepoRoot = Split-Path -Parent $PSScriptRoot
}
Push-Location $RepoRoot

try {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  LichessSharp NuGet Package Builder" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""

    # Clean previous artifacts
    if (Test-Path $OutputDir) {
        Write-Host "[1/4] Cleaning previous artifacts..." -ForegroundColor Yellow
        Remove-Item -Path "$OutputDir/*.nupkg" -Force -ErrorAction SilentlyContinue
        Remove-Item -Path "$OutputDir/*.snupkg" -Force -ErrorAction SilentlyContinue
    } else {
        New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    }

    # Build
    Write-Host "[2/4] Building in Release configuration..." -ForegroundColor Yellow
    $buildArgs = @("build", $ProjectPath, "--configuration", "Release")
    & dotnet @buildArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
    Write-Host "  Build successful!" -ForegroundColor Green

    # Test
    if (-not $SkipTests) {
        Write-Host "[3/4] Running unit tests..." -ForegroundColor Yellow
        $testArgs = @("test", "--configuration", "Release", "--filter", "Category!=Integration")
        & dotnet @testArgs
        if ($LASTEXITCODE -ne 0) {
            throw "Tests failed with exit code $LASTEXITCODE"
        }
        Write-Host "  All tests passed!" -ForegroundColor Green
    } else {
        Write-Host "[3/4] Skipping tests (--SkipTests)" -ForegroundColor DarkGray
    }

    # Pack
    Write-Host "[4/4] Creating NuGet package..." -ForegroundColor Yellow
    $packArgs = @("pack", $ProjectPath, "--configuration", "Release", "--output", $OutputDir, "--no-build")
    if ($Version) {
        $packArgs += "/p:Version=$Version"
    }
    & dotnet @packArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Pack failed with exit code $LASTEXITCODE"
    }

    # Summary
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  Package created successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Packages:" -ForegroundColor Cyan
    Get-ChildItem -Path $OutputDir -Filter "*.nupkg" | ForEach-Object {
        Write-Host "  $($_.Name)" -ForegroundColor White
    }
    Get-ChildItem -Path $OutputDir -Filter "*.snupkg" | ForEach-Object {
        Write-Host "  $($_.Name) (symbols)" -ForegroundColor DarkGray
    }
    Write-Host ""
    Write-Host "To publish to NuGet.org:" -ForegroundColor Cyan
    Write-Host "  dotnet nuget push $OutputDir/LichessSharp.*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor White
    Write-Host ""

} finally {
    Pop-Location
}
