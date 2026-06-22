using CommunityToolkit.Mvvm.ComponentModel;
using LibreHardwareMonitor.Hardware;
using MonitorIsland.Abstractions;
using MonitorIsland.Interfaces;
using MonitorIsland.Models.MonitorProviderSettings;
using System.Text.Json.Serialization;

namespace MonitorIsland.Models.ComponentSettings
{
    public partial class MonitorComponentSettings : ObservableObject
    {
        [ObservableProperty]
        [property: JsonIgnore]
        private string _displayData = string.Empty;

        [ObservableProperty]
        private string _displayPrefix = string.Empty;

        [ObservableProperty]
        private bool _showUnit = true;

        [ObservableProperty]
        private MonitorProvider? _selectedProvider = null;

        [ObservableProperty]
        [property: JsonIgnore]
        private MonitorProviderBase? _selectedProviderBase = null;

        [ObservableProperty]
        [property: JsonIgnore]
        private string? _selectedProviderId;

        [ObservableProperty]
        private DisplayUnit? _selectedUnit;

        [ObservableProperty]
        private List<DisplayUnit> _availableUnits = [];

        [ObservableProperty]
        private int _decimalPlaces = 2;

        [ObservableProperty]
        private bool _showProviderSettingsControl = false;

        [ObservableProperty]
        private int _refreshInterval = 1000;

        // ===== 旧版迁移字段（仅用于反序列化旧设置，迁移后清除） =====

#pragma warning disable CS0618 // 迁移代码中使用 [Obsolete] 类型

        /// <summary>
        /// [已废弃] 旧版监控类型。升级后自动迁移到对应的 Provider，请勿使用。
        /// </summary>
        [Obsolete("Use SelectedProvider instead.")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MonitorOption? MonitorType { get; set; }

        private string? _driveNameForMigration;
        /// <summary>
        /// [已废弃] 旧版驱动器名称。升级后自动迁移到 DiskSpaceSettings.DriveName，请勿使用。
        /// </summary>
        [Obsolete("Use DiskSpaceSettings.DriveName instead.")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DriveName
        {
            get => _driveNameForMigration;
            set => SetProperty(ref _driveNameForMigration, value);
        }

        private string? _selectedCpuTemperatureSensorIdForMigration;
        /// <summary>
        /// [已废弃] 旧版 CPU 温度传感器 ID。升级后自动迁移到 SensorMonitorSettings.SelectedSensor，请勿使用。
        /// 注意：旧版传感器 ID 格式与新版格式不兼容，迁移时尽力匹配。
        /// </summary>
        [Obsolete("Use SensorMonitorSettings.SelectedSensor instead.")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SelectedCpuTemperatureSensorId
        {
            get => _selectedCpuTemperatureSensorIdForMigration;
            set => SetProperty(ref _selectedCpuTemperatureSensorIdForMigration, value);
        }

        // ===== 迁移逻辑 =====

        private bool _migrationAttempted;

        /// <summary>
        /// 尝试从旧版设置迁移到新版 Provider 架构。
        /// 仅映射监控类型到对应 Provider，不迁移具体设置值（如盘符、传感器等）。
        /// 幂等操作：仅当存在旧版 MonitorType 且尚未迁移时执行。
        /// </summary>
        public void TryMigrateFromOldSettings()
        {
            if (_migrationAttempted || MonitorType is null)
                return;
            _migrationAttempted = true;

            try
            {
                // 映射旧 MonitorOption 到新 Provider ID
                string? providerId = MonitorType.Value switch
                {
                    MonitorOption.MemoryUsage => "monitorisland.memoryusage",
                    MonitorOption.CpuUsage => "monitorisland.cpuusage",
                    MonitorOption.CpuTemperature => "monitorisland.sensor",
                    MonitorOption.DiskSpace => "monitorisland.diskspace",
                    MonitorOption.MemoryUsageRate => "monitorisland.memoryusagerate",
                    _ => null
                };

                if (string.IsNullOrEmpty(providerId))
                    return;

                var template = IMonitorService.MonitorProviders.FirstOrDefault(p => p.Id == providerId);
                if (template is null)
                    return;

                var selected = template.CopyWithoutSettings();

                // 为有专属设置的 Provider 构造默认设置对象，避免后续 Settings 为 null
                switch (MonitorType.Value)
                {
                    case MonitorOption.DiskSpace:
                        selected.Settings = new DiskSpaceSettings
                        {
                            DriveName = DriveName
                        };
                        break;
                    case MonitorOption.CpuTemperature:
                        if (SelectedCpuTemperatureSensorId is null)
                            break;
                        int index = SelectedCpuTemperatureSensorId.IndexOf('_');
                        string identifier = index != -1 ? SelectedCpuTemperatureSensorId[(index + 1)..] : string.Empty;
                        selected.Settings = new SensorMonitorSettings
                        {
                            SelectedSensor = new()
                            {
                                Identifier = identifier,
                                SensorType = SensorType.Temperature
                            }
                        };
                        break;
                }

                SelectedProvider = selected;

                SelectedProviderId = providerId;
            }
            finally
            {
                MonitorType = null;
                DriveName = null;
                SelectedCpuTemperatureSensorId = null;
            }
        }

#pragma warning restore CS0618

    }
}
