using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using QUAN_LY_THUE_XE.ViewModels;

namespace QUAN_LY_THUE_XE.Views
{
    public partial class VehicleView : UserControl
    {
        private readonly VehicleViewModel _viewModel;
        private bool _daTaiLanDau;

        public VehicleView()
        {
            InitializeComponent();
            _viewModel = new VehicleViewModel();
            DataContext = _viewModel;
        }

        private void VehicleView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_daTaiLanDau)
            {
                _viewModel.LoadDuLieu();
            }
            _daTaiLanDau = true;
        }

        private void VehicleImage_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (sender is Image img)
            {
                img.Visibility = Visibility.Visible;
            }
        }

        private void VehicleImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (sender is Image img)
            {
                img.Visibility = Visibility.Collapsed;
            }
        }
    }

    public class SafeImageConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? path = value as string;
            if (string.IsNullOrWhiteSpace(path)) return null;

            path = path.Trim();

            try
            {
                // 1. Hỗ trợ đường dẫn ảnh trực tuyến (https:// hoặc http://) như trong DbInitializer
                if (Uri.TryCreate(path, UriKind.Absolute, out var webUri) &&
                    (webUri.Scheme == Uri.UriSchemeHttp || webUri.Scheme == Uri.UriSchemeHttps))
                {
                    var webBitmap = new BitmapImage();
                    webBitmap.BeginInit();
                    webBitmap.UriSource = webUri;
                    webBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    webBitmap.EndInit();
                    return webBitmap;
                }

                // 2. Hỗ trợ đường dẫn tệp cục bộ (tuyệt đối hoặc tương đối trong thư mục chạy)
                string fullPath = Path.IsPathRooted(path)
                    ? path
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

                if (!File.Exists(fullPath)) return null;

                var localBitmap = new BitmapImage();
                localBitmap.BeginInit();
                localBitmap.CacheOption = BitmapCacheOption.OnLoad;
                localBitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                localBitmap.EndInit();
                localBitmap.Freeze();
                return localBitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class StringNotEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !string.IsNullOrWhiteSpace(value as string) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class StatusPillBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString() ?? string.Empty;
            string hex = status switch
            {
                "Sẵn sàng" => "#E6F6EC",
                "Đang thuê" or "Đang cho thuê" => "#FFF1E0",
                "Đã đặt" or "Đang được đặt" => "#E0F2FE",
                "Đang bảo dưỡng" => "#FEE2E2",
                "Ngưng hoạt động" => "#E2E8F0",
                _ => "#F1F5F9"
            };
            return (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class StatusPillForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString() ?? string.Empty;
            string hex = status switch
            {
                "Sẵn sàng" => "#157347",
                "Đang thuê" or "Đang cho thuê" => "#C2410C",
                "Đã đặt" or "Đang được đặt" => "#0369A1",
                "Đang bảo dưỡng" => "#DC2626",
                "Ngưng hoạt động" => "#475569",
                _ => "#475569"
            };
            return (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}