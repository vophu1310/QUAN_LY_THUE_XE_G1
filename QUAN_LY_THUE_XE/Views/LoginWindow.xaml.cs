using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using QUAN_LY_THUE_XE.ViewModels;

namespace QUAN_LY_THUE_XE.Views
{
    public partial class LoginWindow : Window
    {
        private bool _isPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        // Kéo di chuyển cửa sổ
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        // Nút (✕) đóng ứng dụng
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Khi gõ vào ô PasswordBox (dấu chấm)
        public void PwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && !_isPasswordVisible)
            {
                vm.MatKhau = PwdBox.Password;
            }
        }

        // Khi gõ vào ô TextBox (hiện chữ)
        public void TxtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && _isPasswordVisible)
            {
                vm.MatKhau = TxtPasswordVisible.Text;
            }
        }

        // Click con mắt để chuyển đổi Ẩn / Hiện mật khẩu
        private void TogglePasswordVisibility_Click(object sender, MouseButtonEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            // Đổi hình icon con mắt
            if (sender is Border border && border.Child is PackIcon eyeIcon)
            {
                eyeIcon.Kind = _isPasswordVisible ? PackIconKind.EyeOffOutline : PackIconKind.EyeOutline;
            }

            if (_isPasswordVisible)
            {
                // Chuyển sang HIỆN CHỮ RÕ
                TxtPasswordVisible.Text = PwdBox.Password;
                PwdBox.Visibility = Visibility.Collapsed;
                TxtPasswordVisible.Visibility = Visibility.Visible;
                TxtPasswordVisible.Focus();// Đặt con trỏ vào ô TextBox
                TxtPasswordVisible.CaretIndex = TxtPasswordVisible.Text.Length;// Đặt con trỏ ở cuối
            }
            else
            {
                // Chuyển về CHE DẤU CHẤM
                PwdBox.Password = TxtPasswordVisible.Text;
                TxtPasswordVisible.Visibility = Visibility.Collapsed;
                PwdBox.Visibility = Visibility.Visible;
                PwdBox.Focus();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}