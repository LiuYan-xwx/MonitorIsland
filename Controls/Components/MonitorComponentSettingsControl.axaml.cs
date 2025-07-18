using Avalonia.Data.Converters;
using ClassIsland.Core.Abstractions.Controls;
using Microsoft.Extensions.Logging;
using MonitorIsland.Extensions;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using MonitorIsland.Models.ComponentSettings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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

        //[GeneratedRegex("[^0-9]+")]
        //private static partial Regex NumberRegex();
        //void TextBoxNumberCheck(object sender, TextCompositionEventArgs e)
        //{
        //    Regex re = NumberRegex();
        //    e.Handled = re.IsMatch(e.Text);
        //}
    }

    public class IntervalToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString(culture);
            }
            return 1000;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string strValue && int.TryParse(strValue, NumberStyles.Integer, culture, out int i))
            {
                if (i < 250)
                {
                    return 250;
                }
                return i;
            }
            return 1000;
        }
    }

}
