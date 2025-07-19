using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Helpers;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Services
{
    public class MonitorService(ILogger<MonitorService> logger) : IMonitorService
    {
        private readonly Dictionary<string, ISensor> _temperatureSensors = [];
        private readonly ulong _totalMemory = MemoryHelper.GetTotalPhysicalMemory();

        private readonly Lazy<PerformanceCounter> _memoryCounter = new(() =>
        {
            logger.LogDebug("初始化内存计数器");
            return new PerformanceCounter("Memory", "Available MBytes");
        });

        private readonly Lazy<PerformanceCounter> _cpuCounter = new(() =>
        {
            logger.LogDebug("初始化 CPU 利用率计数器");
            return new PerformanceCounter("Processor", "% Processor Time", "_Total");
        });

        private readonly Lazy<Computer> _computer = new(() =>
        {
            logger.LogDebug("初始化硬件监控组件");
            var computer = new Computer
            {
                IsCpuEnabled = true
            };
            computer.Open();
            return computer;
        });

        private int _disposed;

        public string GetFormattedMonitorValue(MonitorOption monitorType, DisplayUnit unit, string? driveName = null, string? cpuTemperatureSensorId = null)
        {
            return monitorType switch
            {
                MonitorOption.MemoryUsage => FormatValue(GetMemoryUsage(), unit, "F1"),
                MonitorOption.MemoryUsageRate => FormatValue(GetMemoryUsage() / _totalMemory * 100, unit, "F2"),
                MonitorOption.CpuUsage => FormatValue(GetCpuUsage(), unit, "F2"),
                MonitorOption.CpuTemperature => FormatValue(GetCpuTemperature(cpuTemperatureSensorId), unit, "F1"),
                MonitorOption.DiskSpace => FormatValue(GetDiskFreeSpace(driveName ?? "C"), unit, "F1"),
                _ => "未知类型"
            };
        }

        private static string FormatValue(float? value, DisplayUnit unit, string format = "")
        {
            if (!value.HasValue)
                return "N/A";

            var (convertedValue, unitString) = ConvertValue(value.Value, unit);

            return string.IsNullOrEmpty(format)
                ? $"{convertedValue} {unitString}"
                : $"{convertedValue.ToString(format)} {unitString}";
        }

        private static (float, string) ConvertValue(float value, DisplayUnit unit)
        {
            return unit switch
            {
                DisplayUnit.MB => (value / 1024 / 1024, "MB"),
                DisplayUnit.GB => (value / 1024 / 1024 / 1024, "GB"),
                DisplayUnit.TB => (value / 1024 / 1024 / 1024 / 1024, "TB"),
                DisplayUnit.Percent => (value, "%"),
                DisplayUnit.Celsius => (value, "°C"),
                _ => (value, "")
            };
        }

        public float? GetMemoryUsage()
        {
            try
            {
                var availableMemory = _memoryCounter.Value.NextValue() * 1024 * 1024; // to bytes
                return _totalMemory - availableMemory;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取内存使用量失败");
                return null;
            }
        }

        public float? GetDiskFreeSpace(string driveName)
        {
            try
            {
                DriveInfo drive = new(driveName);
                if (!drive.IsReady)
                {
                    logger.LogError($"磁盘 {driveName} 未就绪");
                    return null;
                }

                return drive.TotalFreeSpace;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"获取 {driveName[0]} 盘剩余空间失败");
                return null;
            }
        }

        public float? GetCpuUsage()
        {
            try
            {
                return _cpuCounter.Value.NextValue();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取 CPU 利用率失败");
                return null;
            }
        }

        public float? GetCpuTemperature(string? sensorId = null)
        {
            try
            {
                // 如果没有指定传感器ID，返回null
                if (string.IsNullOrEmpty(sensorId))
                {
                    logger.LogWarning("未指定 CPU 温度传感器ID");
                    return null;
                }

                if (!_temperatureSensors.TryGetValue(sensorId, out var sensor))
                {
                    logger.LogWarning("未找到指定的 CPU 温度传感器ID: {SensorId}", sensorId);
                    return null;
                }

                sensor.Hardware.Update();

                if (sensor.Value.HasValue)
                    return sensor.Value.Value;

                logger.LogWarning("传感器 {SensorId} 没有可用的温度值", sensorId);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取 CPU 温度失败");
            }
            return null;
        }

        public List<CpuTemperatureSensorInfo> GetAvailableCpuTemperatureSensors()
        {
            var sensors = new List<CpuTemperatureSensorInfo>();
            _temperatureSensors.Clear();

            try
            {
                var computer = _computer.Value;

                foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == HardwareType.Cpu))
                {
                    hardware.Update();

                    foreach (var sensor in hardware.Sensors.Where(s => s.SensorType == SensorType.Temperature))
                    {
                        var sensorInfo = new CpuTemperatureSensorInfo
                        {
                            Id = $"{hardware.Identifier}_{sensor.Identifier}",
                            Name = sensor.Name,
                            HardwareName = hardware.Name
                        };

                        sensors.Add(sensorInfo);
                        _temperatureSensors[sensorInfo.Id] = sensor;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取可用 CPU 温度传感器失败");
            }
            return sensors;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            if (_memoryCounter.IsValueCreated)
            {
                _memoryCounter.Value.Dispose();
                logger.LogDebug("释放内存计数器资源");
            }

            if (_cpuCounter.IsValueCreated)
            {
                _cpuCounter.Value.Dispose();
                logger.LogDebug("释放 CPU 利用率计数器资源");
            }
            if (_computer.IsValueCreated)
            {
                _computer.Value.Close();
                _temperatureSensors.Clear();
                logger.LogDebug("释放硬件监控组件资源");
            }

            logger.LogDebug("MonitorService 已释放资源");
        }
    }
}
