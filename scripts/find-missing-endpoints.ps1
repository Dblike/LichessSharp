$openapi = Get-Content 'docs/openapi/lichess.openapi.json' -Raw | ConvertFrom-Json
$implemented = Get-Content 'src/LichessSharp/Coverage/ImplementedEndpoints.cs' -Raw

$httpMethods = @('get', 'post', 'put', 'delete', 'patch')

Write-Host "Missing endpoints:" -ForegroundColor Yellow
Write-Host ""

foreach ($pathKey in $openapi.paths.PSObject.Properties.Name) {
    $pathItem = $openapi.paths.$pathKey
    foreach ($method in $httpMethods) {
        if ($pathItem.PSObject.Properties.Name -contains $method) {
            $upperMethod = $method.ToUpper()
            $searchPattern = "`"$upperMethod`", `"$pathKey`""
            if ($implemented -notlike "*$searchPattern*") {
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
