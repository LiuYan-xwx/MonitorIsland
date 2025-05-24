using System.Diagnostics;
using Grpc.Core.Logging;
using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;

namespace MonitorIsland.Services
{
    public class MonitorService(ILogger<MonitorService> logger) : IMonitorService
    {
        private readonly ulong _totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024);
        private readonly Lazy<PerformanceCounter> _memoryCounter = new(() => new PerformanceCounter("Memory", "Available MBytes"));
        private readonly Lazy<PerformanceCounter> _cpuCounter = new(() => new PerformanceCounter("Processor", "% Processor Time", "_Total"));

        private readonly Lazy<Computer> _computer = new(() =>
        {
            var computer = new Computer
            {
                IsCpuEnabled = true
            };
            computer.Open();
            return computer;
        });

        private int _disposed;

        public string GetFormattedMonitorValue(int monitorType)
        {
            return monitorType switch
            {
                0 => $"{GetMemoryUsage()} MB",
                1 => $"{GetCpuUsage():F2} %",
                2 => $"{GetCpuTemperature()} °C",
                _ => "未知数据"
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
                _memoryCounter.Value.Dispose();

            if (_cpuCounter.IsValueCreated)
                _cpuCounter.Value.Dispose();

            if (_computer.IsValueCreated)
                _computer.Value.Close();
        }
    }
}
