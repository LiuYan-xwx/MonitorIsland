using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using MonitorIsland.Models.ComponentSettings;
using MonitorIsland.Services;
using System.ComponentModel;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace MonitorIsland.Controls.Components
{
    /// <summary>
    /// MonitorComponent.xaml 的交互逻辑
    /// </summary>
    [ComponentInfo(
        "AE533FE2-A53F-4104-8C38-37DA018A98BB",
        "监控",
        "\uEE21",
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
            _timer.Tick += OnTimer_Ticked;
        }

        private void OnTimer_Ticked(object? sender, EventArgs e)
        {
            UpdateMonitorData();
        }

        /// <summary>
        /// 异步更新监控数据
        /// </summary>
        private async void UpdateMonitorData()
        {
            var monitorType = Settings.MonitorType;
            if (monitorType == MonitorOption.CpuTemperature && string.IsNullOrEmpty(Settings.SelectedCpuTemperatureSensorId))
                return;

            var request = new MonitorRequest
            {
                MonitorType = monitorType,
                Unit = Settings.SelectedUnit,
                DriveName = monitorType == MonitorOption.DiskSpace ? Settings.DriveName : null,
                CpuTemperatureSensorId = monitorType == MonitorOption.CpuTemperature ? Settings.SelectedCpuTemperatureSensorId : null
            };

            var rawValue = await Task.Run(() => MonitorService.GetMonitorValue(request));
            var displayValue = MonitorValueFormatter.Format(rawValue, request.MonitorType, request.Unit);

            Settings.DisplayData = displayValue;
        }

        private void LoadAvailableCpuTemperatureSensors()
        {
            try
            {
                var sensors = MonitorService.GetAvailableCpuTemperatureSensors();
                Settings.AvailableCpuTemperatureSensors = sensors;

                // 如果当前没有选择传感器或选择的传感器不在可用列表中，使用默认选择逻辑
                if (string.IsNullOrEmpty(Settings.SelectedCpuTemperatureSensorId) ||
                    !sensors.Any(s => s.Id == Settings.SelectedCpuTemperatureSensorId))
                {
                    SelectDefaultCpuTemperatureSensor(sensors);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "加载可用CPU温度传感器时发生错误");
            }
        }

        private void SelectDefaultCpuTemperatureSensor(List<CpuTemperatureSensorInfo> sensors)
        {
            if (sensors.Count == 0)
            {
                Settings.SelectedCpuTemperatureSensorId = null;
                return;
            }

            // 优先选择"CPU Package"传感器
            var cpuPackageSensor = sensors.FirstOrDefault(s => s.Name == "CPU Package");
            if (cpuPackageSensor != null)
            {
                Settings.SelectedCpuTemperatureSensorId = cpuPackageSensor.Id;
                return;
            }

            // 如果没有找到CPU Package，选择第一个可用的传感器
            var firstSensor = sensors[0];
            Settings.SelectedCpuTemperatureSensorId = firstSensor.Id;
        }

        private void MonitorComponent_OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs routedEventArgs)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
            Settings.PropertyChanged += OnSettingsPropertyChanged;
            if (Settings.MonitorType == MonitorOption.CpuTemperature)
            {
                LoadAvailableCpuTemperatureSensors();
            }
            _timer.Start();
        }

        private void MonitorComponent_OnUnloaded(object? sender, RoutedEventArgs routedEventArgs)
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
                    if (Settings.MonitorType == MonitorOption.CpuTemperature)
                    {
                        LoadAvailableCpuTemperatureSensors();
                    }
                    break;
            }
        }
    }
}