using Avalonia.Data.Converters;
using MonitorIsland.Models;
using System.Globalization;

namespace MonitorIsland.Converters
{
    public class MonitorTypeToCpuTemperatureSensorVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is MonitorOption monitorType)
            {
                return monitorType == MonitorOption.CpuTemperature;
            }
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}