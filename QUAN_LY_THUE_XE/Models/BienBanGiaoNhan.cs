using System;
using System.ComponentModel.DataAnnotations; // <-- Bắt buộc phải có dòng này

namespace QUAN_LY_THUE_XE.Models
{
    public class BienBanGiaoNhan
    {
        [Key] // <-- Báo EF Core biết đây là Khóa chính
        public int MaBB { get; set; } 

        public int MaDon { get; set; }
        public string LoaiBienBan { get; set; } // "Giao xe" hoặc "Nhận xe"
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public int SoKm { get; set; }
        public string MucNhienLieu { get; set; } 
        public string TinhTrangXe { get; set; }
        public string GhiChu { get; set; }
    }
}