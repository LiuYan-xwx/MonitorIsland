using System.Windows;
using System.Windows.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using MonitorIsland.Models.ComponentSettings;
using MonitorIsland.Interfaces;

namespace MonitorIsland.Controls.Components
{
    /// <summary>
    /// MonitorComponent.xaml 的交互逻辑
    /// </summary>
    [ComponentInfo(
    "AE533FE2-A53F-4104-8C38-37DA018A98BB",
    "监控",
    PackIconKind.Pulse,
    "监控您电脑的各种信息"
    )]
    public partial class MonitorComponent : ComponentBase<MonitorComponentSettings>
    {
        private readonly DispatcherTimer _timer;
        public ILogger<MonitorComponent> Logger { get; }
        public IMonitorService MonitorService {  get; }

        public MonitorComponent(ILogger<MonitorComponent> logger, IMonitorService monitorService)
        {
            Logger = logger;
            MonitorService = monitorService;
            InitializeComponent();

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
                    Settings.MemoryUsage = MonitorService.GetMemoryUsage();
                    break;
                case 1:
                    Settings.CpuUsage = MonitorService.GetCpuUsage();
                    break;
                case 2:
                    Settings.CpuTemperature = MonitorService.GetCpuTemperature();
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
                0 => $"{Settings.MemoryUsage} MB",
                1 => $"{Settings.CpuUsage:F2} %",
                2 => $"{Settings.CpuTemperature} °C",
                _ => "未知数据"
            };

            Settings.DisplayText = $"{Settings.DisplayPrefix}{value}";
        }

        private void MonitorComponent_OnLoaded(object sender, RoutedEventArgs e)
        {
            // 初始化设置
            _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);

            // 监听设置变化
            Settings.PropertyChanged += OnSettingsPropertyChanged;

            // 初始化显示
            UpdateMonitorData();
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
                    Settings.DisplayPrefix = Settings.GetDefaultDisplayPrefix();
                    UpdateMonitorData();
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
            MonitorService.Dispose();
        }
    }
}
