using System.Windows;
using System.Windows.Input;
using QUAN_LY_THUE_XE.Data;
using QUAN_LY_THUE_XE.Helpers;
using QUAN_LY_THUE_XE.Models;
using QUAN_LY_THUE_XE.Services;

namespace QUAN_LY_THUE_XE.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private string _tenDangNhap = "";
        private string _matKhau = "";
        private string _thongBaoLoi = "";
        private string _loiTenDangNhap = "";
        private string _loiMatKhau = "";
        private bool _ghiNhoDangNhap;

        public string TenDangNhap
        {
            get => _tenDangNhap;
            set
            {
                _tenDangNhap = value;
                OnPropertyChanged();
                // Khi người dùng gõ chữ, tự động xóa thông báo lỗi
                if (!string.IsNullOrWhiteSpace(_tenDangNhap)) LoiTenDangNhap = "";
            }
        }

        public string MatKhau
        {
            get => _matKhau;
            set
            {
                _matKhau = value;
                OnPropertyChanged();
                // Khi người dùng gõ chữ, tự động xóa thông báo lỗi
                if (!string.IsNullOrWhiteSpace(_matKhau)) LoiMatKhau = "";
            }
        }

        // Các thuộc tính báo lỗi riêng biệt cho từng ô nhập
        public string LoiTenDangNhap
        {
            get => _loiTenDangNhap;
            set { _loiTenDangNhap = value; OnPropertyChanged(); }
        }

        public string LoiMatKhau
        {
            get => _loiMatKhau;
            set { _loiMatKhau = value; OnPropertyChanged(); }
        }

        public string ThongBaoLoi
        {
            get => _thongBaoLoi;
            set { _thongBaoLoi = value; OnPropertyChanged(); }
        }

        public bool GhiNhoDangNhap
        {
            get => _ghiNhoDangNhap;
            set { _ghiNhoDangNhap = value; OnPropertyChanged(); }
        }

        public ICommand DangNhapCommand { get; }

        public LoginViewModel()
        {
            var context = new AppDbContext();
            _authService = new AuthService(context);
            DangNhapCommand = new RelayCommand(ThucHienDangNhap);
        }

        private void ThucHienDangNhap(object? parameter)
        {
            // Reset các thông báo lỗi cũ
            LoiTenDangNhap = "";
            LoiMatKhau = "";
            ThongBaoLoi = "";

            bool coLoi = false;

            // 1. VALIDATION TÊN ĐĂNG NHẬP
            if (string.IsNullOrWhiteSpace(TenDangNhap))
            {
                LoiTenDangNhap = "Vui lòng nhập tên đăng nhập!";
                coLoi = true;
            }

            // 2. VALIDATION MẬT KHẨU
            if (string.IsNullOrWhiteSpace(MatKhau))
            {
                LoiMatKhau = "Vui lòng nhập mật khẩu!";
                coLoi = true;
            }
            else if (MatKhau.Length < 6)
            {
                LoiMatKhau = "Mật khẩu phải có ít nhất 6 ký tự!";
                coLoi = true;
            }

            // Nếu có lỗi nhập liệu thì dừng lại ngay, không truy vấn CSDL
            if (coLoi) return;

            // 3. XÁC THỰC VỚI CSDL
            var taiKhoan = _authService.KiemTraDangNhap(TenDangNhap, MatKhau);

            if (taiKhoan != null)
            {
                // Đăng nhập thành công -> Mở MainWindow
                MainWindow mainWin = new MainWindow();
                mainWin.Show();

                // Đóng cửa sổ Login
                if (parameter is Window currentWindow)
                {
                    currentWindow.Close();
                }
            }
            else
            {
                // Thông báo chung khi sai tài khoản hoặc mật khẩu
                ThongBaoLoi = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            }
        }
    }
}