using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions.Interfaces;
using MonitorIsland.Abstractions.Models;

namespace MonitorIsland.Windows.Services;

public sealed class WindowsHardwareMonitorService : IHardwareMonitorService
{
    private readonly ILogger<WindowsHardwareMonitorService> _logger;
    private readonly object _lock = new();
    private readonly Dictionary<string, ISensor> _sensorCache = [];
    private Computer? _computer;
    private volatile bool _isReady;
    private bool _disposed;

    public WindowsHardwareMonitorService(ILogger<WindowsHardwareMonitorService> logger)
    {
        _logger = logger;
        ReadyTask = InitializeAsync();
    }

    public bool IsSupported => true;

    public bool IsReady => _isReady;

    public Task ReadyTask { get; }

    private async Task InitializeAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                var computer = new Computer
                {
                    IsCpuEnabled = true,
                    IsGpuEnabled = true,
                    IsMotherboardEnabled = true,
                    IsStorageEnabled = true,
                    IsMemoryEnabled = true,
                    IsNetworkEnabled = true,
                    IsBatteryEnabled = true
                };
                computer.Open();

                lock (_lock)
                {
                    _computer = computer;
                    _isReady = true;
                    CacheSensorsInternal();
                }
            });

            _logger.LogInformation("LibreHardwareMonitor 已初始化");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化 LibreHardwareMonitor 失败");
        }
    }

    public IReadOnlyList<HardwareSensorGroup> GetSensorGroups(
        IReadOnlySet<SensorKind>? sensorKinds = null)
    {
        lock (_lock)
        {
            if (!_isReady || _computer is null)
                return [];

            var kinds = sensorKinds ?? new HashSet<SensorKind> { SensorKind.Temperature };
            var groups = new List<HardwareSensorGroup>();

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();
                var sensors = hardware.Sensors
                    .Where(sensor => kinds.Contains(ToSensorKind(sensor.SensorType)))
                    .OrderBy(sensor => sensor.SensorType)
                    .ThenBy(sensor => sensor.Name)
                    .ToList();

                if (sensors.Count == 0)
                    continue;

                var group = new HardwareSensorGroup
                {
                    Name = hardware.Name,
                    HardwareKind = ToHardwareKind(hardware.HardwareType),
                    Sensors = sensors
                        .Select(sensor => CreateSensorInfo(hardware, sensor))
                        .ToList()
                };

                groups.Add(group);
            }

            return groups;
        }
    }

    public float? GetSensorValue(string identifier)
    {
        lock (_lock)
        {
            if (!_isReady || _computer is null)
                return null;

            if (TryReadCachedSensor(identifier, out var value))
                return value;

            CacheSensorsInternal();
            if (TryReadCachedSensor(identifier, out value))
                return value;

            _logger.LogWarning("传感器 {Identifier} 未找到或没有可用值", identifier);
            return null;
        }
    }

    private bool TryReadCachedSensor(string identifier, out float value)
    {
        if (_sensorCache.TryGetValue(identifier, out var sensor))
        {
            sensor.Hardware.Update();
            if (sensor.Value is { } sensorValue)
            {
                value = sensorValue;
                return true;
            }
        }

        value = default;
        return false;
    }

    private void CacheSensorsInternal()
    {
        _sensorCache.Clear();
        if (_computer is null)
            return;

        foreach (var hardware in _computer.Hardware)
        {
            hardware.Update();
            foreach (var sensor in hardware.Sensors)
                _sensorCache[sensor.Identifier.ToString()] = sensor;
        }
    }

    private static SensorInfo CreateSensorInfo(IHardware hardware, ISensor sensor)
    {
        return new SensorInfo
        {
            Identifier = sensor.Identifier.ToString(),
            Name = sensor.Name,
            HardwareName = hardware.Name,
            SensorType = ToSensorKind(sensor.SensorType)
        };
    }

    private static SensorKind ToSensorKind(SensorType sensorType)
    {
        return Enum.TryParse<SensorKind>(sensorType.ToString(), out var kind)
            ? kind
            : SensorKind.Data;
    }

    private static HardwareKind ToHardwareKind(HardwareType hardwareType)
    {
        return hardwareType switch
        {
            HardwareType.Cpu => HardwareKind.Cpu,
            HardwareType.GpuNvidia => HardwareKind.Gpu,
            HardwareType.GpuAmd => HardwareKind.Gpu,
            HardwareType.GpuIntel => HardwareKind.Gpu,
            HardwareType.Motherboard => HardwareKind.Motherboard,
            HardwareType.Storage => HardwareKind.Storage,
            HardwareType.Memory => HardwareKind.Memory,
            HardwareType.Network => HardwareKind.Network,
            HardwareType.Battery => HardwareKind.Battery,
            HardwareType.SuperIO => HardwareKind.SuperIo,
            HardwareType.Cooler => HardwareKind.Cooler,
            _ => HardwareKind.Unknown
        };
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;
            _isReady = false;
            _sensorCache.Clear();
            _computer?.Close();
            _computer = null;
        }

        _logger.LogInformation("LibreHardwareMonitor 已关闭");
    }
}
