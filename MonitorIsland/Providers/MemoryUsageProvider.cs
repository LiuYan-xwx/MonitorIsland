using ByteSizeLib;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Helpers;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.memoryusage",
        "内存使用量",
        "显示已经使用的内存量",
        [DisplayUnit.GB, DisplayUnit.TB, DisplayUnit.MB])]
    public class MemoryUsageProvider : MonitorProviderBase
    {
        public override string DefaultPrefix => "内存使用量：";

        private readonly ByteSize _totalMemory = ByteSize.FromBytes(MemoryHelper.GetTotalPhysicalMemory());
        private readonly PerformanceCounter _memoryCounter = new("Memory", "Available Bytes");

        public override string? GetData(MonitorRequest request)
        {
            var availableMemory = ByteSize.FromBytes(_memoryCounter.NextValue());
            var usedMemory = _totalMemory - availableMemory;

            return request.SelectedUnit switch
            {
                DisplayUnit.MB => usedMemory.MebiBytes.ToString(),
                DisplayUnit.GB => usedMemory.GibiBytes.ToString(),
                DisplayUnit.TB => usedMemory.TebiBytes.ToString(),
                _ => null
            };
        }
    }
}
