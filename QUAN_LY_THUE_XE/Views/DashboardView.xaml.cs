using Microsoft.Data.SqlClient;
using QUAN_LY_THUE_XE.Data;
using System;
using System.Windows.Controls;

namespace QUAN_LY_THUE_XE.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            LoadStats();
        }

        // Truy vấn 3 con số trực tiếp từ bảng Xes qua EF Core.
        private void LoadStats()
        {
            try
            {
                using var context = new AppDbContext();
                int total     = context.Xes.Count();
                int renting   = context.Xes.Count(x => x.TrangThai == "Đang thuê");
                int available = context.Xes.Count(x => x.TrangThai == "Sẵn sàng");

                TotalCarsText.Text     = total.ToString();
                RentingCarsText.Text   = renting.ToString();
                AvailableCarsText.Text = available.ToString();
                DashboardStatus.Text   = $"Cập nhật lúc {DateTime.Now:HH:mm  dd/MM/yyyy}";
            }
            catch (SqlException ex)
            {
                DashboardStatus.Text = $"Không thể kết nối cơ sở dữ liệu: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                DashboardStatus.Text = $"Lỗi khi tải dữ liệu: {ex.Message}";
            }
        }
    }
}
