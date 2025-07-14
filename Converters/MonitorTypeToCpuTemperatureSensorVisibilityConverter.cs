using MonitorIsland.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MonitorIsland.Converters
{
    public class MonitorTypeToCpuTemperatureSensorVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MonitorOption monitorType)
            {
                return monitorType == MonitorOption.CpuTemperature ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}