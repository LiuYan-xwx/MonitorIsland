using Microsoft.Extensions.Logging;
using MonitorIsland.Helpers;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Services.Providers
{
    /// <summary>
    /// 内存监控提供器
    /// </summary>
    public class MemoryProvider : IMonitorProvider
    {
        private readonly ILogger<MemoryProvider> _logger;
        private readonly ulong _totalMemory;
        private readonly Lazy<PerformanceCounter> _memoryCounter;
        private int _disposed;

        public IReadOnlyList<MonitorOption> SupportedTypes { get; } =
        [
            MonitorOption.MemoryUsage,
            MonitorOption.MemoryUsageRate
        ];

        public bool IsAvailable { get; private set; }

        public MemoryProvider(ILogger<MemoryProvider> logger)
        {
            _logger = logger;
            _totalMemory = MemoryHelper.GetTotalPhysicalMemory();
            _memoryCounter = new Lazy<PerformanceCounter>(() =>
            {
                _logger.LogDebug("初始化内存计数器");
                return new PerformanceCounter("Memory", "Available MBytes");
            });
            IsAvailable = true;
        }

        public void Initialize()
        {
            try
            {
                // 预热性能计数器
                _ = _memoryCounter.Value;
                _logger.LogInformation("内存监控提供器初始化成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "内存监控提供器初始化失败");
                IsAvailable = false;
            }
        }

        public float? GetValue(MonitorRequest request)
        {
            if (!IsAvailable)
                return null;

            try
            {
                return request.MonitorType switch
                {
                    MonitorOption.MemoryUsage => GetMemoryUsage(),
                    MonitorOption.MemoryUsageRate => GetMemoryUsageRate(),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取内存监控值失败: {MonitorType}", request.MonitorType);
                return null;
            }
        }

        private float? GetMemoryUsage()
        {
            var availableMemory = _memoryCounter.Value.NextValue() * 1024 * 1024; // to bytes
            return _totalMemory - availableMemory;
        }

        private float? GetMemoryUsageRate()
        {
            var memoryUsage = GetMemoryUsage();
            return memoryUsage.HasValue ? memoryUsage.Value / _totalMemory * 100 : null;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            if (_memoryCounter.IsValueCreated)
            {
                _memoryCounter.Value.Dispose();
                _logger.LogDebug("释放内存计数器资源");
            }
        }
    }
}