using System.Linq;
using Microsoft.EntityFrameworkCore;
using QUAN_LY_THUE_XE.Data;
using QUAN_LY_THUE_XE.Models;

namespace QUAN_LY_THUE_XE.Services
{
    // Lớp này chịu trách nhiệm duy nhất: Xác thực đăng nhập//Không làm ảnh hưởng giao diện, khác với ViewModel
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // Hàm kiểm tra tài khoản và mật khẩu
        public TaiKhoan? KiemTraDangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
                return null;

            // Tìm tài khoản trùng khớp trong CSDL
            return _context.TaiKhoans
                           .FirstOrDefault(tk => tk.TenDangNhap == tenDangNhap && tk.MatKhau == matKhau);
        }
    }
}