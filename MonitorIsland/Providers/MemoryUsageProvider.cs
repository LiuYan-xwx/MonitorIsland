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

        public MemoryUsageProvider()
        {
            _memoryCounter.NextValue();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _memoryCounter?.Dispose();
            }
            base.Dispose(disposing);
        }

        public override MonitorDataResult GetData(MonitorRequest request)
        {
            var availableMemory = ByteSize.FromBytes(_memoryCounter.NextValue());
            var usedMemory = _totalMemory - availableMemory;

            return request.SelectedUnit switch
            {
                DisplayUnit.MB => MonitorDataResult.Success(usedMemory.MebiBytes.ToString(), DisplayUnit.MB),
                DisplayUnit.GB => MonitorDataResult.Success(usedMemory.GibiBytes.ToString(), DisplayUnit.GB),
                DisplayUnit.TB => MonitorDataResult.Success(usedMemory.TebiBytes.ToString(), DisplayUnit.TB),
                _ => MonitorDataResult.Error("未选择有效的显示单位")
            };
        }
    }
}
