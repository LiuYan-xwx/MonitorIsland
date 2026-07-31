using MonitorIsland.Abstractions.Attributes;
using MonitorIsland.Abstractions.Models;
using MonitorIsland.Abstractions.Providers;
using MonitorIsland.Helpers;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.cpuusage",
        "CPU 使用率",
        "监控 CPU 的使用率",
        [DisplayUnit.Percent])]
    public class CpuUsageProvider : MonitorProviderBase
    {
        private readonly SystemMetrics _systemMetrics = SystemMetrics.Instance;

        public override string DefaultPrefix => "CPU使用率：";

        public override MonitorDataResult GetData(MonitorRequest request)
        {
            double? cpuUsage = _systemMetrics.GetCpuUsage();

            return cpuUsage is null
                ? MonitorDataResult.Error("当前平台暂不支持 CPU 使用率")
                : MonitorDataResult.Success(cpuUsage.Value.ToString());
        }

    }
}
