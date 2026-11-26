using System.ComponentModel.DataAnnotations;

namespace QUAN_LY_THUE_XE.Models
{
    // Danh mục phân loại phương tiện, được xe tham chiếu qua MaLoaiXe.
    public class LoaiXe
    {
        [Key]
        public int MaLoaiXe { get; set; }
        public string TenLoaiXe { get; set; } // "Xe 4 chỗ - Sedan", "Xe 7 chỗ - SUV"
    }
}