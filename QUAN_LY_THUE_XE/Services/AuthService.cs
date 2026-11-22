using System.Linq;
using Microsoft.EntityFrameworkCore;
using QUAN_LY_THUE_XE.Data;
using QUAN_LY_THUE_XE.Models;

namespace QUAN_LY_THUE_XE.Services
{
    //  Check đăng nhập của người dùng => đúng login 
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