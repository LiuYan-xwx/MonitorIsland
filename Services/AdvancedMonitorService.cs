using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonitorIsland.Services
{
    public class AdvancedMonitorService : IAdvancedMonitorService
    {
        private readonly ILogger<AdvancedMonitorService> _logger;
        private readonly Lazy<Computer> _computer;
        private readonly Dictionary<string, ISensor> _sensorCache = new();
        private int _disposed;

        public AdvancedMonitorService(ILogger<AdvancedMonitorService> logger)
        {
            _logger = logger;
            _computer = new Lazy<Computer>(() =>
            {
                var computer = new Computer
                {
                    IsCpuEnabled = true,
                    IsGpuEnabled = true,
                    IsMemoryEnabled = true,
                    IsMotherboardEnabled = true,
                    IsControllerEnabled = true,
                    IsNetworkEnabled = true,
                    IsStorageEnabled = true
                };
                computer.Open();
                logger.LogDebug("初始化高级监控资源");
                return computer;
            });
        }

        public List<HardwareInfo> GetAllAvailableHardware()
        {
            var result = new List<HardwareInfo>();
            try
            {
                var computer = _computer.Value;
                foreach (var hardware in computer.Hardware)
                {
                    hardware.Update();
                    AddHardwareSensors(hardware, result);

                    // 递归处理子硬件
                    foreach (var subHardware in hardware.SubHardware)
                    {
                        subHardware.Update();
                        AddHardwareSensors(subHardware, result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取硬件信息失败");
            }

            return result
                .OrderBy(h => h.HardwareName)
                .ThenBy(h => h.SensorName)
                .ToList();
        }

        private void AddHardwareSensors(IHardware hardware, List<HardwareInfo> result)
        {
            foreach (var sensor in hardware.Sensors.Where(s => s.Value.HasValue))
            {
                var hardwareInfo = new HardwareInfo
                {
                    Id = $"{hardware.Identifier}_{sensor.Identifier}",
                    HardwareName = hardware.Name,
                    SensorName = sensor.Name,
                    SensorType = sensor.SensorType.ToString(),
                    Unit = GetSensorUnit(sensor.SensorType),
                    DisplayName = $"{hardware.Name} - {sensor.Name} ({GetSensorUnit(sensor.SensorType)})"
                };

                result.Add(hardwareInfo);
                _sensorCache[hardwareInfo.Id] = sensor;
            }
        }

        private static string GetSensorUnit(SensorType sensorType) => sensorType switch
        {
            SensorType.Voltage => "V",
            SensorType.Current => "A",
            SensorType.Power => "W",
            SensorType.Clock => "MHz",
            SensorType.Temperature => "°C",
            SensorType.Load => "%",
            SensorType.Frequency => "Hz",
            SensorType.Fan => "RPM",
            SensorType.Flow => "L/h",
            SensorType.Control => "%",
            SensorType.Level => "%",
            SensorType.Factor => "",
            SensorType.Data => "GB",
            SensorType.SmallData => "MB",
            SensorType.Throughput => "B/s",
            SensorType.TimeSpan => "",
            SensorType.Energy => "mWh",
            SensorType.Noise => "dBA",
            SensorType.Conductivity => "µS/cm",
            SensorType.Humidity => "%",
            _ => ""
        };

        public string GetSensorValue(string hardwareId)
        {
            try
            {
                if (!_sensorCache.TryGetValue(hardwareId, out var sensor))
                    return "-1";

                sensor.Hardware.Update();

                if (!sensor.Value.HasValue)
                    return "-1";

                return sensor.SensorType switch
                {
                    SensorType.Clock or SensorType.Load or SensorType.Temperature
                    or SensorType.Flow or SensorType.Control or SensorType.Level
                    or SensorType.Power or SensorType.Data or SensorType.SmallData
                    or SensorType.Frequency or SensorType.Throughput or SensorType.Conductivity
                        => $"{sensor.Value.Value:F1}",

                    SensorType.Fan or SensorType.Energy or SensorType.Noise or SensorType.Humidity
                        => $"{sensor.Value.Value:F0}",

                    SensorType.Voltage or SensorType.Current or SensorType.Factor
                        => $"{sensor.Value.Value:F3}",

                    SensorType.TimeSpan
                        => $"{sensor.Value.Value:g}",

                    // 默认格式
                    _ => $"{sensor.Value.Value:F0}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取传感器 {HardwareId} 的值失败", hardwareId);
                return "-1";
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            if (_computer.IsValueCreated)
            {
                _computer.Value.Close();
                _logger.LogDebug("释放高级监控硬件资源");
            }

            _sensorCache.Clear();
        }
    }
}