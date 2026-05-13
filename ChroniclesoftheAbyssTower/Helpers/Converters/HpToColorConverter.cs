using System.Globalization;

namespace ChroniclesoftheAbyssTower.Helpers.Converters
{
    /// <summary>
    /// แปลงค่า HP ratio (0.0 - 1.0) เป็นสี
    /// HP สูง = เขียว, HP กลาง = เหลือง, HP ต่ำ = แดง
    /// </summary>
    public class HpToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double ratio) return Colors.Red;

            return ratio switch
            {
                > 0.6 => Color.FromArgb("#48BB78"), // SuccessColor
                > 0.3 => Color.FromArgb("#ED8936"), // WarningColor
                _ => Color.FromArgb("#E53E3E")       // DangerColor
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
