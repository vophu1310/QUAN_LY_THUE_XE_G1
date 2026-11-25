using System;
using System.Linq;
using QUAN_LY_THUE_XE.Models;

namespace QUAN_LY_THUE_XE.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {

            //context.Database.EnsureDeleted();// Xóa CSDL cũ (nếu có) để tạo lại từ đầu, chỉ dùng khi phát triển
            // Tự động tạo CSDL nếu chưa có
            context.Database.EnsureCreated();

            // 1. Tạo tài khoản mặc định nếu bảng Tài Khoản đang trống
            if (!context.TaiKhoans.Any())
            {
                context.TaiKhoans.Add(new TaiKhoan
                {
                    TenDangNhap = "admin",
                    MatKhau = "123456", // Trong thực tế nên hash mật khẩu
                    HoTen = "Quản Trị Viên",
                    VaiTro = "Admin"
                });
                context.SaveChanges();
            }

            // 2. Tạo sẵn dữ liệu Xe mẫu nếu chưa có
            if (!context.Xes.Any())
            {
                context.Xes.AddRange(
                    new Xe
                    {
                        BienSo = "51F-123.45",
                        TenXe = "Toyota Vios 1.5G",
                        HangXe = "Toyota",
                        NamSanXuat = 2022,
                        SoChoNgoi = 5,
                        GiaThueNgay = 700000,
                        SoKmHienTai = 15000,
                        TrangThai = "Sẵn sàng",
                        HinhAnh = ""
                    },
                    new Xe
                    {
                        BienSo = "51H-999.88",
                        TenXe = "Mazda CX-5 2.0 Premium",
                        HangXe = "Mazda",
                        NamSanXuat = 2023,
                        SoChoNgoi = 5,
                        GiaThueNgay = 1200000,
                        SoKmHienTai = 8000,
                        TrangThai = "Sẵn sàng",
                        HinhAnh = "",
                    },
                    new Xe
                    {
                        BienSo = "59A-678.90",
                        TenXe = "Ford Everest Titanium",
                        HangXe = "Ford",
                        NamSanXuat = 2023,
                        SoChoNgoi = 7,
                        GiaThueNgay = 1600000,
                        SoKmHienTai = 20000,
                        TrangThai = "Đang bảo dưỡng",
                        HinhAnh = "",
                    }
                ); 
                context.SaveChanges();
            }
        }
    }
}