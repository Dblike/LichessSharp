<#
.SYNOPSIS
    Generates a structured diff between two OpenAPI spec versions.

.DESCRIPTION
    Compares two bundled OpenAPI JSON specs and produces a report of:
    - New, removed, and changed endpoints
    - New, removed, and modified schemas (with full property-level detail)
    - Cross-references with ImplementedEndpoints.cs to flag affected implementations

.PARAMETER OldSpecPath
    Path to the older OpenAPI JSON file.

.PARAMETER NewSpecPath
    Path to the newer OpenAPI JSON file.

.PARAMETER ImplementedPath
    Path to ImplementedEndpoints.cs for cross-referencing. Defaults to src/LichessSharp/Coverage/ImplementedEndpoints.cs

.PARAMETER OutputPath
    Optional: write the report to a file instead of (in addition to) console output.

.EXAMPLE
    ./scripts/diff-openapi-specs.ps1 -OldSpecPath docs/openapi/snapshots/lichess.openapi.2.0.106.json -NewSpecPath docs/openapi/snapshots/lichess.openapi.2.0.110.json

.EXAMPLE
    ./scripts/diff-openapi-specs.ps1 -OldSpecPath docs/openapi/snapshots/lichess.openapi.2.0.112.json -NewSpecPath docs/openapi/lichess.openapi.json
#>

param(
    [Parameter(Mandatory)]
    [string]$OldSpecPath,
    [Parameter(Mandatory)]
    [string]$NewSpecPath,
    [string]$ImplementedPath = "src/LichessSharp/Coverage/ImplementedEndpoints.cs",
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

# Resolve paths relative to repo root
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir

function Resolve-RepoPath {
    param([string]$Path)
    if ([System.IO.Path]::IsPathRooted($Path)) { return $Path }
    return Join-Path $RepoRoot $Path
}

$OldSpecFullPath = Resolve-RepoPath $OldSpecPath
$NewSpecFullPath = Resolve-RepoPath $NewSpecPath
$ImplementedFullPath = Resolve-RepoPath $ImplementedPath

# Output buffer for optional file output
$outputLines = [System.Collections.Generic.List[string]]::new()

function Write-Report {
    param(
        [string]$Text,
        [string]$Color = "White"
    )
    Write-Host $Text -ForegroundColor $Color
    $outputLines.Add($Text)
}

# ─── Load specs ───

if (-not (Test-Path $OldSpecFullPath)) {
    Write-Error "Old spec not found: $OldSpecFullPath"
    exit 1
}
if (-not (Test-Path $NewSpecFullPath)) {
    Write-Error "New spec not found: $NewSpecFullPath"
    exit 1
}

Write-Report "Loading specs..." "Cyan"
$oldSpec = Get-Content $OldSpecFullPath -Raw | ConvertFrom-Json
$newSpec = Get-Content $NewSpecFullPath -Raw | ConvertFrom-Json

$oldVersion = $oldSpec.info.version
$newVersion = $newSpec.info.version

Write-Report ""
Write-Report "=============================================" "Cyan"
Write-Report "  API Spec Diff: v$oldVersion -> v$newVersion" "Cyan"
Write-Report "=============================================" "Cyan"
Write-Report ""

# ─── Load implemented endpoints for cross-referencing ───

$implementedKeys = @{}
if (Test-Path $ImplementedFullPath) {
    $csContent = Get-Content $ImplementedFullPath -Raw
    $pattern = 'new\s*\(\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*\)'
    $csMatches = [regex]::Matches($csContent, $pattern)
    foreach ($m in $csMatches) {
        $key = "$($m.Groups[1].Value) $($m.Groups[2].Value)"
        $implementedKeys[$key] = "$($m.Groups[3].Value).$($m.Groups[4].Value)"
    }
}

# ─── Extract endpoints from a spec ───

function Get-Endpoints {
    param($spec)
    $endpoints = @{}
    $httpMethods = @("get", "post", "put", "delete", "patch", "head", "options")

    foreach ($pathKey in $spec.paths.PSObject.Properties.Name) {
        $pathItem = $spec.paths.$pathKey
        foreach ($method in $httpMethods) {
            if ($pathItem.PSObject.Properties.Name -contains $method) {
                $operation = $pathItem.$method
                $key = "$($method.ToUpper()) $pathKey"
                $tags = @()
                if ($operation.tags) {
                    $tags = @($operation.tags)
                }
                $endpoints[$key] = @{
                    Method     = $method.ToUpper()
                    Path       = $pathKey
                    Summary    = if ($operation.summary) { $operation.summary } else { "" }
                    Tags       = $tags
                    Deprecated = [bool]$operation.deprecated
                    Parameters = if ($operation.parameters) { $operation.parameters } else { @() }
                }
            }
        }
    }
    return $endpoints
}

# ─── Extract schemas from a spec ───

function Get-Schemas {
    param($spec)
    $schemas = @{}

    if ($spec.components -and $spec.components.schemas) {
        foreach ($prop in $spec.components.schemas.PSObject.Properties) {
            $schema = $prop.Value
            $properties = @{}
            $required = @()

            if ($schema.properties) {
                foreach ($p in $schema.properties.PSObject.Properties) {
                    $propType = ""
                    if ($p.Value.type) {
                        # type can be a string or array (e.g., ["integer", "null"])
                        # Normalize by sorting array types for consistent comparison
                        if ($p.Value.type -is [System.Array] -or $p.Value.type -is [System.Collections.IEnumerable] -and $p.Value.type -isnot [string]) {
                            $propType = (@($p.Value.type) | Sort-Object) -join "|"
                        } else {
                            $propType = [string]$p.Value.type
                        }
                    }
                    elseif ($p.Value.'$ref') { $propType = "ref:$($p.Value.'$ref'.Split('/')[-1])" }
                    elseif ($p.Value.oneOf -or $p.Value.anyOf) {
                        $union = if ($p.Value.oneOf) { $p.Value.oneOf } else { $p.Value.anyOf }
                        $types = @($union | ForEach-Object {
                            if ($_.type) { $_.type }
                            elseif ($_.'$ref') { "ref:$($_.'$ref'.Split('/')[-1])" }
                            else { "unknown" }
                        }) | Sort-Object
                        $propType = ($types -join "|")
                    }

                    $properties[$p.Name] = $propType
                }
            }

            if ($schema.required) {
                $required = @($schema.required)
            }

            $schemas[$prop.Name] = @{
                Properties = $properties
                Required   = $required
                Type       = if ($schema.type) { $schema.type } else { "object" }
                Enum       = if ($schema.enum) { @($schema.enum) } else { @() }
            }
        }
    }

    return $schemas
}

# ─── Compute endpoint diff ───

$oldEndpoints = Get-Endpoints $oldSpec
$newEndpoints = Get-Endpoints $newSpec

$addedEndpoints = @()
$removedEndpoints = @()
$changedEndpoints = @()

foreach ($key in $newEndpoints.Keys) {
    if (-not $oldEndpoints.ContainsKey($key)) {
        $addedEndpoints += $newEndpoints[$key]
    }
}

foreach ($key in $oldEndpoints.Keys) {
    if (-not $newEndpoints.ContainsKey($key)) {
        $removedEndpoints += $oldEndpoints[$key]
    }
}

foreach ($key in $newEndpoints.Keys) {
    if ($oldEndpoints.ContainsKey($key)) {
        $old = $oldEndpoints[$key]
        $new = $newEndpoints[$key]
        $changes = @()

        if ($old.Deprecated -ne $new.Deprecated) {
            if ($new.Deprecated) { $changes += "now deprecated" }
            else { $changes += "no longer deprecated" }
        }

        if ($old.Summary -ne $new.Summary) {
            $changes += "summary changed"
        }

        $oldParamCount = if ($old.Parameters) { @($old.Parameters).Count } else { 0 }
        $newParamCount = if ($new.Parameters) { @($new.Parameters).Count } else { 0 }
        if ($oldParamCount -ne $newParamCount) {
            $changes += "parameters changed ($oldParamCount -> $newParamCount)"
        }

        $oldTags = ($old.Tags | Sort-Object) -join ","
        $newTags = ($new.Tags | Sort-Object) -join ","
        if ($oldTags -ne $newTags) {
            $changes += "tags changed"
        }

        if ($changes.Count -gt 0) {
            $changedEndpoints += @{
                Key     = $key
                Method  = $new.Method
                Path    = $new.Path
                Tags    = $new.Tags
                Changes = $changes
            }
        }
    }
}

# ─── Compute schema diff ───

$oldSchemas = Get-Schemas $oldSpec
$newSchemas = Get-Schemas $newSpec

$addedSchemas = @()
$removedSchemas = @()
$modifiedSchemas = @()

foreach ($name in $newSchemas.Keys) {
    if (-not $oldSchemas.ContainsKey($name)) {
        $addedSchemas += @{ Name = $name; Schema = $newSchemas[$name] }
    }
}

foreach ($name in $oldSchemas.Keys) {
    if (-not $newSchemas.ContainsKey($name)) {
        $removedSchemas += @{ Name = $name }
    }
}

foreach ($name in $newSchemas.Keys) {
    if ($oldSchemas.ContainsKey($name)) {
        $old = $oldSchemas[$name]
        $new = $newSchemas[$name]
        $changes = @()

        # Check for added properties
        foreach ($propName in $new.Properties.Keys) {
            if (-not $old.Properties.ContainsKey($propName)) {
                $changes += "+ $propName ($($new.Properties[$propName]))"
            }
        }

        # Check for removed properties
        foreach ($propName in $old.Properties.Keys) {
            if (-not $new.Properties.ContainsKey($propName)) {
                $changes += "- $propName"
            }
        }

        # Check for type changes on existing properties
        foreach ($propName in $new.Properties.Keys) {
            if ($old.Properties.ContainsKey($propName)) {
                if ($old.Properties[$propName] -ne $new.Properties[$propName]) {
                    $changes += "~ ${propName}: $($old.Properties[$propName]) -> $($new.Properties[$propName])"
                }
            }
        }

        # Check for required field changes
        $oldRequired = ($old.Required | Sort-Object) -join ","
        $newRequired = ($new.Required | Sort-Object) -join ","
        if ($oldRequired -ne $newRequired) {
            $addedRequired = $new.Required | Where-Object { $_ -notin $old.Required }
            $removedRequired = $old.Required | Where-Object { $_ -notin $new.Required }
            if ($addedRequired) {
                $changes += "required+: $($addedRequired -join ', ')"
            }
            if ($removedRequired) {
                $changes += "required-: $($removedRequired -join ', ')"
            }
        }

        # Check for enum changes
        $oldEnum = ($old.Enum | Sort-Object) -join ","
        $newEnum = ($new.Enum | Sort-Object) -join ","
        if ($oldEnum -ne $newEnum) {
            $addedValues = $new.Enum | Where-Object { $_ -notin $old.Enum }
            $removedValues = $old.Enum | Where-Object { $_ -notin $new.Enum }
            if ($addedValues) {
                $changes += "enum+: $($addedValues -join ', ')"
            }
            if ($removedValues) {
                $changes += "enum-: $($removedValues -join ', ')"
            }
        }

        if ($changes.Count -gt 0) {
            $modifiedSchemas += @{
                Name    = $name
                Changes = $changes
            }
        }
    }
}

# ─── Output report ───

# Endpoints
Write-Report "NEW ENDPOINTS ($($addedEndpoints.Count)):" "Green"
if ($addedEndpoints.Count -eq 0) {
    Write-Report "  (none)"
} else {
    $addedEndpoints | Sort-Object { $_.Tags[0] }, { $_.Path } | ForEach-Object {
        $tag = if ($_.Tags.Count -gt 0) { $_.Tags[0] } else { "Other" }
        Write-Report ("  {0,-6} {1,-45} [{2}]" -f $_.Method, $_.Path, $tag)
        if ($_.Summary) {
            Write-Report "         $($_.Summary)" "Gray"
        }
    }
}
Write-Report ""

Write-Report "REMOVED ENDPOINTS ($($removedEndpoints.Count)):" "Red"
if ($removedEndpoints.Count -eq 0) {
    Write-Report "  (none)"
} else {
    $removedEndpoints | Sort-Object { $_.Tags[0] }, { $_.Path } | ForEach-Object {
        $tag = if ($_.Tags.Count -gt 0) { $_.Tags[0] } else { "Other" }
        $impl = if ($implementedKeys.ContainsKey("$($_.Method) $($_.Path)")) { " [IMPLEMENTED]" } else { "" }
        Write-Report ("  {0,-6} {1,-45} [{2}]{3}" -f $_.Method, $_.Path, $tag, $impl) "Red"
    }
}
Write-Report ""

Write-Report "CHANGED ENDPOINTS ($($changedEndpoints.Count)):" "Yellow"
if ($changedEndpoints.Count -eq 0) {
    Write-Report "  (none)"
} else {
    $changedEndpoints | Sort-Object { $_.Tags[0] }, { $_.Path } | ForEach-Object {
        $tag = if ($_.Tags.Count -gt 0) { $_.Tags[0] } else { "Other" }
        $impl = if ($implementedKeys.ContainsKey($_.Key)) { " [IMPLEMENTED: $($implementedKeys[$_.Key])]" } else { "" }
        Write-Report ("  {0,-6} {1,-45} [{2}]{3}" -f $_.Method, $_.Path, $tag, $impl) "Yellow"
        foreach ($change in $_.Changes) {
            Write-Report "         $change" "Gray"
        }
    }
}
Write-Report ""

# Schemas
Write-Report "NEW SCHEMAS ($($addedSchemas.Count)):" "Green"
if ($addedSchemas.Count -eq 0) {
    Write-Report "  (none)"
} else {
    $addedSchemas | Sort-Object { $_.Name } | ForEach-Object {
        $propList = ""
        if ($_.Schema.Properties.Count -gt 0) {
            $names = ($_.Schema.Properties.Keys | Sort-Object | Select-Object -First 5) -join ", "
            if ($_.Schema.Properties.Count -gt 5) { $names += ", ..." }
            $propList = " ($names)"
        }
        Write-Report "  $($_.Name)$propList" "Green"
    }
}
Write-Report ""

Write-Report "REMOVED SCHEMAS ($($removedSchemas.Count)):" "Red"
if ($removedSchemas.Count -eq 0) {
    Write-Report "  (none)"
} else {
    $removedSchemas | Sort-Object { $_.Name } | ForEach-Object {
        Write-Report "  $($_.Name)" "Red"
    }
}
Write-Report ""

Write-Report "MODIFIED SCHEMAS ($($modifiedSchemas.Count)):" "Yellow"
if ($modifiedSchemas.Count -eq 0) {
    Write-Report "  (none)"
} else {
    $modifiedSchemas | Sort-Object { $_.Name } | ForEach-Object {
        Write-Report "  $($_.Name)" "Yellow"
        foreach ($change in $_.Changes) {
            Write-Report "    $change" "Gray"
        }
    }
}
Write-Report ""

# Summary
$totalChanges = $addedEndpoints.Count + $removedEndpoints.Count + $changedEndpoints.Count
$totalSchemaChanges = $addedSchemas.Count + $removedSchemas.Count + $modifiedSchemas.Count
Write-Report "=============================================" "Cyan"
Write-Report "  Summary: $totalChanges endpoint changes, $totalSchemaChanges schema changes" "Cyan"
Write-Report "=============================================" "Cyan"

# Write to file if requested
if ($OutputPath) {
    $outputFullPath = Resolve-RepoPath $OutputPath
    $outputLines | Out-File -FilePath $outputFullPath -Encoding utf8
    Write-Host ""
    Write-Host "Report written to: $outputFullPath" -ForegroundColor Green
}

exit 0
