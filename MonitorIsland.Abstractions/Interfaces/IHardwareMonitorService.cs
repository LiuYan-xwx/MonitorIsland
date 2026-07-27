using MonitorIsland.Abstractions.Models;

namespace MonitorIsland.Abstractions.Interfaces;

public interface IHardwareMonitorService : IDisposable
{
    bool IsSupported { get; }

    bool IsReady { get; }

    Task ReadyTask { get; }

    IReadOnlyList<HardwareSensorGroup> GetSensorGroups(
        IReadOnlySet<SensorKind>? sensorKinds = null);

    float? GetSensorValue(string identifier);
}
