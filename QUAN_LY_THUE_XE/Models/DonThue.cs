using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUAN_LY_THUE_XE.Models
{
    // Hợp đồng liên kết một khách hàng với một xe trong khoảng thời gian thuê.
    public class DonThue
    {
        [Key]
        public int MaDon { get; set; }

        // Hai khóa ngoại xác định khách thuê và phương tiện được thuê.
        public int MaKH { get; set; }
        [ForeignKey(nameof(MaKH))]
        public virtual KhachHang KhachHang { get; set; }

        public int MaXe { get; set; }
        [ForeignKey(nameof(MaXe))]
        public virtual Xe Xe { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayBatDauThue { get; set; }
        public DateTime NgayDuKienTra { get; set; }
        public DateTime? NgayTraThucTe { get; set; } // Null nếu chưa trả

        public decimal TienCoc { get; set; }
        public decimal TongTienDuKien { get; set; }

        // "Chờ duyệt", "Đang thuê", "Đã hoàn thành", "Đã hủy"
        public string TrangThai { get; set; } = "Đang thuê";

        public virtual ICollection<ChiTietPhuPhi> DanhSachPhuPhi { get; set; }
        public virtual ICollection<BienBanGiaoNhan> DanhSachBienBan { get; set; }
    }
}