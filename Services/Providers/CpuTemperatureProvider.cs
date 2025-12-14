using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;

namespace MonitorIsland.Services.Providers
{
    /// <summary>
    /// CPU温度监控提供器
    /// </summary>
    public class CpuTemperatureProvider : IMonitorProvider
    {
        private readonly ILogger<CpuTemperatureProvider> _logger;
        private readonly Dictionary<string, ISensor> _temperatureSensors = [];
        private readonly Lazy<Computer> _computer;
        private int _disposed;

        public IReadOnlyList<MonitorOption> SupportedTypes { get; } = new[]
        {
            MonitorOption.CpuTemperature
        };

        public bool IsAvailable { get; private set; }

        public CpuTemperatureProvider(ILogger<CpuTemperatureProvider> logger)
        {
            _logger = logger;
            _computer = new Lazy<Computer>(() =>
            {
                _logger.LogDebug("初始化硬件监控组件");
                var computer = new Computer
                {
                    IsCpuEnabled = true
                };
                computer.Open();
                return computer;
            });
            IsAvailable = true;
        }

        public void Initialize()
        {
            try
            {
                // 预加载传感器列表
                LoadAvailableSensors();
                _logger.LogInformation("CPU温度监控提供器初始化成功，找到 {Count} 个传感器", _temperatureSensors.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CPU温度监控提供器初始化失败");
                IsAvailable = false;
            }
        }

        public float? GetValue(MonitorRequest request)
        {
            if (!IsAvailable || request.MonitorType != MonitorOption.CpuTemperature)
                return null;

            try
            {
                if (string.IsNullOrEmpty(request.CpuTemperatureSensorId))
                {
                    _logger.LogWarning("未指定 CPU 温度传感器ID");
                    return null;
                }

                if (!_temperatureSensors.TryGetValue(request.CpuTemperatureSensorId, out var sensor))
                {
                    _logger.LogWarning("未找到指定的 CPU 温度传感器ID: {SensorId}", request.CpuTemperatureSensorId);
                    return null;
                }

                sensor.Hardware.Update();

                if (sensor.Value.HasValue)
                    return sensor.Value.Value;

                _logger.LogWarning("传感器 {SensorId} 没有可用的温度值", request.CpuTemperatureSensorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取 CPU 温度失败");
            }

            return null;
        }

        public List<CpuTemperatureSensorInfo> GetAvailableSensors()
        {
            LoadAvailableSensors();
            return _temperatureSensors.Select(kvp => new CpuTemperatureSensorInfo
            {
                Id = kvp.Key,
                Name = kvp.Value.Name,
                HardwareName = kvp.Value.Hardware.Name
            }).ToList();
        }

        private void LoadAvailableSensors()
        {
            _temperatureSensors.Clear();

            try
            {
                var computer = _computer.Value;

                foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == HardwareType.Cpu))
                {
                    hardware.Update();

                    foreach (var sensor in hardware.Sensors.Where(s => s.SensorType == SensorType.Temperature))
                    {
                        var sensorId = $"{hardware.Identifier}_{sensor.Identifier}";
                        _temperatureSensors[sensorId] = sensor;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载可用 CPU 温度传感器失败");
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            if (_computer.IsValueCreated)
            {
                _computer.Value.Close();
                _temperatureSensors.Clear();
                _logger.LogDebug("释放硬件监控组件资源");
            }
        }
    }
}