using System;
using System.ComponentModel.DataAnnotations;

namespace QUAN_LY_THUE_XE.Models
{
    // Lưu tình trạng xe và chỉ số vận hành tại thời điểm giao hoặc nhận xe.
    public class BienBanGiaoNhan
    {
        [Key]
        public int MaBB { get; set; }

        // Khóa ngoại về hợp đồng thuê liên quan.
        public int MaDon { get; set; }
        public string LoaiBienBan { get; set; } // "Giao xe" hoặc "Nhận xe"
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public int SoKm { get; set; }
        public string MucNhienLieu { get; set; } // Ví dụ: "Vạch 3/4", "Đầy bình"
        public string TinhTrangXe { get; set; }
        public string GhiChu { get; set; }
    }
}