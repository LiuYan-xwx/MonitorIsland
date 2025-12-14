using Avalonia.Data.Converters;
using MonitorIsland.Models;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace MonitorIsland.Converters
{
    public class DisplayUnitToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not DisplayUnit unit)
                return string.Empty;

            var fieldInfo = unit.GetType().GetField(unit.ToString());
            var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description ?? unit.ToString();
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
