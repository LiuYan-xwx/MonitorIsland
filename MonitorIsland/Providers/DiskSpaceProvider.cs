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

        public override MonitorDataResult GetData(MonitorRequest request)
        {
            var driveName = Settings.DriveName;

            if (string.IsNullOrEmpty(driveName))
                return MonitorDataResult.Error("未选择驱动器");

            var drive = new DriveInfo(driveName);
            if (!drive.IsReady)
                return MonitorDataResult.Error("驱动器未就绪");

            var freeSpace = ByteSize.FromBytes(drive.TotalFreeSpace);

            return request.SelectedUnit switch
            {
                DisplayUnit.MB => MonitorDataResult.Success(freeSpace.MebiBytes.ToString(), DisplayUnit.MB),
                DisplayUnit.GB => MonitorDataResult.Success(freeSpace.GibiBytes.ToString(), DisplayUnit.GB),
                DisplayUnit.TB => MonitorDataResult.Success(freeSpace.TebiBytes.ToString(), DisplayUnit.TB),
                _ => MonitorDataResult.Error("未选择有效的显示单位")
            };
        }
    }
}
