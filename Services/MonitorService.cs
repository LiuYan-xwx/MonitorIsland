using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;

namespace MonitorIsland.Services
{
    public class MonitorService(ILogger<MonitorService> logger) : IMonitorService
    {
        private ISensor? _tempSensor;
        private readonly Lazy<ulong> _totalMemory = new(() =>
        {
            logger.LogDebug("获取总内存大小");
            using var searcher = new System.Management.ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
            foreach (System.Management.ManagementObject obj in searcher.Get())
            {
                return Convert.ToUInt64(obj["TotalPhysicalMemory"]) / (1024 * 1024);
            }
            return 0;
        });
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

        public string GetFormattedMonitorValue(MonitorOption monitorType)
        {
            return monitorType switch
            {
                MonitorOption.MemoryUsage => FormatValue(GetMemoryUsage(), "MB"),
                MonitorOption.CpuUsage => FormatValue(GetCpuUsage(), "%", "F2"),
                MonitorOption.CpuTemperature => FormatValue(GetCpuTemperature(), "°C", "F1"),
                _ => "未知类型"
            };
        }

        private static string FormatValue(float? value, string unit, string format = "")
        {
            if (!value.HasValue)
                return "N/A";

            return string.IsNullOrEmpty(format)
                ? $"{value.Value} {unit}"
                : $"{value.Value.ToString(format)} {unit}";
        }

        public float? GetMemoryUsage()
        {
            try
            {
                var availableMemory = _memoryCounter.Value.NextValue();
                return _totalMemory.Value - availableMemory;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取内存使用量失败");
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

        public float? GetCpuTemperature()
        {
            try
            {
                if (_tempSensor != null && _tempSensor.Value.HasValue)
                {
                    _tempSensor.Hardware.Update();
                    return _tempSensor.Value.Value;
                }

                var computer = _computer.Value;
                bool sensorFound = false;

                foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == HardwareType.Cpu))
                {
                    hardware.Update();

                    _tempSensor = hardware.Sensors.FirstOrDefault(s =>
                        s.SensorType == SensorType.Temperature && s.Name == "CPU Package");

                    if (_tempSensor != null && _tempSensor.Value.HasValue)
                    {
                        sensorFound = true;
                        return _tempSensor.Value.Value;
                    }

                    _tempSensor = hardware.Sensors.FirstOrDefault(s =>
                        s.SensorType == SensorType.Temperature && s.Name == "Core Average");

                    if (_tempSensor != null && _tempSensor.Value.HasValue)
                    {
                        sensorFound = true;
                        return _tempSensor.Value.Value;
                    }
                }

                if (!sensorFound)
                {
                    logger.LogError("未找到可用的 CPU 温度传感器");
                    _tempSensor = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取 CPU 温度失败");
                _tempSensor = null;
            }
            return null;
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
                _tempSensor = null;
                logger.LogDebug("释放硬件监控组件资源");
            }

            logger.LogDebug("MonitorService 已释放资源");
        }
    }
}
