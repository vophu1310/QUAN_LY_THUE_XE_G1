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
                        HinhAnh = "https://i.postimg.cc/HW72tK0P/images.jpg"
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
                        HinhAnh = "https://i.postimg.cc/QdT44Lxc/2022-mazda-cx-5-2p5-turbo-signature-123-1657559083.avif",
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
                        HinhAnh = "https://upload.wikimedia.org/wikipedia/commons/4/4c/FORD_EVEREST_%28U375%2CUA%29_China_%285%29.jpg",
                    },
                    new Xe
                    {
                        BienSo = "30K-516.82",
                        TenXe = "Hyundai Santa Fe",
                        HangXe = "Hyundai",
                        NamSanXuat = 2024,
                        SoChoNgoi = 5,
                        GiaThueNgay = 1200000,
                        SoKmHienTai = 9800,
                        TrangThai = "Đang thuê",
                        HinhAnh = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e0/Hyundai_Santa_Fe_Calligraphy_2.5T_AWD_%28MX5%29_%E2%80%93_f_17072024.jpg/800px-Hyundai_Santa_Fe_Calligraphy_2.5T_AWD_%28MX5%29_%E2%80%93_f_17072024.jpg",
                    },
                    new Xe
                    {
                        BienSo = "30A-667.67",
                        TenXe = "VinFast VF8",
                        HangXe = "VinFast",
                        NamSanXuat = 2024,
                        SoChoNgoi = 5,
                        GiaThueNgay = 950000,
                        SoKmHienTai = 5200,
                        TrangThai = "Sẵn sàng",
                        HinhAnh = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6f/VinFast_VF_8_%28cropped%29.jpg/800px-VinFast_VF_8_%28cropped%29.jpg",
                    }
                ); 
                context.SaveChanges();
            }
            else
            {
                // Cập nhật ảnh cho Ford Everest nếu trong CSDL đang rỗng
                var ford = context.Xes.FirstOrDefault(x => x.BienSo == "59A-678.90");
                if (ford != null && string.IsNullOrEmpty(ford.HinhAnh))
                {
                    ford.HinhAnh = "https://upload.wikimedia.org/wikipedia/commons/4/4c/FORD_EVEREST_%28U375%2CUA%29_China_%285%29.jpg";
                    context.SaveChanges();
                }

                // Bổ sung Hyundai Santa Fe nếu chưa có
                if (!context.Xes.Any(x => x.BienSo == "30K-516.82"))
                {
                    context.Xes.Add(new Xe
                    {
                        BienSo = "30K-516.82",
                        TenXe = "Hyundai Santa Fe",
                        HangXe = "Hyundai",
                        NamSanXuat = 2024,
                        SoChoNgoi = 5,
                        GiaThueNgay = 1200000,
                        SoKmHienTai = 9800,
                        TrangThai = "Đang thuê",
                        HinhAnh = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e0/Hyundai_Santa_Fe_Calligraphy_2.5T_AWD_%28MX5%29_%E2%80%93_f_17072024.jpg/800px-Hyundai_Santa_Fe_Calligraphy_2.5T_AWD_%28MX5%29_%E2%80%93_f_17072024.jpg"
                    });
                    context.SaveChanges();
                }

                // Bổ sung VinFast VF8 nếu chưa có
                if (!context.Xes.Any(x => x.BienSo == "30A-667.67"))
                {
                    context.Xes.Add(new Xe
                    {
                        BienSo = "30A-667.67",
                        TenXe = "VinFast VF8",
                        HangXe = "VinFast",
                        NamSanXuat = 2024,
                        SoChoNgoi = 5,
                        GiaThueNgay = 950000,
                        SoKmHienTai = 5200,
                        TrangThai = "Sẵn sàng",
                        HinhAnh = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6f/VinFast_VF_8_%28cropped%29.jpg/800px-VinFast_VF_8_%28cropped%29.jpg"
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}