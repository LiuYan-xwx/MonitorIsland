using ByteSizeLib;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Helpers;
using MonitorIsland.Models;

namespace MonitorIsland.Providers;

[MonitorProviderInfo(
    "monitorisland.memoryusage",
    "内存使用量",
    "显示已经使用的内存量",
    [DisplayUnit.GB, DisplayUnit.TB, DisplayUnit.MB])]
public class MemoryUsageProvider : MonitorProviderBase
{
    private readonly SystemMetrics _systemMetrics = SystemMetrics.Instance;
    private readonly ulong? _totalMemory;

    public override string DefaultPrefix => "内存使用量：";

    public MemoryUsageProvider()
    {
        _totalMemory = _systemMetrics.GetTotalMemory();
    }

    public override MonitorDataResult GetData(MonitorRequest request)
    {
        double? availableBytes = _systemMetrics.GetAvailableMemory();
        if (_totalMemory is null || availableBytes is null)
            return MonitorDataResult.Error("当前平台暂不支持内存使用量");

        double usedBytes = Math.Max(_totalMemory.Value - availableBytes.Value, 0);
        ByteSize usedMemory = ByteSize.FromBytes(usedBytes);

        return request.SelectedUnit switch
        {
            DisplayUnit.MB => MonitorDataResult.Success(
                usedMemory.MebiBytes.ToString(),
                DisplayUnit.MB),
            DisplayUnit.GB => MonitorDataResult.Success(
                usedMemory.GibiBytes.ToString(),
                DisplayUnit.GB),
            DisplayUnit.TB => MonitorDataResult.Success(
                usedMemory.TebiBytes.ToString(),
                DisplayUnit.TB),
            _ => MonitorDataResult.Error("未选择有效的显示单位")
        };
    }
}
