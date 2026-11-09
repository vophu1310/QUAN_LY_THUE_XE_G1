using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUAN_LY_THUE_XE.Models
{
    public class HoaDon
    {
        [Key]
        public int MaHD { get; set; }

        public int MaDon { get; set; }
        [ForeignKey(nameof(MaDon))]
        public virtual DonThue DonThue { get; set; }

        public DateTime NgayLap { get; set; } = DateTime.Now;
        public decimal TienThue { get; set; }
        public decimal TongPhuPhi { get; set; }
        public decimal TienCocDaTru { get; set; } // Tiền cọc đã cấn trừ
        public decimal SoTienThanhToan { get; set; } // = TienThue + TongPhuPhi - TienCocDaTru

        public string PhuongThucThanhToan { get; set; } // "Tiền mặt", "Chuyển khoản"
        public string TrangThai { get; set; } // "Đã thanh toán", "Chưa thanh toán"
        public string GhiChu { get; set; }
    }
}