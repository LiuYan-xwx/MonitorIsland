using MonitorIsland.Models;
using System.Globalization;
using System.Windows.Data;

namespace MonitorIsland.Converters
{
    public class DisplayUnitToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DisplayUnit unit)
                return string.Empty;

            return unit switch
            {
                DisplayUnit.MB => "MB",
                DisplayUnit.GB => "GB",
                DisplayUnit.TB => "TB",
                DisplayUnit.Percent => "%",
                DisplayUnit.Celsius => "��C",
                _ => string.Empty
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
