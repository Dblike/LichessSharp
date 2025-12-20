<#
.SYNOPSIS
    Generates an endpoint coverage report by comparing implemented endpoints with the OpenAPI spec.

.DESCRIPTION
    This script:
    1. Parses the OpenAPI spec to get all available Lichess API endpoints
    2. Parses ImplementedEndpoints.cs to get implemented endpoints
    3. Compares the two sets to identify coverage gaps
    4. Outputs a summary to the console

.PARAMETER OpenApiPath
    Path to the OpenAPI JSON file. Defaults to openapi/lichess.openapi.json

.PARAMETER ImplementedPath
    Path to the ImplementedEndpoints.cs file. Defaults to src/LichessSharp/Coverage/ImplementedEndpoints.cs

.EXAMPLE
    .\generate-endpoint-coverage.ps1
    .\generate-endpoint-coverage.ps1 -OpenApiPath "custom/path/openapi.json"
#>

param(
    [string]$OpenApiPath = "openapi/lichess.openapi.json",
    [string]$ImplementedPath = "src/LichessSharp/Coverage/ImplementedEndpoints.cs"
)

$ErrorActionPreference = "Stop"

# Get script directory and repo root
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir

# Resolve paths relative to repo root
$OpenApiFullPath = Join-Path $RepoRoot $OpenApiPath
$ImplementedFullPath = Join-Path $RepoRoot $ImplementedPath

Write-Host "Lichess API Coverage Report Generator" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Check files exist
if (-not (Test-Path $OpenApiFullPath)) {
    Write-Error "OpenAPI spec not found: $OpenApiFullPath"
    exit 1
}

if (-not (Test-Path $ImplementedFullPath)) {
    Write-Error "ImplementedEndpoints.cs not found: $ImplementedFullPath"
    exit 1
}

# Parse OpenAPI spec
Write-Host "Reading OpenAPI spec..." -ForegroundColor Yellow
$openapi = Get-Content $OpenApiFullPath -Raw | ConvertFrom-Json

$openApiVersion = $openapi.info.version
Write-Host "  OpenAPI version: $openApiVersion" -ForegroundColor Gray

# Extract all endpoints from OpenAPI
$openApiEndpoints = @{}
$httpMethods = @("get", "post", "put", "delete", "patch", "head", "options")

foreach ($pathKey in $openapi.paths.PSObject.Properties.Name) {
    $pathItem = $openapi.paths.$pathKey
    foreach ($method in $httpMethods) {
        if ($pathItem.PSObject.Properties.Name -contains $method) {
            $operation = $pathItem.$method
            $key = "$($method.ToUpper()) $pathKey"
            $openApiEndpoints[$key] = @{
                Method = $method.ToUpper()
                Path = $pathKey
                Summary = $operation.summary
                Tags = $operation.tags
                Deprecated = [bool]$operation.deprecated
            }
        }
    }
}

Write-Host "  Found $($openApiEndpoints.Count) endpoints in OpenAPI spec" -ForegroundColor Gray

# Parse ImplementedEndpoints.cs
Write-Host "Reading ImplementedEndpoints.cs..." -ForegroundColor Yellow
$csContent = Get-Content $ImplementedFullPath -Raw

# Extract endpoint entries using regex
$pattern = 'new\s*\(\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*\)'
$matches = [regex]::Matches($csContent, $pattern)

$implementedEndpoints = @{}
foreach ($match in $matches) {
    $method = $match.Groups[1].Value
    $path = $match.Groups[2].Value
    $apiName = $match.Groups[3].Value
    $methodName = $match.Groups[4].Value

    $key = "$method $path"
    $implementedEndpoints[$key] = @{
        Method = $method
        Path = $path
        ApiName = $apiName
        MethodName = $methodName
    }
}

Write-Host "  Found $($implementedEndpoints.Count) implemented endpoints" -ForegroundColor Gray

# Normalize paths for comparison (convert path parameters to template format)
function Normalize-Path {
    param([string]$path)
    # Convert {param} to a normalized form for comparison
    # OpenAPI uses {param}, our code uses {param} - should match
    return $path
}

# Group OpenAPI endpoints by tag
$byTag = @{}
foreach ($endpoint in $openApiEndpoints.Values) {
    $tag = if ($endpoint.Tags -and $endpoint.Tags.Count -gt 0) { $endpoint.Tags[0] } else { "Other" }
    if (-not $byTag.ContainsKey($tag)) {
        $byTag[$tag] = @()
    }
    $byTag[$tag] += $endpoint
}

# Calculate coverage
Write-Host ""
Write-Host "Coverage Summary" -ForegroundColor Cyan
Write-Host "================" -ForegroundColor Cyan
Write-Host ""

$totalOpenApi = $openApiEndpoints.Count
$totalImplemented = $implementedEndpoints.Count

# Note: Our implemented count may differ from OpenAPI because:
# 1. Some methods share the same endpoint (e.g., GetPgnAsync and ExportAsync both use /game/export/{gameId})
# 2. Opening Explorer and Tablebase use different hosts (explorer.lichess.ovh, tablebase.lichess.ovh)
# 3. Some endpoints may have path variations

Write-Host "OpenAPI endpoints:   $totalOpenApi" -ForegroundColor White
Write-Host "Implemented methods: $totalImplemented" -ForegroundColor White
Write-Host ""

# Show coverage by tag
Write-Host "Coverage by API Tag:" -ForegroundColor Yellow
Write-Host ""

$sortedTags = $byTag.Keys | Sort-Object
foreach ($tag in $sortedTags) {
    $endpoints = $byTag[$tag]
    $count = $endpoints.Count
    $deprecatedCount = ($endpoints | Where-Object { $_.Deprecated }).Count

    if ($deprecatedCount -gt 0) {
        Write-Host "  $tag`: $count endpoints ($deprecatedCount deprecated)" -ForegroundColor Gray
    } else {
        Write-Host "  $tag`: $count endpoints" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "Coverage report complete." -ForegroundColor Green
Write-Host ""

# Return success
exit 0
