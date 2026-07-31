namespace MonitorIsland.Abstractions.Models;

public sealed class HardwareSensorGroup
{
    public required string Name { get; init; }

    public HardwareKind HardwareKind { get; init; }

    public IReadOnlyList<SensorInfo> Sensors { get; init; } = [];
}
