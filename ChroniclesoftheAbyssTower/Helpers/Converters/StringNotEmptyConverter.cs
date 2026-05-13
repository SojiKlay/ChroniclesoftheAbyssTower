using System.Globalization;

namespace ChroniclesoftheAbyssTower.Helpers.Converters
{
    /// <summary>
    /// แปลง string → bool โดยคืน true ถ้าไม่ใช่ค่าว่าง
    /// ใช้กับการแสดง error label เมื่อมีข้อความ
    /// </summary>
    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
