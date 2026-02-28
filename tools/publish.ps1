if (Test-Path -Path "./MonitorIsland/bin") {
    Remove-Item ./MonitorIsland/bin -recurse
}
dotnet publish -c Release -p:CreateCipx=true