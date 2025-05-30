using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Services
{
    public class MonitorService(ILogger<MonitorService> logger) : IMonitorService
    {
        private readonly ulong _totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024);
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

        private readonly Lazy<LibreHardwareMonitor.Hardware.Computer> _computer = new(() =>
        {
            logger.LogDebug("初始化硬件监控组件");
            var computer = new LibreHardwareMonitor.Hardware.Computer
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
                MonitorOption.MemoryUsage => $"{GetMemoryUsage()} MB",
                MonitorOption.CpuUsage => $"{GetCpuUsage():F2} %",
                MonitorOption.CpuTemperature => $"{GetCpuTemperature()} °C",
                _ => "未知类型"
            };
        }

        public float GetMemoryUsage()
        {
            try
            {
                var availableMemory = _memoryCounter.Value.NextValue();
                return _totalMemory - availableMemory;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取内存使用量失败");
                return -1;
            }
        }

        public float GetCpuUsage()
        {
            try
            {
                return _cpuCounter.Value.NextValue();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取 CPU 利用率失败");
                return -1;
            }
        }

        public float GetCpuTemperature()
        {
            try
            {
                var cpu = _computer.Value.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
                if (cpu == null)
                    return -1f;

                cpu.Update();

                // 优先找 "CPU Package"  
                var packageSensor = cpu.Sensors
                    .FirstOrDefault(s => s.SensorType == SensorType.Temperature &&
                                         s.Name.Equals("CPU Package", StringComparison.OrdinalIgnoreCase) &&
                                         s.Value.HasValue);

                if (packageSensor?.Value != null)
                    return packageSensor.Value.Value;

                // 没有 "CPU Package" 就取第一个可用温度  
                var firstTemp = cpu.Sensors
                    .FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Value.HasValue);

                return firstTemp?.Value ?? -1f;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取 CPU 温度失败");
                return -1f;
            }
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
                logger.LogDebug("释放硬件监控组件资源");
            }

            logger.LogDebug("MonitorService 已释放资源");
        }
    }
}
