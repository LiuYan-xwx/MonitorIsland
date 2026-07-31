using MonitorIsland.Abstractions.Interfaces;
using MonitorIsland.Abstractions.Models;

namespace MonitorIsland.Services;

public sealed class UnsupportedHardwareMonitorService : IHardwareMonitorService
{
    public bool IsSupported => false;

    public bool IsReady => true;

    public Task ReadyTask => Task.CompletedTask;

    public IReadOnlyList<HardwareSensorGroup> GetSensorGroups(
        IReadOnlySet<SensorKind>? sensorKinds = null) => [];

    public float? GetSensorValue(string identifier) => null;

    public void Dispose()
    {
    }
}
