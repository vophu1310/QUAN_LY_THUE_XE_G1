using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QUAN_LY_THUE_XE.Data;
using QUAN_LY_THUE_XE.Models;

namespace QUAN_LY_THUE_XE.Services
{
    public class VehicleService
    {
        private static readonly HashSet<string> TrangThaiDangBan = new(StringComparer.OrdinalIgnoreCase)
        {
            "Đang thuê",
            "Đang cho thuê",
            "Đã đặt",
            "Đang được đặt"
        };

        // 1. Lấy toàn bộ danh sách xe từ SQL Server (bảng Xe)
        public List<Xe> LayTatCaXe()
        {
            using var context = new AppDbContext();
            return context.Xes
                .AsNoTracking()
                .OrderByDescending(x => x.MaXe)
                .ToList();
        }

        // 2. Lấy 3 chỉ số KPI hiển thị góc trên phải
        public (int TongSo, int SanSang, int DangThue) LayThongKeXe()
        {
            using var context = new AppDbContext();
            var danhSachTrangThai = context.Xes
                .AsNoTracking()
                .Select(x => x.TrangThai)
                .ToList();

            int tong = danhSachTrangThai.Count;
            int sanSang = danhSachTrangThai.Count(t => t == "Sẵn sàng");
            int dangThue = danhSachTrangThai.Count(t => t == "Đang thuê" || t == "Đang cho thuê");

            return (tong, sanSang, dangThue);
        }

        // 3. Thêm xe mới vào bảng Xe
        public (bool ThanhCong, string ThongBao) ThemXe(Xe xeMoi)
        {
            try
            {
                using var context = new AppDbContext();

                string bienSoChuan = xeMoi.BienSo.Trim().ToUpper();
                if (context.Xes.Any(x => x.BienSo.ToUpper() == bienSoChuan))
                {
                    return (false, $"Biển số {bienSoChuan} đã tồn tại trong hệ thống.");
                }

                xeMoi.BienSo = bienSoChuan;
                xeMoi.TenXe = xeMoi.TenXe.Trim();
                xeMoi.HangXe = xeMoi.HangXe.Trim();
                xeMoi.HinhAnh = xeMoi.HinhAnh?.Trim() ?? string.Empty;
                xeMoi.TrangThai = string.IsNullOrWhiteSpace(xeMoi.TrangThai) ? "Sẵn sàng" : xeMoi.TrangThai;

                context.Xes.Add(xeMoi);
                context.SaveChanges();
                return (true, $"Đã thêm xe {xeMoi.TenXe} ({xeMoi.BienSo}) vào SQL Server.");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể thêm xe: {ex.Message}");
            }
        }

        // 4. Cập nhật thông tin xe
        public (bool ThanhCong, string ThongBao) CapNhatXe(Xe xeCapNhat)
        {
            try
            {
                using var context = new AppDbContext();
                var xeTrongDb = context.Xes.Find(xeCapNhat.MaXe);
                if (xeTrongDb == null)
                {
                    return (false, "Không tìm thấy phương tiện trong cơ sở dữ liệu.");
                }

                string bienSoChuan = xeCapNhat.BienSo.Trim().ToUpper();
                if (context.Xes.Any(x => x.MaXe != xeCapNhat.MaXe && x.BienSo.ToUpper() == bienSoChuan))
                {
                    return (false, $"Biển số {bienSoChuan} đang thuộc về một xe khác.");
                }

                // BR-01: Không cho chuyển xe đang thuê/đặt sang Ngưng hoạt động hoặc Đang bảo dưỡng
                if (TrangThaiDangBan.Contains(xeTrongDb.TrangThai) &&
                    (xeCapNhat.TrangThai == "Ngưng hoạt động" || xeCapNhat.TrangThai == "Đang bảo dưỡng"))
                {
                    return (false, $"BR-01: Xe đang ở trạng thái '{xeTrongDb.TrangThai}', chỉ được đổi sang '{xeCapNhat.TrangThai}' khi xe đã trả về bãi.");
                }

                xeTrongDb.BienSo = bienSoChuan;
                xeTrongDb.TenXe = xeCapNhat.TenXe.Trim();
                xeTrongDb.HangXe = xeCapNhat.HangXe.Trim();
                xeTrongDb.NamSanXuat = xeCapNhat.NamSanXuat;
                xeTrongDb.SoChoNgoi = xeCapNhat.SoChoNgoi;
                xeTrongDb.GiaThueNgay = xeCapNhat.GiaThueNgay;
                xeTrongDb.TienCocMacDinh = xeCapNhat.TienCocMacDinh;
                xeTrongDb.SoKmHienTai = xeCapNhat.SoKmHienTai;
                xeTrongDb.TrangThai = xeCapNhat.TrangThai;
                xeTrongDb.HinhAnh = xeCapNhat.HinhAnh?.Trim() ?? string.Empty;

                context.SaveChanges();
                return (true, $"Đã cập nhật thông tin xe {xeTrongDb.TenXe} ({xeTrongDb.BienSo}).");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể cập nhật xe: {ex.Message}");
            }
        }

        // 5. Đổi trạng thái sang "Đang bảo dưỡng" (hoặc hoàn tất bảo dưỡng về lại "Sẵn sàng")
        public (bool ThanhCong, string ThongBao) ChuyenTrangThaiBaoDuong(int maXe)
        {
            try
            {
                using var context = new AppDbContext();
                var xe = context.Xes.Find(maXe);
                if (xe == null)
                {
                    return (false, "Không tìm thấy phương tiện trong cơ sở dữ liệu.");
                }

                if (TrangThaiDangBan.Contains(xe.TrangThai))
                {
                    return (false, $"Không thể chuyển sang 'Đang bảo dưỡng' vì xe '{xe.TenXe}' hiện đang ở trạng thái '{xe.TrangThai}'.");
                }

                string trangThaiMoi = xe.TrangThai == "Đang bảo dưỡng" ? "Sẵn sàng" : "Đang bảo dưỡng";
                xe.TrangThai = trangThaiMoi;
                context.SaveChanges();

                return (true, $"Đã chuyển xe {xe.TenXe} ({xe.BienSo}) sang trạng thái '{trangThaiMoi}'.");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể cập nhật trạng thái bảo dưỡng: {ex.Message}");
            }
        }

        // 6. Xóa mềm: chuyển sang "Ngưng hoạt động" (hoặc khôi phục về "Sẵn sàng")
        public (bool ThanhCong, string ThongBao) XoaMemXe(int maXe)
        {
            try
            {
                using var context = new AppDbContext();
                var xe = context.Xes.Find(maXe);
                if (xe == null)
                {
                    return (false, "Không tìm thấy phương tiện trong cơ sở dữ liệu.");
                }

                if (TrangThaiDangBan.Contains(xe.TrangThai))
                {
                    return (false, $"BR-01: Không thể xóa mềm xe '{xe.TenXe}' vì đang ở trạng thái '{xe.TrangThai}'.");
                }

                string trangThaiMoi = xe.TrangThai == "Ngưng hoạt động" ? "Sẵn sàng" : "Ngưng hoạt động";
                xe.TrangThai = trangThaiMoi;
                context.SaveChanges();

                return (true, trangThaiMoi == "Ngưng hoạt động"
                    ? $"Đã xóa mềm xe {xe.TenXe} ({xe.BienSo}) — chuyển sang 'Ngưng hoạt động'."
                    : $"Đã khôi phục xe {xe.TenXe} ({xe.BienSo}) về trạng thái 'Sẵn sàng'.");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể cập nhật trạng thái xe: {ex.Message}");
            }
        }
    }
}