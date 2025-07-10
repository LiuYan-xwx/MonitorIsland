using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using MonitorIsland.Models.ComponentSettings;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;

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
        private IMonitorService MonitorService { get; }

        public MonitorComponent(ILogger<MonitorComponent> logger, IMonitorService monitorService)
        {
            Logger = logger;
            MonitorService = monitorService;
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateMonitorData();
        }

        /// <summary>
        /// 异步更新监控数据
        /// </summary>
        private async void UpdateMonitorData()
        {
            var monitorType = Settings.MonitorType;
            var driveName = monitorType == MonitorOption.DiskSpace ? Settings.DriveName : null;
            var displayValue = await Task.Run(() => MonitorService.GetFormattedMonitorValue(monitorType, driveName));
            Settings.DisplayData = displayValue;
        }
        private void MonitorComponent_OnLoaded(object sender, RoutedEventArgs e)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
            Settings.PropertyChanged += OnSettingsPropertyChanged;
            _timer.Start();
        }

        private void MonitorComponent_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
        }

        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.RefreshInterval):
                    _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
                    break;
                case nameof(Settings.MonitorType):
                case nameof(Settings.DriveName):
                    Settings.DisplayPrefix = Settings.GetDefaultDisplayPrefix();
                    break;
            }
        }
    }
}