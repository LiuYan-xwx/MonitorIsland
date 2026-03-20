using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Helpers;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.memoryusagerate",
        "内存使用率",
        "显示内存使用率",
        [DisplayUnit.Percent])]
    public class MemoryUsageRateProvider : MonitorProviderBase
    {
        public override string DefaultPrefix => "内存使用率：";

        private readonly PerformanceCounter _memoryCounter = new("Memory", "Available Bytes");

        private readonly ulong _totalMemory = MemoryHelper.GetTotalPhysicalMemory();

        public override string? GetData(MonitorRequest monitorRequest)
        {
            var availableBytes = _memoryCounter.NextValue();
            return ((_totalMemory - availableBytes) / _totalMemory * 100).ToString();
        }
    }
}
