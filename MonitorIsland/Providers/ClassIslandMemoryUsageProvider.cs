using ByteSizeLib;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.classislandmemoryusage",
        "ClassIsland 内存使用",
        "监控 ClassIsland 的内存使用情况",
        [DisplayUnit.MB, DisplayUnit.GB])]
    public class ClassIslandMemoryUsageProvider : MonitorProviderBase
    {
        public override string DefaultPrefix => "ClassIsland内存：";

        private readonly Process _process = Process.GetCurrentProcess();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _process?.Dispose();
            }
            base.Dispose(disposing);
        }

        public override MonitorDataResult GetData(MonitorRequest request)
        {
            _process.Refresh();
            var memory = ByteSize.FromBytes(OperatingSystem.IsMacOS()
                ? _process.WorkingSet64
                : _process.PrivateMemorySize64);

            return request.SelectedUnit switch
            {
                DisplayUnit.MB => MonitorDataResult.Success(memory.MebiBytes.ToString(), DisplayUnit.MB),
                DisplayUnit.GB => MonitorDataResult.Success(memory.GibiBytes.ToString(), DisplayUnit.GB),
                _ => MonitorDataResult.Error("未选择有效的显示单位")
            };
        }
    }
}
