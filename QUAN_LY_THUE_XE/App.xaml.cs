using System.Windows;
using QUAN_LY_THUE_XE.Data;

namespace QUAN_LY_THUE_XE
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Tự động khởi tạo CSDL và nạp dữ liệu mẫu
            using (var context = new AppDbContext())
            {
                DbInitializer.Initialize(context);
            }
        }
    }
}
