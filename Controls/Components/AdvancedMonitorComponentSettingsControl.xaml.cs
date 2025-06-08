using ClassIsland.Core.Abstractions.Controls;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models.ComponentSettings;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MonitorIsland.Controls.Components
{
    public partial class AdvancedMonitorComponentSettingsControl : ComponentBase<AdvancedMonitorComponentSettings>
    {
        public ILogger<AdvancedMonitorComponentSettingsControl> Logger { get; }
        public IAdvancedMonitorService MonitorService { get; }

        public AdvancedMonitorComponentSettingsControl(
            ILogger<AdvancedMonitorComponentSettingsControl> logger,
            IAdvancedMonitorService monitorService)
        {
            Logger = logger;
            MonitorService = monitorService;
            InitializeComponent();
        }

        private async void AdvancedMonitorComponentSettingsControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Settings.AvailableHardware.Count == 0)
            {
                await LoadHardwareListAsync();
            }
        }

        private async Task LoadHardwareListAsync()
        {
            try
            {
                IsEnabled = false;

                var hardware = await Task.Run(() => MonitorService.GetAllAvailableHardware());

                foreach (var item in hardware)
                {
                    Settings.AvailableHardware.Add(item);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "加载硬件列表失败");
            }
            finally
            {
                IsEnabled = true;
            }
        }
    }
}