using Avalonia.Data.Converters;
using MonitorIsland.Models;
using System.Globalization;

namespace MonitorIsland.Converters
{
    public class MonitorTypeToUnitVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not MonitorOption monitorType)
                return false;

            return monitorType switch
            {
                MonitorOption.MemoryUsage => true,
                MonitorOption.DiskSpace => true,
                _ => false
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}