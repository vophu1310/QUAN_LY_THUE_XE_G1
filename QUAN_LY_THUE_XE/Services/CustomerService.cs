using QUAN_LY_THUE_XE.Models;
using System.Data;

namespace QUAN_LY_THUE_XE.Services
{
   
    /// Tầng Service trung gian giữa ViewModel và Model (KhachHang).
    /// Mọi truy vấn SQL đều đi qua KhachHang → KHProvider → SQL Server.
    
    public class CustomerService
    {
        // Lấy toàn bộ danh sách khách hàng, chuyển từ DataTable sang List<KhachHang>.
        public List<KhachHang> LayTatCaKhachHang()
        {
            var dt = new KhachHang().LayTatCaKhachHang();
            return ChuyenDataTableSangList(dt);
        }

        // Tìm khách hàng theo từ khóa (CCCD hoặc SĐT).
        public List<KhachHang> TimKiemKhachHang(string tuKhoa)
        {
            var dt = new KhachHang().TimKiemKhachHang(tuKhoa);
            return ChuyenDataTableSangList(dt);
        }

        // Thêm khách hàng mới vào SQL Server. Trả về số dòng bị ảnh hưởng.
        public int ThemKhachHang(KhachHang kh)
        {
            return new KhachHang().ThemKhachHang(kh);
        }

        // Cập nhật thông tin khách hàng. Trả về số dòng bị ảnh hưởng.
        public int CapNhatKhachHang(KhachHang kh)
        {
            return new KhachHang().CapNhatKhachHang(kh);
        }

        // Xóa khách hàng theo mã. Trả về số dòng bị ảnh hưởng.
        public int XoaKhachHang(int maKH)
        {
            return new KhachHang().XoaKhachHang(maKH);
        }

        // Lấy mã khách hàng kế tiếp (MAX(MaKH) + 1).
        public int LayMaKHKeTiep()
        {
            return new KhachHang().LayMaKHKeTiep();
        }

        // Đếm tổng số khách hàng (cho badge hiển thị trên giao diện).
        public int DemTongKhachHang()
        {
            var dt = new KhachHang().LayTatCaKhachHang();
            return dt.Rows.Count;
        }

        // Chuyển đổi DataTable (kết quả SQL) thành List<KhachHang> để ViewModel có thể bind.
        private static List<KhachHang> ChuyenDataTableSangList(DataTable dt)
        {
            return dt.AsEnumerable()
                .Select(row => new KhachHang
                {
                    MaKH = Convert.ToInt32(row["MaKH"]),
                    CCCD = DocCot(row, "CCCD"),
                    HoTen = DocCot(row, "HoTen"),
                    SODT = DocCot(row, "SODT"),
                    GPLX = DocCot(row, "GPLX"),
                    IsBlacklisted = dt.Columns.Contains("IsBlacklisted")
                        && !row.IsNull("IsBlacklisted")
                        && Convert.ToBoolean(row["IsBlacklisted"])
                })
                .ToList();
        }

        // Đọc giá trị cột an toàn — trả về chuỗi rỗng nếu cột không tồn tại hoặc null.
        private static string DocCot(DataRow row, string tenCot)
        {
            return row.Table.Columns.Contains(tenCot) && !row.IsNull(tenCot)
                ? Convert.ToString(row[tenCot]) ?? string.Empty
                : string.Empty;
        }
    }
}

