using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;

namespace MonitorIsland.Services.Providers
{
    /// <summary>
    /// 磁盘空间监控提供器
    /// </summary>
    public class DiskSpaceProvider : IMonitorProvider
    {
        private readonly ILogger<DiskSpaceProvider> _logger;

        public IReadOnlyList<MonitorOption> SupportedTypes { get; } =
        [
            MonitorOption.DiskSpace
        ];

        public bool IsAvailable { get; private set; } = true;

        public DiskSpaceProvider(ILogger<DiskSpaceProvider> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            try
            {
                // 检查是否可以访问驱动器信息
                _ = DriveInfo.GetDrives();
                _logger.LogInformation("磁盘空间监控提供器初始化成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "磁盘空间监控提供器初始化失败");
                IsAvailable = false;
            }
        }

        public float? GetValue(MonitorRequest request)
        {
            if (!IsAvailable || request.MonitorType != MonitorOption.DiskSpace)
                return null;

            try
            {
                var driveName = request.DriveName ?? "C";
                DriveInfo drive = new(driveName);

                if (!drive.IsReady)
                {
                    _logger.LogError("磁盘 {DriveName} 未就绪", driveName);
                    return null;
                }

                return drive.TotalFreeSpace;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取 {DriveName} 盘剩余空间失败", request.DriveName);
                return null;
            }
        }

        public void Dispose()
        {
            // 无需释放资源
        }
    }
}