$ErrorActionPreference = "Stop"

$cipxPath = "./cipx"
$publishPath = "./MonitorIsland/bin/Release/net8.0/publish"

if (Test-Path -Path $publishPath)
{
    Remove-Item $publishPath -Recurse -Force
}

dotnet publish ./MonitorIsland/MonitorIsland.csproj -c Release

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

if (Test-Path -Path $cipxPath)
{
    Remove-Item $cipxPath -Recurse -Force
}

New-Item -Path $cipxPath -ItemType Directory

$compress = @{
  Path = "$publishPath/*"
  DestinationPath = "$cipxPath/MonitorIsland.cipx"
  Force = $True
}
Compress-Archive @compress

pwsh -ep bypass "./tools/generate-md5.ps1" $cipxPath