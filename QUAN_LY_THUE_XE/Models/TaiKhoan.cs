using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QUAN_LY_THUE_XE.Models
{
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
