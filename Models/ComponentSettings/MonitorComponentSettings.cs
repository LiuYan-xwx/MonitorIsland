using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace MonitorIsland.Models.ComponentSettings
{
    public class MonitorComponentSettings : ObservableRecipient
    {
        private MonitorOption _monitorType = MonitorOption.MemoryUsage;
        private int _refreshInterval = 1000;
        private string? _displayPrefix;
        private string? _displayData;
        private string _driveName = "C:\\";
        private DisplayUnit _selectedUnit;
        private List<DisplayUnit> _availableUnits = [];
        private string? _selectedCpuTemperatureSensorId;
        private List<CpuTemperatureSensorInfo> _availableCpuTemperatureSensors = [];

        public MonitorComponentSettings()
        {
            UpdateAvailableUnits();
        }

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
                UpdateAvailableUnits();
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
        /// 选择的CPU温度传感器ID（仅在监控类型为CPU温度时使用）
        /// </summary>
        public string? SelectedCpuTemperatureSensorId
        {
            get => _selectedCpuTemperatureSensorId;
            set
            {
                if (value == _selectedCpuTemperatureSensorId) return;
                _selectedCpuTemperatureSensorId = value ?? AvailableCpuTemperatureSensors.FirstOrDefault()?.Id;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 可用的CPU温度传感器列表
        /// </summary>
        [JsonIgnore]
        public List<CpuTemperatureSensorInfo> AvailableCpuTemperatureSensors
        {
            get => _availableCpuTemperatureSensors;
            set
            {
                if (Equals(value, _availableCpuTemperatureSensors)) return;
                _availableCpuTemperatureSensors = value;
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
        [JsonIgnore]
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

        /// <summary>
        /// 选择的显示单位
        /// </summary>
        public DisplayUnit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                if (value == _selectedUnit) return;
                _selectedUnit = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 可用的显示单位
        /// </summary>
        public List<DisplayUnit> AvailableUnits
        {
            get => _availableUnits;
            set
            {
                if (Equals(value, _availableUnits)) return;
                _availableUnits = value;
                OnPropertyChanged();
            }
        }

        private void UpdateAvailableUnits()
        {
            AvailableUnits = GetUnitsForMonitorType(MonitorType);
            if (!AvailableUnits.Contains(SelectedUnit))
            {
                SelectedUnit = AvailableUnits.FirstOrDefault();
            }
        }

        private static List<DisplayUnit> GetUnitsForMonitorType(MonitorOption monitorType)
        {
            return monitorType switch
            {
                MonitorOption.MemoryUsage => [DisplayUnit.MB, DisplayUnit.GB],
                MonitorOption.DiskSpace => [DisplayUnit.MB, DisplayUnit.GB, DisplayUnit.TB],
                MonitorOption.MemoryUsageRate => [DisplayUnit.Percent],
                MonitorOption.CpuUsage => [DisplayUnit.Percent],
                MonitorOption.CpuTemperature => [DisplayUnit.Celsius],
                _ => []
            };
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
