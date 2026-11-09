using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUAN_LY_THUE_XE.Models
{
    public class ChiTietPhuPhi
    {
        [Key]
        public int MaCTPP { get; set; }
        public int MaDon { get; set; }
        public int MaPhuPhi { get; set; }

        public int SoLuong { get; set; } = 1;
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }

        [ForeignKey(nameof(MaDon))]
        public virtual DonThue DonThue { get; set; }

        [ForeignKey(nameof(MaPhuPhi))]
        public virtual PhuPhi PhuPhi { get; set; }
    }
}