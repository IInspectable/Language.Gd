
$versioningScripts=Join-Path $PSScriptRoot Versioning.ps1

. $versioningScripts

GetTargetFile | % { IncreaseMinor $_ -verbose }