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

            //Tạo tài khoản mặc định nếu bảng Tài Khoản đang trống
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

            // Tạo sẵn dữ liệu Xe mẫu nếu chưa có
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
                        HinhAnh = "https://i.postimg.cc/XNzFQp47/image-1200x-6221935e5b1da.jpg",
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
                        HinhAnh = "https://i.postimg.cc/JhxHJhpG/images-(1).jpg",
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
                        HinhAnh = "https://i.postimg.cc/ncms223T/image-2048x-6a14fef69c3e4.jpg",
                    }
                ); 
                context.SaveChanges();
            }
            else
            {
                // Cập nhật ảnh cho Ford Everest 
                var ford = context.Xes.FirstOrDefault(x => x.BienSo == "59A-678.90" || x.TenXe.Contains("Everest"));
                if (ford != null && ford.HinhAnh != "https://i.postimg.cc/XNzFQp47/image-1200x-6221935e5b1da.jpg")
                {
                    ford.HinhAnh = "https://i.postimg.cc/XNzFQp47/image-1200x-6221935e5b1da.jpg";
                    context.SaveChanges();
                }

                // Cập nhật / Bổ sung Hyundai Santa Fe 
                var santaFe = context.Xes.FirstOrDefault(x => x.BienSo == "30K-516.82" || x.TenXe.Contains("Santa Fe"));
                if (santaFe == null)
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
                        HinhAnh = "https://i.postimg.cc/JhxHJhpG/images-(1).jpg"
                    });
                    context.SaveChanges();
                }
                else if (santaFe.HinhAnh != "https://i.postimg.cc/JhxHJhpG/images-(1).jpg")
                {
                    santaFe.HinhAnh = "https://i.postimg.cc/JhxHJhpG/images-(1).jpg";
                    context.SaveChanges();
                }

                // Cập nhật / Bổ sung VinFast VF8
                var vf8 = context.Xes.FirstOrDefault(x => x.BienSo == "30A-667.67" || x.TenXe.Contains("VF8") || x.TenXe.Contains("VF 8"));
                if (vf8 == null)
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
                        HinhAnh = "https://i.postimg.cc/ncms223T/image-2048x-6a14fef69c3e4.jpg"
                    });
                    context.SaveChanges();
                }
                else if (vf8.HinhAnh != "https://i.postimg.cc/ncms223T/image-2048x-6a14fef69c3e4.jpg")
                {
                    vf8.HinhAnh = "https://i.postimg.cc/ncms223T/image-2048x-6a14fef69c3e4.jpg";
                    context.SaveChanges();
                }
            }
        }
    }
}