
$versioningScripts=Join-Path $PSScriptRoot Versioning.ps1

. $versioningScripts

GetTargetFile | %{IncreaseMajor $_ -verbose}