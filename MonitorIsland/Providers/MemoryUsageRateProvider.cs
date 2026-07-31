using MonitorIsland.Abstractions.Attributes;
using MonitorIsland.Abstractions.Models;
using MonitorIsland.Abstractions.Providers;
using MonitorIsland.Helpers;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.memoryusagerate",
        "内存使用率",
        "显示内存使用率",
        [DisplayUnit.Percent])]
    public class MemoryUsageRateProvider : MonitorProviderBase
    {
        private readonly SystemMetrics _systemMetrics = SystemMetrics.Instance;
        private readonly ulong? _totalMemory;

        public override string DefaultPrefix => "内存使用率：";

        public MemoryUsageRateProvider()
        {
            _totalMemory = _systemMetrics.GetTotalMemory();
        }

        public override MonitorDataResult GetData(MonitorRequest monitorRequest)
        {
            double? availableBytes = _systemMetrics.GetAvailableMemory();
            if (_totalMemory is null || _totalMemory.Value == 0 || availableBytes is null)
                return MonitorDataResult.Error("当前平台暂不支持内存使用率");

            double value = (_totalMemory.Value - availableBytes.Value)
                           / _totalMemory.Value
                           * 100;
            return MonitorDataResult.Success(value.ToString());
        }
    }
}
