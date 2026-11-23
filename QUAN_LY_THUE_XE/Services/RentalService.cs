using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QUAN_LY_THUE_XE.Data;
using QUAN_LY_THUE_XE.Models;

namespace QUAN_LY_THUE_XE.Services
{
    /// <summary>
    /// Lớp dịch vụ phụ trách giao tiếp với CSDL SQL Server về nghiệp vụ Thuê & Trả xe
    /// </summary>
    public class RentalService
    {
        private readonly AppDbContext _context;

      // Tạo DB kết nối SQL Sever 
        public RentalService(AppDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách tất cả Khách hàng để đổ vào ComboBox chọn người thuê
        public List<KhachHang> LayDanhSachKhachHang()
        {
            return _context.KhachHangs.ToList();
        }

        // 2. Chỉ lọc lấy những chiếc xe đang "Sẵn sàng" để cho thuê
        public List<Xe> LayDanhSachXeSanSang()
        {
            return _context.Xes.Where(x => x.TrangThai == "Sẵn sàng").ToList();
        }

        // 3. Lấy danh sách các hợp đồng thuê xe đang có trong CSDL
        public List<DonThue> LayDanhSachDonThue()
        {
            // Dùng Include để lấy kèm thông tin Xe và Khách hàng liên quan tới đơn đó
            return _context.DonThues
                           .Include(d => d.Xe)
                           .Include(d => d.KhachHang)
                           .OrderByDescending(d => d.NgayTao)
                           .ToList();
        }

        // 4. NGHIỆP VỤ CỐT LÕI: Tạo hợp đồng thuê xe mới
        public bool TaoDonThue(int maKH, int maXe, DateTime ngayBatDau, DateTime ngayKetThuc, decimal tongTien, decimal tienCoc)
        {
            try
            {
                // Kiểm tra tính hợp lệ: Xe phải tồn tại và đang ở trạng thái "Sẵn sàng"
                var xe = _context.Xes.Find(maXe);
                if (xe == null || xe.TrangThai != "Sẵn sàng")
                {
                    return false;
                }

                // Bước 4.1: Tạo đơn thuê mới
                var donThue = new DonThue
                {
                    MaKH = maKH,
                    MaXe = maXe,
                    NgayTao = DateTime.Now,
                    NgayBatDauThue = ngayBatDau,
                    NgayDuKienTra = ngayKetThuc,
                    TongTienDuKien = tongTien,
                    TienCoc = tienCoc,
                    TrangThai = "Đang thuê"
                };
                _context.DonThues.Add(donThue);

                // Bước 4.2: Tự động đổi trạng thái chiếc xe sang "Đang thuê"
                xe.TrangThai = "Đang thuê";

                // Lưu thay đổi xuống CSDL SQL Server
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 5. NGHIỆP VỤ TRẢ XE: Đổi xe về lại "Sẵn sàng"
        public bool XacNhanTraXe(int maDonThue)
        {
            try
            {
                var donThue = _context.DonThues.Find(maDonThue);
                if (donThue == null) return false;

                // Đổi trạng thái đơn thuê
                donThue.TrangThai = "Đã hoàn thành";

                // Đổi trạng thái xe về lại "Sẵn sàng" để có thể cho thuê tiếp
                var xe = _context.Xes.Find(donThue.MaXe);
                if (xe != null)
                {
                    xe.TrangThai = "Sẵn sàng";
                }

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}