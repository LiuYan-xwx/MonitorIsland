using CommunityToolkit.Mvvm.ComponentModel;

namespace MonitorIsland.Models.ComponentSettings
{
    public class MonitorComponentSettings : ObservableRecipient
    {
        private int _monitorType;
        private int _refreshInterval = 1000;
        private string? _displayPrefix;
        private string? _displayText;

        /// <summary>
        /// 0 - 内存使用量<br/>
        /// 1 - CPU 利用率<br/>
        /// 2 - CPU 温度<br/>
        /// </summary>
        public int MonitorType
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
        /// 显示文本
        /// </summary>
        public string DisplayText
        {
            get => _displayText ?? string.Empty;
            set
            {
                if (value == _displayText) return;
                _displayText = value;
                OnPropertyChanged();
            }
        }

        // 获取当前监控类型的默认显示前缀
        public string GetDefaultDisplayPrefix() => MonitorType switch
        {
            0 => "内存使用量: ",
            1 => "CPU 利用率: ",
            2 => "CPU 温度: ",
            _ => string.Empty
        };
    }
}
