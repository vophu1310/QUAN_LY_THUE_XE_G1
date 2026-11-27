using System.Windows;
using System.Windows.Input;
using QUAN_LY_THUE_XE.Views;

namespace QUAN_LY_THUE_XE
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Nạp giao diện Thuê xe vào khung hiển thị mặc định
            MainContentArea.Content = new RentalView();
        }

        // Chuyển sang màn hình Quản lý Phương tiện 
        private void BtnNavPhuongTien_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new VehicleView();
            BorderNavHopDong.Background = System.Windows.Media.Brushes.Transparent;
            BtnNavHopDong.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(162, 183, 173));
            BtnNavKhachHang.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(162, 183, 173));
            BtnNavPhuongTien.Foreground = System.Windows.Media.Brushes.White;
        }

        // Chuyển sang màn hình Quản lý Khách hàng
        private void BtnNavKhachHang_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new CustomerView();
            BorderNavHopDong.Background = System.Windows.Media.Brushes.Transparent;
            BtnNavHopDong.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(162, 183, 173));
            BtnNavKhachHang.Foreground = System.Windows.Media.Brushes.White;
            BtnNavPhuongTien.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(162, 183, 173));
        }

        // Chuyển về màn hình Quản lý Hợp đồng Thuê xe
        private void BtnNavHopDong_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new RentalView();
            BorderNavHopDong.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(36, 71, 59));
            BtnNavHopDong.Foreground = System.Windows.Media.Brushes.White;
            BtnNavKhachHang.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(162, 183, 173));
            BtnNavPhuongTien.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(162, 183, 173));
        }

        // Cho phép nắm giữ chuột ở bất cứ vùng trống nào trên thanh tiêu đề/sidebar để kéo di chuyển cửa sổ
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        // Nút Thu nhỏ cửa sổ xuống Taskbar
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Nút Phóng to / Trở lại kích thước cũ
        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized)
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        // Nút Đóng ứng dụng
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}