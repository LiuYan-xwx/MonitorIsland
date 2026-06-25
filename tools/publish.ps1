$cipxPath = "./MonitorIsland/cipx"
$publishPath = "./MonitorIsland/bin/Release/net8.0/publish"

dotnet publish ./MonitorIsland/MonitorIsland.csproj -c Release

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