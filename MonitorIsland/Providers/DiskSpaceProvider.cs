using ByteSizeLib;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Models;
using MonitorIsland.Models.MonitorProviderSettings;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.diskspace",
        "驱动器剩余空间",
        "监控驱动器的剩余空间",
        [DisplayUnit.GB, DisplayUnit.TB, DisplayUnit.MB])]
    public class DiskSpaceProvider : MonitorProviderBase<DiskSpaceSettings>
    {
        public override string DefaultPrefix => "磁盘: ";

        public override string? GetData(MonitorRequest request)
        {
            var driveName = Settings.DriveName ?? "C:\\";
            var drive = new DriveInfo(driveName);
            if (!drive.IsReady)
                return null;

            var freeSpace = ByteSize.FromBytes(drive.TotalFreeSpace);

            return request.SelectedUnit switch
            {
                DisplayUnit.MB => freeSpace.MebiBytes.ToString(),
                DisplayUnit.GB => freeSpace.GibiBytes.ToString(),
                DisplayUnit.TB => freeSpace.TebiBytes.ToString(),
                _ => null
            };
        }
    }
}
