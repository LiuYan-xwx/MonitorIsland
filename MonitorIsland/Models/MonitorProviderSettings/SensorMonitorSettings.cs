using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MonitorIsland.Models.MonitorProviderSettings
{
    public partial class SensorMonitorSettings : ObservableObject
    {
        /// <summary>
        /// 选中的传感器
        /// </summary>
        [ObservableProperty]
        private SensorInfo? _selectedSensor;

        /// <summary>
        /// 可用的传感器列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SensorInfo> _availableSensors = [];
    }
}
