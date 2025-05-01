using ClassIsland.Core.Abstractions.Controls;
using MonitorIsland.Models.ComponentSettings;
using System.Windows;

namespace MonitorIsland.Controls.Components
{
    /// <summary>
    /// MonitorComponentSettingsControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorComponentSettingsControl : ComponentBase<MonitorComponentSettings>
    {
        public MonitorComponentSettingsControl()
        {
            InitializeComponent();
        }
        private void ResetDisplayText_Click(object sender, RoutedEventArgs e)
        {
            if (Settings != null)
            {
                Settings.DisplayPrefix = Settings.GetDefaultDisplayPrefix();
            }
        }
    }

}
