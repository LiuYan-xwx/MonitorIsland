using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Services.Providers
{
    /// <summary>
    /// CPU利用率监控提供器
    /// </summary>
    public class CpuUsageProvider : IMonitorProvider
    {
        private readonly ILogger<CpuUsageProvider> _logger;
        private readonly Lazy<PerformanceCounter> _cpuCounter;
        private int _disposed;

        public IReadOnlyList<MonitorOption> SupportedTypes { get; } =
        [
            MonitorOption.CpuUsage
        ];

        public bool IsAvailable { get; private set; }

        public CpuUsageProvider(ILogger<CpuUsageProvider> logger)
        {
            _logger = logger;
            _cpuCounter = new Lazy<PerformanceCounter>(() =>
            {
                _logger.LogDebug("初始化 CPU 利用率计数器");
                return new PerformanceCounter("Processor", "% Processor Time", "_Total");
            });
            IsAvailable = true;
        }

        public void Initialize()
        {
            try
            {
                // 预热性能计数器
                _ = _cpuCounter.Value.NextValue();
                _logger.LogInformation("CPU利用率监控提供器初始化成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CPU利用率监控提供器初始化失败");
                IsAvailable = false;
            }
        }

        public float? GetValue(MonitorRequest request)
        {
            if (!IsAvailable || request.MonitorType != MonitorOption.CpuUsage)
                return null;

            try
            {
                return _cpuCounter.Value.NextValue();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取 CPU 利用率失败");
                return null;
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            if (_cpuCounter.IsValueCreated)
            {
                _cpuCounter.Value.Dispose();
                _logger.LogDebug("释放 CPU 利用率计数器资源");
            }
        }
    }
}