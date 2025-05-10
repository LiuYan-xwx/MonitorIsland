using ClassIsland.Core.Abstractions.Controls;
using MonitorIsland.Models.ComponentSettings;
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
            return System.Convert.ToString((int)value!);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                int i = System.Convert.ToInt32((string)value!);
                if (i < 100)
                {
                    i = 100;
                }
                return i;
            }
            catch
            {
                return 1000;
            }
        }
    }

}
