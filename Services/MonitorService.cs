using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Services
{
    public class MonitorService(ILogger<MonitorService> logger, LibreHardwareMonitorService hardwareMonitor) : IMonitorService
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

        private readonly LibreHardwareMonitorService _hardwareMonitor = hardwareMonitor;
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
            return _hardwareMonitor.GetCpuTemperature();
        }

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

            _hardwareMonitor.Dispose();

            logger.LogDebug("MonitorService 已释放资源");
        }
    }
}
