$openapi = Get-Content 'docs/openapi/lichess.openapi.json' -Raw | ConvertFrom-Json
$implemented = Get-Content 'src/LichessSharp/Coverage/ImplementedEndpoints.cs' -Raw

$httpMethods = @('get', 'post', 'put', 'delete', 'patch')

# Parse intentionally-excluded endpoints (3-arg ExcludedEndpoint records: "METHOD", "/path", "reason").
# The 4-arg implemented entries won't match this pattern.
$excludedPattern = 'new\s*\(\s*"(GET|POST|PUT|DELETE|PATCH)"\s*,\s*"([^"]+)"\s*,\s*(?:"[^"]*"\s*\+\s*)*"[^"]*"\s*\)'
$excluded = @{}
foreach ($m in [regex]::Matches($implemented, $excludedPattern)) {
    $excluded["$($m.Groups[1].Value) $($m.Groups[2].Value)"] = $true
}

Write-Host "Missing endpoints:" -ForegroundColor Yellow
Write-Host ""

foreach ($pathKey in $openapi.paths.PSObject.Properties.Name) {
    $pathItem = $openapi.paths.$pathKey
    foreach ($method in $httpMethods) {
        if ($pathItem.PSObject.Properties.Name -contains $method) {
            $upperMethod = $method.ToUpper()
            $endpointKey = "$upperMethod $pathKey"
            $searchPattern = "`"$upperMethod`", `"$pathKey`""
            if (($implemented -notlike "*$searchPattern*") -and (-not $excluded.ContainsKey($endpointKey))) {
                $operation = $pathItem.$method
                $tags = if ($operation.tags) { $operation.tags -join ', ' } else { 'N/A' }
                Write-Host "$upperMethod $pathKey"
                Write-Host "  Tags: $tags"
                Write-Host "  Summary: $($operation.summary)"
                Write-Host ""
            }
        }
    }
}

if ($excluded.Count -gt 0) {
    Write-Host "Intentionally not implemented (excluded from coverage gaps):" -ForegroundColor Cyan
    Write-Host ""
    foreach ($key in $excluded.Keys) {
        Write-Host "  $key" -ForegroundColor Gray
    }
    Write-Host ""
    Write-Host "  See ImplementedEndpoints.IntentionallyNotImplemented for rationale." -ForegroundColor DarkGray
}
