using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUAN_LY_THUE_XE.Models
{
    public class Xe
    {
        [Key]
        public int MaXe { get; set; }

        [Required, MaxLength(20)]
        public string BienSo { get; set; } 

        [Required, MaxLength(100)]
        public string TenXe { get; set; } 

        public string HangXe { get; set; } 
        public int NamSanXuat { get; set; }
        public int SoChoNgoi { get; set; } 
        public decimal GiaThueNgay { get; set; } 
        public decimal TienCocMacDinh { get; set; } 
        public int SoKmHienTai { get; set; }

        public string HinhAnh { get; set; } = ""; 

       
        public string TrangThai { get; set; } = "Sẵn sàng";

        public int? MaLoaiXe { get; set; }
        [ForeignKey(nameof(MaLoaiXe))]
        public virtual LoaiXe LoaiXe { get; set; }
    }
}
