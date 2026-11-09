using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUAN_LY_THUE_XE.Models
{
    public class Xe
    {
        [Key]
        public int MaXe { get; set; }

        [Required, MaxLength(20)]
        public string BienSo { get; set; } // Ví dụ: 51F-123.45

        [Required, MaxLength(100)]
        public string TenXe { get; set; } // Ví dụ: Toyota Camry 2.5Q

        public string HangXe { get; set; } // Toyota, Honda, Mazda, VinFast
        public int NamSanXuat { get; set; }
        public int SoChoNgoi { get; set; } // 4 chỗ, 7 chỗ
        public decimal GiaThueNgay { get; set; } // Giá thuê 1 ngày (VD: 800.000)
        public decimal TienCocMacDinh { get; set; } // Tiền cọc xe yêu cầu
        public int SoKmHienTai { get; set; }

        public string HinhAnh { get; set; } = ""; // Đường dẫn ảnh hoặc tên file ảnh

        // "Sẵn sàng", "Đang thuê", "Đang bảo dưỡng"
        public string TrangThai { get; set; } = "Sẵn sàng";

        public int? MaLoaiXe { get; set; }
        [ForeignKey(nameof(MaLoaiXe))]
        public virtual LoaiXe LoaiXe { get; set; }
    }
}
