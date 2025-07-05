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
        
        /// <summary>
        /// 验证文本框输入，仅允许数字字符。
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">文本组合事件参数</param>
        private void OnTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = NumberRegex();
            e.Handled = re.IsMatch(e.Text);
        }
    }

    public class IntervalToStringConverter : IValueConverter
    {
        private const int DefaultInterval = 1000;
        private const int MinimumInterval = 250;

        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString(culture);
            }
            return DefaultInterval;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string strValue && int.TryParse(strValue, NumberStyles.Integer, culture, out int i))
            {
                if (i < MinimumInterval)
                {
                    return MinimumInterval;
                }
                return i;
            }
            return DefaultInterval;
        }
    }

}
