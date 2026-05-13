using System.Globalization;

namespace ChroniclesoftheAbyssTower.Helpers.Converters
{
    /// <summary>
    /// แปลง bool เป็นค่าตรงข้าม (true → false, false → true)
    /// ใช้กับการ binding IsVisible / IsEnabled
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return false;
        }
    }
}
