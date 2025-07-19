using Avalonia.Data.Converters;
using MonitorIsland.Models;
using System.Globalization;

namespace MonitorIsland.Converters
{
    public class MonitorTypeToDiskVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is MonitorOption monitorType)
            {
                return monitorType == MonitorOption.DiskSpace;
            }
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}