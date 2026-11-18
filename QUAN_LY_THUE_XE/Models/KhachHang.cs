using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace QUAN_LY_THUE_XE.Models
{
    public class KhachHang
    {
        // Lấy danh sách khách trực tiếp từ bảng DSKHACHHANG bằng KHProvider (ADO.NET).
        public DataTable LayTatCaKhachHang()
        {
            // Lấy toàn bộ cột và bản ghi khách hàng, sắp xếp theo họ tên để dễ tra cứu.
            const string sql = "SELECT * FROM DSKHACHHANG ORDER BY HoTen";
            // Chuyển truy vấn SELECT cho KHProvider thực thi và nhận kết quả dạng DataTable.
            return new KHProvider().Select(CommandType.Text, sql);
        }

        // Tìm khách theo CCCD hoặc SODT; tham số SQL giúp truyền từ khóa an toàn thay vì ghép chuỗi SQL.
        public DataTable TimKiemKhachHang(string tuKhoa)
        {
            const string sql = "SELECT * FROM DSKHACHHANG WHERE CCCD LIKE @TuKhoa OR SODT LIKE @TuKhoa ORDER BY HoTen";
            var parameter = new SqlParameter("@TuKhoa", SqlDbType.NVarChar, 100)
            {
                Value = $"%{tuKhoa.Trim()}%"
            };
            return new KHProvider().Select(CommandType.Text, sql, parameter);
        }

        public int LayMaKHKeTiep()
        {
            const string sql = "SELECT ISNULL(MAX(MaKH), 0) + 1 FROM DSKHACHHANG";
            var value = new KHProvider().ExecuteScalar(CommandType.Text, sql);
            return Convert.ToInt32(value);
        }

        public int ThemKhachHang(KhachHang kh)
        {
            const string sql = "INSERT INTO DSKHACHHANG (MaKH, CCCD, HoTen, SODT, GPLX) VALUES (@MaKH, @CCCD, @HoTen, @SODT, @GPLX)";
            return new KHProvider().ExecuteNonQuery(CommandType.Text, sql,
                new SqlParameter("@MaKH", SqlDbType.Int) { Value = kh.MaKH },
                new SqlParameter("@CCCD", SqlDbType.NVarChar, 20) { Value = kh.CCCD },
                new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = kh.HoTen },
                new SqlParameter("@SODT", SqlDbType.NVarChar, 20) { Value = kh.SODT },
                new SqlParameter("@GPLX", SqlDbType.NVarChar, 30) { Value = kh.GPLX }
            );
        }

        public int CapNhatKhachHang(KhachHang kh)
        {
            const string sql = "UPDATE DSKHACHHANG SET CCCD = @CCCD, HoTen = @HoTen, SODT = @SODT, GPLX = @GPLX WHERE MaKH = @MaKH; IF COL_LENGTH(N'dbo.DSKHACHHANG', N'IsBlacklisted') IS NOT NULL UPDATE DSKHACHHANG SET IsBlacklisted = @IsBlacklisted WHERE MaKH = @MaKH";
            return new KHProvider().ExecuteNonQuery(CommandType.Text, sql,
                new SqlParameter("@MaKH", SqlDbType.Int) { Value = kh.MaKH },
                new SqlParameter("@CCCD", SqlDbType.NVarChar, 20) { Value = kh.CCCD },
                new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = kh.HoTen },
                new SqlParameter("@SODT", SqlDbType.NVarChar, 20) { Value = kh.SODT },
                new SqlParameter("@GPLX", SqlDbType.NVarChar, 30) { Value = kh.GPLX },
                new SqlParameter("@IsBlacklisted", SqlDbType.Bit) { Value = kh.IsBlacklisted }
            );
        }

        public int XoaKhachHang(int maKH)
        {
            const string sql = "DELETE FROM DSKHACHHANG WHERE MaKH = @MaKH";
            return new KHProvider().ExecuteNonQuery(CommandType.Text, sql,
                new SqlParameter("@MaKH", SqlDbType.Int) { Value = maKH }
            );
        }

        [Key]
        public int MaKH { get; set; }
        public int MaTK { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string SODT { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string GPLX { get; set; } = string.Empty;
        public string TrangThaiXacMinh { get; set; } = "Chưa gửi";
        public bool IsBlacklisted { get; set; }
    }
}
