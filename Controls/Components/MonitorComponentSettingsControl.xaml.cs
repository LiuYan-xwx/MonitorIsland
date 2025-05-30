using ClassIsland.Core.Abstractions.Controls;
using MonitorIsland.Models.ComponentSettings;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;

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

        [GeneratedRegex("[^0-9]+")]
        private static partial Regex NumberRegex();
        void TextBoxNumberCheck(object sender, TextCompositionEventArgs e)
        {
            Regex re = NumberRegex();
            e.Handled = re.IsMatch(e.Text);
        }
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
