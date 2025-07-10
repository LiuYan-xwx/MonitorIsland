using MonitorIsland.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MonitorIsland.Converters
{
    public class MonitorTypeToUnitVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not MonitorOption monitorType)
                return Visibility.Collapsed;

            return monitorType switch
            {
                MonitorOption.MemoryUsage => Visibility.Visible,
                MonitorOption.DiskSpace => Visibility.Visible,
                _ => Visibility.Collapsed
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
