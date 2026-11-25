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
        private static readonly HashSet<string> TrangThaiKhongDuocXoa = new(StringComparer.OrdinalIgnoreCase)
        {
            "Đang thuê",
            "Đang cho thuê",
            "Đã đặt",
            "Đang được đặt"
        };

        public List<Xe> LayTatCaXe()
        {
            using var context = new AppDbContext();
            return context.Xes
                .AsNoTracking()
                .OrderByDescending(x => x.MaXe)
                .ToList();
        }

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

                // BR-01: Chỉ được chuyển sang Ngưng hoạt động khi xe đang ở bãi
                if (TrangThaiKhongDuocXoa.Contains(xeTrongDb.TrangThai) && xeCapNhat.TrangThai == "Ngưng hoạt động")
                {
                    return (false, "BR-01: Xe đang được đặt hoặc đang cho thuê, chỉ được chuyển sang 'Ngưng hoạt động' khi xe đang ở bãi.");
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

        public (bool ThanhCong, string ThongBao) XoaXe(int maXe)
        {
            try
            {
                using var context = new AppDbContext();
                var xe = context.Xes.Find(maXe);
                if (xe == null)
                {
                    return (false, "Phương tiện không còn tồn tại trong hệ thống.");
                }

                // BR-01: Chặn xóa xe đang được đặt hoặc đang cho thuê
                if (TrangThaiKhongDuocXoa.Contains(xe.TrangThai))
                {
                    return (false, $"BR-01: Không thể xóa xe '{xe.TenXe}' vì đang ở trạng thái '{xe.TrangThai}'.");
                }

                if (context.DonThues.Any(d => d.MaXe == maXe))
                {
                    return (false, $"Xe '{xe.TenXe}' đã phát sinh hợp đồng thuê trước đây nên không thể xóa cứng. Hãy chuyển trạng thái sang 'Ngưng hoạt động'.");
                }

                context.Xes.Remove(xe);
                context.SaveChanges();
                return (true, $"Đã xóa xe {xe.TenXe} ({xe.BienSo}) khỏi hệ thống.");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể xóa xe: {ex.Message}");
            }
        }

        public (bool ThanhCong, string ThongBao) ChuyenTrangThaiNhanh(int maXe, string trangThaiMoi)
        {
            try
            {
                using var context = new AppDbContext();
                var xe = context.Xes.Find(maXe);
                if (xe == null)
                {
                    return (false, "Không tìm thấy phương tiện.");
                }

                if (TrangThaiKhongDuocXoa.Contains(xe.TrangThai) && trangThaiMoi == "Ngưng hoạt động")
                {
                    return (false, "BR-01: Chỉ được chuyển sang 'Ngưng hoạt động' khi xe đang ở bãi.");
                }

                xe.TrangThai = trangThaiMoi;
                context.SaveChanges();
                return (true, $"Đã chuyển xe {xe.TenXe} sang trạng thái '{trangThaiMoi}'.");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể đổi trạng thái: {ex.Message}");
            }
        }
    }
}
