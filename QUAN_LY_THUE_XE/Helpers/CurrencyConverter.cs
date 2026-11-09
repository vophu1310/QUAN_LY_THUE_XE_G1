using System;
using System.Globalization;
using System.Windows.Data;

namespace QUAN_LY_THUE_XE.Helpers
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                return string.Format(new CultureInfo("vi-VN"), "{0:C0}", amount);
            }
            return "0 đ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}