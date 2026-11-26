using Microsoft.Data.SqlClient;
using System.Data;

namespace QUAN_LY_THUE_XE.Models
{
    public class KHProvider
    {
        public static string ConnectionString { get; set; } = "Data Source=MSI;Initial Catalog=KHACHHANG;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        // Mở kết nối, chạy câu SELECT, đổ kết quả vào DataTable rồi tự giải phóng tài nguyên.
        public DataTable Select(CommandType commandType, string sql, params SqlParameter[] parameters)
        {
            // Tạo kết nối SQL Server theo connection string đã cấu hình.
            using var connection = new SqlConnection(ConnectionString);
            // Chuẩn bị câu lệnh SQL và gắn câu lệnh đó với kết nối vừa tạo.
            using var command = new SqlCommand(sql, connection) { CommandType = commandType };
            // Thêm tham số nếu câu SQL cần giá trị truyền vào, ví dụ từ khóa tìm kiếm.
            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            // Adapter thực thi SELECT và chuyển kết quả thành dữ liệu dạng bảng trong bộ nhớ.
            using var adapter = new SqlDataAdapter(command);
            var table = new DataTable();
            // Mở kết nối trước khi lấy dữ liệu.
            connection.Open();
            // Đổ các hàng và cột nhận được từ SQL Server vào DataTable.
            adapter.Fill(table);
            // Trả DataTable cho tầng gọi để ánh xạ sang đối tượng khách hàng.
            return table;
        }

        // Dùng cho INSERT/UPDATE/DELETE; bind tham số, thực thi và trả số hàng bị ảnh hưởng.
        public int ExecuteNonQuery(CommandType commandType, string sql, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(sql, connection) { CommandType = commandType };
            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            return command.ExecuteNonQuery();
        }

        // Dùng cho truy vấn trả về một giá trị đơn, ví dụ MAX(MaKH) + 1; caller chuyển kết quả về kiểu cần dùng.
        public object? ExecuteScalar(CommandType commandType, string sql, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(sql, connection) { CommandType = commandType };
            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            return command.ExecuteScalar();
        }
    }
}
