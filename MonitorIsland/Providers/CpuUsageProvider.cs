using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.cpuusage",
        "CPU 使用率",
        "监控 CPU 的使用率",
        [DisplayUnit.Percent])]
    public class CpuUsageProvider : MonitorProviderBase
    {
        public override string DefaultPrefix => "CPU使用率：";

        private readonly PerformanceCounter _cpuCounter = new("Processor", "% Processor Time", "_Total");

        public override string? GetData(MonitorRequest request)
        {
            var cpuUsage = _cpuCounter.NextValue();
            return cpuUsage.ToString();
        }
    }
}
