param(
    [string]$Root = (Get-Location).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$guardErrors = New-Object System.Collections.Generic.List[string]

function Add-GuardError {
    param([string]$Message)
    $guardErrors.Add($Message)
    Write-Host "::error::$Message"
}

function Get-RelativePath {
    param([string]$BasePath, [string]$TargetPath)
    return [IO.Path]::GetRelativePath($BasePath, $TargetPath)
}

Write-Host "Running architecture guardrails in: $Root"

# Rule 1: Folder normalization guardrail
$legacyFolder = Join-Path $Root "AlfaCharge.OcppServer\Versioned Handlers"
if (Test-Path $legacyFolder) {
    Add-GuardError "Legacy folder detected: $(Get-RelativePath -BasePath $Root -TargetPath $legacyFolder). Use Versioned_Handlers only."
}

# Rule 2: Project reference dependency direction
$allowedReferences = @{
    "AlfaCharge.Domain.csproj" = @()
    "AlfaCharge.Infrastructure.csproj" = @("AlfaCharge.Domain.csproj")
    "AlfaCharge.OcppServer.csproj" = @("AlfaCharge.Domain.csproj", "AlfaCharge.Infrastructure.csproj")
    "AlfaCharge.Api.csproj" = @("AlfaCharge.Infrastructure.csproj", "AlfaCharge.OcppServer.csproj")
    "AlfaCharge.Admin.csproj" = @("AlfaCharge.Domain.csproj")
    "AlfaGrid.csproj" = @()
}

$projectFiles = Get-ChildItem -Path $Root -Recurse -Filter *.csproj -File |
    Where-Object { $_.FullName -notmatch "\\(bin|obj)\\" }

foreach ($projectFile in $projectFiles) {
    $projectName = $projectFile.Name
    if (-not $allowedReferences.ContainsKey($projectName)) {
        continue
    }

    [xml]$xml = Get-Content -Path $projectFile.FullName -Raw
    $actualRefs = @()

    foreach ($itemGroup in $xml.Project.ItemGroup) {
        $projectReferenceProperty = $itemGroup.PSObject.Properties["ProjectReference"]
        if ($null -eq $projectReferenceProperty) {
            continue
        }

        foreach ($reference in $projectReferenceProperty.Value) {
            if ($null -ne $reference -and $null -ne $reference.Include) {
                $actualRefs += [IO.Path]::GetFileName([string]$reference.Include)
            }
        }
    }

    $allowed = $allowedReferences[$projectName]
    foreach ($ref in $actualRefs) {
        if ($allowed -notcontains $ref) {
            $relativeProject = Get-RelativePath -BasePath $Root -TargetPath $projectFile.FullName
            Add-GuardError "$relativeProject has forbidden reference '$ref'. Allowed: $($allowed -join ', ')"
        }
    }
}

# Rule 3: Avoid NotImplementedException in API/Infrastructure production code
$scanRoots = @(
    (Join-Path -Path $Root -ChildPath "AlfaCharge.Api"),
    (Join-Path -Path $Root -ChildPath "AlfaCharge.Infrastructure")
)

# Rule 4: Namespace consistency for OCPP prefix
$ocppSourceRoot = Join-Path -Path $Root -ChildPath "AlfaCharge.OcppServer"
if (Test-Path $ocppSourceRoot) {
    $ocppCodeFiles = Get-ChildItem -Path $ocppSourceRoot -Recurse -Filter *.cs -File |
        Where-Object { $_.FullName -notmatch "\\(bin|obj)\\" }

    foreach ($file in $ocppCodeFiles) {
        $content = Get-Content -Path $file.FullName -Raw
        if ($content -match "AlphaCharge\.OcppServer") {
            $relativeFile = Get-RelativePath -BasePath $Root -TargetPath $file.FullName
            Add-GuardError "Legacy namespace prefix found: $relativeFile. Use 'AlfaCharge.OcppServer'."
        }
    }
}

foreach ($scanRoot in $scanRoots) {
    if (-not (Test-Path $scanRoot)) {
        continue
    }

    $codeFiles = Get-ChildItem -Path $scanRoot -Recurse -Filter *.cs -File |
        Where-Object { $_.FullName -notmatch "\\(bin|obj)\\" }

    foreach ($file in $codeFiles) {
        $content = Get-Content -Path $file.FullName -Raw
        if ($content -match "throw\s+new\s+NotImplementedException\s*\(") {
            $relativeFile = Get-RelativePath -BasePath $Root -TargetPath $file.FullName
            Add-GuardError "NotImplementedException found in production path: $relativeFile"
        }
    }
}

if ($guardErrors.Count -gt 0) {
    Write-Host "Architecture guardrails failed with $($guardErrors.Count) violation(s)."
    exit 1
}

Write-Host "Architecture guardrails passed."
