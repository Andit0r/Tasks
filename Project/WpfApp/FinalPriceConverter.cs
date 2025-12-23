using System.Globalization;
using System.Windows.Data;

namespace WpfApp
{
    // Реализация конвертера для цены со скидкой
    public class FinalPriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int price && values[1] is byte discount)
                return (price - price * discount / 100).ToString();
            return values[0]?.ToString() ?? "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}