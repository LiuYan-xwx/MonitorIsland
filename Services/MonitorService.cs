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
        private readonly Lazy<PerformanceCounter> _cpuCounter = new(() =>
        {
            var counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            counter.NextValue(); // 预热
            return counter;
        });
        private readonly Lazy<Computer> _computer = new(() =>
        {
            var computer = new Computer
            {
                IsCpuEnabled = true // 启用 CPU 监控
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
                logger.LogError($"获取内存使用量失败: {ex.Message}");
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
                logger.LogError($"获取 CPU 利用率失败: {ex.Message}");
                return -1;
            }
        }

        public float GetCpuTemperature()
        {
            float temperature = -1;

            try
            {
                foreach (var hardware in _computer.Value.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.Cpu)
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                temperature = sensor.Value.GetValueOrDefault();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"获取 CPU 温度失败: {ex.Message}");
            }

            return temperature;
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
