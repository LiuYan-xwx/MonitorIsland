using ClassIsland.Core.Abstractions.Controls;
using Microsoft.Extensions.Logging;
using MonitorIsland.Extensions;
using MonitorIsland.Models;
using MonitorIsland.Models.ComponentSettings;
using System.Collections.ObjectModel;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace MonitorIsland.Controls.Components
{
    /// <summary>
    /// MonitorComponentSettingsControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorComponentSettingsControl : ComponentBase<MonitorComponentSettings>
    {
        public ObservableCollection<string> AvailableDriveNames { get; } = [];
        public ILogger<MonitorComponentSettingsControl> Logger { get; }

        public MonitorComponentSettingsControl(ILogger<MonitorComponentSettingsControl> logger)
        {
            Logger = logger;
            InitializeComponent();
        }

        public MonitorOption[] MonitorOptions { get; } = MonitorOptionExtensions.DisplayOrder;

        private void MonitorComponentSettingsControl_OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            if (AvailableDriveNames.Count == 0)
            {
                LoadAvailableDrives();
            }
        }

        private void LoadAvailableDrives()
        {
            AvailableDriveNames.Clear();

            try
            {
                var drives = DriveInfo.GetDrives();

                foreach (var drive in drives)
                {
                    AvailableDriveNames.Add(drive.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "加载可用驱动器时发生错误");
            }
        }

    }
}
