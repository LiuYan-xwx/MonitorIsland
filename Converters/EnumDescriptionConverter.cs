using Avalonia.Data.Converters;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace MonitorIsland.Converters
{
    /// <summary>
    /// 将枚举值转换为其Description属性的文本
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
                var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
                return descriptionAttribute?.Description ?? enumValue.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for EnumDescriptionConverter.");
        }
    }
}
