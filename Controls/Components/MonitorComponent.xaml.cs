using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using MonitorIsland.Helpers;
using MonitorIsland.Models.ComponentSettings;
using System.Windows;
using System.Windows.Threading;

namespace MonitorIsland.Controls.Components
{
    /// <summary>
    /// MonitorComponent.xaml 的交互逻辑
    /// </summary>
    [ComponentInfo(
    "AE533FE2-A53F-4104-8C38-37DA018A98BB",
    "监控",
    PackIconKind.ThermostatBox,
    "监控您电脑的各种信息"
    )]
    public partial class MonitorComponent : ComponentBase<MonitorComponentSettings>
    {
        private readonly DispatcherTimer _timer;
        private readonly MonitorHelper _monitorHelper;
        public ILogger<MonitorComponent> Logger { get; }

        public MonitorComponent(ILogger<MonitorComponent> logger)
        {
            Logger = logger;
            InitializeComponent();

            _monitorHelper = new MonitorHelper();
            _timer = new DispatcherTimer();
            _timer.Tick += (s, e) =>
            {
                // 更新监控数据
                UpdateMonitorData();
                // 更新显示文本
                UpdateDisplayText();
            };
        }

        // 根据监控类型更新相应数据
        private void UpdateMonitorData()
        {
            switch (Settings.MonitorType)
            {
                case 0:
                    Settings.MemoryUsage = _monitorHelper.GetMemoryUsage();
                    break;
                case 1:
                    Settings.CpuUsage = _monitorHelper.GetCpuUsage();
                    break;
                case 2:
                    Settings.CpuTemperature = _monitorHelper.GetCpuTemperature();
                    break;
                default:
                    Logger.LogWarning($"未知的监控类型: {Settings.MonitorType}");
                    break;
            }
        }

        // 更新显示文本
        private void UpdateDisplayText()
        {
            string value = Settings.MonitorType switch
            {
                0 => $"{Settings.MemoryUsage:F2} MB",
                1 => $"{Settings.CpuUsage:F2} %",
                2 => $"{Settings.CpuTemperature:F2} °C",
                _ => "未知数据"
            };

            // 前缀自动处理空值的情况
            Settings.DisplayText = $"{Settings.DisplayPrefix} {value}";
        }

        private void MonitorComponent_OnLoaded(object sender, RoutedEventArgs e)
        {
            // 初始化设置
            _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);

            // 监听设置变化
            Settings.PropertyChanged += OnSettingsPropertyChanged;

            // 初始化显示
            UpdateDisplayText();

            // 启动定时器
            _timer.Start();
        }

        private void OnSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.RefreshInterval):
                    _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
                    break;

                case nameof(Settings.MonitorType):
                    // 不设置为null，而是直接设置默认前缀
                    Settings.DisplayPrefix = Settings.GetDefaultDisplayPrefix();
                    UpdateMonitorData(); // 立即更新新类型的数据
                    UpdateDisplayText();
                    break;

                case nameof(Settings.DisplayPrefix):
                    UpdateDisplayText();
                    break;
            }
        }

        private void MonitorComponent_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
        }
    }
}
