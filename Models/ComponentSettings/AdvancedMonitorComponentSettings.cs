using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MonitorIsland.Models.ComponentSettings
{
    public class AdvancedMonitorComponentSettings : ObservableRecipient
    {
        private HardwareInfo? _selectedHardware;
        private int _refreshInterval = 1000;
        private string? _displayPrefix;
        private string? _displayData;
        private ObservableCollection<HardwareInfo> _availableHardware = new();

        public HardwareInfo? SelectedHardware
        {
            get => _selectedHardware;
            set
            {
                if (value == _selectedHardware) return;
                _selectedHardware = value;
                OnPropertyChanged();
            }
        }

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

        public ObservableCollection<HardwareInfo> AvailableHardware
        {
            get => _availableHardware;
            set
            {
                if (value == _availableHardware) return;
                _availableHardware = value;
                OnPropertyChanged();
            }
        }

        public string GetDefaultDisplayPrefix() => SelectedHardware?.SensorName + ": " ?? "监控数据: ";
    }
}