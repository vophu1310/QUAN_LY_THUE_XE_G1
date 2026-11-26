using System.ComponentModel.DataAnnotations;

namespace QUAN_LY_THUE_XE.Models
{
    // Danh mục khoản phụ phí có thể phát sinh trong quá trình thuê xe.
    public class PhuPhi
    {
        [Key]
        public int MaPhuPhi { get; set; }
        public int MaDon { get; set; }
        public string Loai { get; set; }                // Trễ giờ, Nhiên liệu, Hư tổn, Vệ sinh
        public decimal SoTien { get; set; }
    }
}