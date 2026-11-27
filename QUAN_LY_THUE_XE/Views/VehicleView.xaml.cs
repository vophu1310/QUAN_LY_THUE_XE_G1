using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using QUAN_LY_THUE_XE.ViewModels;

namespace QUAN_LY_THUE_XE.Views
{
    /// <summary>
    /// Màn hình Quản lý Đội xe (Card View chuẩn Figma).
    /// </summary>
    public partial class VehicleView : UserControl
    {
        public VehicleView()
        {
            InitializeComponent();
            DataContext = new VehicleViewModel();
            Loaded += VehicleView_Loaded;
        }

        // Mỗi khi người dùng quay lại tab Phương tiện, tự nạp lại dữ liệu mới nhất từ SQL Server
        private void VehicleView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is VehicleViewModel vm)
            {
                vm.LoadVehicles();
            }
        }
    }

    /// <summary>
    /// Converter tải ảnh an toàn từ URL web hoặc đường dẫn máy tính;
    /// trả về null nếu rỗng/lỗi để hiển thị khung placeholder thay vì làm lỗi giao diện.
    /// </summary>
    public class SafeImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? path = value as string;
            if (string.IsNullOrWhiteSpace(path)) return null;

            path = path.Trim();
            try
            {
                // Hỗ trợ URL http/https
                if (Uri.TryCreate(path, UriKind.Absolute, out Uri? uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = uri;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    bitmap.EndInit();
                    return bitmap;
                }

                // Hỗ trợ đường dẫn file ảnh trên máy
                if (File.Exists(path))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch
            {
                // Bỏ qua lỗi định dạng ảnh không hỗ trợ (ví dụ .avif khi thiếu codec) để hiện placeholder
            }

            return null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Chuyển bool sang Visibility (true -> Visible, false -> Collapsed).
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is bool b && b ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Trả về "Active" khi phân khúc đang chọn trùng với CommandParameter của nút chip.
    /// </summary>
    public class CategoryMatchConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? current = value as string;
            string? target = parameter as string;
            return string.Equals(current, target, StringComparison.OrdinalIgnoreCase) ? "Active" : "Inactive";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
