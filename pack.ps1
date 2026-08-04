<#
    Builds a release archive laid out the way Nexus and Vortex expect:

        BepInEx/plugins/CoffinBreak/CoffinBreak.dll

    Deliberately not the dev deploy path (plugins/MoonlightPeaksMods/CoffinBreak), which only
    exists to keep hand-built DLLs clear of Vortex during development.

    No test project for this mod: its behaviour is timers, window focus and a Harmony patch
    against live game state, none of which a headless runner can assert. ChestLabels has one
    because its label store is pure data. The checklist in RELEASING.md carries the weight here.
#>

$ErrorActionPreference = 'Stop'

$modRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $modRoot 'src\CoffinBreak.csproj'

# This mod lives in two places: on its own as github.com/dirtyredz/Coffin-Break, and inside the
# notes monorepo under mods/CoffinBreak. dist/ belongs at the repo root in both, so work out
# which layout is in play rather than assuming one.
$parentName = Split-Path -Leaf (Split-Path -Parent $modRoot)
if ($parentName -eq 'mods') {
    $repoRoot = Split-Path -Parent (Split-Path -Parent $modRoot)
} else {
    $repoRoot = $modRoot
}

# Single source of truth for the version, so the archive can never disagree with the DLL.
$version = ([xml](Get-Content $project)).Project.PropertyGroup.Version | Where-Object { $_ }
if (-not $version) { throw "Could not read <Version> from $project" }

# The csproj names the archive but BepInEx reports the attribute, so a mismatch ships an
# archive that lies about its contents. Cheap to check, expensive to discover later.
$pluginSource = Join-Path $modRoot 'src\Plugin.cs'
$declared = [regex]::Match(
    (Get-Content $pluginSource -Raw),
    'PluginVersion\s*=\s*"([^"]+)"').Groups[1].Value
if ($declared -ne $version) {
    throw "Version mismatch: csproj says $version, Plugin.cs says $declared"
}

Write-Host "Packing Coffin Break $version"

# SkipDeploy keeps a release build from overwriting the copy under test in the game folder.
dotnet build $project -c Release -p:SkipDeploy=true
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }

$dll = Join-Path $modRoot 'src\bin\Release\netstandard2.1\CoffinBreak.dll'
if (-not (Test-Path $dll)) { throw "Built DLL not found at $dll" }

$staging = Join-Path $env:TEMP "CoffinBreak-pack-$([guid]::NewGuid().ToString('N'))"
$target  = Join-Path $staging 'BepInEx\plugins\CoffinBreak'
New-Item -ItemType Directory -Force -Path $target | Out-Null
Copy-Item $dll $target

$dist = Join-Path $repoRoot 'dist'
New-Item -ItemType Directory -Force -Path $dist | Out-Null

$archive = Join-Path $dist "CoffinBreak-$version.zip"
if (Test-Path $archive) { Remove-Item $archive }

Compress-Archive -Path (Join-Path $staging 'BepInEx') -DestinationPath $archive
Remove-Item $staging -Recurse -Force

Write-Host "Created $archive"
Write-Host 'Extract it over the game folder to install.'
