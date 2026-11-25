using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QUAN_LY_THUE_XE.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }
        public int MaTK { get; set; }
        public string HoTen { get; set; }
        public string SDT { get; set; }
        public string CCCD { get; set; }
        public string GPLX { get; set; }
        public string TrangThaiXacMinh { get; set; }    // Chưa gửi, Chờ duyệt, Đã xác minh

    }
}
