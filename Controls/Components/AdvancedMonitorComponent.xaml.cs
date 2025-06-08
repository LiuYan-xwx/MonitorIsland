using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models.ComponentSettings;
using System.Windows;
using System.Windows.Threading;

namespace MonitorIsland.Controls.Components
{
    [ComponentInfo(
        "B7F8A2E1-C4D9-4F3A-9B8E-1D2C3E4F5A6B",
        "高级监控",
        PackIconKind.MonitorDashboard,
        "监控您电脑的任意硬件信息"
    )]
    public partial class AdvancedMonitorComponent : ComponentBase<AdvancedMonitorComponentSettings>
    {
        private readonly DispatcherTimer _timer;
        public ILogger<AdvancedMonitorComponent> Logger { get; }
        public IAdvancedMonitorService MonitorService { get; }

        public AdvancedMonitorComponent(ILogger<AdvancedMonitorComponent> logger, IAdvancedMonitorService monitorService)
        {
            Logger = logger;
            MonitorService = monitorService;
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Tick += (s, e) => UpdateMonitorData();
        }

        private async void UpdateMonitorData()
        {
            if (Settings.SelectedHardware == null) return;

            string displayValue = await Task.Run(() =>
                MonitorService.GetSensorValue(Settings.SelectedHardware.Id));

            if (Dispatcher.CheckAccess())
            {
                Settings.DisplayData = $"{displayValue} {Settings.SelectedHardware.Unit}";
            }
            else
            {
                Dispatcher.Invoke(() => Settings.DisplayData = $"{displayValue} {Settings.SelectedHardware.Unit}");
            }
        }

        private async void AdvancedMonitorComponent_OnLoaded(object sender, RoutedEventArgs e)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
            Settings.PropertyChanged += OnSettingsPropertyChanged;

            await Task.Run(() =>
            {
                var hardware = MonitorService.GetAllAvailableHardware();
                Dispatcher.Invoke(() =>
                {
                    foreach (var item in hardware)
                    {
                        Settings.AvailableHardware.Add(item);
                    }
                    if (Settings.AvailableHardware.Count > 0)
                    {
                        Settings.SelectedHardware = Settings.AvailableHardware[0];
                    }
                });
            });

            UpdateMonitorData();
            _timer.Start();
        }

        private void OnSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.RefreshInterval):
                    _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
                    break;
                case nameof(Settings.SelectedHardware):
                    Settings.DisplayPrefix = Settings.GetDefaultDisplayPrefix();
                    UpdateMonitorData();
                    break;
            }
        }

        private void AdvancedMonitorComponent_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
        }
    }
}