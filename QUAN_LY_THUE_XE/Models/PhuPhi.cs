using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QUAN_LY_THUE_XE.Models
{
    public class PhuPhi
    {
        [Key]
        public int MaPhuPhi { get; set; }
        public int MaDon { get; set; }
        public string Loai { get; set; }                // Trễ giờ, Nhiên liệu, Hư tổn, Vệ sinh
        public decimal SoTien { get; set; }
    }
}