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
            // Nạp giao diện Thuê xe vào khung hiển thị
            MainContentArea.Content = new RentalView();
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