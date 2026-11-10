using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QUAN_LY_THUE_XE.Helpers
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString();

            return status switch
            {
                "Sẵn sàng" => new SolidColorBrush(Colors.ForestGreen),
                "Đã đặt" or "Đang được đặt" => new SolidColorBrush(Colors.DodgerBlue), // Màu xanh dương
                "Đang thuê" or "Đang cho thuê" => new SolidColorBrush(Colors.DarkOrange),
                "Đang bảo dưỡng" => new SolidColorBrush(Colors.Crimson),
                "Ngưng hoạt động" => new SolidColorBrush(Colors.DimGray), // Màu xám tối
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}