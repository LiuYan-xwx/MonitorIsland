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

        public override string? GetData(MonitorRequest request)
        {
            var memory = ByteSize.FromBytes(OperatingSystem.IsMacOS()
                ? Process.GetCurrentProcess().WorkingSet64
                : Process.GetCurrentProcess().PrivateMemorySize64);

            return request.SelectedUnit switch
            {
                DisplayUnit.MB => memory.MebiBytes.ToString(),
                DisplayUnit.GB => memory.GibiBytes.ToString(),
                _ => null
            };
        }
    }
}
