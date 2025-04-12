using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using MonitorIsland.Helpers;
using MonitorIsland.Models.ComponentSettings;
using MonitorIsland.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MonitorIsland.Controls.Components
{
    /// <summary>
    /// MonitorComponent.xaml 的交互逻辑
    /// </summary>
    [ComponentInfo(
    "AE533FE2-A53F-4104-8C38-37DA018A98BB",
    "系统监控",
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

            // 初始化 MonitorHelper 和定时器
            _monitorHelper = new MonitorHelper();
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

            // 订阅 Loaded 事件
            Loaded += MonitorComponent_OnLoaded;
            Unloaded += MonitorComponent_OnUnloaded;
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            switch (Settings.MonitorType)
            {
                case 0:
                    Settings.MemoryUsage = _monitorHelper.GetMemoryUsage();
                    //Logger.LogTrace($"内存使用量: {Settings.MemoryUsage} MB");
                    break;
                case 1:
                    Settings.CpuUsage = _monitorHelper.GetCpuUsage();
                    //Logger.LogTrace($"CPU 利用率: {Settings.CpuUsage} %");
                    break;
                case 2:
                    Settings.CpuTemperature = _monitorHelper.GetCpuTemperature();
                    //Logger.LogTrace($"CPU 温度: {Settings.CpuTemperature} °C");
                    break;
                default:
                    Logger.LogWarning("未知的监控类型");
                    break;
            }
        }

        private void MonitorComponent_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Settings == null)
            {
                Logger.LogError("Settings 属性尚未初始化");
                return;
            }

            // 设置初始刷新间隔
            _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);

            // 监听 RefreshInterval 的变化
            Settings.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(Settings.RefreshInterval))
                {
                    _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
                }
            };

            // 启动定时器
            _timer.Start();
        }

        private void MonitorComponent_OnUnloaded(object sender, RoutedEventArgs e)
        {
            // 停止定时器并取消事件订阅
            _timer.Stop();
            if (Settings != null)
            {
                Settings.PropertyChanged -= (s, args) =>
                {
                    if (args.PropertyName == nameof(Settings.RefreshInterval))
                    {
                        _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
                    }
                };
            }
        }
    }
}
