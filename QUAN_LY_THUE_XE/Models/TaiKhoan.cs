using System.ComponentModel.DataAnnotations;

namespace QUAN_LY_THUE_XE.Models
{
    // Dữ liệu tài khoản đăng nhập và vai trò người dùng trong hệ thống.
    public class TaiKhoan
    {
        [Key]
        public int MaTK { get; set; } // hoặc MaTaiKhoan
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string VaiTro { get; set; }              // Admin, NhanVien, KhachHang
        public string HoTen { get; internal set; }
    }
}
