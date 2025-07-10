using CommunityToolkit.Mvvm.ComponentModel;

namespace MonitorIsland.Models.ComponentSettings
{
    public class MonitorComponentSettings : ObservableRecipient
    {
        private MonitorOption _monitorType = MonitorOption.MemoryUsage;
        private int _refreshInterval = 1000;
        private string? _displayPrefix;
        private string? _displayData;
        private string _driveName = "C";

        /// <summary>
        /// 选择要监控的项目类型。
        /// </summary>
        public MonitorOption MonitorType
        {
            get => _monitorType;
            set
            {
                if (value == _monitorType) return;
                _monitorType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 刷新间隔（毫秒）
        /// </summary>
        public int RefreshInterval
        {
            get => _refreshInterval;
            set
            {
                if (value == _refreshInterval) return;
                _refreshInterval = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 要监控的磁盘盘符（仅在监控类型为磁盘空间时使用）
        /// </summary>
        public string DriveName
        {
            get => _driveName;
            set
            {
                if (value == _driveName) return;
                _driveName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 显示文本前缀，如果未设置则返回默认前缀
        /// </summary>
        public string DisplayPrefix
        {
            get => _displayPrefix ?? GetDefaultDisplayPrefix();
            set
            {
                if (value == _displayPrefix) return;
                _displayPrefix = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 显示数据
        /// </summary>
        public string DisplayData
        {
            get => _displayData ?? string.Empty;
            set
            {
                if (value == _displayData) return;
                _displayData = value;
                OnPropertyChanged();
            }
        }

        // 获取当前监控类型的默认显示前缀
        public string GetDefaultDisplayPrefix() => MonitorType switch
        {
            MonitorOption.MemoryUsage => "内存使用量: ",
            MonitorOption.MemoryUsageRate => "内存使用率: ",
            MonitorOption.CpuUsage => "CPU 利用率: ",
            MonitorOption.CpuTemperature => "CPU 温度: ",
            MonitorOption.DiskSpace => $"{DriveName[0]}盘剩余空间: ",
            _ => string.Empty
        };
    }
}
