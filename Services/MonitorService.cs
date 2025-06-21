using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using System.Diagnostics;
using System.Linq;

namespace MonitorIsland.Services
{
    public class MonitorService(ILogger<MonitorService> logger) : IMonitorService
    {
        private ISensor? _tempSensor;
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
                if (_tempSensor != null && _tempSensor.Values != null)
                {
                    _tempSensor.Hardware.Update();
                    return (float)_tempSensor.Value;
                }

                var computer = _computer.Value;
                
                foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == HardwareType.Cpu))
                {
                    hardware.Update();

                    _tempSensor = hardware.Sensors.FirstOrDefault(s =>
                        s.SensorType == SensorType.Temperature && s.Name == "CPU Package");

                    if (_tempSensor != null && _tempSensor.Values != null)
                        return (float)_tempSensor.Value;

                    _tempSensor = hardware.Sensors.FirstOrDefault(s =>
                        s.SensorType == SensorType.Temperature && s.Name == "Core Average");

                    if (_tempSensor != null && _tempSensor.Values != null)
                        return (float)_tempSensor.Value;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取 CPU 温度失败");
            }
            return -1;
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
                _tempSensor=null;
                logger.LogDebug("释放硬件监控组件资源");
            }

            logger.LogDebug("MonitorService 已释放资源");
        }
    }
}
